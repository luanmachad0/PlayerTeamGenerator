namespace WebApi.Helpers
{
    public class ValidateSkillNameHelper
    {
        public static bool Validate(string skillName)
        {
            switch (skillName)
            {
                case "defense":
                    return true;
                case "attack":
                    return true;
                case "speed":
                    return true;
                case "strength":
                    return true;
                case "stamina":
                    return true;
                default:
                    return false;
            };
        }
    }
}