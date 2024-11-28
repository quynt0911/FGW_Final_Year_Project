using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Blank.Areas.Identity.Pages.Account
{
    public class ConfirmedModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmedModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                StatusMessage = "Invalid email confirmation request.";
                return Page();
            }

            UserId = userId;
            Code = code;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Code))
            {
                StatusMessage = "Invalid email confirmation request.";
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                StatusMessage = "Unable to load user.";
                return Page();
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if (result.Succeeded)
            {
                StatusMessage = "Your email has been confirmed!";
                return RedirectToPage("/Account/Login");
            }

            StatusMessage = "Error confirming your email.";
            return Page();
        }
    }
}
