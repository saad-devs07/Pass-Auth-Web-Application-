using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PassAuthWebApp_.Areas.Identity.Data;

namespace PassAuthWebApp_.Services
{
    public class PasswordHistoryService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PassAuthWebApp_.Areas.Identity.Data.DbContext _context;

        public PasswordHistoryService(UserManager<ApplicationUser> userManager, PassAuthWebApp_.Areas.Identity.Data.DbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> IsPasswordUsedBefore(string userId, string newPassword)
        {
            var passwordHistory = await _context.PasswordHistories.FirstOrDefaultAsync(ph => ph.UserId == userId);
            if (passwordHistory == null)
            {
                return false; // No history exists, meaning no password has been used before
            }

            // Compare new password with all six previous passwords
            for (int i = 1; i <= 6; i++)
            {
                var oldPassword = (string)passwordHistory.GetType().GetProperty($"Password{i}").GetValue(passwordHistory);
                if (!string.IsNullOrEmpty(oldPassword))
                {
                    var passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(null, oldPassword, newPassword);
                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        return true; // The new password matches one of the old passwords
                    }
                }
            }
            return false; // New password doesn't match any of the old passwords
        }

        public async Task UpdatePasswordHistory(string userId, string newPassword)
        {
            var passwordHistory = await _context.PasswordHistories.FirstOrDefaultAsync(ph => ph.UserId == userId);
            var user = await _userManager.FindByIdAsync(userId);
            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, newPassword);

            if (passwordHistory == null)
            {
                // Create a new entry if this user has no password history yet
                passwordHistory = new PasswordHistory
                {
                    UserId = userId,
                    Password1 = hashedPassword,
                    LastUpdated = DateTime.Now
                };
                _context.PasswordHistories.Add(passwordHistory);
            }
            else
            {
                // Shift the old passwords and insert the new one
                passwordHistory.Password6 = passwordHistory.Password5;
                passwordHistory.Password5 = passwordHistory.Password4;
                passwordHistory.Password4 = passwordHistory.Password3;
                passwordHistory.Password3 = passwordHistory.Password2;
                passwordHistory.Password2 = passwordHistory.Password1;
                passwordHistory.Password1 = hashedPassword;

                passwordHistory.LastUpdated = DateTime.Now;
                _context.PasswordHistories.Update(passwordHistory);
            }

            await _context.SaveChangesAsync();
        }
    }
}
