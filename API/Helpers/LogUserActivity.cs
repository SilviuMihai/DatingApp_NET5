using System;
using System.Threading.Tasks;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContext = await next();

           if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return; //do nothing

            //If they are authenticated, update that lastActive property
            var userId = resultContext.HttpContext.User.GetUserId();

            //to access the repository
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
              
            var user = await repo.GetUserByIdAsync(userId);

            user.LastActive = DateTime.Now;
            
            await repo.SaveAllAsync();
        }
    }
}