using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second")]
    public float rotationSpeed = 50f;
    
    [Tooltip("Axis of rotation (normalized automatically)")]
    public Vector3 rotationAxis = Vector3.up;
    
    [Tooltip("Use local space instead of world space")]
    public bool useLocalSpace = true;
    
    [Header("Advanced Options")]
    [Tooltip("Randomize initial rotation on start")]
    public bool randomizeInitialRotation = false;
    
    [Tooltip("Reverse rotation direction")]
    public bool reverseDirection = false;
    
    [Tooltip("Enable rotation")]
    public bool enableRotation = true;
    
    [Header("Time Settings")]
    [Tooltip("Use unscaled time (ignores Time.timeScale)")]
    public bool useUnscaledTime = false;

    private void Start()
    {
        // Normalize the rotation axis
        if (rotationAxis != Vector3.zero)
        {
            rotationAxis.Normalize();
        }
        else
        {
            rotationAxis = Vector3.up;
        }
        
        // Apply random initial rotation if enabled
        if (randomizeInitialRotation)
        {
            if (useLocalSpace)
            {
                transform.localRotation = Random.rotation;
            }
            else
            {
                transform.rotation = Random.rotation;
            }
        }
    }

    private void Update()
    {
        if (!enableRotation || rotationSpeed == 0f)
            return;
        
        // Calculate delta time based on settings
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
        // Calculate rotation amount
        float rotationAmount = rotationSpeed * deltaTime;
        if (reverseDirection)
        {
            rotationAmount = -rotationAmount;
        }
        
        // Apply rotation
        if (useLocalSpace)
        {
            transform.Rotate(rotationAxis, rotationAmount, Space.Self);
        }
        else
        {
            transform.Rotate(rotationAxis, rotationAmount, Space.World);
        }
    }
    
    /// <summary>
    /// Set rotation speed at runtime
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
    
    /// <summary>
    /// Set rotation axis at runtime
    /// </summary>
    public void SetRotationAxis(Vector3 axis)
    {
        rotationAxis = axis.normalized;
    }
    
    /// <summary>
    /// Toggle rotation on/off
    /// </summary>
    public void ToggleRotation()
    {
        enableRotation = !enableRotation;
    }
    
    /// <summary>
    /// Set rotation enabled state
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        enableRotation = enabled;
    }
}
