using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PassAuthWebApp_.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class DormantModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
