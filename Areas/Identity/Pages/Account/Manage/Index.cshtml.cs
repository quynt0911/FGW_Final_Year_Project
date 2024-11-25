// #nullable disable

// using System;
// using System.ComponentModel.DataAnnotations;
// using System.Text.Encodings.Web;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace Blank.Areas.Identity.Pages.Account.Manage
// {
//     public class IndexModel : PageModel
//     {
//         private readonly UserManager<IdentityUser> _userManager;
//         private readonly SignInManager<IdentityUser> _signInManager;

//         public IndexModel(
//             UserManager<IdentityUser> userManager,
//             SignInManager<IdentityUser> signInManager)
//         {
//             _userManager = userManager;
//             _signInManager = signInManager;
//         }
//         public string Username { get; set; }
//         [TempData]
//         public string StatusMessage { get; set; }

//         [BindProperty]
//         public InputModel Input { get; set; }

//         public class InputModel
//         {
//             [Phone]
//             [Display(Name = "Phone number")]
//             public string PhoneNumber { get; set; }
//         }

//         private async Task LoadAsync(IdentityUser user)
//         {
//             var userName = await _userManager.GetUserNameAsync(user);
//             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

//             Username = userName;

//             Input = new InputModel
//             {
//                 PhoneNumber = phoneNumber
//             };
//         }

//         public async Task<IActionResult> OnGetAsync()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null)
//             {
//                 return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
//             }

//             await LoadAsync(user);
//             return Page();
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null)
//             {
//                 return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
//             }

//             if (!ModelState.IsValid)
//             {
//                 await LoadAsync(user);
//                 return Page();
//             }

//             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
//             if (Input.PhoneNumber != phoneNumber)
//             {
//                 var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
//                 if (!setPhoneResult.Succeeded)
//                 {
//                     StatusMessage = "Unexpected error when trying to set phone number.";
//                     return RedirectToPage();
//                 }
//             }

//             await _signInManager.RefreshSignInAsync(user);
//             StatusMessage = "Your profile has been updated";
//             return RedirectToPage();
//         }
//     }
// }



// #nullable disable

// using System;
// using System.ComponentModel.DataAnnotations;
// using System.IO;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.Extensions.Hosting;

// namespace Blank.Areas.Identity.Pages.Account.Manage
// {
//     public class IndexModel : PageModel
//     {
//         private readonly UserManager<IdentityUser> _userManager;
//         private readonly SignInManager<IdentityUser> _signInManager;
//         private readonly IWebHostEnvironment _webHostEnvironment;

//         public IndexModel(
//             UserManager<IdentityUser> userManager,
//             SignInManager<IdentityUser> signInManager,
//             IWebHostEnvironment webHostEnvironment)
//         {
//             _userManager = userManager;
//             _signInManager = signInManager;
//             _webHostEnvironment = webHostEnvironment;
//         }

//         public string Username { get; set; }

//         [TempData]
//         public string StatusMessage { get; set; }

//         [BindProperty]
//         public InputModel Input { get; set; }

//         public class InputModel
//         {
//             [Phone]
//             [Display(Name = "Phone number")]
//             public string PhoneNumber { get; set; }

//             [Display(Name = "Profile picture")]
//             public IFormFile ProfilePicture { get; set; }
//         }

//         private async Task LoadAsync(IdentityUser user)
//         {
//             var userName = await _userManager.GetUserNameAsync(user);
//             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

//             // Lấy đường dẫn ảnh đại diện từ thuộc tính đã có trong cơ sở dữ liệu
//             var profileImagePath = user.GetType().GetProperty("ProfileImagePath")?.GetValue(user)?.ToString();

//             Username = userName;

//             Input = new InputModel
//             {
//                 PhoneNumber = phoneNumber
//             };
//         }

//         public async Task<IActionResult> OnGetAsync()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null)
//             {
//                 return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
//             }

//             await LoadAsync(user);
//             return Page();
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null)
//             {
//                 return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
//             }

//             if (!ModelState.IsValid)
//             {
//                 await LoadAsync(user);
//                 return Page();
//             }

//             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
//             if (Input.PhoneNumber != phoneNumber)
//             {
//                 var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
//                 if (!setPhoneResult.Succeeded)
//                 {
//                     StatusMessage = "Unexpected error when trying to set phone number.";
//                     return RedirectToPage();
//                 }
//             }

//             // Xử lý và lưu ảnh đại diện
//             if (Input.ProfilePicture != null)
//             {
//                 // Tạo tên tệp duy nhất cho ảnh đại diện
//                 string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
//                 string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile");
//                 string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                 // Đảm bảo thư mục tồn tại
//                 if (!Directory.Exists(uploadsFolder))
//                 {
//                     Directory.CreateDirectory(uploadsFolder);
//                 }

//                 // Lưu tệp vào thư mục wwwroot/images/profile
//                 using (var fileStream = new FileStream(filePath, FileMode.Create))
//                 {
//                     await Input.ProfilePicture.CopyToAsync(fileStream);
//                 }

//                 // Lưu đường dẫn tệp vào thuộc tính ProfileImagePath của người dùng
//                 user.GetType().GetProperty("ProfileImagePath")?.SetValue(user, uniqueFileName);
//             }

//             // Cập nhật thông tin người dùng trong cơ sở dữ liệu
//             await _userManager.UpdateAsync(user);
//             await _signInManager.RefreshSignInAsync(user);
//             StatusMessage = "Your profile has been updated";
//             return RedirectToPage();
//         }
//     }
// }



#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace Blank.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public string Username { get; set; }
        public string ProfileImagePath { get; set; }  // Thêm thuộc tính này để lưu đường dẫn ảnh đại diện

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile picture")]
            public IFormFile ProfilePicture { get; set; }
        }


        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            // Lấy đường dẫn ảnh đại diện từ thuộc tính đã có trong cơ sở dữ liệu
            ProfileImagePath = user.GetType().GetProperty("ProfileImagePath")?.GetValue(user)?.ToString();

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Xử lý và lưu ảnh đại diện
            if (Input.ProfilePicture != null)
            {
                // Tạo tên tệp duy nhất cho ảnh đại diện
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Lưu tệp vào thư mục wwwroot/images/profile
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn tệp vào thuộc tính ProfileImagePath của người dùng
                user.GetType().GetProperty("ProfileImagePath")?.SetValue(user, uniqueFileName);
            }

            // Cập nhật thông tin người dùng trong cơ sở dữ liệu
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
