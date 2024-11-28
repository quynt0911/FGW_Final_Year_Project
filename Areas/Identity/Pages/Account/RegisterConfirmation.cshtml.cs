using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blank.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string EmailConfirmationUrl { get; set; }

        public void OnGet(string email, bool displayConfirmAccountLink = false, string emailConfirmationUrl = null)
        {
            Email = email;
            DisplayConfirmAccountLink = displayConfirmAccountLink;
            EmailConfirmationUrl = emailConfirmationUrl;
        }
    }
}
