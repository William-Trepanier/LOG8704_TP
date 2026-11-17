using System;
using UnityEngine;

public class SimpleAnimation : MonoBehaviour
{
    public GameObject objectToRotate;
    public float clickDuration = 0.25f;
    public float durationInSeconds = 1f;

    private float _clickTime;
    private Quaternion _initialRotation;
    private float _deltaAngle;
    private bool _isPlaying;

    public void PrePlay()
    {
        if (_isPlaying) return;
        _clickTime = Time.time;
    }
    
    public void Play()
    {
        if (_isPlaying) return;
        if (Time.time - _clickTime > clickDuration) return;
        
        _isPlaying = true;
        _deltaAngle = 0;
        _initialRotation = objectToRotate.transform.rotation;
    }
    
    private void Awake()
    {
        if (!objectToRotate)
            objectToRotate = gameObject;
    }

    private void Update()
    {
        if (!_isPlaying) return;
        
        var anglePerSecond = 360f / durationInSeconds;
        var rotation = anglePerSecond * Time.deltaTime;
        _deltaAngle += rotation;
        
        if (_deltaAngle >= 360f)
        {
            objectToRotate.transform.rotation = _initialRotation;
            _isPlaying = false;
            return;
        }

        objectToRotate.transform.Rotate(Vector3.up * rotation);
    }
}
