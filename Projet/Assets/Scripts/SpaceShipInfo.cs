using System;
using TMPro;
using UnityEngine;

public class SpaceShipInfo : MonoBehaviour
{
    public TextMeshProUGUI distanceSun;
    public TextMeshProUGUI distanceProxima;
    
    private SpaceShip _spaceShip;

    private void Start()
    {
        _spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").GetComponent<SpaceShip>();
    }

    void Update()
    {
        distanceSun.text = _spaceShip.DistanceInfo.label;
        distanceProxima.text = _spaceShip.DestinationInfo.distance.label;
    }
}
