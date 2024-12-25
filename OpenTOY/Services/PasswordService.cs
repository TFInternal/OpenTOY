using System.Security.Cryptography;
using System.Text;

namespace OpenTOY.Services;

// I'm not an expert in cryptography, but as far as I know I'm doing everything correctly here
// Please do correct me if I'm wrong though

public interface IPasswordService
{
    string HashPassword(string password, out byte[] salt);
    bool VerifyPassword(string password, string hash, string salt);
    bool VerifyPassword(string password, string hash, byte[] salt);
}

public class PasswordService : IPasswordService
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;
    
    public string HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KeySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            _hashAlgorithm,
            KeySize
        );
        
        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        return VerifyPassword(password, hash, Convert.FromHexString(salt));
    }

    public bool VerifyPassword(string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            _hashAlgorithm,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}