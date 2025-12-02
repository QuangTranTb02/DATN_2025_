using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _inactiveColor = Color.gray;
    [SerializeField] private Color _activeColor = Color.green;
    [SerializeField] private ParticleSystem _activationEffect; // Optional

    [Header("Audio")]
    [SerializeField] private AudioClip _activationSound; // Optional

    private bool _isActivated = false;

    private void Start()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _inactiveColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActivated) return; // Đã kích hoạt rồi thì không làm gì

        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        _isActivated = true;

        // Lưu checkpoint
        CheckpointManager.Instance.SetCheckpoint(transform.position);

        // Visual feedback
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _activeColor;
        }

        if (_activationEffect != null)
        {
            _activationEffect.Play();
        }

        // Audio feedback
        if (_activationSound != null)
        {
            AudioSource.PlayClipAtPoint(_activationSound, transform.position);
        }
    }

    // Để reset checkpoint khi restart level
    public void ResetCheckpoint()
    {
        _isActivated = false;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _inactiveColor;
        }
    }
}