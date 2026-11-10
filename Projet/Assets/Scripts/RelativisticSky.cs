using UnityEngine;

/// <summary>
/// Gère la modification du shader du ciel selon la rapidité
/// </summary>
[ExecuteAlways]
public class RelativisticSky : MonoBehaviour
{
    [Header("Whiteout flash")] public Renderer flashQuad;
    public float maxFlash = 0.8f;
    public float whiteTemperature = 6500f;
    public float cmbTemperature = 300f;
    public float flashStart = 3000f;
    public float flashFull = 8000f;
    public float flashFadeStart = 15000f;
    public float flashFadeEnd = 60000f;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int FlashDirId = Shader.PropertyToID("_FlashDir");
    private MaterialPropertyBlock _materialPropertyBlock;

    public Material skyMaterial;
    [Range(0f, 12f)] public float rapidity;
    private static readonly int RapidityId = Shader.PropertyToID("_Rapidity");
    public Vector3 velocityDirection = Vector3.forward;
    private static readonly int VelId = Shader.PropertyToID("_Velocity");

    private void Update()
    {
        if (!skyMaterial) return;
        var normalizedVelocityDir = velocityDirection.sqrMagnitude > 1e-5f
            ? velocityDirection.normalized
            : Vector3.forward;
        skyMaterial.SetVector(VelId, normalizedVelocityDir);
        skyMaterial.SetFloat(RapidityId, rapidity);
        UpdateFlash(normalizedVelocityDir);
    }

    private void UpdateFlash(Vector3 normalizedVelocityDir)
    {
        if (!flashQuad) return;
        var temperatureForward = Mathf.Exp(rapidity) * cmbTemperature;
        var rise = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(flashStart, flashFull, temperatureForward));
        var fall = 1f - Mathf.SmoothStep(0f, 1f,
            Mathf.InverseLerp(flashFadeStart, flashFadeEnd, temperatureForward));
        var intensity = rise * fall;
        intensity *= maxFlash;

        _materialPropertyBlock ??= new MaterialPropertyBlock();
        flashQuad.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetVector(FlashDirId, normalizedVelocityDir);
        _materialPropertyBlock.SetColor(ColorId, BlackbodyColor(temperatureForward));
        _materialPropertyBlock.SetFloat(IntensityId, intensity);
        flashQuad.SetPropertyBlock(_materialPropertyBlock);
        flashQuad.enabled = intensity > 0.001f;
    }

    // Même fonction que le shader RelativisticSkybox
    private float PlanckRelative(float lambda, float T)
    {
        var x = 14388f / (lambda * T);
        return x > 80f ? 0f : 1f / (Mathf.Exp(x) - 1f); // Si x > 80, le résultat est pratiquement 0
    }

    // Même fonction que le shader RelativisticSkybox
    private Color BlackbodyColor(float T)
    {
        const float r = 0.62f, g = 0.54f, b = 0.46f; // Longueur d'onde des couleurs primaires
        var c = new Color(
            PlanckRelative(r, T) / PlanckRelative(r, whiteTemperature),
            PlanckRelative(g, T) / PlanckRelative(g, whiteTemperature),
            PlanckRelative(b, T) / PlanckRelative(b, whiteTemperature),
            1f);

        // Normalisation de la couleur
        var maxColor = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
        return maxColor > 0f ? new Color(c.r / maxColor, c.g / maxColor, c.b / maxColor, 1f) : Color.white;
    }
}