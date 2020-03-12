﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.Reports.Dto;
using RX.Nyss.Web.Utils;
using RX.Nyss.Web.Utils.DataContract;

namespace RX.Nyss.Web.Features.Reports
{
    [Route("api/report")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Gets a report.
        /// </summary>
        /// <param name="reportId">An identifier of a report</param>
        /// <returns>A report</returns>
        [HttpGet("{reportId:int}/get")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager), NeedsPolicy(Policy.ReportAccess)]
        public Task<Result<ReportResponseDto>> Get(int reportId) =>
            _reportService.Get(reportId);

        /// <summary>
        /// Gets a list of reports in a project.
        /// </summary>
        [HttpPost("list")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<Result<PaginatedList<ReportListResponseDto>>> List(int projectId, int pageNumber, [FromBody] ReportListFilterRequestDto filterRequest) =>
            await _reportService.List(projectId, pageNumber, filterRequest);

        /// <summary>
        /// Gets filters data for the project report list.
        /// </summary>
        /// <param name="projectId">An identifier of a project</param>
        [HttpGet("filters")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<Result<ReportListFilterResponseDto>> GetFilters(int projectId) =>
            await _reportService.GetFilters(projectId);

        /// <summary>
        /// Export the list of reports in a project to a csv file.
        /// </summary>
        /// <param name="projectId">The ID of the project to export the reports from</param>
        /// <param name="filterRequest">The filters object</param>
        [HttpPost("exportToCsv")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<IActionResult> ExportToCsv(int projectId, [FromBody] ReportListFilterRequestDto filterRequest)
        {
            var excelSheetBytes = await _reportService.Export(projectId, filterRequest);
            return File(excelSheetBytes, "text/csv");
        }

        /// <summary>
        /// Export the list of reports in a project to a xlsx file.
        /// </summary>
        /// <param name="projectId">The ID of the project to export the reports from</param>
        /// <param name="filterRequest">The filters object</param>
        [HttpPost("exportToExcel")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<IActionResult> ExportToExcel(int projectId, [FromBody] ReportListFilterRequestDto filterRequest)
        {
            var excelSheetBytes = await _reportService.Export(projectId, filterRequest, useExcelFormat: true);
            return File(excelSheetBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Mark the selected report as error.
        /// </summary>
        /// <param name="reportId">The ID of the report to be marked as error</param>
        [HttpPost("{reportId:int}/markAsError")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ReportAccess)]
        public async Task<Result> MarkAsError(int reportId) =>
            await _reportService.MarkAsError(reportId);

        /// <summary>
        /// Gets human health risks for the project.
        /// </summary>
        /// <param name="projectId">An identifier of a project</param>
        [HttpGet("humanHealthRisksForProject/{projectId:int}/get")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<Result<HumanHealthRiskResponseDto>> GetHumanHealthRisksForProject(int projectId) =>
            await _reportService.GetHumanHealthRisksForProject(projectId);

        /// <summary>
        /// Edits a report.
        /// </summary>
        /// <param name="reportId">An identifier of a report</param>
        /// <param name="reportRequestDto">A report</param>
        [HttpPost("{reportId:int}/edit")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager), NeedsPolicy(Policy.ReportAccess)]
        public async Task<Result> Edit(int reportId, [FromBody] ReportRequestDto reportRequestDto) =>
            await _reportService.Edit(reportId, reportRequestDto);
    }
}
