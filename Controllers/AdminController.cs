////using Microsoft.AspNetCore.Identity;
////using Microsoft.AspNetCore.Mvc;
//////using PassAuthWebApp.Models; // Ensure this is imported
////using PassAuthWebApp_.Areas.Identity.Data;
////using System;
////using System.Threading.Tasks;

////namespace PassAuthWebApp.Controllers
////{
////    public class AdminController : Controller
////    {
////        private readonly UserManager<ApplicationUser> _userManager;

////        public AdminController(UserManager<ApplicationUser> userManager)
////        {
////            _userManager = userManager;
////        }

////        [HttpPost]
////        public async Task<IActionResult> ResetPassword(string userId, string newPassword)
////        {
////            var user = await _userManager.FindByIdAsync(userId);
////            if (user == null)
////            {
////                return NotFound();
////            }

////            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);

////            // Check if the new password is in the password history
////            if (user.PasswordHistory.Contains(newPasswordHash))
////            {
////                ModelState.AddModelError("", "Cannot reuse previous passwords.");
////                return View(); // Consider returning an appropriate view
////            }

////            // Reset password and update history
////            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
////            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

////            if (result.Succeeded)
////            {
////                // Update password history (keep only the last 6)
////                user.PasswordHistory.Add(newPasswordHash);
////                if (user.PasswordHistory.Count > 6)
////                {
////                    user.PasswordHistory.RemoveAt(0); // Remove the oldest password if more than 6
////                }

////                user.LastPasswordChangedDate = DateTime.UtcNow;
////                await _userManager.UpdateAsync(user);
////                return RedirectToAction("ManageUsers");
////            }

////            foreach (var error in result.Errors)
////            {
////                ModelState.AddModelError("", error.Description);
////            }

////            return View(); // Consider returning an appropriate view
////        }
////    }
////}
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using PassAuthWebApp_.Areas.Identity.Data;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PassAuthWebApp_.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public AdminController(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        // View for managing users
//        public IActionResult ManageUsers()
//        {
//            var users = _userManager.Users.ToList(); // Get all users
//            return View(users);  // Pass the users to the view
//        }

//        // Reset Password action
//        [HttpPost]
//        //public async Task<IActionResult> ResetPassword(string userId, string newPassword)
//        //{
//        //    var user = await _userManager.FindByIdAsync(userId);
//        //    if (user == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    // Hash the new password
//        //    var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);

//        //    // Check if the new password is in the password history
//        //    if (user.PasswordHistory.Contains(newPasswordHash))
//        //    {
//        //        ModelState.AddModelError("", "Cannot reuse previous passwords.");
//        //        return RedirectToAction("ManageUsers");
//        //    }

//        //    // Generate reset token
//        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

//        //    // Reset password
//        //    var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
//        //    if (result.Succeeded)
//        //    {
//        //        // Add the new password hash to the history
//        //        user.PasswordHistory.Add(newPasswordHash);

//        //        // Keep only the last 6 passwords
//        //        if (user.PasswordHistory.Count > 6)
//        //        {
//        //            user.PasswordHistory.RemoveAt(0);
//        //        }

//        //        user.LastPasswordChangedDate = DateTime.UtcNow;
//        //        await _userManager.UpdateAsync(user);

//        //        return RedirectToAction("ManageUsers");
//        //    }

//        //    foreach (var error in result.Errors)
//        //    {
//        //        ModelState.AddModelError("", error.Description);
//        //    }

//        //    return RedirectToAction("ManageUsers");
//        //}

//        [HttpPost]
//        public async Task<IActionResult> ResetPassword(string userId, string newPassword)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Hash the new password
//            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);

//            // Check if the new password is in the password history
//            if (user.PasswordHistory.Contains(newPasswordHash))
//            {
//                ModelState.AddModelError("", "Cannot reuse previous passwords.");
//                return RedirectToAction("ManageUsers");
//            }

//            // Generate reset token
//            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

//            // Reset password
//            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
//            if (result.Succeeded)
//            {
//                // Add the new password hash to the history
//                user.PasswordHistory.Add(newPasswordHash);

//                // If there are more than 6 passwords, remove the oldest ones
//                if (user.PasswordHistory.Count > 6)
//                {
//                    // Remove the oldest password(s) until the count is 6
//                    while (user.PasswordHistory.Count > 6)
//                    {
//                        user.PasswordHistory.RemoveAt(0); // Remove the oldest password (first element)
//                    }
//                }

//                user.LastPasswordChangedDate = DateTime.UtcNow;
//                await _userManager.UpdateAsync(user);

//                return RedirectToAction("ManageUsers");
//            }

//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error.Description);
//            }

//            return RedirectToAction("ManageUsers");
//        }

//        // Unlock Account action
//        [HttpPost]
//        public async Task<IActionResult> UnlockAccount(string userId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Unlock the user by setting LockoutEnd to null
//            user.LockoutEnd = null;
//            await _userManager.UpdateAsync(user);

//            return RedirectToAction("ManageUsers");
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PassAuthWebApp_.Areas.Identity.Data;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Method for listing users
    public async Task<IActionResult> ManageUsers()
    {
        //var users = _userManager.Users.ToList();
        //return View(users);
        var users = _userManager.Users.ToList();
        var nonAdminUsers = new List<ApplicationUser>();

        foreach (var user in users)
        {
            // Check if the user is an admin
            var isAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;
            if (!isAdmin)
            {
                nonAdminUsers.Add(user);
            }
        }

        return View(nonAdminUsers);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Generate password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Reset password with the token
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
        {
            user.LastLoginDate = DateTime.Now;
            // Optional: Update the last password changed date
            user.LastPasswordChangedDate = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("ManageUsers");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View("ManageUsers", _userManager.Users.ToList());
    }

    // Lock Account
    [HttpPost]
    public async Task<IActionResult> LockAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.LockoutEnabled = true;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ManageUsers");
        }

        ModelState.AddModelError(string.Empty, "Failed to lock the account.");
        return View("ManageUsers", _userManager.Users.ToList());
    }

    // Unlock Account 
    [HttpPost]
    public async Task<IActionResult> UnlockAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.LockoutEnabled = false;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ManageUsers");
        }

        ModelState.AddModelError(string.Empty, "Failed to unlock the account.");
        return View("ManageUsers", _userManager.Users.ToList());
    }

    // Make Dormant 
    [HttpPost]
    public async Task<IActionResult> DormantTheUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.IsDormant = true;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ManageUsers");
        }

        ModelState.AddModelError(string.Empty, "Failed to dormant the account.");
        return View("ManageUsers", _userManager.Users.ToList());
    }

    // Remove Dormancy
    [HttpPost]
    public async Task<IActionResult> RemoveDormancy(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.IsDormant = false;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("ManageUsers");
        }

        ModelState.AddModelError(string.Empty, "Failed to remove the dormancy from account.");
        return View("ManageUsers", _userManager.Users.ToList());
    }
}
