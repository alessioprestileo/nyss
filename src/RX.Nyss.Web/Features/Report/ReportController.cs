﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Common;
using RX.Nyss.Web.Features.Report.Dto;
using RX.Nyss.Web.Utils;
using RX.Nyss.Web.Utils.DataContract;

namespace RX.Nyss.Web.Features.Report
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
        /// Gets a list of reports in a project.
        /// </summary>
        [HttpPost("list")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<Result<PaginatedList<ReportListResponseDto>>> List(int projectId, int pageNumber, [FromBody] ReportListFilterRequestDto filterRequest) =>
            await _reportService.List(projectId, pageNumber, filterRequest);

        /// <summary>
        /// Export the list of reports in a project to a csv file
        /// </summary>
        /// <param name="projectId">The ID of the project to export the reports from</param>
        /// <param name="filterRequest">The filters object</param>
        [HttpPost("exportToExcel")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ProjectAccess)]
        public async Task<IActionResult> Export(int projectId, [FromBody] ReportListFilterRequestDto filterRequest)
        {
            var excelSheetBytes = await _reportService.Export(projectId, filterRequest);
            return File(excelSheetBytes, "text/csv");
        }

        /// <summary>
        /// Mark the selected report as error
        /// </summary>
        /// <param name="reportId">The ID of the report to be marked as error</param>
        [HttpPost("{reportId}/markAsError")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ReportAccess)]
        public async Task<Result> MarkAsError(int reportId) =>
            await _reportService.MarkAsError(reportId);

        /// <summary>
        /// Un-mark the selected report as error
        /// </summary>
        /// <param name="reportId">The ID of the report to be remove the error flag from</param>
        [HttpPost("{reportId}/unmarkAsError")]
        [NeedsRole(Role.Administrator, Role.TechnicalAdvisor, Role.Manager, Role.Supervisor), NeedsPolicy(Policy.ReportAccess)]
        public async Task<Result> UnmarkAsError(int reportId) =>
            await _reportService.UnmarkAsError(reportId);
    }
}
