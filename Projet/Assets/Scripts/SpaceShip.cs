using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private const double LightSpeed = 299792458;
    private const double MaxRapidityM = 3e-6;
    private const double MaxRapidityKm = 0.01;
    private const double MaxRapidityBeta = 3.8;
    
    private RelativisticSky _relativisticSky;
    public double increment = 0.1f;
    
    public double Beta => Math.Tanh(Rapidity);

    public double Rapidity
    {
        get => _relativisticSky.rapidity;

        set
        {
            _relativisticSky.rapidity = value switch
            {
                < 0 => 0,
                > 12 => 12,
                _ => (float)value
            };
        }
    }
    
    public SpeedInfo Speed => new SpeedInfo(Rapidity);

    private void Awake()
    {
        _relativisticSky = GetComponent<RelativisticSky>();
    }
    
    public void IncrementRapidity()
    {
        Rapidity += increment;
    }
    
    public void DecrementRapidity()
    {
        Rapidity -= increment;
    }
    
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
                <= MaxRapidityM => (SpeedScale.Meters, LightSpeed * Math.Tanh(rapidity)),
                <= MaxRapidityKm => (SpeedScale.Kilometers, LightSpeed * Math.Tanh(rapidity) / 1000.0),
                <= MaxRapidityBeta => (SpeedScale.Beta, Math.Tanh(rapidity)),
                _ => (SpeedScale.OneMinusBeta, LightSpeed * (Math.Exp(-rapidity) / Math.Cosh(rapidity)))
            };
            
            label = rapidity switch
            {
                <= MaxRapidityM => $"{value:F0} m/s",
                <= MaxRapidityKm => $"{value:F0} km/s",
                <= MaxRapidityBeta => $"{value:F2} %",
                _ => $"{value:G3} m/s until light speed"
            };
        }
    }
}
