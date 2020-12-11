using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   //baseapicontroller will be inherited in 
                                  //all the other controllers, replacing the ControllerBase
    public class BaseApiController: ControllerBase
    {
        
    }
}