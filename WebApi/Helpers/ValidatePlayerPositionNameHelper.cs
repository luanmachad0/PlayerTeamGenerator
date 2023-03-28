using WebApi.Entities;

namespace WebApi.Helpers
{
    public class ValidatePlayerPositionNameHelper
    {
        public static bool Validate(string position)
        {
            switch (position)
            {
                case "defender":
                    return true;
                case "midfielder":
                    return true;
                case "forward":
                 return true;
                default:
                    return false;
            };
        }
    }
}