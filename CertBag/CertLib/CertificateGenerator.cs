using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace CertBag.CertLib
{
    public class CertificateGenerator : ICertificateGenerator
    {
        private byte[] _caPfxBytes;
        public CertificateGenerator(byte[] caPfxBytes)
        {
            _caPfxBytes = caPfxBytes;

        }

        public CertificateGenerator(IConfiguration config)
        {
            var caPath = config.GetValue<string>("CA_PATH");
            _caPfxBytes = File.ReadAllBytes(caPath);
        }
        public X509Certificate2 Generate(string commonName, string caPassword)
        {
            using (var privateKey = RSA.Create())
            {
                var request = new CertificateRequest(
                    $"CN={commonName}",
                    privateKey,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                // Load CA keys
                var caCert = new X509Certificate2(_caPfxBytes, caPassword);

                // Add extensions
                request.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(
                        certificateAuthority: false,
                        hasPathLengthConstraint: false,
                        pathLengthConstraint: 0,
                        critical: false)
                );
                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature
                        | X509KeyUsageFlags.KeyEncipherment
                        | X509KeyUsageFlags.NonRepudiation,
                        false));

                var oid = new Oid("clientAuth");
                var oidCol = new OidCollection();
                oidCol.Add(oid);

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        oidCol, false));

                var serialNumber = generateSerialNumber(commonName);

                var cert = request.Create(
                    caCert,
                    new DateTimeOffset(DateTime.UtcNow),
                    new DateTimeOffset(DateTime.UtcNow.AddDays(365)),
                    serialNumber);

                var certWithPrivateKey = cert.CopyWithPrivateKey(privateKey);

                return certWithPrivateKey;
            }

        }
        private byte[] generateSerialNumber(string commonName)
        {
            var sha = SHA1.Create();

            return sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(commonName));
        }

    }
}