using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipleExtentions
    {
            public static string GetUsername(this ClaimsPrincipal user)
            {
                //gives us the username (line bellow)
                return user.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            }
    }
}