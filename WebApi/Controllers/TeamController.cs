// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly DataContext Context;

        public TeamController(DataContext context)
        {
            Context = context;
        }

        [HttpPost("process")]
        public async Task<ActionResult<List<Player>>> Process(List<TeamProcessViewModel> teamProcessModel)
        {
            List<Player> playersAvailable;
            List<PlayerSkill> filteredPlayersSkills;
            Player desiredPlayer;
            int maximumSkillValueFound;
            Dictionary<string, string> skillsAndPositionsUsed = new Dictionary<string, string>();
            var players = new List<Player>();
            var lastPosition = "";

            foreach (var player in teamProcessModel)
            {
                if (!lastPosition.Equals(player.Position))
                {
                    skillsAndPositionsUsed.Clear();
                    lastPosition = player.Position;
                }

                if (skillsAndPositionsUsed.ContainsKey(player.MainSkill) && skillsAndPositionsUsed.ContainsValue(player.Position))
                    return BadRequest($"The skill: {player.MainSkill} cannot be required two times for the same position: {player.Position}");

                skillsAndPositionsUsed.Add(player.MainSkill, player.Position);

                for (var i = 0; i < player.NumberOfPlayers; i++)
                {
                    var playersPerPosition = await Context.Players.Include(p => p.PlayerSkills).Where(p => p.Position == player.Position).ToListAsync();
                    playersAvailable = playersPerPosition.Where(p => players.All(p2 => p2.Id != p.Id)).ToList();

                    if (!playersAvailable.Any())
                        return BadRequest($"Insufficient number of players for position: {player.Position}");

                    var playersWithTheSkillRequired = playersAvailable.Where(pp => pp.PlayerSkills.Any(ps => ps.Skill == player.MainSkill));

                    if (!playersWithTheSkillRequired.Any())
                    {
                        filteredPlayersSkills = playersAvailable.SelectMany(p => p.PlayerSkills).ToList();
                        maximumSkillValueFound = filteredPlayersSkills.Max(p => p.Value);
                        desiredPlayer = playersAvailable.Where(p => p.Id == (filteredPlayersSkills.Where(t => t.Value == maximumSkillValueFound).FirstOrDefault()).PlayerId).FirstOrDefault();

                        if (!players.Contains(desiredPlayer))
                            players.Add(desiredPlayer);
                    }
                    else
                    {
                        filteredPlayersSkills = playersWithTheSkillRequired.SelectMany(p => p.PlayerSkills).ToList();
                        maximumSkillValueFound = filteredPlayersSkills.Where(t => t.Skill == player.MainSkill).Max(p => p.Value);
                        desiredPlayer = playersWithTheSkillRequired.Where(p => p.Id == (filteredPlayersSkills.Where(t => t.Skill == player.MainSkill && t.Value == maximumSkillValueFound).FirstOrDefault()).PlayerId).FirstOrDefault();

                        if (!players.Contains(desiredPlayer))
                            players.Add(desiredPlayer);
                    }
                }
            }

            return Ok(players);
        }
    }
}
