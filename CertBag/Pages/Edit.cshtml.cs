using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CertBag.Models;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;

namespace CertBag.Pages.Certificates
{
    public class EditModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;

        public EditModel(CertBag.Models.CertDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateExists(Certificate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostGenerateAsync(int id)
        {
            var certificate = await _context.Certificate.FirstAsync(c => c.ID == id);
            certificate.LastGenerationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return File(
                fileContents: System.IO.File.ReadAllBytes("/tmp/cert.pfx"),
                contentType: MediaTypeNames.Application.Octet,
                fileDownloadName: $"{certificate.CommonName}.pfx");
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificate.Any(e => e.ID == id);
        }
    }
}
