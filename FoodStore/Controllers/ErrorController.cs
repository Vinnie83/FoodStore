using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    [AllowAnonymous]
    [Route("Error")]
    public class ErrorController : BaseController
    {
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        [Route("Error/500")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }
    }
}
