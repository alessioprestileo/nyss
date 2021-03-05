﻿using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Web.Features.Projects.Dto
{
    public class ProjectHealthRiskResponseDto
    {
        public int? Id { get; set; }

        public int HealthRiskId { get; set; }

        public HealthRiskType HealthRiskType { get; set; }

        public int HealthRiskCode { get; set; }

        public string HealthRiskName { get; set; }

        public int? AlertRuleCountThreshold { get; set; }

        public int? AlertRuleDaysThreshold { get; set; }

        public int? AlertRuleKilometersThreshold { get; set; }

        public string FeedbackMessage { get; set; }

        public string CaseDefinition { get; set; }

        public bool ContainsReports { get; set; }
    }
}
