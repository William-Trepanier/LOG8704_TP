using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
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
    
    public SpeedInfo SpeedInfo => new SpeedInfo(Rapidity);
    public DistanceInfo DistanceInfo => new DistanceInfo(Distance);
    
    public String ShipTimeLabel => $"{ShipTime}";
    
    public String EarthTimeLabel => $"{EarthTime}";
    
    public double ShipTime { get; private set; }
    public double EarthTime { get; private set; }
    public double Distance { get; private set; }

    private void Awake()
    {
        _relativisticSky = GetComponent<RelativisticSky>();
    }

    private void Start()
    {
        ShipTime = EarthTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private void Update()
    {
        ShipTime += Time.deltaTime;
        EarthTime += Math.Cosh(Rapidity) * Time.deltaTime;
        Distance += Constants.LightSpeed * Math.Sinh(Rapidity) * Time.deltaTime;
    }
    
    public void IncrementRapidity()
    {
        Rapidity += increment;
    }
    
    public void DecrementRapidity()
    {
        Rapidity -= increment;
    }

    private string GetLabelFromSeconds(double seconds)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(seconds));
        return dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
