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
using Microsoft.Extensions.Logging;
using CertBag.CertLib;
using System.Security.Cryptography.X509Certificates;

namespace CertBag.Pages.Certificates
{
    public class GenerateModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;
        private readonly ILogger _logger;
        private readonly ICertificateGenerator _certGenerator;
        private readonly IConfiguration _config;

        public GenerateModel(CertBag.Models.CertDbContext context,
                             ILogger<GenerateModel> logger,
                             IConfiguration config,
                             ICertificateGenerator certGenerator)
        {
            _context = context;
            _logger = logger;
            _config = config;
            _certGenerator = certGenerator;
        }

        [BindProperty]
        public Certificate Certificate { get; set; }

        [BindProperty]
        public string CAPassword { get; set; }
        [BindProperty]
        public string CertPassword { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var certificate = await _context.Certificate.FirstAsync(c => c.ID == id);
            var generatedCert = _certGenerator.Generate(
                commonName: certificate.CommonName,
                caPassword: CAPassword
            );
            certificate.LastGenerationDate = DateTime.UtcNow;
            certificate.ExpiryDate = generatedCert.NotAfter;

            await _context.SaveChangesAsync();

            var certBytes = generatedCert.Export(X509ContentType.Pfx, CertPassword);
            return File(
                fileContents: certBytes,
                contentType: MediaTypeNames.Application.Octet,
                fileDownloadName: $"{certificate.CommonName}.pfx");
        }
    }
}
