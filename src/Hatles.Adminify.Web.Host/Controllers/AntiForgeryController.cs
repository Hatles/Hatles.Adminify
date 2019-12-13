using Microsoft.AspNetCore.Antiforgery;
using Hatles.Adminify.Controllers;

namespace Hatles.Adminify.Web.Host.Controllers
{
    public class AntiForgeryController : AdminifyControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
