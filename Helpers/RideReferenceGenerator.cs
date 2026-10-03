namespace MinimalRideHailingAPI.Helpers;

public static class RideReferenceGenerator
{
    private static readonly Random _random = new();

    public static string GenerateRideReference()
    {
        return $"RIDE-{_random.Next(100000, 1000000)}";
    }

}