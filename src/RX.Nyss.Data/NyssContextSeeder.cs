﻿using Microsoft.EntityFrameworkCore;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;

namespace RX.Nyss.Data
{
    public static class NyssContextSeeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            SeedContentLanguages(modelBuilder);
            SeedCountries(modelBuilder);
            SeedApplicationLanguages(modelBuilder);
            SeedAdministrator(modelBuilder);
        }

        private static void SeedApplicationLanguages(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<ApplicationLanguage>().HasData(
                new ApplicationLanguage
                {
                    Id = 1,
                    LanguageCode = "en",
                    DisplayName = "English"
                },
                new ApplicationLanguage
                {
                    Id = 2,
                    LanguageCode = "fr",
                    DisplayName = "Français"
                },
                new ApplicationLanguage
                {
                    Id = 3,
                    LanguageCode = "es",
                    DisplayName = "Español"
                });

        private static void SeedAdministrator(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<AdministratorUser>().HasData(new
            {
                Id = 1,
                IdentityUserId = "9c1071c1-fa69-432a-9cd0-2c4baa703a67",
                Name = "Administrator",
                Role = Role.Administrator,
                EmailAddress = "admin@domain.com",
                PhoneNumber = "",
                IsFirstLogin = false,
                ApplicationLanguageId = 1
            });

        private static void SeedCountries(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    CountryCode = "AX",
                    Name = "Åland Islands"
                },
                new Country
                {
                    Id = 154,
                    CountryCode = "AN",
                    Name = "Netherlands Antilles"
                },
                new Country
                {
                    Id = 155,
                    CountryCode = "NC",
                    Name = "New Caledonia"
                },
                new Country
                {
                    Id = 156,
                    CountryCode = "NZ",
                    Name = "New Zealand"
                },
                new Country
                {
                    Id = 157,
                    CountryCode = "NI",
                    Name = "Nicaragua"
                },
                new Country
                {
                    Id = 158,
                    CountryCode = "NE",
                    Name = "Niger"
                },
                new Country
                {
                    Id = 159,
                    CountryCode = "NG",
                    Name = "Nigeria"
                },
                new Country
                {
                    Id = 160,
                    CountryCode = "NU",
                    Name = "Niue"
                },
                new Country
                {
                    Id = 161,
                    CountryCode = "NF",
                    Name = "Norfolk Island"
                },
                new Country
                {
                    Id = 162,
                    CountryCode = "MP",
                    Name = "Northern Mariana Islands"
                },
                new Country
                {
                    Id = 163,
                    CountryCode = "NO",
                    Name = "Norway"
                },
                new Country
                {
                    Id = 164,
                    CountryCode = "OM",
                    Name = "Oman"
                },
                new Country
                {
                    Id = 165,
                    CountryCode = "PK",
                    Name = "Pakistan"
                },
                new Country
                {
                    Id = 153,
                    CountryCode = "NL",
                    Name = "Netherlands"
                },
                new Country
                {
                    Id = 166,
                    CountryCode = "PW",
                    Name = "Palau"
                },
                new Country
                {
                    Id = 168,
                    CountryCode = "PA",
                    Name = "Panama"
                },
                new Country
                {
                    Id = 169,
                    CountryCode = "PG",
                    Name = "Papua New Guinea"
                },
                new Country
                {
                    Id = 170,
                    CountryCode = "PY",
                    Name = "Paraguay"
                },
                new Country
                {
                    Id = 171,
                    CountryCode = "PE",
                    Name = "Peru"
                },
                new Country
                {
                    Id = 172,
                    CountryCode = "PH",
                    Name = "Philippines"
                },
                new Country
                {
                    Id = 173,
                    CountryCode = "PN",
                    Name = "Pitcairn"
                },
                new Country
                {
                    Id = 174,
                    CountryCode = "PL",
                    Name = "Poland"
                },
                new Country
                {
                    Id = 175,
                    CountryCode = "PT",
                    Name = "Portugal"
                },
                new Country
                {
                    Id = 176,
                    CountryCode = "PR",
                    Name = "Puerto Rico"
                },
                new Country
                {
                    Id = 177,
                    CountryCode = "QA",
                    Name = "Qatar"
                },
                new Country
                {
                    Id = 178,
                    CountryCode = "RE",
                    Name = "Reunion"
                },
                new Country
                {
                    Id = 179,
                    CountryCode = "RO",
                    Name = "Romania"
                },
                new Country
                {
                    Id = 167,
                    CountryCode = "PS",
                    Name = "Palestinian Territory, Occupied"
                },
                new Country
                {
                    Id = 180,
                    CountryCode = "RU",
                    Name = "Russian Federation"
                },
                new Country
                {
                    Id = 152,
                    CountryCode = "NP",
                    Name = "Nepal"
                },
                new Country
                {
                    Id = 150,
                    CountryCode = "NA",
                    Name = "Namibia"
                },
                new Country
                {
                    Id = 124,
                    CountryCode = "LY",
                    Name = "Libyan Arab Jamahiriya"
                },
                new Country
                {
                    Id = 125,
                    CountryCode = "LI",
                    Name = "Liechtenstein"
                },
                new Country
                {
                    Id = 126,
                    CountryCode = "LT",
                    Name = "Lithuania"
                },
                new Country
                {
                    Id = 127,
                    CountryCode = "LU",
                    Name = "Luxembourg"
                },
                new Country
                {
                    Id = 128,
                    CountryCode = "MO",
                    Name = "Macao"
                },
                new Country
                {
                    Id = 129,
                    CountryCode = "MK",
                    Name = "Macedonia, The Former Yugoslav Republic of"
                },
                new Country
                {
                    Id = 130,
                    CountryCode = "MG",
                    Name = "Madagascar"
                },
                new Country
                {
                    Id = 131,
                    CountryCode = "MW",
                    Name = "Malawi"
                },
                new Country
                {
                    Id = 132,
                    CountryCode = "MY",
                    Name = "Malaysia"
                },
                new Country
                {
                    Id = 133,
                    CountryCode = "MV",
                    Name = "Maldives"
                },
                new Country
                {
                    Id = 134,
                    CountryCode = "ML",
                    Name = "Mali"
                },
                new Country
                {
                    Id = 135,
                    CountryCode = "MT",
                    Name = "Malta"
                },
                new Country
                {
                    Id = 151,
                    CountryCode = "NR",
                    Name = "Nauru"
                },
                new Country
                {
                    Id = 136,
                    CountryCode = "MH",
                    Name = "Marshall Islands"
                },
                new Country
                {
                    Id = 138,
                    CountryCode = "MR",
                    Name = "Mauritania"
                },
                new Country
                {
                    Id = 139,
                    CountryCode = "MU",
                    Name = "Mauritius"
                },
                new Country
                {
                    Id = 140,
                    CountryCode = "YT",
                    Name = "Mayotte"
                },
                new Country
                {
                    Id = 141,
                    CountryCode = "MX",
                    Name = "Mexico"
                },
                new Country
                {
                    Id = 142,
                    CountryCode = "FM",
                    Name = "Micronesia, Federated States of"
                },
                new Country
                {
                    Id = 143,
                    CountryCode = "MD",
                    Name = "Moldova, Republic of"
                },
                new Country
                {
                    Id = 144,
                    CountryCode = "MC",
                    Name = "Monaco"
                },
                new Country
                {
                    Id = 145,
                    CountryCode = "MN",
                    Name = "Mongolia"
                },
                new Country
                {
                    Id = 146,
                    CountryCode = "MS",
                    Name = "Montserrat"
                },
                new Country
                {
                    Id = 147,
                    CountryCode = "MA",
                    Name = "Morocco"
                },
                new Country
                {
                    Id = 148,
                    CountryCode = "MZ",
                    Name = "Mozambique"
                },
                new Country
                {
                    Id = 149,
                    CountryCode = "MM",
                    Name = "Myanmar"
                },
                new Country
                {
                    Id = 137,
                    CountryCode = "MQ",
                    Name = "Martinique"
                },
                new Country
                {
                    Id = 181,
                    CountryCode = "RW",
                    Name = "RWANDA"
                },
                new Country
                {
                    Id = 182,
                    CountryCode = "SH",
                    Name = "Saint Helena"
                },
                new Country
                {
                    Id = 183,
                    CountryCode = "KN",
                    Name = "Saint Kitts and Nevis"
                },
                new Country
                {
                    Id = 215,
                    CountryCode = "TL",
                    Name = "Timor-Leste"
                },
                new Country
                {
                    Id = 216,
                    CountryCode = "TG",
                    Name = "Togo"
                },
                new Country
                {
                    Id = 217,
                    CountryCode = "TK",
                    Name = "Tokelau"
                },
                new Country
                {
                    Id = 218,
                    CountryCode = "TO",
                    Name = "Tonga"
                },
                new Country
                {
                    Id = 219,
                    CountryCode = "TT",
                    Name = "Trinidad and Tobago"
                },
                new Country
                {
                    Id = 220,
                    CountryCode = "TN",
                    Name = "Tunisia"
                },
                new Country
                {
                    Id = 221,
                    CountryCode = "TR",
                    Name = "Turkey"
                },
                new Country
                {
                    Id = 222,
                    CountryCode = "TM",
                    Name = "Turkmenistan"
                },
                new Country
                {
                    Id = 223,
                    CountryCode = "TC",
                    Name = "Turks and Caicos Islands"
                },
                new Country
                {
                    Id = 224,
                    CountryCode = "TV",
                    Name = "Tuvalu"
                },
                new Country
                {
                    Id = 225,
                    CountryCode = "UG",
                    Name = "Uganda"
                },
                new Country
                {
                    Id = 226,
                    CountryCode = "UA",
                    Name = "Ukraine"
                },
                new Country
                {
                    Id = 214,
                    CountryCode = "TH",
                    Name = "Thailand"
                },
                new Country
                {
                    Id = 227,
                    CountryCode = "AE",
                    Name = "United Arab Emirates"
                },
                new Country
                {
                    Id = 229,
                    CountryCode = "US",
                    Name = "United States"
                },
                new Country
                {
                    Id = 230,
                    CountryCode = "UM",
                    Name = "United States Minor Outlying Islands"
                },
                new Country
                {
                    Id = 231,
                    CountryCode = "UY",
                    Name = "Uruguay"
                },
                new Country
                {
                    Id = 232,
                    CountryCode = "UZ",
                    Name = "Uzbekistan"
                },
                new Country
                {
                    Id = 233,
                    CountryCode = "VU",
                    Name = "Vanuatu"
                },
                new Country
                {
                    Id = 234,
                    CountryCode = "VE",
                    Name = "Venezuela"
                },
                new Country
                {
                    Id = 235,
                    CountryCode = "VN",
                    Name = "Viet Nam"
                },
                new Country
                {
                    Id = 236,
                    CountryCode = "VG",
                    Name = "Virgin Islands, British"
                },
                new Country
                {
                    Id = 237,
                    CountryCode = "VI",
                    Name = "Virgin Islands, U.S."
                },
                new Country
                {
                    Id = 238,
                    CountryCode = "WF",
                    Name = "Wallis and Futuna"
                },
                new Country
                {
                    Id = 239,
                    CountryCode = "EH",
                    Name = "Western Sahara"
                },
                new Country
                {
                    Id = 240,
                    CountryCode = "YE",
                    Name = "Yemen"
                },
                new Country
                {
                    Id = 228,
                    CountryCode = "GB",
                    Name = "United Kingdom"
                },
                new Country
                {
                    Id = 213,
                    CountryCode = "TZ",
                    Name = "Tanzania, United Republic of"
                },
                new Country
                {
                    Id = 212,
                    CountryCode = "TJ",
                    Name = "Tajikistan"
                },
                new Country
                {
                    Id = 211,
                    CountryCode = "TW",
                    Name = "Taiwan, Province of China"
                },
                new Country
                {
                    Id = 184,
                    CountryCode = "LC",
                    Name = "Saint Lucia"
                },
                new Country
                {
                    Id = 185,
                    CountryCode = "PM",
                    Name = "Saint Pierre and Miquelon"
                },
                new Country
                {
                    Id = 186,
                    CountryCode = "VC",
                    Name = "Saint Vincent and the Grenadines"
                },
                new Country
                {
                    Id = 187,
                    CountryCode = "WS",
                    Name = "Samoa"
                },
                new Country
                {
                    Id = 188,
                    CountryCode = "SM",
                    Name = "San Marino"
                },
                new Country
                {
                    Id = 189,
                    CountryCode = "ST",
                    Name = "Sao Tome and Principe"
                },
                new Country
                {
                    Id = 190,
                    CountryCode = "SA",
                    Name = "Saudi Arabia"
                },
                new Country
                {
                    Id = 191,
                    CountryCode = "SN",
                    Name = "Senegal"
                },
                new Country
                {
                    Id = 192,
                    CountryCode = "CS",
                    Name = "Serbia and Montenegro"
                },
                new Country
                {
                    Id = 193,
                    CountryCode = "SC",
                    Name = "Seychelles"
                },
                new Country
                {
                    Id = 194,
                    CountryCode = "SL",
                    Name = "Sierra Leone"
                },
                new Country
                {
                    Id = 195,
                    CountryCode = "SG",
                    Name = "Singapore"
                },
                new Country
                {
                    Id = 196,
                    CountryCode = "SK",
                    Name = "Slovakia"
                },
                new Country
                {
                    Id = 197,
                    CountryCode = "SI",
                    Name = "Slovenia"
                },
                new Country
                {
                    Id = 198,
                    CountryCode = "SB",
                    Name = "Solomon Islands"
                },
                new Country
                {
                    Id = 199,
                    CountryCode = "SO",
                    Name = "Somalia"
                },
                new Country
                {
                    Id = 200,
                    CountryCode = "ZA",
                    Name = "South Africa"
                },
                new Country
                {
                    Id = 201,
                    CountryCode = "GS",
                    Name = "South Georgia and the South Sandwich Islands"
                },
                new Country
                {
                    Id = 202,
                    CountryCode = "ES",
                    Name = "Spain"
                },
                new Country
                {
                    Id = 203,
                    CountryCode = "LK",
                    Name = "Sri Lanka"
                },
                new Country
                {
                    Id = 204,
                    CountryCode = "SD",
                    Name = "Sudan"
                },
                new Country
                {
                    Id = 205,
                    CountryCode = "SR",
                    Name = "Suriname"
                },
                new Country
                {
                    Id = 206,
                    CountryCode = "SJ",
                    Name = "Svalbard and Jan Mayen"
                },
                new Country
                {
                    Id = 207,
                    CountryCode = "SZ",
                    Name = "Swaziland"
                },
                new Country
                {
                    Id = 208,
                    CountryCode = "SE",
                    Name = "Sweden"
                },
                new Country
                {
                    Id = 209,
                    CountryCode = "CH",
                    Name = "Switzerland"
                },
                new Country
                {
                    Id = 210,
                    CountryCode = "SY",
                    Name = "Syrian Arab Republic"
                },
                new Country
                {
                    Id = 123,
                    CountryCode = "LR",
                    Name = "Liberia"
                },
                new Country
                {
                    Id = 122,
                    CountryCode = "LS",
                    Name = "Lesotho"
                },
                new Country
                {
                    Id = 121,
                    CountryCode = "LB",
                    Name = "Lebanon"
                },
                new Country
                {
                    Id = 120,
                    CountryCode = "LV",
                    Name = "Latvia"
                },
                new Country
                {
                    Id = 33,
                    CountryCode = "BG",
                    Name = "Bulgaria"
                },
                new Country
                {
                    Id = 34,
                    CountryCode = "BF",
                    Name = "Burkina Faso"
                },
                new Country
                {
                    Id = 35,
                    CountryCode = "BI",
                    Name = "Burundi"
                },
                new Country
                {
                    Id = 36,
                    CountryCode = "KH",
                    Name = "Cambodia"
                },
                new Country
                {
                    Id = 37,
                    CountryCode = "CM",
                    Name = "Cameroon"
                },
                new Country
                {
                    Id = 38,
                    CountryCode = "CA",
                    Name = "Canada"
                },
                new Country
                {
                    Id = 39,
                    CountryCode = "CV",
                    Name = "Cape Verde"
                },
                new Country
                {
                    Id = 40,
                    CountryCode = "KY",
                    Name = "Cayman Islands"
                },
                new Country
                {
                    Id = 41,
                    CountryCode = "CF",
                    Name = "Central African Republic"
                },
                new Country
                {
                    Id = 42,
                    CountryCode = "TD",
                    Name = "Chad"
                },
                new Country
                {
                    Id = 43,
                    CountryCode = "CL",
                    Name = "Chile"
                },
                new Country
                {
                    Id = 44,
                    CountryCode = "CN",
                    Name = "China"
                },
                new Country
                {
                    Id = 32,
                    CountryCode = "BN",
                    Name = "Brunei Darussalam"
                },
                new Country
                {
                    Id = 45,
                    CountryCode = "CX",
                    Name = "Christmas Island"
                },
                new Country
                {
                    Id = 47,
                    CountryCode = "CO",
                    Name = "Colombia"
                },
                new Country
                {
                    Id = 48,
                    CountryCode = "KM",
                    Name = "Comoros"
                },
                new Country
                {
                    Id = 49,
                    CountryCode = "CG",
                    Name = "Congo"
                },
                new Country
                {
                    Id = 50,
                    CountryCode = "CD",
                    Name = "Congo, The Democratic Republic of the"
                },
                new Country
                {
                    Id = 51,
                    CountryCode = "CK",
                    Name = "Cook Islands"
                },
                new Country
                {
                    Id = 52,
                    CountryCode = "CR",
                    Name = "Costa Rica"
                },
                new Country
                {
                    Id = 53,
                    CountryCode = "CI",
                    Name = "Cote D\"Ivoire"
                },
                new Country
                {
                    Id = 54,
                    CountryCode = "HR",
                    Name = "Croatia"
                },
                new Country
                {
                    Id = 55,
                    CountryCode = "CU",
                    Name = "Cuba"
                },
                new Country
                {
                    Id = 56,
                    CountryCode = "CY",
                    Name = "Cyprus"
                },
                new Country
                {
                    Id = 57,
                    CountryCode = "CZ",
                    Name = "Czech Republic"
                },
                new Country
                {
                    Id = 58,
                    CountryCode = "DK",
                    Name = "Denmark"
                },
                new Country
                {
                    Id = 46,
                    CountryCode = "CC",
                    Name = "Cocos (Keeling) Islands"
                },
                new Country
                {
                    Id = 31,
                    CountryCode = "IO",
                    Name = "British Indian Ocean Territory"
                },
                new Country
                {
                    Id = 30,
                    CountryCode = "BR",
                    Name = "Brazil"
                },
                new Country
                {
                    Id = 29,
                    CountryCode = "BV",
                    Name = "Bouvet Island"
                },
                new Country
                {
                    Id = 2,
                    CountryCode = "AL",
                    Name = "Albania"
                },
                new Country
                {
                    Id = 3,
                    CountryCode = "DZ",
                    Name = "Algeria"
                },
                new Country
                {
                    Id = 4,
                    CountryCode = "AS",
                    Name = "American Samoa"
                },
                new Country
                {
                    Id = 5,
                    CountryCode = "AD",
                    Name = "Andorra"
                },
                new Country
                {
                    Id = 6,
                    CountryCode = "AO",
                    Name = "Angola"
                },
                new Country
                {
                    Id = 7,
                    CountryCode = "AI",
                    Name = "Anguilla"
                },
                new Country
                {
                    Id = 8,
                    CountryCode = "AQ",
                    Name = "Antarctica"
                },
                new Country
                {
                    Id = 9,
                    CountryCode = "AG",
                    Name = "Antigua and Barbuda"
                },
                new Country
                {
                    Id = 10,
                    CountryCode = "AR",
                    Name = "Argentina"
                },
                new Country
                {
                    Id = 11,
                    CountryCode = "AM",
                    Name = "Armenia"
                },
                new Country
                {
                    Id = 12,
                    CountryCode = "AW",
                    Name = "Aruba"
                },
                new Country
                {
                    Id = 13,
                    CountryCode = "AU",
                    Name = "Australia"
                },
                new Country
                {
                    Id = 14,
                    CountryCode = "AT",
                    Name = "Austria"
                },
                new Country
                {
                    Id = 15,
                    CountryCode = "AZ",
                    Name = "Azerbaijan"
                },
                new Country
                {
                    Id = 16,
                    CountryCode = "BS",
                    Name = "Bahamas"
                },
                new Country
                {
                    Id = 17,
                    CountryCode = "BH",
                    Name = "Bahrain"
                },
                new Country
                {
                    Id = 18,
                    CountryCode = "BD",
                    Name = "Bangladesh"
                },
                new Country
                {
                    Id = 19,
                    CountryCode = "BB",
                    Name = "Barbados"
                },
                new Country
                {
                    Id = 20,
                    CountryCode = "BY",
                    Name = "Belarus"
                },
                new Country
                {
                    Id = 21,
                    CountryCode = "BE",
                    Name = "Belgium"
                },
                new Country
                {
                    Id = 22,
                    CountryCode = "BZ",
                    Name = "Belize"
                },
                new Country
                {
                    Id = 23,
                    CountryCode = "BJ",
                    Name = "Benin"
                },
                new Country
                {
                    Id = 24,
                    CountryCode = "BM",
                    Name = "Bermuda"
                },
                new Country
                {
                    Id = 25,
                    CountryCode = "BT",
                    Name = "Bhutan"
                },
                new Country
                {
                    Id = 26,
                    CountryCode = "BO",
                    Name = "Bolivia"
                },
                new Country
                {
                    Id = 27,
                    CountryCode = "BA",
                    Name = "Bosnia and Herzegovina"
                },
                new Country
                {
                    Id = 28,
                    CountryCode = "BW",
                    Name = "Botswana"
                },
                new Country
                {
                    Id = 59,
                    CountryCode = "DJ",
                    Name = "Djibouti"
                },
                new Country
                {
                    Id = 241,
                    CountryCode = "ZM",
                    Name = "Zambia"
                },
                new Country
                {
                    Id = 60,
                    CountryCode = "DM",
                    Name = "Dominica"
                },
                new Country
                {
                    Id = 62,
                    CountryCode = "EC",
                    Name = "Ecuador"
                },
                new Country
                {
                    Id = 94,
                    CountryCode = "HM",
                    Name = "Heard Island and Mcdonald Islands"
                },
                new Country
                {
                    Id = 95,
                    CountryCode = "VA",
                    Name = "Holy See (Vatican City State)"
                },
                new Country
                {
                    Id = 96,
                    CountryCode = "HN",
                    Name = "Honduras"
                },
                new Country
                {
                    Id = 97,
                    CountryCode = "HK",
                    Name = "Hong Kong"
                },
                new Country
                {
                    Id = 98,
                    CountryCode = "HU",
                    Name = "Hungary"
                },
                new Country
                {
                    Id = 99,
                    CountryCode = "IS",
                    Name = "Iceland"
                },
                new Country
                {
                    Id = 100,
                    CountryCode = "IN",
                    Name = "India"
                },
                new Country
                {
                    Id = 101,
                    CountryCode = "ID",
                    Name = "Indonesia"
                },
                new Country
                {
                    Id = 102,
                    CountryCode = "IR",
                    Name = "Iran, Islamic Republic Of"
                },
                new Country
                {
                    Id = 103,
                    CountryCode = "IQ",
                    Name = "Iraq"
                },
                new Country
                {
                    Id = 104,
                    CountryCode = "IE",
                    Name = "Ireland"
                },
                new Country
                {
                    Id = 105,
                    CountryCode = "IM",
                    Name = "Isle of Man"
                },
                new Country
                {
                    Id = 93,
                    CountryCode = "HT",
                    Name = "Haiti"
                },
                new Country
                {
                    Id = 106,
                    CountryCode = "IL",
                    Name = "Israel"
                },
                new Country
                {
                    Id = 108,
                    CountryCode = "JM",
                    Name = "Jamaica"
                },
                new Country
                {
                    Id = 109,
                    CountryCode = "JP",
                    Name = "Japan"
                },
                new Country
                {
                    Id = 110,
                    CountryCode = "JE",
                    Name = "Jersey"
                },
                new Country
                {
                    Id = 111,
                    CountryCode = "JO",
                    Name = "Jordan"
                },
                new Country
                {
                    Id = 112,
                    CountryCode = "KZ",
                    Name = "Kazakhstan"
                },
                new Country
                {
                    Id = 113,
                    CountryCode = "KE",
                    Name = "Kenya"
                },
                new Country
                {
                    Id = 114,
                    CountryCode = "KI",
                    Name = "Kiribati"
                },
                new Country
                {
                    Id = 115,
                    CountryCode = "KP",
                    Name = "Korea, Democratic People\"S Republic of"
                },
                new Country
                {
                    Id = 116,
                    CountryCode = "KR",
                    Name = "Korea, Republic of"
                },
                new Country
                {
                    Id = 117,
                    CountryCode = "KW",
                    Name = "Kuwait"
                },
                new Country
                {
                    Id = 118,
                    CountryCode = "KG",
                    Name = "Kyrgyzstan"
                },
                new Country
                {
                    Id = 119,
                    CountryCode = "LA",
                    Name = "Lao People\"S Democratic Republic"
                },
                new Country
                {
                    Id = 107,
                    CountryCode = "IT",
                    Name = "Italy"
                },
                new Country
                {
                    Id = 92,
                    CountryCode = "GY",
                    Name = "Guyana"
                },
                new Country
                {
                    Id = 91,
                    CountryCode = "GW",
                    Name = "Guinea-Bissau"
                },
                new Country
                {
                    Id = 90,
                    CountryCode = "GN",
                    Name = "Guinea"
                },
                new Country
                {
                    Id = 63,
                    CountryCode = "EG",
                    Name = "Egypt"
                },
                new Country
                {
                    Id = 64,
                    CountryCode = "SV",
                    Name = "El Salvador"
                },
                new Country
                {
                    Id = 65,
                    CountryCode = "GQ",
                    Name = "Equatorial Guinea"
                },
                new Country
                {
                    Id = 66,
                    CountryCode = "ER",
                    Name = "Eritrea"
                },
                new Country
                {
                    Id = 67,
                    CountryCode = "EE",
                    Name = "Estonia"
                },
                new Country
                {
                    Id = 68,
                    CountryCode = "ET",
                    Name = "Ethiopia"
                },
                new Country
                {
                    Id = 69,
                    CountryCode = "FK",
                    Name = "Falkland Islands (Malvinas)"
                },
                new Country
                {
                    Id = 70,
                    CountryCode = "FO",
                    Name = "Faroe Islands"
                },
                new Country
                {
                    Id = 71,
                    CountryCode = "FJ",
                    Name = "Fiji"
                },
                new Country
                {
                    Id = 72,
                    CountryCode = "FI",
                    Name = "Finland"
                },
                new Country
                {
                    Id = 73,
                    CountryCode = "FR",
                    Name = "France"
                },
                new Country
                {
                    Id = 74,
                    CountryCode = "GF",
                    Name = "French Guiana"
                },
                new Country
                {
                    Id = 75,
                    CountryCode = "PF",
                    Name = "French Polynesia"
                },
                new Country
                {
                    Id = 76,
                    CountryCode = "TF",
                    Name = "French Southern Territories"
                },
                new Country
                {
                    Id = 77,
                    CountryCode = "GA",
                    Name = "Gabon"
                },
                new Country
                {
                    Id = 78,
                    CountryCode = "GM",
                    Name = "Gambia"
                },
                new Country
                {
                    Id = 79,
                    CountryCode = "GE",
                    Name = "Georgia"
                },
                new Country
                {
                    Id = 80,
                    CountryCode = "DE",
                    Name = "Germany"
                },
                new Country
                {
                    Id = 81,
                    CountryCode = "GH",
                    Name = "Ghana"
                },
                new Country
                {
                    Id = 82,
                    CountryCode = "GI",
                    Name = "Gibraltar"
                },
                new Country
                {
                    Id = 83,
                    CountryCode = "GR",
                    Name = "Greece"
                },
                new Country
                {
                    Id = 84,
                    CountryCode = "GL",
                    Name = "Greenland"
                },
                new Country
                {
                    Id = 85,
                    CountryCode = "GD",
                    Name = "Grenada"
                },
                new Country
                {
                    Id = 86,
                    CountryCode = "GP",
                    Name = "Guadeloupe"
                },
                new Country
                {
                    Id = 87,
                    CountryCode = "GU",
                    Name = "Guam"
                },
                new Country
                {
                    Id = 88,
                    CountryCode = "GT",
                    Name = "Guatemala"
                },
                new Country
                {
                    Id = 89,
                    CountryCode = "GG",
                    Name = "Guernsey"
                },
                new Country
                {
                    Id = 61,
                    CountryCode = "DO",
                    Name = "Dominican Republic"
                },
                new Country
                {
                    Id = 242,
                    CountryCode = "ZW",
                    Name = "Zimbabwe"
                }
            );

        private static void SeedContentLanguages(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<ContentLanguage>().HasData(
                new ContentLanguage
                {
                    Id = 1,
                    DisplayName = "English",
                    LanguageCode = "en"
                },
                new ContentLanguage
                {
                    Id = 2,
                    DisplayName = "Français",
                    LanguageCode = "fr"
                },
                new ContentLanguage
                {
                    Id = 3,

                    DisplayName = "Español",
                    LanguageCode = "es"
                }
            );
    }
}
