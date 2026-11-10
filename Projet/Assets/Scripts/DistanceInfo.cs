using Unity.Burst.CompilerServices;

public enum DistanceScale
{
    Meters,
    Kilometers,
    LightSeconds,
    AstronomicalUnits,
    LightYears,
    Parsec
}

public struct DistanceInfo
{
    public readonly double value;
    public readonly DistanceScale scale;
    public readonly string label;

    public DistanceInfo(double distance)
    {
        (scale, value) = distance switch
        {
            <= Constants.MaxDistanceM => (DistanceScale.Meters, distance),
            <= Constants.MaxDistanceKm => (DistanceScale.Kilometers, distance / Constants.KilometersToMeters),
            <= Constants.MaxDistanceLightSeconds => (DistanceScale.LightSeconds, distance / Constants.LightSecondsToMeters),
            <= Constants.MaxDistanceAstronomicalUnits => (DistanceScale.AstronomicalUnits, distance / Constants.AstronomicalUnitsToMeters),
            <= Constants.MaxDistanceLightYears => (DistanceScale.LightYears, distance / Constants.LightYearsToMeters),
            _ => (DistanceScale.Parsec, distance / Constants.ParsecToMeters)
        };

        label = distance switch
        {
            <= Constants.MaxDistanceM => $"{value:F4} m",
            <= Constants.MaxDistanceKm => $"{value:F4} km",
            <= Constants.MaxDistanceLightSeconds => $"{value:F4} ls",
            <= Constants.MaxDistanceAstronomicalUnits => $"{value:F4} au",
            <= Constants.MaxDistanceLightYears => $"{value:F4} ly",
            _ => $"{value:F4} pc"
        };
    }
}