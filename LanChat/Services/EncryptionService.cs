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
}