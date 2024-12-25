namespace OpenTOY.Utils;

public static class ToyUser
{
    public static long GenerateNpsn(int serviceId, int userId)
    {
        var digitsServiceId = (int) Math.Log10(serviceId) + 1;
        var digitsUserId = (int) Math.Log10(userId) + 1;
        var zerosToAdd = 17 - digitsServiceId - digitsUserId;

        var npsn = serviceId * (long) Math.Pow(10, zerosToAdd + digitsUserId) + userId;
        return npsn;
    }
}