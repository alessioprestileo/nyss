﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Utils;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Configuration;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.Common.Dto;
using RX.Nyss.Web.Features.Common.Extensions;
using RX.Nyss.Web.Features.NationalSocieties;
using RX.Nyss.Web.Features.NationalSocietyReports.Dto;
using RX.Nyss.Web.Features.Projects;
using RX.Nyss.Web.Features.Users;
using RX.Nyss.Web.Services.Authorization;
using RX.Nyss.Web.Utils.DataContract;
using RX.Nyss.Web.Utils.Extensions;
using static RX.Nyss.Common.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.NationalSocietyReports
{
    public interface INationalSocietyReportService
    {
        Task<Result<PaginatedList<NationalSocietyReportListResponseDto>>> List(int nationalSocietyId, int pageNumber, NationalSocietyReportListFilterRequestDto filter);
        Task<Result<NationalSocietyReportListFilterResponseDto>> Filters(int nationalSocietyId);
    }

    public class NationalSocietyReportService : INationalSocietyReportService
    {
        private readonly INyssWebConfig _config;
        private readonly INyssContext _nyssContext;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly INationalSocietyService _nationalSocietyService;
        private readonly IAuthorizationService _authorizationService;

        public NationalSocietyReportService(
            INyssContext nyssContext,
            IUserService userService,
            IProjectService projectService, 
            INationalSocietyService nationalSocietyService, 
            INyssWebConfig config,
            IAuthorizationService authorizationService
            )
        {
            _nyssContext = nyssContext;
            _userService = userService;
            _projectService = projectService;
            _nationalSocietyService = nationalSocietyService;
            _config = config;
            _authorizationService = authorizationService;
        }

        public async Task<Result<PaginatedList<NationalSocietyReportListResponseDto>>> List(int nationalSocietyId, int pageNumber, NationalSocietyReportListFilterRequestDto filter)
        {
            var userApplicationLanguageCode = await _userService.GetUserApplicationLanguageCode(_authorizationService.GetCurrentUserName());
            var supervisorProjectIds = await _projectService.GetSupervisorProjectIds(_authorizationService.GetCurrentUserName());
            var rowsPerPage = _config.PaginationRowsPerPage;

            var currentUser = await _authorizationService.GetCurrentUser();

            var currentUserOrganizationId = await _nyssContext.UserNationalSocieties
                .Where(uns => uns.NationalSocietyId == nationalSocietyId && uns.User == currentUser)
                .Select(uns => uns.OrganizationId)
                .SingleOrDefaultAsync();

            var baseQuery = _nyssContext.RawReports
                .Where(r => r.NationalSociety.Id == nationalSocietyId)
                .Where(r => r.IsTraining == null || r.IsTraining == false)
                .Where(r =>
                    filter.ReportsType == NationalSocietyReportListType.FromDcp
                        ? r.DataCollector.DataCollectorType == DataCollectorType.CollectionPoint
                        : (filter.ReportsType == NationalSocietyReportListType.Main 
                            ? r.DataCollector.DataCollectorType == DataCollectorType.Human
                            : r.DataCollector == null
                        )
                )
                .Where(r => filter.HealthRiskId == null || r.Report.ProjectHealthRisk.HealthRiskId == filter.HealthRiskId)
                .Where(r => filter.Status
                    ? r.Report != null && !r.Report.MarkedAsError
                    : r.Report == null || (r.Report != null && r.Report.MarkedAsError))
                .FilterByArea(MapToArea(filter.Area));

            if (_authorizationService.IsCurrentUserInRole(Role.Supervisor))
            {
                baseQuery = baseQuery
                    .Where(r => r.DataCollector == null || supervisorProjectIds == null || supervisorProjectIds.Contains(r.DataCollector.Project.Id));
            }

            var result = await baseQuery.Select(r => new NationalSocietyReportListResponseDto
                {
                    Id = r.Id,
                    DateTime = r.ReceivedAt,
                    HealthRiskName = r.Report.ProjectHealthRisk.HealthRisk.LanguageContents.Where(lc => lc.ContentLanguage.LanguageCode == userApplicationLanguageCode).Select(lc => lc.Name).Single(),
                    IsValid = r.Report != null,
                    IsMarkedAsError = r.Report.MarkedAsError,
                    IsAnonymized = currentUser.Role != Role.Administrator && !r.NationalSociety.NationalSocietyUsers.Any(
                        nsu => nsu.UserId == r.DataCollector.Supervisor.Id && nsu.OrganizationId == currentUserOrganizationId),
                    OrganizationName = r.NationalSociety.NationalSocietyUsers
                        .Where(nsu => nsu.UserId == r.DataCollector.Supervisor.Id)
                        .Select(nsu => nsu.Organization.Name)
                        .FirstOrDefault(),
                    ProjectName = r.Report != null
                        ? r.Report.ProjectHealthRisk.Project.Name
                        : r.DataCollector.Project.Name,
                    ProjectTimeZone = r.Report != null
                        ? r.Report.ProjectHealthRisk.Project.TimeZone
                        : r.DataCollector.Project.TimeZone,
                    Region = r.Village.District.Region.Name,
                    District = r.Village.District.Name,
                    Village = r.Village.Name,
                    Zone = r.Zone.Name,
                    DataCollectorDisplayName = r.DataCollector.DataCollectorType == DataCollectorType.CollectionPoint
                        ? r.DataCollector.Name
                        : r.DataCollector.DisplayName,
                    PhoneNumber = r.Sender,
                    Message = r.Text,
                    CountMalesBelowFive = r.Report.ReportedCase.CountMalesBelowFive,
                    CountMalesAtLeastFive = r.Report.ReportedCase.CountMalesAtLeastFive,
                    CountFemalesBelowFive = r.Report.ReportedCase.CountFemalesBelowFive,
                    CountFemalesAtLeastFive = r.Report.ReportedCase.CountFemalesAtLeastFive,
                    ReferredCount = r.Report.DataCollectionPointCase.ReferredCount,
                    DeathCount = r.Report.DataCollectionPointCase.DeathCount,
                    FromOtherVillagesCount = r.Report.DataCollectionPointCase.FromOtherVillagesCount
                })
                //ToDo: order base on filter.OrderBy property
                .OrderBy(r => r.DateTime, filter.SortAscending)
                .Page(pageNumber, rowsPerPage)
                .ToListAsync();

            foreach (var report in result)
            {
                if (report.ProjectTimeZone != null)
                {
                    report.DateTime = TimeZoneInfo.ConvertTimeFromUtc(report.DateTime, TimeZoneInfo.FindSystemTimeZoneById(report.ProjectTimeZone));
                }

                if (report.IsAnonymized)
                {
                    report.DataCollectorDisplayName = report.OrganizationName;
                    report.PhoneNumber = report.PhoneNumber.Length > 4 ? "***" + report.PhoneNumber.SubstringFromEnd(4) : "***";
                    report.Region = "";
                    report.Zone = "";
                    report.District = "";
                    report.Village = "";
                }
            }

            return Success(result.AsPaginatedList(pageNumber, await baseQuery.CountAsync(), rowsPerPage));
        }

        public async Task<Result<NationalSocietyReportListFilterResponseDto>> Filters(int nationalSocietyId)
        {
            var nationalSocietyHealthRiskNames = await _nationalSocietyService.GetHealthRiskNames(nationalSocietyId, false);

            var dto = new NationalSocietyReportListFilterResponseDto
            {
                HealthRisks = nationalSocietyHealthRiskNames
                    .Select(p => new HealthRiskDto
                    {
                        Id = p.Id,
                        Name = p.Name
                    })
            };

            return Success(dto);
        }

        private static Area MapToArea(AreaDto area) =>
            area == null
                ? null
                : new Area
                {
                    AreaType = area.Type,
                    AreaId = area.Id
                };
    }
}
