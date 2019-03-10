using System;

namespace CertBag.Models
{
    public class Certificate
    {

        public int ID { get; set; }

        public string CommonName { get; set; }
        public string Email { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

    }
}