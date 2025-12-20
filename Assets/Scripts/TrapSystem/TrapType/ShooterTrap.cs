using NUnit.Framework;
using System;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEditorInternal.ReorderableList;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ShooterTrap : BaseTrap
{
    [Header("Shooter References")]
    [SerializeField] private Transform _shootPoint; // Điểm bắn
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _player; // Tìm tự động nếu null

    [Header("Targeting")]
    [SerializeField] private TargetingMode _targetingMode = TargetingMode.Fixed;
    [SerializeField] private Vector2 _fixedDirection = Vector2.right;
    [SerializeField] private float _rotationSpeed = 180f; // Độ cho tracking mode
    [SerializeField] private bool _smoothRotation = true;

    [Header("Detection")]
    [SerializeField] private bool _useDetectionRange = true;
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private bool _requireLineOfSight = true;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Firing Settings")]
    [SerializeField] private FireMode _fireMode = FireMode.Single;
    [SerializeField] private float _fireRate = 1f; // Shots per second
    [SerializeField] private int _burstCount = 3; // For burst mode
    [SerializeField] private float _burstDelay = 0.1f; // Delay between shots in burst
    [SerializeField] private float _cooldownTime = 2f; // Thời gian nghỉ sau khi bắn
    [SerializeField] private bool _autoStart = true;

    [Header("Projectile Settings")]
    [SerializeField] private float _projectileSpeed = 15f;
    [SerializeField] private float _spreadAngle = 0f; // For spread mode (degrees)
    [SerializeField] private int _spreadCount = 3; // Number of projectiles in spread

    [Header("Warning")]
    [SerializeField] private bool _useWarning = true;
    [SerializeField] private float _warningDuration = 0.5f;
    [SerializeField] private SpriteRenderer _warningIndicator;
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] private LineRenderer _aimLine; // Laser sight

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private SpriteRenderer _turretSprite;
    [SerializeField] private Transform _turretRotator; // Transform để xoay (nếu có)

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _chargeSound;
    [SerializeField] private AudioClip _shootSound;

    [Header("Debug")]
    [SerializeField] private bool _showDebugGizmos = true;

    // State
    private ShooterState _currentState = ShooterState.Idle;
    private float _nextFireTime = 0f;
    private Vector2 _currentAimDirection;
    private bool _playerInRange = false;
    private bool _hasLineOfSight = false;

    private enum TargetingMode
    {
        Fixed,          // Bắn theo hướng cố định
        TrackPlayer,    // Xoay theo player
        PredictPlayer   // Dự đoán vị trí player (nâng cao)
    }

    private enum FireMode
    {
        Single,   // Bắn 1 viên
        Burst,    // Bắn burst (3-5 viên)
        Spread    // Bắn nhiều viên theo hình quạt
    }

    private enum ShooterState
    {
        Idle,
        Detecting,
        Aiming,
        Charging,
        Firing,
        Cooldown
    }

    private void Start()
    {
        InitializeShooter();

        if (_autoStart)
        {
            StartCoroutine(ShootingCycle());
        }
    }

    private void InitializeShooter()
    {
        // Validate
        if (_shootPoint == null)
        {
            _shootPoint = transform;
            Debug.LogWarning($"[ShooterTrap] {gameObject.name}: Shoot Point not assigned, using self transform.");
        }

        if (_projectilePrefab == null)
        {
            Debug.LogError($"[ShooterTrap] {gameObject.name}: Projectile Prefab not assigned!");
            enabled = false;
            return;
        }

        // Find player
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        // Setup audio
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;

        // Setup warning indicator
        if (_warningIndicator != null)
        {
            _warningIndicator.enabled = false;
        }

        // Setup aim line
        if (_aimLine != null)
        {
            _aimLine.enabled = false;
            _aimLine.startWidth = 0.05f;
            _aimLine.endWidth = 0.05f;
            _aimLine.startColor = _warningColor;
            _aimLine.endColor = new Color(_warningColor.r, _warningColor.g, _warningColor.b, 0f);
        }

        // Initial direction
        _currentAimDirection = _fixedDirection.normalized;
    }

    private void Update()
    {
        UpdateTargeting();
        UpdateWarningVisuals();
    }

    // ===== SHOOTING CYCLE =====

    private IEnumerator ShootingCycle()
    {
        while (true)
        {
            // Wait for detection
            if (_useDetectionRange)
            {
                yield return new WaitUntil(() => IsPlayerDetected());
            }

            // Aim
            _currentState = ShooterState.Aiming;
            yield return new WaitForSeconds(0.2f); // Brief aim time

            // Warning
            if (_useWarning)
            {
                yield return StartCoroutine(WarningPhase());
            }

            // Fire
            yield return StartCoroutine(FirePhase());

            // Cooldown
            _currentState = ShooterState.Cooldown;
            yield return new WaitForSeconds(_cooldownTime);

            _currentState = ShooterState.Idle;

            // If not using detection, wait for fire rate
            if (!_useDetectionRange)
            {
                yield return new WaitForSeconds(1f / _fireRate);
            }
        }
    }

    private IEnumerator WarningPhase()
    {
        _currentState = ShooterState.Charging;

        // Show warning
        if (_warningIndicator != null)
        {
            _warningIndicator.enabled = true;
        }

        if (_aimLine != null)
        {
            _aimLine.enabled = true;
        }

        // Play charge sound
        if (_audioSource != null && _chargeSound != null)
        {
            _audioSource.PlayOneShot(_chargeSound);
        }

        // Blink warning
        float elapsed = 0f;
        while (elapsed < _warningDuration)
        {
            if (_warningIndicator != null)
            {
                float alpha = Mathf.PingPong(elapsed * 8f, 1f);
                Color color = _warningColor;
                color.a = alpha;
                _warningIndicator.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hide warning
        if (_warningIndicator != null)
        {
            _warningIndicator.enabled = false;
        }

        if (_aimLine != null)
        {
            _aimLine.enabled = false;
        }
    }

    private IEnumerator FirePhase()
    {
        _currentState = ShooterState.Firing;

        switch (_fireMode)
        {
            case FireMode.Single:
                FireProjectile(_currentAimDirection);
                break;

            case FireMode.Burst:
                for (int i = 0; i < _burstCount; i++)
                {
                    FireProjectile(_currentAimDirection);
                    yield return new WaitForSeconds(_burstDelay);
                }
                break;

            case FireMode.Spread:
                FireSpread();
                break;
        }

        yield return null;
    }

    // ===== FIRING METHODS =====

    private void FireProjectile(Vector2 direction)
    {
        if (_projectilePrefab == null) return;

        // Spawn projectile
        GameObject projectileObj = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(direction, _projectileSpeed);
        }

        // Muzzle flash
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }

        // Sound
        if (_audioSource != null && _shootSound != null)
        {
            _audioSource.PlayOneShot(_shootSound);
        }

        // Recoil animation (optional)
        if (_turretRotator != null)
        {
            StartCoroutine(RecoilAnimation());
        }
    }

    private void FireSpread()
    {
        float angleStep = _spreadAngle / (_spreadCount - 1);
        float startAngle = -_spreadAngle / 2f;

        for (int i = 0; i < _spreadCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * _currentAimDirection;
            FireProjectile(direction);
        }
    }

    private IEnumerator RecoilAnimation()
    {
        Vector3 originalPos = _turretRotator.localPosition;
        Vector3 recoilPos = originalPos - (Vector3)_currentAimDirection * 0.2f;

        float duration = 0.1f;
        float elapsed = 0f;

        // Recoil back
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _turretRotator.localPosition = Vector3.Lerp(originalPos, recoilPos, t);
            yield return null;
        }

        // Return
        elapsed = 0f;
        while (elapsed < duration * 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 2f);
            _turretRotator.localPosition = Vector3.Lerp(recoilPos, originalPos, t);
            yield return null;
        }

        _turretRotator.localPosition = originalPos;
    }

    // ===== TARGETING =====

    private void UpdateTargeting()
    {
        if (_player == null) return;

        switch (_targetingMode)
        {
            case TargetingMode.Fixed:
                _currentAimDirection = _fixedDirection.normalized;
                break;

            case TargetingMode.TrackPlayer:
                UpdateTrackPlayer();
                break;

            case TargetingMode.PredictPlayer:
                UpdatePredictPlayer();
                break;
        }

        // Update turret rotation
        if (_turretRotator != null)
        {
            float targetAngle = Mathf.Atan2(_currentAimDirection.y, _currentAimDirection.x) * Mathf.Rad2Deg;

            if (_smoothRotation)
            {
                float currentAngle = _turretRotator.eulerAngles.z;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);
                _turretRotator.rotation = Quaternion.Euler(0, 0, newAngle);
            }
            else
            {
                _turretRotator.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }

        // Update aim line
        if (_aimLine != null && _aimLine.enabled)
        {
            _aimLine.SetPosition(0, _shootPoint.position);
            _aimLine.SetPosition(1, _shootPoint.position + (Vector3)_currentAimDirection * 10f);
        }
    }

    private void UpdateTrackPlayer()
    {
        Vector2 directionToPlayer = (_player.position - _shootPoint.position).normalized;

        if (_smoothRotation && _currentState != ShooterState.Idle)
        {
            _currentAimDirection = Vector2.Lerp(_currentAimDirection, directionToPlayer, _rotationSpeed * Time.deltaTime * 0.01f);
        }
        else
        {
            _currentAimDirection = directionToPlayer;
        }
    }

    private void UpdatePredictPlayer()
    {
        // Dự đoán vị trí player
        Rigidbody2D playerRb = _player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 playerVelocity = playerRb.linearVelocity;
            float distance = Vector2.Distance(_shootPoint.position, _player.position);
            float timeToReach = distance / _projectileSpeed;

            Vector2 predictedPosition = (Vector2)_player.position + playerVelocity * timeToReach;
            _currentAimDirection = (predictedPosition - (Vector2)_shootPoint.position).normalized;
        }
        else
        {
            UpdateTrackPlayer(); // Fallback
        }
    }

    // ===== DETECTION =====

    private bool IsPlayerDetected()
    {
        if (_player == null) return false;

        // Check range
        float distance = Vector2.Distance(_shootPoint.position, _player.position);
        _playerInRange = distance <= _detectionRange;

        if (!_playerInRange) return false;

        // Check line of sight
        if (_requireLineOfSight)
        {
            Vector2 directionToPlayer = (_player.position - _shootPoint.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_shootPoint.position, directionToPlayer, distance, _obstacleLayer);

            _hasLineOfSight = (hit.collider == null);
            return _hasLineOfSight;
        }

        return true;
    }

    private void UpdateWarningVisuals()
    {
        // Update warning indicator color based on state
        if (_warningIndicator != null && _warningIndicator.enabled)
        {
            // Handled in WarningPhase
        }
    }

    // ===== PUBLIC METHODS =====

    public void TriggerShoot()
    {
        if (_currentState == ShooterState.Idle || _currentState == ShooterState.Cooldown)
        {
            StartCoroutine(FirePhase());
        }
    }

    // ===== DEBUG =====

    private void OnDrawGizmos()
    {
        if (!_showDebugGizmos) return;

        if (_shootPoint == null) return;

        // Draw detection range
        if (_useDetectionRange)
        {
            Gizmos.color = _playerInRange ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(_shootPoint.position, _detectionRange);
        }

        // Draw aim direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_shootPoint.position, _currentAimDirection * 3f);

        // Draw spread angle
        if (_fireMode == FireMode.Spread && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            float angleStep = _spreadAngle / (_spreadCount - 1);
            float startAngle = -_spreadAngle / 2f;

            for (int i = 0; i < _spreadCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector2 direction = Quaternion.Euler(0, 0, angle) * _currentAimDirection;
                Gizmos.DrawRay(_shootPoint.position, direction * 2f);
            }
        }

        // Draw line of sight
        if (_requireLineOfSight && _player != null && Application.isPlaying)
        {
            Gizmos.color = _hasLineOfSight ? Color.green : Color.red;
            Gizmos.DrawLine(_shootPoint.position, _player.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showDebugGizmos) return;

        // Draw shoot point
        if (_shootPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_shootPoint.position, 0.2f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(_shootPoint.position + Vector3.up * 0.5f, "Shoot Point", new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.yellow },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            });
#endif
        }
    }
}