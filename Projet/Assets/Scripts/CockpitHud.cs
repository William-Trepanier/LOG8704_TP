using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tableau de bord affiché sur l'écran existant du cockpit (canvas SpaceShipInfoUI) :
/// barre de progression, vitesse, heure Terre / heure à bord (avec date), temps écoulé
/// depuis le début du voyage et panneau d'information qui explique ce qui se passe
/// à chaque échelle de vitesse (selon la proposition de projet).
/// </summary>
public class CockpitHud : MonoBehaviour
{
    public RectTransform targetCanvas;
    public string targetCanvasName = "SpaceShipInfoUI";
    public Vector2 hudAreaCenter = new(0, 0);
    public Vector2 hudAreaSize = new(1840, 1000);
    public float refreshInterval = 0.1f;

    private static readonly Vector2 LayoutSize = new(920, 500);

    private static readonly (float upTo, string title, string text)[] Tiers =
    {
        (1f, "0 – 1 % of c",
            "No visible effect on the stars. You feel motionless."),
        (10f, "1 – 10 % of c",
            "Earth drifts away. The stars and celestial bodies slowly start to scroll past."),
        (30f, "10 – 30 % of c",
            "First relativistic effect: aberration shifts the stars toward the front, which feels like moving backwards. " +
            "The stars bunch up ahead (6° to 20°). First Doppler effect: red stars turn orange and white."),
        (85f, "30 – 85 % of c",
            "Aberration gets stronger. At 50 % the stars are packed within 60° in front of the ship. " +
            "The color of the stars shifts toward blue."),
        (99f, "85 – 99 % of c",
            "The stars condense into a disc ahead of the ship. The ship clock now lags clearly behind the clock on Earth."),
        (float.PositiveInfinity, "99 % of c and beyond",
            "The stars vanish: they leave the visible spectrum. Doppler-shifted cosmic background radiation becomes visible " +
            "as a disc that grows brighter and brighter. Light-years pass in only minutes of ship time thanks to time dilation.")
    };

    private SpaceShip _ship;
    private TMP_FontAsset _font;
    private Color _accent = new(0.2f, 0.8f, 1f, 1f);

    private Image _progressFill;
    private TextMeshProUGUI _progressLabel, _clockText, _tierTitle, _tierText, _sunValue, _proximaValue;

    private double _startShip, _startEarth;
    private float _timer;

    private void Start()
    {
        var shipGo = GameObject.FindGameObjectWithTag("SpaceShip");
        if (shipGo) _ship = shipGo.GetComponent<SpaceShip>();

        if (!targetCanvas)
        {
            var found = GameObject.Find(targetCanvasName);
            if (found) targetCanvas = found.GetComponent<RectTransform>();
        }

        if (!_ship || !targetCanvas)
        {
            Debug.LogWarning("CockpitHud: SpaceShip (tag 'SpaceShip') or target canvas not found.", this);
            enabled = false;
            return;
        }

        var existing = targetCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
        if (existing)
        {
            _font = existing.font;
            _accent = existing.color;
        }
        else
        {
            _font = TMP_Settings.defaultFontAsset;
        }

        foreach (var t in targetCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
            t.gameObject.SetActive(false);

        _startShip = _ship.ShipTimeInfo.seconds;
        _startEarth = _ship.EarthTimeInfo.seconds;

        BuildUi();
        Refresh();
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < refreshInterval) return;
        _timer = 0f;
        Refresh();
    }

    private void Refresh()
    {
        var destination = _ship.DestinationInfo;
        var progress = Mathf.Clamp01(destination.distanceProgression);
        _sunValue.text = _ship.DistanceInfo.label;
        _proximaValue.text = destination.distance.label;
        _progressFill.fillAmount = progress;
        _progressLabel.text = $"Trip progress : {progress * 100f:F2}%";

        var shipTime = _ship.ShipTimeInfo;
        var earthTime = _ship.EarthTimeInfo;
        _clockText.text =
            $"Speed : {_ship.SpeedInfo.label}\n" +
            $"Earth time : {earthTime.label}\n" +
            $"Ship time : {shipTime.label}\n" +
            $"Elapsed (Earth) : {new TimeInfo(_startEarth, earthTime.seconds - _startEarth).label}\n" +
            $"Elapsed (ship) : {new TimeInfo(_startShip, shipTime.seconds - _startShip).label}";

        var beta = (float)_ship.SpeedInfo.Get(SpeedScale.Beta); // en % de c
        foreach (var tier in Tiers)
        {
            if (beta >= tier.upTo) continue;
            _tierTitle.text = tier.title;
            _tierText.text = tier.text;
            break;
        }
    }


