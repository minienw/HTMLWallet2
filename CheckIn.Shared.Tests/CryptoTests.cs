using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CheckInQrWeb.Core;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Xunit;

namespace CheckInQrWebTests
{
    public class CryptoTests
    {
        [Fact]
        public void ParseKeys()
        {
            var pk = Crypto.GetRsaPublicKey("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEArTl66RAAnSYtDm6rb2dCclaozgPSr6Oxu/whHlknTke5E28YGiIWKSiuTR530GO/Wie22FHIIUoIyaZT6mmCC3XTZiQ8V+fqFGaqr7uQooNzJT6sNXRj+iqZxueDKEClry/6Rsq8mfZw+K7UD7hdn9EWfFR5VWY+PgbWPZkSaRVldCpjZrNwECAsyBNTFSDZcMJ7hoofrp/g5+qms8OjwPuc1Jw3yg0qNVig3sSDNqbXSkGimrmWWCpGZ255zCgVJbQTwOgRqrpZAoIq2sJNdKaVQ8aCwKQeZo85jcXS1iB8meG0GFiWI8/A8+mNodiAZNLxxrbiRFkh6posVbmxo/gyvlVmyaYXg09CZrNNCmicTyQ4tC7Oz0PNrr+/ZQA7UvyPnPQs1j9YGCeG1HhHwT58d9d6/01a29YHuxa+bwr/Qey4QEOX+n1+DDTGrRN9TySr/+uP+CJk2yeXBwHbywKPfC/3mOur47jCyy3aaozWkDsSZsNePfHpPjULyawt817IQ6/b3Le0oklmlpB8I+5BeicO8oEmPoFr9QCq6IxhJ1RDNJquESX5s71HS3Y8nZ98TQrZUpigI+w06IsaQgR4VCVhbn5LvE93A+RWOldaM+WvpZwHh4UoUHOBPmxof8cb5xoCUBbgel/ASMz66H9zSiFWBr2c3lXafbfMV20CAwEAAQ==");
            Assert.Equal(4096, pk.KeySize);
        }

        [Fact]
        public void SigningRoundTrip()
        {
            var keyPair = Crypto.GenerateEcKeyPair();
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            Trace.WriteLine($"Data: {Convert.ToBase64String(data)}");
            var sig = Crypto.Sign(data, keyPair.Private);
            Trace.WriteLine($"Sig: {Convert.ToBase64String(sig)}");
            Trace.WriteLine($"Public Key: {keyPair.Public.EncodeDerBase64()}");
            Assert.True(Crypto.Verify(data, sig, keyPair.Public));
        }

        [Fact]
        public void EncryptCbc()
        {
            var secretKey = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var plaintText = "argle";
            var cipherText = Crypto.EncryptAesCbc(Encoding.UTF8.GetBytes(plaintText), secretKey, iv);

            var cipher2 = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            cipher2.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", secretKey), iv));
            Assert.Equal(plaintText, Encoding.UTF8.GetString(cipher2.DoFinal(cipherText)));
        }

        [Fact]
        public void EncryptGcm()
        {
            var secretKey = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var plainText = "argle";
            var cipherText = Crypto.EncryptAesGcm(Encoding.UTF8.GetBytes(plainText), secretKey, iv);

            var cipher2 = CipherUtilities.GetCipher("AES/GCM");
            cipher2.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", secretKey), iv, iv.Length / 8, iv.Length)); //TODO I think this is wrong...
            Assert.Equal(plainText, Encoding.UTF8.GetString(cipher2.DoFinal(cipherText)));

        }
    }
}