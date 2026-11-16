using System;
using UnityEngine;
using UnityEngine.Events;

public class RotationEffector : MonoBehaviour
{
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float minValue = -1f;
    public float maxValue = 1f;
    public UnityEvent<float> onRotationChanged = new();
    [SerializeField] private int observedAngle;

    private float _lastAngle;

    private void Start()
    {
        _lastAngle = GetAngle();
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
        return observedAngle switch
        {
            0 => transform.eulerAngles.x,
            1 => transform.eulerAngles.y,
            2 => transform.eulerAngles.z,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private float GetValueFromAngle(float angle)
    {
        return (angle - minAngle) / (maxAngle - minAngle) * (maxValue - minValue) + minValue;
    }
}
