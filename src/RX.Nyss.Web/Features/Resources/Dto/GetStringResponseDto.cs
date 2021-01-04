﻿using System.Collections.Generic;

namespace RX.Nyss.Web.Features.Resources.Dto
{
    public class GetStringResponseDto
    {
        public string Key { get; set; }

        public bool NeedsImprovement { get; set; }

        public IEnumerable<GetEntryDto> Translations { get; set; }

        public class GetEntryDto
        {
            public string LanguageCode { get; set; }

            public string Value { get; set; }

            public string Name { get; set; }
        }
    }
}
