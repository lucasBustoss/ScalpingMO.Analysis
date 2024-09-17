﻿using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response.Teams;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Models.Fixture
{
    public class FixtureTeam
    {
        public FixtureTeam()
        {
            
        }

        public FixtureTeam(FixtureTeamDataResponse team)
        {
            Id = team.Id;
            Name = team.Name;
            Logo = team.Logo;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
    }
}
