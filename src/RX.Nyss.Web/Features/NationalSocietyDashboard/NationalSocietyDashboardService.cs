﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.NationalSocieties;
using RX.Nyss.Web.Features.NationalSocietyDashboard.Dto;
using RX.Nyss.Web.Features.Reports;
using RX.Nyss.Web.Services.ReportsDashboard;
using RX.Nyss.Web.Services.ReportsDashboard.Dto;
using static RX.Nyss.Common.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.NationalSocietyDashboard
{
    public interface INationalSocietyDashboardService
    {
        Task<Result<NationalSocietyDashboardFiltersResponseDto>> GetFiltersData(int nationalSocietyId);

        Task<Result<NationalSocietyDashboardResponseDto>> GetData(int nationalSocietyId, NationalSocietyDashboardFiltersRequestDto filtersDto);

        Task<Result<IEnumerable<ReportsSummaryHealthRiskResponseDto>>> GetReportHealthRisks(int nationalSocietyId, double latitude, double longitude,
            NationalSocietyDashboardFiltersRequestDto filtersDto);
    }

    public class NationalSocietyDashboardService : INationalSocietyDashboardService
    {
        private readonly INationalSocietyService _nationalSocietyService;
        private readonly INationalSocietyDashboardSummaryService _nationalSocietyDashboardSummaryService;
        private readonly IReportsDashboardMapService _reportsDashboardMapService;
        private readonly IReportsDashboardByVillageService _reportsDashboardByVillageService;
        private readonly INyssContext _nyssContext;

        public NationalSocietyDashboardService(
            INationalSocietyService nationalSocietyService,
            INationalSocietyDashboardSummaryService nationalSocietyDashboardSummaryService,
            IReportsDashboardMapService reportsDashboardMapService,
            IReportsDashboardByVillageService reportsDashboardByVillageService,
            INyssContext nyssContext)
        {
            _nationalSocietyService = nationalSocietyService;
            _nationalSocietyDashboardSummaryService = nationalSocietyDashboardSummaryService;
            _reportsDashboardMapService = reportsDashboardMapService;
            _reportsDashboardByVillageService = reportsDashboardByVillageService;
            _nyssContext = nyssContext;
        }

        public async Task<Result<NationalSocietyDashboardFiltersResponseDto>> GetFiltersData(int nationalSocietyId)
        {
            var healthRiskNames = await _nationalSocietyService.GetHealthRiskNames(nationalSocietyId, true);

            var organizations = await _nyssContext.Organizations
                .Where(o => o.NationalSocietyId == nationalSocietyId)
                .Select(o => new NationalSocietyDashboardFiltersResponseDto.OrganizationDto
                {
                    Id = o.Id,
                    Name = o.Name
                }).ToListAsync();

            var dto = new NationalSocietyDashboardFiltersResponseDto
            {
                HealthRisks = healthRiskNames,
                Organizations = organizations
            };

            return Success(dto);
        }

        public async Task<Result<NationalSocietyDashboardResponseDto>> GetData(int nationalSocietyId, NationalSocietyDashboardFiltersRequestDto filtersDto)
        {
            if (filtersDto.EndDate < filtersDto.StartDate)
            {
                return Success(new NationalSocietyDashboardResponseDto());
            }

            var filters = MapToReportFilters(nationalSocietyId, filtersDto);
            var reportsGroupedByVillageAndDate = await _reportsDashboardByVillageService.GetReportsGroupedByVillageAndDate(filters, filtersDto.GroupingType);

            var dashboardDataDto = new NationalSocietyDashboardResponseDto
            {
                Summary = await _nationalSocietyDashboardSummaryService.GetData(filters),
                ReportsGroupedByLocation = await _reportsDashboardMapService.GetProjectSummaryMap(filters),
                ReportsGroupedByVillageAndDate = reportsGroupedByVillageAndDate
            };

            return Success(dashboardDataDto);
        }

        public async Task<Result<IEnumerable<ReportsSummaryHealthRiskResponseDto>>> GetReportHealthRisks(int nationalSocietyId, double latitude, double longitude,
            NationalSocietyDashboardFiltersRequestDto filtersDto)
        {
            var filters = MapToReportFilters(nationalSocietyId, filtersDto);
            var data = await _reportsDashboardMapService.GetProjectReportHealthRisks(filters, latitude, longitude);
            return Success(data);
        }

        private ReportsFilter MapToReportFilters(int nationalSocietyId, NationalSocietyDashboardFiltersRequestDto filtersDto) =>
            new ReportsFilter
            {
                StartDate = filtersDto.StartDate,
                EndDate = filtersDto.EndDate.AddDays(1),
                HealthRiskId = filtersDto.HealthRiskId,
                NationalSocietyId = nationalSocietyId,
                OrganizationId = filtersDto.OrganizationId,
                Area = filtersDto.Area == null
                    ? null
                    : new Area
                    {
                        AreaType = filtersDto.Area.Type,
                        AreaId = filtersDto.Area.Id
                    },
                DataCollectorType = MapToDataCollectorType(filtersDto.ReportsType),
                IsTraining = filtersDto.IsTraining
            };

        private static DataCollectorType? MapToDataCollectorType(NationalSocietyDashboardFiltersRequestDto.NationalSocietyReportsTypeDto nationalSocietyReportsType) =>
            nationalSocietyReportsType switch
            {
                NationalSocietyDashboardFiltersRequestDto.NationalSocietyReportsTypeDto.DataCollector => DataCollectorType.Human,
                NationalSocietyDashboardFiltersRequestDto.NationalSocietyReportsTypeDto.DataCollectionPoint => DataCollectorType.CollectionPoint,
                _ => null as DataCollectorType?
            };
    }
}
