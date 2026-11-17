using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

/// <summary>
/// Différentes échelles de distance
/// </summary>
public enum DistanceScale
{
    Meters,
    Kilometers,
    LightSeconds,
    AstronomicalUnits,
    LightYears,
    Parsec
}

/// <summary>
/// Distance que la navette spatiale a parcouru dans une échelle spécifique
/// </summary>
public struct DistanceInfo
{
    public readonly string label;
    
    private readonly double _distance;
    private readonly DistanceScale _scale;

    public DistanceInfo(double distanceInMeters)
    {
        (_scale, _distance) = distanceInMeters switch
        {
            <= Constants.MaxDistanceM => (DistanceScale.Meters, distanceInMeters),
            <= Constants.MaxDistanceKm => (DistanceScale.Kilometers, distanceInMeters / Constants.KilometersToMeters),
            <= Constants.MaxDistanceLightSeconds => (DistanceScale.LightSeconds, distanceInMeters / Constants.LightSecondsToMeters),
            <= Constants.MaxDistanceAstronomicalUnits => (DistanceScale.AstronomicalUnits, distanceInMeters / Constants.AstronomicalUnitsToMeters),
            <= Constants.MaxDistanceLightYears => (DistanceScale.LightYears, distanceInMeters / Constants.LightYearsToMeters),
            _ => (DistanceScale.Parsec, distanceInMeters / Constants.ParsecToMeters)
        };
        
        label = GetLabel(_distance, _scale);
    }
    
    public DistanceInfo(double distance, DistanceScale scale) 
        : this(Convert(distance, scale, DistanceScale.Meters))
    {
    }

    public double Get(DistanceScale scale)
    {
        return scale == _scale ? _distance : Convert(_distance, _scale, scale);
    }
    
    public static double Convert(double distance, DistanceScale fromScale, DistanceScale toScale)
    {
        return distance * GetMetersPerUnit(fromScale) / GetMetersPerUnit(toScale);
    }

    private static double GetMetersPerUnit(DistanceScale unitScale)
    {
        return unitScale switch
        {
            DistanceScale.Meters => 1.0,
            DistanceScale.Kilometers => Constants.KilometersToMeters,
            DistanceScale.LightSeconds => Constants.LightSecondsToMeters,
            DistanceScale.AstronomicalUnits => Constants.AstronomicalUnitsToMeters,
            DistanceScale.LightYears => Constants.LightYearsToMeters,
            _ => Constants.ParsecToMeters
        };
    }

    private static string GetLabel(double value, DistanceScale scale)
    {
        return scale switch
        {
            DistanceScale.Meters => $"{value:F4} m",
            DistanceScale.Kilometers => $"{value:F4} km",
            DistanceScale.LightSeconds => $"{value:F4} ls",
            DistanceScale.AstronomicalUnits => $"{value:F4} au",
            DistanceScale.LightYears => $"{value:F4} ly",
            _ => $"{value:F4} pc"
        };
    }
}