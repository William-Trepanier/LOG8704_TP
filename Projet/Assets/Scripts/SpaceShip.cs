using System;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [Header("Thrust")] 
    public float minStep = 1e-6f;
    public float maxStep = 0.5f;
    
    public double Rapidity
    {
        get => _relativisticSky.rapidity;

        set
        {
            _relativisticSky.rapidity = value switch
            {
                < 0 => 0,
                _ => (float)value
            };
        }
    }
    
    public double Beta => Math.Tanh(Rapidity);
    
    public SpeedInfo SpeedInfo => new(Rapidity);
    public DistanceInfo DistanceInfo => new(Distance);
    
    public String ShipTimeLabel => $"{GetLabelFromSeconds(ShipTime)}";
    
    public String EarthTimeLabel => $"{GetLabelFromSeconds(EarthTime)}";
    
    public double ShipTime { get; private set; }
    public double EarthTime { get; private set; }
    public double Distance { get; private set; }
    
    private RelativisticSky _relativisticSky;

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
    
    private double GetRapidityStep(double rapidity) => Math.Clamp(rapidity, minStep, maxStep);
    
    public void IncrementRapidity(bool inUpdate)
    {
        Rapidity += GetRapidityStep(Rapidity) * (inUpdate ? Time.deltaTime : 1);
    }
    
    public void DecrementRapidity(bool inUpdate)
    {
        Rapidity = Math.Max(0f, Rapidity - GetRapidityStep(Rapidity) * (inUpdate ? Time.deltaTime : 1));
    }

    private string GetLabelFromSeconds(double seconds)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(seconds));
        return dateTimeOffset.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
    }
}
