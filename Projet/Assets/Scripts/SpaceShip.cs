using System;
using UnityEngine;

/// <summary>
/// Controller de la navette spatiale
/// </summary>
public class SpaceShip : MonoBehaviour
{
    [Header("Thrust")] 
    [Range(-1f, 1f)] public float throttle;
    public float growthFactor = 0.5f;
    public float minStep = 1e-6f;
    public float maxStep = 0.1f;
    
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

    public float Throttle
    {
        get => throttle;
        set
        {
            throttle = value switch
            {
                < -1 => -1,
                > 1 => 1,
                _ => value
            };
            Acceleration = GetRapidityStep(Rapidity) * throttle * Constants.LightSpeed;
        }
    }
    
    public double Beta => Math.Tanh(Rapidity);
    
    public AccelerationInfo AccelerationInfo => new(Acceleration);
    
    public SpeedInfo SpeedInfo => new(Rapidity);
    public DistanceInfo DistanceInfo => new(Distance);
    
    public String ShipTimeLabel => $"{GetLabelFromSeconds(ShipTime)}";
    
    public String EarthTimeLabel => $"{GetLabelFromSeconds(EarthTime)}";
    
    public double ShipTime { get; private set; }
    public double EarthTime { get; private set; }
    public double Distance { get; private set; }
    public double Acceleration { get; private set; }
    
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
        ApplyThrottle();
        ShipTime += Time.deltaTime;
        EarthTime += Math.Cosh(Rapidity) * Time.deltaTime;
        Distance += Constants.LightSpeed * Math.Sinh(Rapidity) * Time.deltaTime;
    }
    
    private void ApplyThrottle()
    {
        if (Mathf.Approximately(throttle, 0f)) return;
        Rapidity += GetRapidityStep(Rapidity) * Throttle * Time.deltaTime;
    }
    
    private double GetRapidityStep(double rapidity) => Math.Clamp(growthFactor * rapidity, minStep, maxStep);
    
    /// <summary>
    /// Augmente la rapidité de la navette spatiale
    /// </summary>
    /// <param name="inUpdate">Vraie si la fonction est appelé à l'intérieur de Update()</param>
    public void IncrementRapidity(bool inUpdate)
    {
        Rapidity += GetRapidityStep(Rapidity) * (inUpdate ? Time.deltaTime : 1);
    }
    
    /// <summary>
    /// Diminue la rapidité de la navette spatiale
    /// </summary>
    /// <param name="inUpdate">Vraie si la fonction est appelé à l'intérieur de Update()</param>
    public void DecrementRapidity(bool inUpdate)
    {
        Rapidity = Math.Max(0f, Rapidity - GetRapidityStep(Rapidity) * (inUpdate ? Time.deltaTime : 1));
    }

    /// <summary>
    /// Retourne une chaîne de caractères représentant une date et une heure à partir d'un nombre de secondes
    /// </summary>
    /// <param name="seconds">Nombre de secondes écoulées depuis 1970</param>
    /// <returns>Chaîne de caractères représentant une date et une heure</returns>
    private string GetLabelFromSeconds(double seconds)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(seconds));
        return dateTimeOffset.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
    }
}
