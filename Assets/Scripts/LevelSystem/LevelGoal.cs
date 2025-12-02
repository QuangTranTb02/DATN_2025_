
using System.Collections;
using UnityEngine;


public class LevelGoal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _requireAllCollectibles = false;
    [SerializeField] private float _completionDelay = 1f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _reachEffect;
    [SerializeField] private Color _activeColor = Color.yellow;
    [SerializeField] private Color _reachedColor = Color.green;

    [Header("Audio")]
    [SerializeField] private AudioClip _goalSound;

    private bool _isActivated = false;

    private void Start()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _activeColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActivated) return;

        if (other.CompareTag("Player"))
        {
            // Check collectibles nếu required
            if (_requireAllCollectibles)
            {
                if (!LevelManager.Instance.HasCollectedAll())
                {
                    Debug.Log("Need to collect all items first!");
                    // TODO: Show notification
                    return;
                }
            }

            ReachGoal(other.gameObject);
        }
    }

    private void ReachGoal(GameObject player)
    {
        _isActivated = true;

        Debug.Log("Goal reached!");

        // Visual
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _reachedColor;
        }

        if (_reachEffect != null)
        {
            _reachEffect.Play();
        }

        // Audio
        if (_goalSound != null)
        {
            AudioSource.PlayClipAtPoint(_goalSound, transform.position);
        }

        // Disable player movement
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Complete level sau delay
        StartCoroutine(CompleteAfterDelay());
    }

    private IEnumerator CompleteAfterDelay()
    {
        yield return new WaitForSeconds(_completionDelay);

        LevelManager.Instance.CompleteLevel();

        // TODO: Show completion UI

        // Auto load next level sau 2 giây
        yield return new WaitForSeconds(2f);
        LevelManager.Instance.LoadNextLevel();
    }
}