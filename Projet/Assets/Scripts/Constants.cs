using System;

/// <summary>
/// Toutes les constantes utilisées dans le projet.
/// </summary>
public class Constants
{
    public const double EarthGravity = 9.80665;
    public const double LightSpeed = 299792458;
    public const double MaxRapidityM = 0.000007;
    public const double MaxRapidityKm = 0.035;
    public const double MaxRapidityBeta = 10;
    
    public const double KilometersToMeters = 1000.0;
    public const double LightSecondsToMeters = 299792458.0;
    public const double AstronomicalUnitsToMeters = 149597870700.0;
    public const double LightYearsToMeters = LightSecondsToMeters * 365.25 * 24 * 3600;
    public const double ParsecToMeters = 648000 * AstronomicalUnitsToMeters / Math.PI;

    public const double MaxDistanceM = KilometersToMeters;
    public const double MaxDistanceKm = LightSecondsToMeters;
    public const double MaxDistanceLightSeconds = AstronomicalUnitsToMeters;
    public const double MaxDistanceAstronomicalUnits = LightYearsToMeters;
    public const double MaxDistanceLightYears = ParsecToMeters;
}