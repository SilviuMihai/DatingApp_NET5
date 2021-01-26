using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]   //baseapicontroller will be inherited in 
                                  //all the other controllers, replacing the ControllerBase
    public class BaseApiController: ControllerBase
    {
        
    }
}