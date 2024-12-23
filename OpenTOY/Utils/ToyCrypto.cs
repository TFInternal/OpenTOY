using System.Text;

namespace OpenTOY.Utils;

public static class ToyCrypto
{
    public static byte[]? GetUserKey(string npsn)
    {
        if (npsn.Length != 17)
        {
            return null;
        }

        var tmpKey = npsn[2..];
        var key = Crypto.Encrypt(Encoding.ASCII.GetBytes(tmpKey),
            Encoding.ASCII.GetBytes(npsn[1..]));
        
        return key;
    }
}