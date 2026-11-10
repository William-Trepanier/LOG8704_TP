using System;

public enum SpeedScale
{
    Meters,
    Kilometers,
    Beta,
    OneMinusBeta
}

public struct SpeedInfo
{
    public readonly double value;
    public readonly SpeedScale scale;
    public readonly string label;

    public SpeedInfo(double rapidity)
    {
        (scale, value) = rapidity switch
        {
            <= Constants.MaxRapidityM => (SpeedScale.Meters, Constants.LightSpeed * Math.Tanh(rapidity)),
            <= Constants.MaxRapidityKm => (SpeedScale.Kilometers, Constants.LightSpeed * Math.Tanh(rapidity) / 1000.0),
            <= Constants.MaxRapidityBeta => (SpeedScale.Beta, Math.Tanh(rapidity)),
            _ => (SpeedScale.OneMinusBeta, Constants.LightSpeed * (Math.Exp(-rapidity) / Math.Cosh(rapidity)))
        };
            
        label = rapidity switch
        {
            <= Constants.MaxRapidityM => $"{value:F0} m/s",
            <= Constants.MaxRapidityKm => $"{value:F0} km/s",
            <= Constants.MaxRapidityBeta => $"{value:F2} %",
            _ => $"{value:G3} m/s until light speed"
        };
    }
}