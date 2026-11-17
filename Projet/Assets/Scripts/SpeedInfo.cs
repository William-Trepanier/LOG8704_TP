using System;

/// <summary>
/// Différentes échelles de vitesse
/// </summary>
public enum SpeedScale
{
    Meters,
    Kilometers,
    Beta,
    OneMinusBeta
}

/// <summary>
/// Vitesse de la navette spatiale selon une échelle spécifique
/// </summary>
public struct SpeedInfo
{
    public readonly string label;
    
    private readonly double _speed;
    private readonly SpeedScale _scale;
    private readonly double _rapidity;

    public SpeedInfo(double rapidity)
    {
        _rapidity = rapidity;
        
        _scale = rapidity switch
        {
            <= Constants.MaxRapidityM => SpeedScale.Meters,
            <= Constants.MaxRapidityKm => SpeedScale.Kilometers,
            <= Constants.MaxRapidityBeta => SpeedScale.Beta,
            _ => SpeedScale.OneMinusBeta
        };

        _speed = FromRapidity(rapidity, _scale);
        label = GetLabel(_speed, _scale);
    }

    public double Get(SpeedScale speedScale)
    {
        return speedScale == _scale ? _speed : FromRapidity(_rapidity, speedScale);
    }
    
    public static double Convert(double speed, SpeedScale fromScale, SpeedScale toScale)
    {
        return fromScale == toScale ? speed : FromRapidity(ToRapidity(speed, fromScale), toScale);
    }

    private static double FromRapidity(double rapidity, SpeedScale speedScale)
    {
        return speedScale switch
        {
            SpeedScale.Meters => Constants.LightSpeed * Math.Tanh(rapidity),
            SpeedScale.Kilometers => Constants.LightSpeed * Math.Tanh(rapidity) / Constants.KilometersToMeters,
            SpeedScale.Beta => Math.Tanh(rapidity) * 100,
            _ => Constants.LightSpeed * (Math.Exp(-rapidity) / Math.Cosh(rapidity))
        };
    }

    private static double ToRapidity(double speed, SpeedScale speedScale)
    {
        return speedScale switch
        {
            SpeedScale.Meters => Math.Atanh(speed / Constants.LightSpeed),
            SpeedScale.Kilometers => Math.Atanh(speed * Constants.KilometersToMeters / Constants.LightSpeed),
            SpeedScale.Beta => Math.Atanh(speed / 100),
            _ => 0.5 * Math.Log(2 * Constants.LightSpeed / speed - 1)
        };
    }

    private static string GetLabel(double value, SpeedScale speedScale)
    {
        return speedScale switch
        {
            SpeedScale.Meters => $"{value:F0} m/s",
            SpeedScale.Kilometers => $"{value:F0} km/s",
            SpeedScale.Beta => $"{value:F8} %",
            _ => $"{value:G3} m/s until light speed"
        };
    }
}