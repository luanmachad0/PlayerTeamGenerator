// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq.Expressions;
using WebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly DataContext Context;

    public PlayerController(DataContext context)
    {
        Context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetAll()
    {
        return await Context.Players.Include(p => p.PlayerSkills).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Player>> PostPlayer([FromBody] PlayerViewModel playerModel)
    {
        if (!ValidatePlayerPositionNameHelper.Validate(playerModel.Position))
            return BadRequest(new ErrorResponse($"Invalid value for position: {playerModel.Position}"));

        if (!playerModel.PlayerSkills.Any())
            return BadRequest(new ErrorResponse("The player needs at least one skill"));

        foreach (var skill in playerModel.PlayerSkills)
        {
            if (!ValidateSkillNameHelper.Validate(skill.Skill))
                return BadRequest(new ErrorResponse($"Invalid skill name: {skill.Skill}"));
        }

        Player player = new()
        {
            Name = playerModel.Name,
            Position = playerModel.Position,
            PlayerSkills = playerModel.PlayerSkills.Select(ps => new PlayerSkill
            {
                Skill = ps.Skill,
                Value = ps.Value
            }).ToList()
        };

        Context.Players.Add(player);
        await Context.SaveChangesAsync();
        return Ok(player);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlayer(int id, PlayerViewModel player)
    {
        var existentPlayer = await Context.Players.Include(p => p.PlayerSkills).FirstOrDefaultAsync(p => p.Id == id);

        if (existentPlayer is null)
            return NotFound();

        existentPlayer.Name = player.Name;
        existentPlayer.Position = player.Position;
        existentPlayer.PlayerSkills = player.PlayerSkills.Select(ps => new PlayerSkill
        {
            Skill = ps.Skill,
            Value = ps.Value
        }).ToList();

        await Context.SaveChangesAsync();

        return Ok();
    }

    [RequireBearerToken]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Player>> DeletePlayer(int id)
    {
        if (await Context.Players.FindAsync(id) is Player player)
        {
            Context.Players.Remove(player);
            await Context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }
}
