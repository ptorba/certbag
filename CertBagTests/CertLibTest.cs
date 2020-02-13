using System.IO;
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
        public void TestGenerateCertificate()
        {
            var assembly = typeof(CertLibTest).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("CertBagTests._testCA.ca.pfx");
            byte[] caBytes;
            using (var br = new BinaryReader(resource))
            {
                caBytes = br.ReadBytes((int)resource.Length);
            }
            var certService = new CertificateGenerator(caPfxBytes: caBytes);
            var commonName = "testName";
            X509Certificate2 cert = certService.Generate(
                    commonName: commonName,
                    caPassword: "123456",
                    durationDays: 365);
            Assert.NotNull(cert);
            Assert.True(cert.HasPrivateKey);
            Console.WriteLine($"cert: {cert.NotBefore} {cert.NotAfter}");
            Assert.Equal(new DateTimeOffset(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                cert.NotBefore.Date.ToString("yyyy-MM-dd"));
            Assert.Equal(new DateTimeOffset(DateTime.UtcNow.AddDays(365)).ToString("yyyy-MM-dd"),
                cert.NotAfter.ToString("yyyy-MM-dd"));
            Assert.Equal($"CN={commonName}", cert.SubjectName.Name);
            Assert.Contains("CN=My local CA", cert.Issuer);
        }
    }
}