﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Data;
using RX.Nyss.Web.Features.Reports;
using RX.Nyss.Web.Services.Geolocation;
using RX.Nyss.Web.Services.ReportsDashboard.Dto;

namespace RX.Nyss.Web.Services.ReportsDashboard
{
    public interface IReportsDashboardMapService
    {
        Task<IEnumerable<ReportsSummaryMapResponseDto>> GetProjectSummaryMap(ReportsFilter filters);
        Task<IEnumerable<ReportsSummaryHealthRiskResponseDto>> GetProjectReportHealthRisks(ReportsFilter filters, double latitude, double longitude);
    }

    public class ReportsDashboardMapService : IReportsDashboardMapService
    {
        private const double DefaultLatitude = 59.90822188626548; // Oslo
        private const double DefaultLongitude = 10.744628906250002;
        private readonly IReportService _reportService;
        private readonly IGeolocationService _geolocationService;
        private readonly INyssContext _nyssContext;

        public ReportsDashboardMapService(
            IReportService reportService,
            IGeolocationService geolocationService,
            INyssContext nyssContext)
        {
            _reportService = reportService;
            _geolocationService = geolocationService;
            _nyssContext = nyssContext;
        }

        public async Task<IEnumerable<ReportsSummaryMapResponseDto>> GetProjectSummaryMap(ReportsFilter filters)
        {
            var reports = _reportService.GetDashboardHealthRiskEventReportsQuery(filters)
                .Where(r => r.Location != null);

            var reportsSummaryMap = await reports
                .GroupBy(report => new
                {
                    report.Location.X,
                    report.Location.Y
                })
                .Select(grouping => new ReportsSummaryMapResponseDto
                {
                    ReportsCount = grouping.Sum(g => g.ReportedCaseCount),
                    Location = new ReportsSummaryMapResponseDto.MapReportLocation
                    {
                        Latitude = grouping.Key.Y,
                        Longitude = grouping.Key.X
                    }
                })
                .ToListAsync();

            if (!reportsSummaryMap.Any())
            {
                var countryName = filters.ProjectId.HasValue
                    ? await _nyssContext.Projects
                        .Where(p => p.Id == filters.ProjectId)
                        .Select(p => p.NationalSociety.Country.Name)
                        .FirstOrDefaultAsync()
                    : await _nyssContext.NationalSocieties
                        .Where(ns => ns.Id == filters.NationalSocietyId)
                        .Select(ns => ns.Country.Name)
                        .FirstOrDefaultAsync();

                var location = await _geolocationService.GetLocationFromCountry(countryName);

                reportsSummaryMap.Add(new ReportsSummaryMapResponseDto
                {
                    ReportsCount = 0,
                    Location = location.IsSuccess
                        ? new ReportsSummaryMapResponseDto.MapReportLocation
                        {
                            Latitude = location.Value.Latitude,
                            Longitude = location.Value.Longitude
                        }
                        : new ReportsSummaryMapResponseDto.MapReportLocation
                        {
                            Latitude = DefaultLatitude,
                            Longitude = DefaultLongitude
                        }
                });
            }

            return reportsSummaryMap;
        }

        public async Task<IEnumerable<ReportsSummaryHealthRiskResponseDto>> GetProjectReportHealthRisks(ReportsFilter filters, double latitude, double longitude)
        {
            var reports = _reportService.GetDashboardHealthRiskEventReportsQuery(filters);

            return await reports
                .Where(r => r.Location.X == longitude && r.Location.Y == latitude)
                .Select(r => new
                {
                    r.ProjectHealthRisk.HealthRiskId,
                    HealthRiskName = r.ProjectHealthRisk.HealthRisk.LanguageContents
                        .Where(lc => lc.ContentLanguage.Id == r.ProjectHealthRisk.Project.NationalSociety.ContentLanguage.Id)
                        .Select(lc => lc.Name).FirstOrDefault(),
                    Total = r.ReportedCaseCount
                })
                .Where(r => r.Total > 0)
                .GroupBy(r => r.HealthRiskId)
                .Select(grouping => new ReportsSummaryHealthRiskResponseDto
                {
                    Name = _nyssContext.HealthRiskLanguageContents.Where(f => f.HealthRisk.Id == grouping.Key).Select(s => s.Name).FirstOrDefault(),
                    Value = grouping.Sum(r => r.Total)
                })
                .ToListAsync();
        }
    }
}
