using System;
using System.IO;
using System.Security.Cryptography;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Utility class for cryptographic operations. </summary>
public static class CryptoUtils
{
    // This should be moved to secure place and not hardcoded here 
    private static readonly string base64StringKey = "QvShpLxgitrotojfPwbZZ7iJdDm6knTsWglMG/ouzLY=";
    private static readonly string base64StringIV = "fsO3n5AJcHVqYQNXwr9GwA==";

    /// <summary> Encrypts a string using AES encryption. </summary>
    /// <param name="plainString">The string to encrypt.</param>
    /// <returns>The encrypted string.</returns>
    public static string EncryptString(string plainString)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(base64StringKey);
        aesAlg.IV = Convert.FromBase64String(base64StringIV);

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream memoryStream = new();
        using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            using StreamWriter streamWriter = new(cryptoStream);
            streamWriter.Write(plainString);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary> Generates a new AES key and initialization vector (IV). </summary>
    /// <returns>The generated AES key.</returns>
    public static Aes GenerateKeyAes()
    {
        using Aes aesAlg = Aes.Create();

        string base64StringKey = Convert.ToBase64String(aesAlg.Key); ;
        GD.Print($"base64StringKey: {base64StringKey}");

        string base64StringIV = Convert.ToBase64String(aesAlg.IV); ;
        GD.Print($"base64StringIV: {base64StringIV}");
        return aesAlg;
    }

    /// <summary> Decrypts an encrypted string using AES decryption. </summary>
    /// <param name="encryptedString">The string to decrypt.</param>
    /// <returns>The decrypted string.</returns>
    public static string DecryptString(string encryptedString)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(base64StringKey);
        aesAlg.IV = Convert.FromBase64String(base64StringIV);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        byte[] buffer = Convert.FromBase64String(encryptedString);

        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new(cryptoStream);
        return streamReader.ReadToEnd();
    }
}

