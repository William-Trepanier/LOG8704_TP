using TMPro;
using UnityEngine;

/// <summary>
/// Affiche des informations de débogage sur l'expérience
/// </summary>
public class DebugInfo : MonoBehaviour
{
    private SpaceShip SpaceShip { get; set; }
    private TextMeshPro _textMeshPro;

    private void Start()
    {
        SpaceShip = GameObject.FindGameObjectWithTag("SpaceShip").GetComponent<SpaceShip>();
        _textMeshPro = GetComponent<TextMeshPro>();
    }
    
    private void Update()
    {
        if (!SpaceShip) return;
        var label = $"Acceleration : {SpaceShip.AccelerationInfo.label}\n";
        label += $"Speed : {SpaceShip.SpeedInfo.label}\n";
        label += $"Odometer : {SpaceShip.DistanceInfo.label}\n";
        label += $"Distance Remaining : {SpaceShip.DestinationInfo.distance.label}\n";
        label += $"Distance Progression : {SpaceShip.DestinationInfo.distanceProgression * 100:F2}%\n";
        label += $"Arrival in : {SpaceShip.DestinationInfo.timeRemaining.label}\n";
        label += $"Ship Time : {SpaceShip.ShipTimeInfo.label}\n";
        label += $"Earth Time : {SpaceShip.EarthTimeInfo.label}";
        _textMeshPro.text = label;
    }
}