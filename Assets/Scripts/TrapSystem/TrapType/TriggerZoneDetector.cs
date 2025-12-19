using UnityEngine;

public class TriggerZoneDetector : MonoBehaviour
{
    [SerializeField] private CrushingPlatform _crushingPlatform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_crushingPlatform != null)
            {
                _crushingPlatform.OnPlayerEnterTrigger();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_crushingPlatform != null)
            {
                _crushingPlatform.OnPlayerExitTrigger();
            }
        }
    }
}