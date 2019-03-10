using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertBag.CertLib
{
    public class CertificateGeneratorService
    {
        public X509Certificate2 Generate(string commonName)
        {
            using (var privateKey = RSA.Create(2048))
            {
                var request = new CertificateRequest($"CN={commonName}", privateKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Load CA private key
                var caKeyBytes = File.ReadAllBytes("/home/tpawel/CertBag/ca/ca.key");
                var caCertBytes = File.ReadAllBytes("/home/tpawel/CertBag/ca/ca.key");
                var caPfx = File.ReadAllBytes("/home/tpawel/CertBag/ca/ca.pfx");
                var caCert = new X509Certificate2(caPfx, "123456");

                // Add extensions
                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));

                var cert = request.Create(
                    caCert,
                    new DateTimeOffset(DateTime.UtcNow.AddDays(3)),
                    new DateTimeOffset(DateTime.UtcNow.AddDays(365)),
                    new byte[] { 1 });
                var signedCert = new X509Certificate2(cert.Export(X509ContentType.Pfx));
                File.WriteAllBytes("/tmp/cert.pfx", signedCert.Export(X509ContentType.Pfx, "12345"));

                return signedCert;
            }
        }
    }
}