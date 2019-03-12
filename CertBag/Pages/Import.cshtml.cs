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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CertBag.Pages.Certificates
{
    public class ImportModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;
        private readonly ILogger _logger;
        private readonly ICertificateGenerator _certGenerator;
        private readonly IConfiguration _config;

        public ImportModel(CertBag.Models.CertDbContext context,
                             ILogger<GenerateModel> logger,
                             IConfiguration config,
                             ICertificateGenerator certGenerator)
        {
            _context = context;
            _logger = logger;
            _config = config;
            _certGenerator = certGenerator;
        }
        [BindProperty, Required, Display(Name = "Password")]
        public string CertPassword { get; set; }

        [BindProperty, Required, Display(Name = "File")]
        public IFormFile FileContent { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var reader = new BinaryReader(FileContent.OpenReadStream()))
            {
                var bytes = reader.ReadBytes((int)FileContent.Length);

                var cert = new X509Certificate2(bytes, CertPassword);
                var cn = cert.SubjectName.Decode(
                    X500DistinguishedNameFlags.UseNewLines
                    | X500DistinguishedNameFlags.Reversed)
                    .Split('\n')[0]
                    .Substring(3);

                var exists = await _context.Certificate.FirstOrDefaultAsync(c => c.CommonName == cn);
                if (exists != null)
                {
                    ModelState.AddModelError("FileContent", $"Certificate with CommonName={cn} already registered");
                    return Page();
                }

                var certModel = new Certificate()
                {
                    CommonName = cn,
                    ExpiryDate = cert.NotAfter
                };

                _context.Add(certModel);
                await _context.SaveChangesAsync();
            }

            return Redirect("./Index");
        }
    }
}
