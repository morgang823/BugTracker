using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BugTracker.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BTUser> _userManager;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTImageService BTImageService;

        public IndexModel(
            UserManager<BTUser> userManager,
            SignInManager<BTUser> signInManager, IBTImageService bTImageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            BTImageService = bTImageService;
        }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
    
        public byte[] ImageData { get; set; }

        public string ImageContentType { get; set; }

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Custom Image")]
            public IFormFile ImageFile { get; set; }

            public byte[] ImageData { get; set;}

            public string ContentType { get; set; }

            [Phone]
            [Display(Name ="Phone number")]

            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(BTUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;
            ImageData = user.AvatarFileData;
            ImageContentType = user.AvatarContentType;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ImageData = user.AvatarFileData,
                ContentType = user.AvatarContentType
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {

            if(User.IsInRole("DemoUser"))
            {
                return RedirectToAction("DemoError", "Home");
            }

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
            if(Input.FirstName is not null)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);

            }
            if (Input.LastName is not null)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);

            }

            if (Input.ImageFile is not null)
            {
                user.AvatarFileData = await BTImageService.EncodeFileAsync(Input.ImageFile);
                user.AvatarContentType = BTImageService.ContentType(Input.ImageFile);

            }
           
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
