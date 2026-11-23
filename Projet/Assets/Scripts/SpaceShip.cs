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

    [Header("Destination")] 
    [Tooltip("Distance in light seconds")] public float destinationDistance = 134009348.4f;

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
        }
    }
    
    private double Rapidity
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
    
    private double _acceleration;
    public AccelerationInfo AccelerationInfo => new(_acceleration);
    
    public SpeedInfo SpeedInfo => new(Rapidity);

    private double _distance;
    public DistanceInfo DistanceInfo => new(_distance);

    private double _shipTime;
    public TimeInfo ShipTimeInfo => new(_shipTime);

    private double _earthTime;
    public TimeInfo EarthTimeInfo => new(_earthTime);
    
    public DestinationInfo DestinationInfo {
        get
        {
            var destinationDistanceM = DistanceInfo.Convert(destinationDistance, DistanceScale.LightSeconds, 
                DistanceScale.Meters);

            var deltaX = destinationDistanceM - _distance;
            var remainingDistance = new DistanceInfo(deltaX);
            var progression = _distance / destinationDistanceM;
            
            var speed = SpeedInfo.Get(SpeedScale.Meters);

            TimeInfo remainingTime;
            if (Mathf.Approximately((float)speed, 0) && Mathf.Approximately((float)_acceleration, 0))
            {
                remainingTime = new TimeInfo();
            }
            else
            {
                var squareRoot = Math.Sqrt(speed * speed + 2 * _acceleration * deltaX);
                var travelTime = Math.Max((-speed - squareRoot) / _acceleration, (-speed + squareRoot) / _acceleration);
                remainingTime = new TimeInfo(_earthTime, travelTime);
            }
            
            return new DestinationInfo(remainingDistance, remainingTime, (float)progression);
        }
    }
    
    private RelativisticSky _relativisticSky;

    private void Awake()
    {
        _relativisticSky = GetComponent<RelativisticSky>();
    }

    private void Start()
    {
        _shipTime = _earthTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private void Update()
    {
        ApplyThrottle();
        _shipTime += Time.deltaTime;
        _earthTime += Math.Cosh(Rapidity) * Time.deltaTime;
        _distance += Constants.LightSpeed * Math.Sinh(Rapidity) * Time.deltaTime;
    }
    
    private void ApplyThrottle()
    {
        _acceleration = GetRapidityStep(Rapidity) * throttle * Constants.LightSpeed;
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
