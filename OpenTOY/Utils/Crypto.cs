using System.Security.Cryptography;
using System.Text;

namespace OpenTOY.Utils;

public static class Crypto
{
    public static string Decrypt(string text, byte[] key)
    {
        var data = HexStringToByteArray(text);
        return Decrypt(data, key);
    }

    public static string Decrypt(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        
        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    public static byte[] Encrypt(byte[] data, string hexKey)
    {
        var key = HexStringToByteArray(hexKey);
        return Encrypt(data, key);
    }
    
    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        
        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }
    
    public static byte[] HexStringToByteArray(string hex)
    {
        var length = hex.Length;
        var bytes = new byte[length / 2];
        for (var i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        
        return bytes;
    }
}