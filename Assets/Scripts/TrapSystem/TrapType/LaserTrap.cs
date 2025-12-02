using System.Collections;
using UnityEngine;

public class LaserTrap : BaseTrap
{
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _onDuration = 2f;
    [SerializeField] private float _offDuration = 1f;
    [SerializeField] private BoxCollider2D _laserCollider;

    private void Start()
    {
        StartCoroutine(LaserCycle());
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            // Laser ON
            SetLaserActive(true);
            yield return new WaitForSeconds(_onDuration);

            // Laser OFF
            SetLaserActive(false);
            yield return new WaitForSeconds(_offDuration);
        }
    }

    private void SetLaserActive(bool active)
    {
        _isActive = active;

        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = active;
        }

        if (_laserCollider != null)
        {
            _laserCollider.enabled = active;
        }
    }
}