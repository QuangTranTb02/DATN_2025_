using UnityEngine;

public class SpikeTrap : BaseTrap
{
    [Header("Spike Visual")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _activeColor = Color.red;
    [SerializeField] private Color _inactiveColor = Color.gray;

    private void Start()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _isActive ? _activeColor : _inactiveColor;
        }
    }

    protected override void OnPlayerHit(PlayerHealth playerHealth)
    {
        base.OnPlayerHit(playerHealth);
        // Có thể thêm effect riêng cho spike
    }
}