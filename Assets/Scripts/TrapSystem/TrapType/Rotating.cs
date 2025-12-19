using UnityEngine;

public class Rotating : BaseTrap
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}