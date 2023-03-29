using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class PlayerViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Position { get; set; }
        public List<PlayerSkillViewModel> PlayerSkills { get; set; }
    }

    public class PlayerSkillViewModel
    {
        public string Skill { get; set; }
        public int Value { get; set; }
    }
}