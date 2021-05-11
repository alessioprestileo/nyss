using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Services.StringsResources;
using RX.Nyss.Common.Utils;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Data.Queries;
using RX.Nyss.Web.Configuration;
using RX.Nyss.Web.Features.Alerts;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.Common.Dto;
using RX.Nyss.Web.Features.Common.Extensions;
using RX.Nyss.Web.Features.Projects;
using RX.Nyss.Web.Features.Reports.Dto;
using RX.Nyss.Web.Features.Users;
using RX.Nyss.Web.Services.Authorization;
using RX.Nyss.Web.Utils.DataContract;
using RX.Nyss.Web.Utils.Extensions;
using static RX.Nyss.Common.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.Reports
{
    public interface IReportService
    {
        Task<Result<ReportResponseDto>> Get(int reportId);
        Task<Result<PaginatedList<ReportListResponseDto>>> List(int projectId, int pageNumber, ReportListFilterRequestDto filter);
        Task<Result<ReportListFilterResponseDto>> GetFilters(int nationalSocietyId);
        Task<Result<HumanHealthRiskResponseDto>> GetHumanHealthRisksForProject(int projectId);
        Task<Result> Edit(int reportId, int projectId, ReportRequestDto reportRequestDto);
        Task<Result> MarkAsError(int reportId);
        IQueryable<RawReport> GetRawReportsWithDataCollectorQuery(ReportsFilter filters);
        IQueryable<Report> GetDashboardHealthRiskEventReportsQuery(ReportsFilter filters);
        IQueryable<Report> GetSuccessReportsQuery(ReportsFilter filters);
        Task<Result> AcceptReport(int reportId);
        Task<Result> DismissReport(int reportId);
    }

    public class ReportService : IReportService
    {
        private readonly INyssWebConfig _config;
        private readonly INyssContext _nyssContext;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IStringsResourcesService _stringsResourcesService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAlertReportService _alertReportService;

        public ReportService(INyssContext nyssContext,
            IUserService userService,
            IProjectService projectService,
            INyssWebConfig config,
            IAuthorizationService authorizationService,
            IStringsResourcesService stringsResourcesService,
            IDateTimeProvider dateTimeProvider,
            IAlertReportService alertReportService)
        {
            _nyssContext = nyssContext;
            _userService = userService;
            _projectService = projectService;
            _config = config;
            _authorizationService = authorizationService;
            _stringsResourcesService = stringsResourcesService;
            _dateTimeProvider = dateTimeProvider;
            _alertReportService = alertReportService;
        }

        public async Task<Result<ReportResponseDto>> Get(int reportId)
        {
            var report = await _nyssContext.Reports
                .Select(r => new ReportResponseDto
                {
                    Id = r.Id,
                    DataCollectorId = r.DataCollector.Id,
                    DataCollectorType = r.ReportType == ReportType.DataCollectionPoint ? DataCollectorType.CollectionPoint : DataCollectorType.Human,
                    ReportType = r.ReportType,
                    ReportStatus = r.Status,
                    ReportVillageId = r.RawReport.Village.Id,
                    ReportZoneId = r.RawReport.Zone.Id,
                    Date = r.ReceivedAt.Date,
                    HealthRiskId = r.ProjectHealthRisk.HealthRiskId,
                    CountMalesBelowFive = r.ReportedCase.CountMalesBelowFive.Value,
                    CountMalesAtLeastFive = r.ReportedCase.CountMalesAtLeastFive.Value,
                    CountFemalesBelowFive = r.ReportedCase.CountFemalesBelowFive.Value,
                    CountFemalesAtLeastFive = r.ReportedCase.CountFemalesAtLeastFive.Value,
                    CountUnspecifiedSexAndAge = r.ReportedCase.CountUnspecifiedSexAndAge.Value,
                    ReferredCount = r.DataCollectionPointCase.ReferredCount.Value,
                    DeathCount = r.DataCollectionPointCase.DeathCount.Value,
                    FromOtherVillagesCount = r.DataCollectionPointCase.FromOtherVillagesCount.Value
                })
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
            {
                return Error<ReportResponseDto>(ResultKey.Report.ReportNotFound);
            }

            var result = Success(report);

            return result;
        }

        public async Task<Result<PaginatedList<ReportListResponseDto>>> List(int projectId, int pageNumber, ReportListFilterRequestDto filter)
        {
            var currentUserName = _authorizationService.GetCurrentUserName();
            var currentRole = (await _authorizationService.GetCurrentUser()).Role;
            var isSupervisor = currentRole == Role.Supervisor;
            var isHeadSupervisor = currentRole == Role.HeadSupervisor;
            var currentUserId = await _nyssContext.Users.FilterAvailable()
                .Where(u => u.EmailAddress == currentUserName)
                .Select(u => u.Id)
                .SingleAsync();

            var userApplicationLanguageCode = await _userService.GetUserApplicationLanguageCode(currentUserName);
            var stringResources = (await _stringsResourcesService.GetStringsResources(userApplicationLanguageCode)).Value;

            var baseQuery = await BuildRawReportsBaseQuery(filter, projectId);

            var currentUserOrganization = await _nyssContext.Projects
                .Where(p => p.Id == projectId)
                .SelectMany(p => p.NationalSociety.NationalSocietyUsers)
                .Where(uns => uns.User.Id == currentUserId)
                .Select(uns => uns.Organization)
                .SingleOrDefaultAsync();


            var result = baseQuery.Select(r => new ReportListResponseDto
                {
                    Id = r.Id,
                    IsAnonymized = isSupervisor || isHeadSupervisor
                        ? (currentRole == Role.HeadSupervisor && r.DataCollector.Supervisor.HeadSupervisor.Id != currentUserId)
                        || (currentRole == Role.Supervisor && r.DataCollector.Supervisor.Id != currentUserId)
                        : currentRole != Role.Administrator && !r.NationalSociety.NationalSocietyUsers.Any(
                            nsu => nsu.UserId == r.DataCollector.Supervisor.Id && nsu.OrganizationId == currentUserOrganization.Id),
                    OrganizationName = r.NationalSociety.NationalSocietyUsers
                        .Where(nsu => nsu.UserId == r.DataCollector.Supervisor.Id)
                        .Select(nsu => nsu.Organization.Name)
                        .FirstOrDefault(),
                    DateTime = r.ReceivedAt.AddHours(filter.UtcOffset),
                    HealthRiskName = r.Report.ProjectHealthRisk.HealthRisk.LanguageContents
                        .Where(lc => lc.ContentLanguage.LanguageCode == userApplicationLanguageCode)
                        .Select(lc => lc.Name)
                        .Single(),
                    IsActivityReport = r.Report.ProjectHealthRisk.HealthRisk.HealthRiskCode == 99
                        || r.Report.ProjectHealthRisk.HealthRisk.HealthRiskCode == 98,
                    IsValid = r.Report != null,
                    Region = r.Village.District.Region.Name,
                    District = r.Village.District.Name,
                    Village = r.Village.Name,
                    Zone = r.Zone.Name,
                    DataCollectorDisplayName = r.DataCollector.DataCollectorType == DataCollectorType.CollectionPoint
                        ? r.DataCollector.Name
                        : r.DataCollector.DisplayName,
                    SupervisorName = r.DataCollector.Supervisor.Name,
                    PhoneNumber = r.Sender,
                    IsMarkedAsError = r.Report.MarkedAsError,
                    Alert = r.Report.ReportAlerts
                        .OrderByDescending(ra => ra.AlertId)
                        .Select(ra => new ReportListAlert
                        {
                            Id = ra.AlertId,
                            Status = ra.Alert.Status,
                            ReportWasCrossCheckedBeforeEscalation = ra.Report.AcceptedAt < ra.Alert.EscalatedAt || ra.Report.RejectedAt < ra.Alert.EscalatedAt
                        })
                        .FirstOrDefault(),
                    ReportId = r.ReportId,
                    ReportType = r.Report.ReportType,
                    Message = r.Text,
                    CountMalesBelowFive = r.Report.ReportedCase.CountMalesBelowFive,
                    CountMalesAtLeastFive = r.Report.ReportedCase.CountMalesAtLeastFive,
                    CountFemalesBelowFive = r.Report.ReportedCase.CountFemalesBelowFive,
                    CountFemalesAtLeastFive = r.Report.ReportedCase.CountFemalesAtLeastFive,
                    ReferredCount = r.Report.DataCollectionPointCase.ReferredCount,
                    DeathCount = r.Report.DataCollectionPointCase.DeathCount,
                    FromOtherVillagesCount = r.Report.DataCollectionPointCase.FromOtherVillagesCount,
                    Status = r.Report.Status,
                    ReportErrorType = r.ErrorType
                })
                //ToDo: order base on filter.OrderBy property
                .OrderBy(r => r.DateTime, filter.SortAscending);

            var rowsPerPage = _config.PaginationRowsPerPage;
            var reports = await result
                .Page(pageNumber, rowsPerPage)
                .ToListAsync<IReportListResponseDto>();

            if(filter.DataCollectorType != ReportListDataCollectorType.UnknownSender)
            {
                AnonymizeCrossOrganizationReports(reports, currentUserOrganization?.Name, stringResources);
            }

            return Success(reports.Cast<ReportListResponseDto>().AsPaginatedList(pageNumber, await baseQuery.CountAsync(), rowsPerPage));
        }

        public async Task<Result<ReportListFilterResponseDto>> GetFilters(int projectId)
        {
            var healthRiskTypes = new List<HealthRiskType>
            {
                HealthRiskType.Human,
                HealthRiskType.NonHuman,
                HealthRiskType.UnusualEvent,
                HealthRiskType.Activity
            };
            var projectHealthRiskNames = await _projectService.GetHealthRiskNames(projectId, healthRiskTypes);

            var dto = new ReportListFilterResponseDto { HealthRisks = projectHealthRiskNames };

            return Success(dto);
        }

        public async Task<Result<HumanHealthRiskResponseDto>> GetHumanHealthRisksForProject(int projectId)
        {
            var humanHealthRiskType = new List<HealthRiskType> { HealthRiskType.Human };
            var projectHealthRisks = await _projectService.GetHealthRiskNames(projectId, humanHealthRiskType);

            var dto = new HumanHealthRiskResponseDto { HealthRisks = projectHealthRisks };

            return Success(dto);
        }

        public async Task<Result> Edit(int reportId, int projectId, ReportRequestDto reportRequestDto)
        {
            var dataCollectorChanged = false;
            var locationChanged = false;

            var report = await _nyssContext.Reports
                .Include(r => r.RawReport).ThenInclude(raw => raw.Zone)
                .Include(r => r.ProjectHealthRisk)
                .Include(r => r.DataCollector).ThenInclude(phr => phr.Project)
                .SingleOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
            {
                return Error<ReportResponseDto>(ResultKey.Report.ReportNotFound);
            }

            var projectHealthRisk = await _nyssContext.ProjectHealthRisks
                .Include(phr => phr.HealthRisk)
                .SingleOrDefaultAsync(
                    phr => phr.HealthRiskId == reportRequestDto.HealthRiskId &&
                    phr.Project.Id == projectId);

            if (projectHealthRisk == null)
            {
                return Error<ReportResponseDto>(ResultKey.Report.Edit.HealthRiskNotAssignedToProject);
            }

            var updatedReceivedAt = new DateTime(reportRequestDto.Date.Year, reportRequestDto.Date.Month, reportRequestDto.Date.Day,
                report.ReceivedAt.Hour, report.ReceivedAt.Minute, report.ReceivedAt.Second);
            report.RawReport.ReceivedAt = updatedReceivedAt;
            report.ReceivedAt = updatedReceivedAt;
            report.ProjectHealthRisk = projectHealthRisk;
            if (report.ReportType != ReportType.Event)
            {
                report.ReportedCase.CountMalesBelowFive = reportRequestDto.CountMalesBelowFive;
                report.ReportedCase.CountMalesAtLeastFive = reportRequestDto.CountMalesAtLeastFive;
                report.ReportedCase.CountFemalesBelowFive = reportRequestDto.CountFemalesBelowFive;
                report.ReportedCase.CountFemalesAtLeastFive = reportRequestDto.CountFemalesAtLeastFive;
                report.ReportedCase.CountUnspecifiedSexAndAge = reportRequestDto.CountUnspecifiedSexAndAge;
            }
            report.ReportedCaseCount = reportRequestDto.CountMalesBelowFive +
                reportRequestDto.CountMalesAtLeastFive +
                reportRequestDto.CountFemalesBelowFive +
                reportRequestDto.CountFemalesAtLeastFive;

            if (report.ReportType == ReportType.DataCollectionPoint)
            {
                report.DataCollectionPointCase.ReferredCount = reportRequestDto.ReferredCount;
                report.DataCollectionPointCase.DeathCount = reportRequestDto.DeathCount;
                report.DataCollectionPointCase.FromOtherVillagesCount = reportRequestDto.FromOtherVillagesCount;
            }

            report.ModifiedAt = _dateTimeProvider.UtcNow;
            report.ModifiedBy = _authorizationService.GetCurrentUserName();

            if (report.DataCollector == null ||
                report.DataCollector.Id != reportRequestDto.DataCollectorId)
            {
                var newDataCollector = await _nyssContext.DataCollectors
                    .Include(dc => dc.DataCollectorLocations).ThenInclude(dcl => dcl.Village)
                    .Include(dc => dc.DataCollectorLocations).ThenInclude(dcl => dcl.Zone)
                    .Where(dc => dc.Id == reportRequestDto.DataCollectorId)
                    .SingleOrDefaultAsync();

                if (newDataCollector == null)
                {
                    return Error<ReportResponseDto>(ResultKey.Report.Edit.SenderDoesNotExist);
                }

                if (newDataCollector.DataCollectorType == DataCollectorType.CollectionPoint &&
                    report.ReportType != ReportType.DataCollectionPoint)
                {
                    return Error<ReportResponseDto>(ResultKey.Report.Edit.SenderEditError);
                }

                if (report.Status != ReportStatus.New)
                {
                    return Error<ReportResponseDto>(ResultKey.Report.Edit.OnlyNewReportsEditable);
                }

                report = SetNewDataCollectorOnReport(report, newDataCollector);
                dataCollectorChanged = true;

            }

            if (report.ReportType != ReportType.DataCollectionPoint &&
                report.ReportType != ReportType.Aggregate)
            {
                report.Status = reportRequestDto.ReportStatus;
            }

            if (reportRequestDto.DataCollectorLocation != null && LocationNeedsUpdate(report, reportRequestDto))
            {
                var newDataCollector = await _nyssContext.DataCollectors
                    .Include(dc => dc.DataCollectorLocations).ThenInclude(lc => lc.Village)
                    .Include(dc => dc.DataCollectorLocations).ThenInclude(lc => lc.Zone)
                    .Where(dc => dc.Id == reportRequestDto.DataCollectorId)
                    .SingleOrDefaultAsync();

                var newLocation = newDataCollector.DataCollectorLocations
                    .FirstOrDefault(location => (location.Village.Id == reportRequestDto.DataCollectorLocation.VillageId &&
                                                (location.Zone == null || location.Zone.Id == reportRequestDto.DataCollectorLocation.ZoneId)));


                report.Location = newLocation.Location;
                report.RawReport.Village = newLocation.Village;
                report.RawReport.Zone = newLocation.Zone;

                locationChanged = true;

            }

            await _nyssContext.SaveChangesAsync();

            if (dataCollectorChanged || locationChanged)
            {
                await _alertReportService.EditReport(report.Id);
            }

            return SuccessMessage(ResultKey.Report.Edit.EditSuccess);
        }

        public IQueryable<RawReport> GetRawReportsWithDataCollectorQuery(ReportsFilter filters) =>
            _nyssContext.RawReports
                .FilterByTrainingMode(filters.IsTraining)
                .FromKnownDataCollector()
                .FilterByArea(filters.Area)
                .FilterByDataCollectorType(filters.DataCollectorType)
                .FilterByOrganization(filters.OrganizationId)
                .FilterByProject(filters.ProjectId)
                .FilterReportsByNationalSociety(filters.NationalSocietyId)
                .FilterByDate(filters.StartDate, filters.EndDate)
                .FilterByHealthRisk(filters.HealthRiskId);

        public IQueryable<Report> GetSuccessReportsQuery(ReportsFilter filters) =>
            GetRawReportsWithDataCollectorQuery(filters)
                .AllSuccessfulReports()
                .Select(r => r.Report)
                .Where(r => !r.MarkedAsError);

        public IQueryable<Report> GetDashboardHealthRiskEventReportsQuery(ReportsFilter filters) =>
            GetSuccessReportsQuery(filters)
                .Where(r => r.ProjectHealthRisk.HealthRisk.HealthRiskType != HealthRiskType.Activity);

        public async Task<Result> MarkAsError(int reportId)
        {
            var report = await _nyssContext.Reports
                .Where(r => !r.ReportAlerts.Any())
                .Include(r => r.ProjectHealthRisk.Project)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report.ProjectHealthRisk.Project.State != ProjectState.Open)
            {
                return Error(ResultKey.Report.ProjectIsClosed);
            }

            report.MarkedAsError = true;
            await _nyssContext.SaveChangesAsync();

            return Success();
        }

        public async Task<Result> AcceptReport(int reportId)
        {
            var currentUser = await _authorizationService.GetCurrentUser();
            var report = await _nyssContext.RawReports
                .Where(r => r.Id == reportId && r.Report != null)
                .Select(r => r.Report)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                return Error(ResultKey.Report.ReportNotFound);
            }

            if (report.Status == ReportStatus.Accepted)
            {
                return Error(ResultKey.Report.AlreadyCrossChecked);
            }

            if (report.MarkedAsError)
            {
                return Error(ResultKey.Report.CannotCrossCheckErrorReport);
            }

            report.AcceptedAt = _dateTimeProvider.UtcNow;
            report.AcceptedBy = currentUser;
            report.Status = ReportStatus.Accepted;

            await _nyssContext.SaveChangesAsync();
            return Success();
        }

        public async Task<Result> DismissReport(int reportId)
        {
            var currentUser = await _authorizationService.GetCurrentUser();
            var report = await _nyssContext.RawReports
                .Where(r => r.Id == reportId && r.Report != null)
                .Select(r => r.Report)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                return Error(ResultKey.Report.ReportNotFound);
            }

            if (report.Status == ReportStatus.Rejected)
            {
                return Error(ResultKey.Report.AlreadyCrossChecked);
            }

            if (report.MarkedAsError)
            {
                return Error(ResultKey.Report.CannotCrossCheckErrorReport);
            }

            report.RejectedAt = _dateTimeProvider.UtcNow;
            report.RejectedBy = currentUser;
            report.Status = ReportStatus.Rejected;

            await _nyssContext.SaveChangesAsync();
            return Success();
        }

        private async Task<IQueryable<RawReport>> BuildRawReportsBaseQuery(ReportListFilterRequestDto filter, int projectId)
        {
            if(filter.DataCollectorType == ReportListDataCollectorType.UnknownSender)
            {
                var nationalSocietyId = await _nyssContext.Projects
                    .Where(p => p.Id == projectId)
                    .Select(p => p.NationalSocietyId)
                    .SingleOrDefaultAsync();

                return _nyssContext.RawReports
                    .Where(r => r.NationalSociety.Id == nationalSocietyId)
                    .FilterByDataCollectorType(filter.DataCollectorType)
                    .FilterByHealthRisk(filter.HealthRiskId)
                    .FilterByFormatCorrectness(filter.FormatCorrect)
                    .FilterByErrorType(filter.ErrorType)
                    .FilterByArea(MapToArea(filter.Area))
                    .FilterByReportStatus(filter.ReportStatus)
                    .FilterByReportType(filter.ReportType);
            }

            return _nyssContext.RawReports
                .FilterByProject(projectId)
                .FilterByHealthRisk(filter.HealthRiskId)
                .FilterByDataCollectorType(filter.DataCollectorType)
                .FilterByArea(MapToArea(filter.Area))
                .FilterByFormatCorrectness(filter.FormatCorrect)
                .FilterByErrorType(filter.ErrorType)
                .FilterByReportStatus(filter.ReportStatus)
                .FilterByReportType(filter.ReportType);
        }

        private static bool LocationNeedsUpdate(Report report, ReportRequestDto reportRequestDto)
        {
            if (report.RawReport.Village == null || report.RawReport.Zone == null)
            {
                return true;
            }

            if (report.RawReport.Village.Id == reportRequestDto.DataCollectorLocation.VillageId && report.RawReport.Zone.Id == reportRequestDto.DataCollectorLocation.ZoneId)
            {
                return false;
            }

            return true;
        }

        private static Report SetNewDataCollectorOnReport(Report report, DataCollector newDataCollector)
        {
            var locationSet = newDataCollector.DataCollectorLocations != null;
            var multipleLocations = locationSet && newDataCollector.DataCollectorLocations.Count > 1;

            // Values to keep from the unknown sender
            report.PhoneNumber = report.DataCollector != null
                ? newDataCollector.PhoneNumber
                : report.PhoneNumber;
            report.RawReport.Sender = report.PhoneNumber;
            report.RawReport.ModemNumber = (report.DataCollector != null && newDataCollector.RawReports != null) ?
                newDataCollector.RawReports.Where(rawReport => rawReport.ReportId == report.Id)
                    .Select(rawReport => rawReport.ModemNumber)
                    .SingleOrDefault() : report.RawReport.ModemNumber;

            // Values to take from the new data collector
            report.DataCollector = newDataCollector;
            report.Location = locationSet ? (multipleLocations ? null : newDataCollector.DataCollectorLocations.First().Location) : null;
            report.RawReport.DataCollector = newDataCollector;
            report.RawReport.Village = locationSet ? (multipleLocations ? null : newDataCollector.DataCollectorLocations.First().Village) : null;
            report.RawReport.Zone = locationSet
                ? (multipleLocations
                    ? null
                    : newDataCollector.DataCollectorLocations.First().Zone)
                : null;

            return report;
        }

        private static string GetStringResource(IDictionary<string, StringResourceValue> stringResources, string key) =>
            stringResources.Keys.Contains(key)
                ? stringResources[key].Value
                : key;

        internal static void AnonymizeCrossOrganizationReports(IEnumerable<IReportListResponseDto> reports, string currentUserOrganizationName, IDictionary<string, StringResourceValue> stringsResources) =>
            reports
                .Where(r => r.IsAnonymized)
                .ToList()
                .ForEach(x =>
                {
                    x.DataCollectorDisplayName = x.OrganizationName == currentUserOrganizationName
                        ? $"{GetStringResource(stringsResources, ResultKey.Report.LinkedToSupervisor)} {x.SupervisorName}"
                        : $"{GetStringResource(stringsResources, ResultKey.Report.LinkedToOrganization)} {x.OrganizationName}";
                    x.PhoneNumber = "***";
                    x.Zone = "";
                    x.Village = "";
                });

        internal static Area MapToArea(AreaDto area) =>
            area == null
                ? null
                : new Area
                {
                    AreaType = area.Type,
                    AreaId = area.Id
                };
    }
}
