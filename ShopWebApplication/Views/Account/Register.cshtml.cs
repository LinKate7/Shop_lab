using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopWebApplication.Models;
using ShopWebApplication.ViewModels;

namespace ShopWebApplication.Views.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ShopWebApplication.Models.IdentityContext _context;

        public RegisterModel(ShopWebApplication.Models.IdentityContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.RegisterViewModel.Add(RegisterViewModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
