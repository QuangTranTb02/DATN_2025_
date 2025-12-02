using UnityEngine;

public abstract class BaseTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] protected bool _isActive = true;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                OnPlayerHit(playerHealth);
            }
        }
    }

    protected virtual void OnPlayerHit(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage();
    }

    public void SetActive(bool active)
    {
        _isActive = active;
    }
}