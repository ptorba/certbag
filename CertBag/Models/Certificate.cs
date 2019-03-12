using System;
using System.ComponentModel.DataAnnotations;

namespace CertBag.Models
{
    public class Certificate
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Common name is required")]
        public string CommonName { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime LastGenerationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ExpiryDate { get; set; }

    }
}