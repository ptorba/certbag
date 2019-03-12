using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CertBag.Models;
using Microsoft.Extensions.Configuration;

namespace CertBag.Pages.Certificates
{
    public class IndexModel : PageModel
    {
        private readonly CertBag.Models.CertDbContext _context;

        public IndexModel(CertBag.Models.CertDbContext context)
        {
            _context = context;
        }

        public IList<Certificate> Certificate { get; set; }

        public async Task OnGetAsync()
        {
            Certificate = await _context.Certificate.OrderBy(c => c.ExpiryDate).ToListAsync();
        }

    }
}
