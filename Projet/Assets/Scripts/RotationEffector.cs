using System;
using UnityEngine;
using UnityEngine.Events;

public class RotationEffector : MonoBehaviour
{
    [Range(0, 360)] public float minAngle = 275f;
    [Range(0, 360)] public float maxAngle = 25f;
    public float minValue = -1f;
    public float maxValue = 1f;
    public UnityEvent<float> onRotationChanged = new();
    [SerializeField] private int observedAngle;

    private float _lastAngle;
    private float _effectiveMaxAngle;

    private void Start()
    {
        _lastAngle = GetAngle();
        _effectiveMaxAngle = GetAngleOffset(maxAngle);
    }

    private void Update()
    {
        if (Mathf.Approximately(_lastAngle, GetAngle()))
            return;

        _lastAngle = GetAngle();
        onRotationChanged.Invoke(GetValueFromAngle(_lastAngle));
    }

    private float GetAngle()
    {
        var angle = observedAngle switch
        {
            0 => transform.eulerAngles.x,
            1 => transform.eulerAngles.y,
            2 => transform.eulerAngles.z,
            _ => throw new ArgumentOutOfRangeException()
        };

        return GetAngleOffset(angle);
    }
    
    private float GetValueFromAngle(float angle)
    {
        return angle / _effectiveMaxAngle * (maxValue - minValue) + minValue;
    }

    private float GetAngleOffset(float angle)
    {
        return (angle - minAngle + 360) % 360;
    }
}
