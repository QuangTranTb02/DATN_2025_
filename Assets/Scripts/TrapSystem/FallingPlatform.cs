using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float _fallDelay = 0.5f;
    [SerializeField] private float _respawnDelay = 3f;
    [SerializeField] private float _shakeAmount = 0.1f;
    [SerializeField] private float _shakeDuration = 0.3f;

    private Vector2 _originalPosition;
    private Rigidbody2D _rb;
    private bool _isFalling = false;
    private Collider2D _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _originalPosition = transform.position;

        if (_rb != null)
        {
            _rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isFalling)
        {
            StartCoroutine(FallSequence());
        }
    }

    private IEnumerator FallSequence()
    {
        _isFalling = true;

        // Shake effect
        yield return StartCoroutine(ShakeCoroutine());

        // Wait before falling
        yield return new WaitForSeconds(_fallDelay);

        // Start falling
        if (_rb != null)
        {
            _rb.isKinematic = false;
        }

        // Wait before respawn
        yield return new WaitForSeconds(_respawnDelay);

        // Respawn
        ResetPlatform();
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < _shakeDuration)
        {
            float offsetX = Random.Range(-_shakeAmount, _shakeAmount);
            float offsetY = Random.Range(-_shakeAmount, _shakeAmount);

            transform.position = _originalPosition + new Vector2(offsetX, offsetY);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _originalPosition;
    }

    private void ResetPlatform()
    {
        transform.position = _originalPosition;

        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.linearVelocity = Vector2.zero;
        }

        _isFalling = false;
    }
}