﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable.NSubstitute;
using NSubstitute;
using RX.Nyss.Data;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.NationalSocietyStructure;
using Shouldly;
using Xunit;

namespace RX.Nyss.Web.Tests.Features.NationalSocietyStructure
{
    public class NationalSocietyStructureServiceTests
    {
        private const int NationalSocietyId = 1;
        private readonly NationalSocietyStructureService _nationalSocietyStructureService;

        public NationalSocietyStructureServiceTests()
        {
            var nyssContext = Substitute.For<INyssContext>();

            var region = new Region
            {
                Id = 1,
                Name = "Region",
                Districts = new List<District>(),
                NationalSociety = new NationalSociety { Id = NationalSocietyId }
            };

            var district = new District
            {
                Id = 1,
                Name = "District",
                Region = region,
                Villages = new List<Village>()
            };

            var village = new Village
            {
                Id = 1,
                Name = "Village",
                District = district,
                Zones = new List<Zone>()
            };

            var zone = new Zone
            {
                Id = 1,
                Name = "Zone",
                Village = village
            };

            region.Districts.Add(district);
            district.Villages.Add(village);
            village.Zones.Add(zone);

            var regions = new List<Region> { region };

            var regionsDbSet = regions.AsQueryable().BuildMockDbSet();
            var districtsDbSet = region.Districts.AsQueryable().BuildMockDbSet();
            var villagesDbSet = district.Villages.AsQueryable().BuildMockDbSet();
            var zonesDbSet = village.Zones.AsQueryable().BuildMockDbSet();

            nyssContext.Regions.Returns(regionsDbSet);
            nyssContext.Districts.Returns(districtsDbSet);
            nyssContext.Villages.Returns(villagesDbSet);
            nyssContext.Zones.Returns(zonesDbSet);

            _nationalSocietyStructureService = new NationalSocietyStructureService(nyssContext);
        }

        [Fact]
        public async Task GetStructure_ShouldBuildTree()
        {
            var structure = await _nationalSocietyStructureService.Get(NationalSocietyId);

            structure.Regions.Count().ShouldBe(1);
            structure.Regions.First().Name.ShouldBe("Region");
            structure.Regions.First().Districts.Count().ShouldBe(1);
            structure.Regions.First().Districts.First().Name.ShouldBe("District");

            structure.Regions.First().Districts.First().Villages.Count().ShouldBe(1);
            structure.Regions.First().Districts.First().Villages.First().Name.ShouldBe("Village");

            structure.Regions.First().Districts.First().Villages.First().Zones.Count().ShouldBe(1);
            structure.Regions.First().Districts.First().Villages.First().Zones.First().Name.ShouldBe("Zone");
        }
    }
}
