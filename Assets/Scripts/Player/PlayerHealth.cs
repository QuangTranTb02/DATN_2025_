using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float _respawnDelay = 1f;
    [SerializeField] private bool _invincibleAfterRespawn = true;
    [SerializeField] private float _invincibilityDuration = 2f;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _deathEffect; // Optional
    [SerializeField] private GameObject _visualModel; // Để ẩn khi chết

    [Header("Audio")]
    [SerializeField] private AudioClip _deathSound;

    private bool _isDead = false;
    private bool _isInvincible = false;

    private PlayerMovement _playerMovement;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage()
    {
        if (_isDead || _isInvincible) return;

        Die();
    }

    private void Die()
    {
        _isDead = true;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterPlayerDeath();
        }
        // Disable movement
        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }

        // Play effects
        if (_deathEffect != null)
        {
            _deathEffect.Play();
        }

        if (_deathSound != null)
        {
            AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        }

        // Ẩn model
        if (_visualModel != null)
        {
            _visualModel.SetActive(false);
        }

        // Stop physics
        _rb.linearVelocity = Vector2.zero;
        _rb.isKinematic = true;

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnDelay);

        Respawn();
    }

    private void Respawn()
    {
        // Lấy vị trí checkpoint
        Vector2 respawnPosition = CheckpointManager.Instance.GetLastCheckpoint();

        // Teleport player
        transform.position = respawnPosition;

        // Reset physics
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector2.zero;

        // Show model
        if (_visualModel != null)
        {
            _visualModel.SetActive(true);
        }

        // Enable movement
        if (_playerMovement != null)
        {
            _playerMovement.enabled = true;
            _playerMovement.Velocity = Vector2.zero; // Reset velocity
        }

        _isDead = false;

        // Invincibility frames
        if (_invincibleAfterRespawn)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincible = true;

        // Blinking effect
        float blinkInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < _invincibilityDuration)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Ensure sprite is visible
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = true;
        }

        _isInvincible = false;
    }

    public bool IsDead() => _isDead;
    public bool IsInvincible() => _isInvincible;
}