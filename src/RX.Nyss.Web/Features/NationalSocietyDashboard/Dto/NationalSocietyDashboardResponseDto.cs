﻿using System.Collections.Generic;
using RX.Nyss.Web.Features.ReportsDashboard.Dto;

namespace RX.Nyss.Web.Features.NationalSocietyDashboard.Dto
{
    public class NationalSocietyDashboardResponseDto
    {
        public NationalSocietySummaryResponseDto Summary { get; set; }

        public IEnumerable<ReportsSummaryMapResponseDto> ReportsGroupedByLocation { get; set; }
    }
}
