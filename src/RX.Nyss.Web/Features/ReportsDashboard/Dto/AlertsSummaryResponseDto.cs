﻿namespace RX.Nyss.Web.Features.ReportsDashboard.Dto
{
    public class AlertsSummaryResponseDto
    {
        public int Escalated { get; set; }

        public int Dismissed { get; set; }

        public int Closed { get; set; }
    }
}
