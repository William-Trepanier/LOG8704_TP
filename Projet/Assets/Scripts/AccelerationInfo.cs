using System;

/// <summary>
/// Différentes échelles d'accélération
/// </summary>
public enum AccelerationScale
{
    Meters,
    G,
    Kilometers,
    LightSeconds
}

/// <summary>
/// Accélération de la navette spatiale selon une échelle spécifique
/// </summary>
public struct AccelerationInfo
{
    public readonly double value;
    public readonly AccelerationScale scale;
    public readonly string label;
    
    public AccelerationInfo(double acceleration)
    {
        (scale, value) = Math.Abs(acceleration) switch
        {
            < Constants.EarthGravity => (AccelerationScale.Meters, acceleration),
            < 1000.0 => (AccelerationScale.G, acceleration / Constants.EarthGravity),
            < Constants.LightSpeed => (AccelerationScale.Kilometers, acceleration / 1000.0),
            _ => (AccelerationScale.LightSeconds, acceleration / Constants.LightSpeed)
        };

        label = scale switch
        {
            AccelerationScale.Meters => $"{value:F2} m/s²",
            AccelerationScale.G => $"{value:F2} g",
            AccelerationScale.Kilometers => $"{value:F2} km/s²",
            _ => $"{value:G3} ls/s²"
        };
    }
}