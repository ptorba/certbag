using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CertBag.Models;
using Microsoft.Extensions.Logging;

namespace CertBag.Pages.Certificates
{
    public class CreateModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;
        private readonly ILogger _logger;

        public CreateModel(CertBag.Models.CertDbContext context,
                           ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            Certificate = new Certificate();
            return Page();
        }

        [BindProperty]
        public Certificate Certificate { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Certificate.LastGenerationDate = new DateTime(1, 1, 1);
            Certificate.ExpiryDate = new DateTime(1, 1, 1);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Certificate.Add(Certificate);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}