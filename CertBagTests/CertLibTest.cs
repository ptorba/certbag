using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

using Xunit;
using CertBag.CertLib;
using System.Text;
using System;

namespace CertBagUnitTests
{
    public class CertLibTest
    {
        [Fact]
        public void TestReadCertificate()
        {
            var certService = new CertificateGeneratorService();
            var commonName = "testName";
            X509Certificate2 cert = certService.Generate(commonName);
            // Assert.NotEmpty(cert.PrivateKey);
            // Assert.NotEmpty(cert.PublicKey);
            Assert.NotNull(cert);
            Console.WriteLine($"cert: {cert.NotBefore} {cert.NotAfter}");
            Assert.Equal(new DateTimeOffset(DateTime.UtcNow.AddDays(3)).ToString("yyyy-MM-dd"),
                cert.NotBefore.Date.ToString("yyyy-MM-dd"));
            Assert.Equal(new DateTimeOffset(DateTime.UtcNow.AddDays(365)).ToString("yyyy-MM-dd"),
                cert.NotAfter.ToString("yyyy-MM-dd"));
            Assert.Equal($"CN={commonName}", cert.SubjectName.Name);
        }
    }
}