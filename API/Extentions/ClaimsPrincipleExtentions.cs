using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipleExtentions
    {
            public static string GetUsername(this ClaimsPrincipal user)
            {
                //gives us the username (line bellow)
                return user.FindFirst(ClaimTypes.Name)?.Value; 
            }

            public static int GetUserId(this ClaimsPrincipal user)
            {
                //returns the integer of the Users Id
                return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value); 
            }
    }
}