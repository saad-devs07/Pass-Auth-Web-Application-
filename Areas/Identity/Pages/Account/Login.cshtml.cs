using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PassAuthWebApp_.Areas.Identity.Data;

namespace PassAuthWebApp_.Areas.Identity.Pages.Account
{
    [All]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //returnUrl ??= Url.Content("~/");
            returnUrl = returnUrl ?? Url.Content("~/Home/Welcome");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user != null)
                    {
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                        _logger.LogInformation("Checking user: {0}, IsAdmin: {1}", user.Email, isAdmin);

                        if (!isAdmin)
                        {
                            //_logger.LogInformation("LastPasswordChangedDate: {0}, CurrentTime: {1}",
                            //    user.LastPasswordChangedDate, DateTime.Now);
                       
                            // Check if the password is expired (expired after 30 days)
                            if (user.LastPasswordChangedDate.AddMinutes(2) < DateTime.Now)
                            {
                                _logger.LogInformation("Password expired for user {0}. Redirecting to password reset.", user.Email);
                                return RedirectToPage("./PasswordExpired");
                            }

                            // Check for account dormancy (marked as dormant after 90 days of inactivity)
                            else if (user.LastLoginDate.AddDays(90) < DateTime.Now)
                            {
                                user.LockoutEnd = DateTimeOffset.MaxValue;
                                user.IsDormant = true;
                                await _userManager.UpdateAsync(user);
                                return RedirectToPage("./DormantAccount");
                            }
                        }

                        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                        // user after 5 attempst 
                        if (user.AccessFailedCount == 5)
                        {
                            ModelState.AddModelError(string.Empty, "You have attempted 5 failed attempts, this is your last chance otherwise your account will be locked.");
                            return Page();
                        }

                        // user after 6 attempst 
                        if (user.AccessFailedCount>=6)
                        {
                            user.LockoutEnabled = true;
                            user.AccessFailedCount = 0;
                            await _userManager.UpdateAsync(user);
                            return RedirectToPage("./Lockout");
                        }
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");

                            user.LastLoginDate = DateTime.UtcNow;
                            await _userManager.UpdateAsync(user);

                            return LocalRedirect(returnUrl);
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }

                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            return Page();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the login process.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
