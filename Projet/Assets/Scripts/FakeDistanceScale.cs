using System;
using UnityEngine;
using UnityEngine.Events;

public class FakeDistanceScale : MonoBehaviour
{
    [Tooltip("In light speed")] public double actualRadius = 1;
    public DistanceInfo SpaceShipDistance { get; set; }
    
    private double _initialScale;
    private double _scaleFactor;

    private void Awake()
    {
        _initialScale = transform.localScale.x;
    }

    private void Start()
    {
        var distanceLs = SpaceShipDistance.Get(DistanceScale.LightSeconds);
        _scaleFactor = _initialScale * Math.Sqrt(distanceLs * distanceLs - actualRadius);
    }
    
    private void Update()
    {
        var distanceLs = SpaceShipDistance.Get(DistanceScale.LightSeconds);
        var newScale = _scaleFactor / Math.Sqrt(distanceLs * distanceLs - actualRadius);
        transform.localScale = Vector3.one * (float)newScale;
    }
}
