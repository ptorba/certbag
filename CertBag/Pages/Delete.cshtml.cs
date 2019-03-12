using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CertBag.Models;

namespace CertBag.Pages.Certificates
{
    public class DeleteModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;

        public DeleteModel(CertBag.Models.CertDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Certificate Certificate { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Certificate = await _context.Certificate.FirstOrDefaultAsync(m => m.ID == id);

            if (Certificate == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Certificate = await _context.Certificate.FindAsync(id);

            if (Certificate != null)
            {
                _context.Certificate.Remove(Certificate);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
