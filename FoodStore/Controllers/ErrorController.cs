using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
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
