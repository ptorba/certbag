using System.Security.Cryptography.X509Certificates;
namespace CertBag.CertLib
{
    public interface ICertificateGenerator
    {
        X509Certificate2 Generate(string commonName, string caPassword, int durationDays);
    }
}