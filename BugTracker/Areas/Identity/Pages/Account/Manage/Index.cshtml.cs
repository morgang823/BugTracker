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

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : BTUser
        {
            [Display(Name = "Custom Image")]
            public IFormFile ImageData { get; set; }
        }

        private async Task LoadAsync(BTUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            string contentType = user.AvatarContentType;

            user = new BTUser
            {
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarFormFile = user.AvatarFormFile
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

            if (user.AvatarFormFile is null)
            {
                user.AvatarFileData = await BTImageService.EncodeFileAsync(user.AvatarFormFile);
                user.AvatarContentType = BTImageService.ContentType(user.AvatarFormFile);

            }
            else
            {
                user.AvatarFileData = await BTImageService.EncodeFileAsync(user.AvatarFormFile);
                user.AvatarContentType = BTImageService.ContentType(user.AvatarFormFile);

            }
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
