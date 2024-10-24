//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.WebUtilities;
//using PassAuthWebApp_.Areas.Identity.Data;
//using System.Text;

//namespace PassAuthWebApp_.Areas.Identity.Pages.Account
//{
//    public class PasswordExpiredModel : PageModel
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public PasswordExpiredModel(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public string? ResetPasswordUrl { get; set; }

//        public async Task<IActionResult> OnGetAsync()
//        {
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return RedirectToPage("/Account/Login");
//            }

//            // Generate the password reset token
//            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
//            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

//            // Generate the reset password URL
//            ResetPasswordUrl = Url.Page("/Account/ResetPassword", new { area = "Identity", code = encodedCode, email = user.Email });

//            return Page();
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PassAuthWebApp_.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class PasswordExpiredModel : PageModel
    {
        public string Message { get; private set; }

        public void OnGet()
        {
            Message = "Your password has expired. Please reset your password to continue using the application.";
        }
    }
}
