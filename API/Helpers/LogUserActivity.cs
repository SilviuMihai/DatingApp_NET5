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
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
              
            var user = await uow.UserRepository.GetUserByIdAsync(userId);

            user.LastActive = DateTime.UtcNow;
            
            await uow.Complete();
        }
    }
}