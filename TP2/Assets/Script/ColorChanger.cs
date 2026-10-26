using System;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private MeshRenderer _meshRenderer;
    private Transform _camera;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (!Camera.main)
        {
            Debug.LogError("No main camera found");
            return;
        }
        
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (!_camera) return;
        var angle = Vector3.SignedAngle(Vector3.up, _camera.up, _camera.forward);
        var hue = Mathf.Repeat(angle, 360f) / 360f;
        _meshRenderer.material.SetColor(BaseColor, Color.HSVToRGB(hue, 1f, 1f));
    }
}
