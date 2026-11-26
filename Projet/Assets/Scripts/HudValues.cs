using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HudValues : MonoBehaviour
{
    [Header("Values (the big texts, not the labels)")]
    public TextMeshProUGUI speed;
    public TextMeshProUGUI shipTime;
    public TextMeshProUGUI earthTime;
    public TextMeshProUGUI elapsedShip;

    [Header("Optional")]
    public TextMeshProUGUI progressText;
    public Image progressFill;
    public TextMeshProUGUI infoText;

    private SpaceShip _ship;
    private double _startShip;

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag("SpaceShip");
        if (go) _ship = go.GetComponent<SpaceShip>();
        if (!_ship)
        {
            Debug.LogWarning("HudValues: no object with tag 'SpaceShip' found.", this);
            enabled = false;
            return;
        }
        _startShip = _ship.ShipTimeInfo.seconds;
    }

    private void Update()
    {
        var ship = _ship.ShipTimeInfo;
        var earth = _ship.EarthTimeInfo;
        var dest = _ship.DestinationInfo;

        if (speed) speed.text = _ship.SpeedInfo.label;
        if (shipTime) shipTime.text = ship.label;
        if (earthTime) earthTime.text = earth.label;
        if (elapsedShip) elapsedShip.text = new TimeInfo(_startShip, ship.seconds - _startShip).label;

        var progress = Mathf.Clamp01(dest.distanceProgression);
        if (progressFill) progressFill.fillAmount = progress;
        if (progressText) progressText.text = $"{progress * 100f:F2}%";

        if (infoText)
        {
            var c = (float)_ship.SpeedInfo.Get(SpeedScale.Beta); 
            infoText.text =
                c < 1 ? "No visible effect on the stars. You feel motionless." :
                c < 10 ? "Earth drifts away. The stars slowly scroll past." :
                c < 30 ? "Aberration pushes the stars forward. Red stars turn orange and white." :
                c < 85 ? "Aberration grows. At 50% the stars sit within 60° ahead. Colors shift to blue." :
                c < 99 ? "Stars condense into a disc ahead. The ship clock lags Earth's clock." :
                         "Stars leave the visible spectrum. Cosmic background radiation forms a bright disc.";
        }
    }
}
