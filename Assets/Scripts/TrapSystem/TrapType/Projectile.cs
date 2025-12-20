using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private bool _useGravity = false;
    [SerializeField] private float _gravityScale = 1f;

    [Header("Lifetime")]
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private bool _destroyOnHit = true;

    [Header("Visual")]
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private ParticleSystem _hitEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip _hitSound;

    private Vector2 _direction;
    private Rigidbody2D _rb;
    private float _spawnTime;
    private bool _hasHit = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Config Rigidbody2D
        _rb.gravityScale = _useGravity ? _gravityScale : 0f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        _spawnTime = Time.time;
    }

    private void Update()
    {
        // Check lifetime
        if (Time.time - _spawnTime >= _lifetime)
        {
            DestroyProjectile();
        }
    }

    private void FixedUpdate()
    {
        if (!_hasHit)
        {
            // Move projectile
            _rb.linearVelocity = _direction * _speed;
        }
    }

    public void Initialize(Vector2 direction, float speed = -1f)
    {
        _direction = direction.normalized;

        if (speed > 0)
        {
            _speed = speed;
        }

        // Rotate to face direction
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        // Check if hit player
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && !health.IsDead())
            {
                health.TakeDamage();
                Debug.Log("Projectile hit player!");
            }

            OnHit(other);
        }
        // Check if hit obstacle
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            OnHit(other);
        }
    }

    private void OnHit(Collider2D other)
    {
        _hasHit = true;

        // Stop movement
        _rb.linearVelocity = Vector2.zero;
        _rb.isKinematic = true;

        // Play hit effect
        if (_hitEffect != null)
        {
            ParticleSystem effect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }

        // Play sound
        if (_hitSound != null)
        {
            AudioSource.PlayClipAtPoint(_hitSound, transform.position);
        }

        // Destroy projectile
        if (_destroyOnHit)
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        // Detach trail
        if (_trail != null)
        {
            _trail.transform.SetParent(null);
            Destroy(_trail.gameObject, _trail.time);
        }

        Destroy(gameObject);
    }

    // Public method để set speed sau khi spawn
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}