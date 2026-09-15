using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LanChat.Services;

    public class EncryptionService
{
    //Hashes password using SHA-256 so the plain password never leaves the clien
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
    //Generates a new 2048-bit RSA key for the user
    public (string publicKey, string PrivateKey) GenerateRSAKeyPair()
    {
        using var rsa = RSA.Create(2048);
        var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
        var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        return (publicKey, privateKey);
    }

    //Encrypts the AES session key using the recipient's RSA public key
    public string EncryptRsa(byte[] data, string receiverPublicKeyBase64)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(receiverPublicKeyBase64), out _);
        var encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encrypted);
    }

    //Decrypts the AES session key using the own RSA private key
    public byte[] DecryptRSA(string encryptedDataBase64, string privateKeyBase64)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
        return rsa.Decrypt(Convert.FromBase64String(encryptedDataBase64), RSAEncryptionPadding.OaepSHA256);
    }

    //Encrypts payload content with AES-256-CBC
    public (string CipherText, string EncryptedAesKey, string Iv) EncryptMessage(string plainText, string receiverPublicKeyBase64)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        var cipherText = Convert.ToBase64String(ms.ToArray());
        var encryptedAesKey = EncryptRsa(aes.Key, receiverPublicKeyBase64);
        var iv = Convert.ToBase64String(aes.IV);

        return (cipherText, encryptedAesKey, iv);
    }
}