    private void BuildUi()
    {
        var host = new GameObject("CockpitHudContainer", typeof(RectTransform));
        var rt = host.GetComponent<RectTransform>();
        rt.SetParent(targetCanvas, false);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = hudAreaCenter;
        rt.sizeDelta = LayoutSize;
        var fit = Mathf.Min(hudAreaSize.x / LayoutSize.x, hudAreaSize.y / LayoutSize.y);
        rt.localScale = Vector3.one * fit;

        var back = NewImage("ProgressBack", rt, new Color(1, 1, 1, 0.12f));
        Place(back.rectTransform, 10, -10, 900, 28);
        _progressFill = NewImage("ProgressFill", back.rectTransform, _accent);
        Stretch(_progressFill.rectTransform);
        _progressFill.sprite = WhiteSprite();
        _progressFill.type = Image.Type.Filled;
        _progressFill.fillMethod = Image.FillMethod.Horizontal;
        _progressFill.fillAmount = 0f;

        _progressLabel = NewText("ProgressText", rt, 24, TextAlignmentOptions.Left);
        Place(_progressLabel.rectTransform, 10, -42, 900, 30);

        var sunLabel = NewText("SunLabel", rt, 26, TextAlignmentOptions.TopLeft);
        sunLabel.text = "FROM THE SUN";
        Place(sunLabel.rectTransform, 10, -82, 440, 34);
        _sunValue = NewText("SunValue", rt, 38, TextAlignmentOptions.TopLeft);
        Place(_sunValue.rectTransform, 10, -116, 440, 50);

        var proximaLabel = NewText("ProximaLabel", rt, 26, TextAlignmentOptions.TopRight);
        proximaLabel.text = "TO PROXIMA";
        Place(proximaLabel.rectTransform, 470, -82, 440, 34);
        _proximaValue = NewText("ProximaValue", rt, 38, TextAlignmentOptions.TopRight);
        Place(_proximaValue.rectTransform, 470, -116, 440, 50);

        var clocks = NewImage("Clocks", rt, new Color(0, 0, 0, 0.25f));
        Place(clocks.rectTransform, 10, -180, 440, 310);
        _clockText = NewText("ClocksText", clocks.rectTransform, 26, TextAlignmentOptions.TopLeft);
        Stretch(_clockText.rectTransform, 12);
        _clockText.enableAutoSizing = true;
        _clockText.fontSizeMin = 14;
        _clockText.fontSizeMax = 26;
        _clockText.lineSpacing = 30;

        var info = NewImage("Info", rt, new Color(0, 0, 0, 0.25f));
        Place(info.rectTransform, 470, -180, 440, 310);
        _tierTitle = NewText("InfoTitle", info.rectTransform, 28, TextAlignmentOptions.TopLeft);
        Place(_tierTitle.rectTransform, 12, -10, 416, 36);
        _tierText = NewText("InfoText", info.rectTransform, 24, TextAlignmentOptions.TopLeft);
        Place(_tierText.rectTransform, 12, -54, 416, 248);
        _tierText.enableAutoSizing = true;
        _tierText.fontSizeMin = 14;
        _tierText.fontSizeMax = 26;
    }

    private Image NewImage(string name, RectTransform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private TextMeshProUGUI NewText(string name, RectTransform parent, float size, TextAlignmentOptions align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<TextMeshProUGUI>();
        if (_font) t.font = _font;
        t.fontSize = size;
        t.alignment = align;
        t.color = _accent;
        t.raycastTarget = false;
        return t;
    }

    private static Sprite _white;
    private static Sprite WhiteSprite()
    {
        if (_white) return _white;
        var tex = Texture2D.whiteTexture;
        _white = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return _white;
    }

    private static void Place(RectTransform r, float x, float y, float w, float h)
    {
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
        r.anchoredPosition = new Vector2(x, y);
        r.sizeDelta = new Vector2(w, h);
    }

    private static void Stretch(RectTransform r, float margin = 0)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = new Vector2(margin, margin);
        r.offsetMax = new Vector2(-margin, -margin);
    }
}
