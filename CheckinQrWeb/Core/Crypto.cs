using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace CheckInQrWeb.Core;

public static class Crypto
{
    public static byte[] EncryptAesCbc(byte[] plainText, byte[] secretKey, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
        cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", secretKey), iv));
        return cipher.DoFinal(plainText);
    }

    public static byte[] EncryptAesGcm(byte[] plainText, byte[] secretKey, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/GCM");
        cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", secretKey), iv, iv.Length / 8, iv.Length)); //TODO I think this is wrong...
        return cipher.DoFinal(plainText);
    }

    public static RSA GetRsaPublicKey(string base64DerValue)
    {
        var derBytes = Convert.FromBase64String(base64DerValue);
        var k = PublicKey.CreateFromSubjectPublicKeyInfo(derBytes, out var _);
        return k.GetRSAPublicKey();
    }

    public static byte[] Digest(byte[] payload, AsymmetricKeyParameter privateKey)
    {
        var signer = SignerUtilities.GetSigner(X9ObjectIdentifiers.ECDsaWithSha256.Id);
        signer.Init(true, privateKey);
        signer.BlockUpdate(payload, 0, payload.Length);
        return signer.GenerateSignature();
    }

    public static string EncodeDerBase64(this AsymmetricKeyParameter key) => Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key).GetDerEncoded());

    public static AsymmetricCipherKeyPair GenerateEcKeyPair()
    {
        var domainParams = new ECDomainParameters(ECNamedCurveTable.GetByName("secp256k1"));
        var keyParams = new ECKeyGenerationParameters(domainParams, new SecureRandom());
        var generator = new ECKeyPairGenerator("ECDSA");
        generator.Init(keyParams);
        return generator.GenerateKeyPair();
    }
}