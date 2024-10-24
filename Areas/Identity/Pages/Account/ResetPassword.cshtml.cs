// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PassAuthWebApp_.Areas.Identity.Data;
using PassAuthWebApp_.Services;

namespace PassAuthWebApp_.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                user.LastPasswordChangedDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(string userId, string newPassword)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var passwordHistoryService = new PasswordHistoryService(_userManager, _context);

        //    // Check if the new password has been used before
        //    if (await passwordHistoryService.IsPasswordUsedBefore(userId, newPassword))
        //    {
        //        ModelState.AddModelError(string.Empty, "You cannot reuse a previously used password.");
        //        return View("ManageUsers", _userManager.Users.ToList());
        //    }

        //    // Generate password reset token
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    // Reset password with the token
        //    var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        //    if (result.Succeeded)
        //    {
        //        // Update password history
        //        await passwordHistoryService.UpdatePasswordHistory(userId, newPassword);

        //        // Update the last password changed date
        //        user.LastPasswordChangedDate = DateTime.Now;
        //        await _userManager.UpdateAsync(user);

        //        return RedirectToAction("ManageUsers");
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //    return View("ManageUsers", _userManager.Users.ToList());
        //}
    }
}
