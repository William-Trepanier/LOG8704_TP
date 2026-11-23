using TMPro;
using UnityEngine;

/// <summary>
/// Affiche des informations de débogage sur l'expérience
/// </summary>
public class DebugInfo : MonoBehaviour
{
    private SpaceShip _spaceShip;
    private TextMeshPro _textMeshPro;

    private void Start()
    {
        _spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").GetComponent<SpaceShip>();
        _textMeshPro = GetComponent<TextMeshPro>();
    }
    
    private void Update()
    {
        if (!_spaceShip) return;
        var label = $"Acceleration : {_spaceShip.AccelerationInfo.label}\n";
        label += $"Speed : {_spaceShip.SpeedInfo.label}\n";
        label += $"Odometer : {_spaceShip.DistanceInfo.label}\n";
        label += $"Distance Remaining : {_spaceShip.DestinationInfo.distance.label}\n";
        label += $"Distance Progression : {_spaceShip.DestinationInfo.distanceProgression * 100:F2}%\n";
        label += $"Arrival in : {_spaceShip.DestinationInfo.timeRemaining.label}\n";
        label += $"Ship Time : {_spaceShip.ShipTimeInfo.label}\n";
        label += $"Earth Time : {_spaceShip.EarthTimeInfo.label}";
        _textMeshPro.text = label;
    }
}