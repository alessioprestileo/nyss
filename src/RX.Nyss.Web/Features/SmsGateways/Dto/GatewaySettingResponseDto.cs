﻿using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Web.Features.SmsGateways.Dto
{
    public class GatewaySettingResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string EmailAddress { get; set; }
        public GatewayType GatewayType { get; set; }
        public string IotHubDeviceName { get; set; }
    }
}
