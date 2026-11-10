using TMPro;
using UnityEngine;

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
        string label = $"Speed : {SpaceShip.SpeedInfo.label}\n";
        label += $"Distance : {SpaceShip.DistanceInfo.label}\n";
        label += $"Ship Time : {SpaceShip.ShipTimeLabel}\n";
        label += $"Earth Time : {SpaceShip.EarthTimeLabel}";
        _textMeshPro.text = label;
    }
}