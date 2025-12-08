using System.Collections;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Rendering.CameraUI;
using static UnityEngine.AudioSettings;
using static UnityEngine.Rendering.STP;

public class LaserTrap : BaseTrap
{
    [Header("Laser Settings")]
    [SerializeField] private Transform _laserStart; // Điểm bắt đầu
    [SerializeField] private Transform _laserEnd;   // Điểm kết thúc
    [SerializeField] private LayerMask _obstacleLayer; // Layer của tường (để laser dừng lại)

    [Header("Timing")]
    [SerializeField] private float _onDuration = 2f;     // Thời gian laser BẬT
    [SerializeField] private float _offDuration = 1f;    // Thời gian laser TẮT
    [SerializeField] private float _warningDuration = 0.5f; // Thời gian cảnh báo
    [SerializeField] private bool _startActive = true;   // Bắt đầu với trạng thái bật

    [Header("Visual - LineRenderer")]
    [SerializeField] private LineRenderer _laserLine;
    [SerializeField] private float _laserWidth = 0.1f;
    [SerializeField] private Color _laserColor = Color.red;
    [SerializeField] private Color _warningColor = Color.yellow;

    [Header("Visual - Glow Effect")]
    [SerializeField] private LineRenderer _glowLine; // Đường glow ngoài
    [SerializeField] private float _glowWidth = 0.3f;
    [SerializeField] private Color _glowColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    [Header("Collider")]
    [SerializeField] private BoxCollider2D _laserCollider;
    [SerializeField] private float _colliderThickness = 0.1f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _startParticles;  // Particles ở đầu laser
    [SerializeField] private ParticleSystem _endParticles;    // Particles ở cuối laser

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _warningSound;
    [SerializeField] private AudioClip _activeSound; // Laser hum sound (loop)
    [SerializeField] private AudioClip _deactivateSound;

    [Header("Special Modes")]
    [SerializeField] private bool _alwaysOn = false;

    [SerializeField] private Transform _player;
    [SerializeField] private float _rotationSpeed = 90f;


    [Header("Debug")]
    [SerializeField] private bool _showDebugGizmos = true;

    // State
    private LaserState _currentState = LaserState.Off;
    private float _laserLength;
    private Vector2 _laserDirection;

    private enum LaserState
    {
        Off,
        Warning,
        On
    }

    private void Start()
    {
        InitializeLaser();
        StartCoroutine(LaserCycle());
    }

    private void InitializeLaser()
    {
        // Validate references
        if (_laserStart == null || _laserEnd == null)
        {
            Debug.LogError($"[LaserTrap] {gameObject.name}: LaserStart or LaserEnd not assigned!");
            enabled = false;
            return;
        }

        // Setup LineRenderer nếu không có
        if (_laserLine == null)
        {
            _laserLine = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer(_laserLine, _laserWidth, _laserColor);

        // Setup glow line
        if (_glowLine != null)
        {
            SetupLineRenderer(_glowLine, _glowWidth, _glowColor);
        }

        // Setup collider nếu không có
        if (_laserCollider == null)
        {
            _laserCollider = gameObject.AddComponent<BoxCollider2D>();
            _laserCollider.isTrigger = true;
        }

        // Setup audio
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;

        // Calculate laser direction
        _laserDirection = (_laserEnd.position - _laserStart.position).normalized;

        // Start state
        if (!_startActive)
        {
            SetLaserState(LaserState.Off);
        }
    }

    private void SetupLineRenderer(LineRenderer line, float width, Color color)
    {
        line.startWidth = width;
        line.endWidth = width;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.sortingOrder = 5; // Render trên hầu hết sprites
    }

    private void Update()
    {
        UpdateLaserPosition();

        if (_player != null)
        {
            Vector2 direction = (_player.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0, 0, targetAngle),
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateLaserPosition()
    {
        if (_laserStart == null || _laserEnd == null) return;

        Vector2 startPos = _laserStart.position;
        Vector2 endPos = _laserEnd.position;

        // Raycast để kiểm tra laser có bị chặn không
        RaycastHit2D hit = Physics2D.Raycast(startPos, _laserDirection,
            Vector2.Distance(startPos, endPos), _obstacleLayer);

        if (hit.collider != null)
        {
            endPos = hit.point;
        }

        _laserLength = Vector2.Distance(startPos, endPos);

        // Update LineRenderer
        if (_laserLine != null)
        {
            _laserLine.SetPosition(0, startPos);
            _laserLine.SetPosition(1, endPos);
        }

        if (_glowLine != null)
        {
            _glowLine.SetPosition(0, startPos);
            _glowLine.SetPosition(1, endPos);
        }

        // Update Collider
        UpdateCollider(startPos, endPos);

        // Update Particles
        UpdateParticles(startPos, endPos);
    }

    private void UpdateCollider(Vector2 startPos, Vector2 endPos)
    {
        if (_laserCollider == null) return;

        // Position ở giữa laser
        Vector2 center = (startPos + endPos) / 2f;
        _laserCollider.transform.position = center;

        // Size
        _laserCollider.size = new Vector2(_laserLength, _colliderThickness);

        // Rotation
        float angle = Mathf.Atan2(_laserDirection.y, _laserDirection.x) * Mathf.Rad2Deg;
        _laserCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Enable/disable theo state
        _laserCollider.enabled = (_currentState == LaserState.On) && _isActive;
    }

    private void UpdateParticles(Vector2 startPos, Vector2 endPos)
    {
        if (_startParticles != null)
        {
            _startParticles.transform.position = startPos;
        }

        if (_endParticles != null)
        {
            _endParticles.transform.position = endPos;
        }
    }

    // ===== LASER CYCLE =====

    private IEnumerator LaserCycle()
    {
        if (_alwaysOn)
        {
            SetLaserState(LaserState.On);
            yield break; // Không cycle
        }
        while (true)
        {
            if (_startActive)
            {
                // Warning → On → Off
                yield return StartCoroutine(WarningPhase());
                yield return StartCoroutine(OnPhase());
                yield return StartCoroutine(OffPhase());
            }
            else
            {
                // Off → Warning → On
                yield return StartCoroutine(OffPhase());
                yield return StartCoroutine(WarningPhase());
                yield return StartCoroutine(OnPhase());
            }
        }
    }

    private IEnumerator WarningPhase()
    {
        SetLaserState(LaserState.Warning);

        // Play warning sound
        if (_audioSource != null && _warningSound != null)
        {
            _audioSource.PlayOneShot(_warningSound);
        }

        // Blink effect
        float elapsed = 0f;
        float blinkInterval = 0.1f;
        bool isVisible = true;

        while (elapsed < _warningDuration)
        {
            isVisible = !isVisible;

            if (_laserLine != null)
            {
                _laserLine.enabled = isVisible;
            }
            if (_glowLine != null)
            {
                _glowLine.enabled = isVisible;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Ensure visible
        if (_laserLine != null) _laserLine.enabled = true;
        if (_glowLine != null) _glowLine.enabled = true;
    }

    private IEnumerator OnPhase()
    {
        SetLaserState(LaserState.On);

        // Play active sound
        if (_audioSource != null && _activeSound != null)
        {
            _audioSource.clip = _activeSound;
            _audioSource.Play();
        }

        // Start particles
        if (_startParticles != null) _startParticles.Play();
        if (_endParticles != null) _endParticles.Play();

        yield return new WaitForSeconds(_onDuration);

        // Stop particles
        if (_startParticles != null) _startParticles.Stop();
        if (_endParticles != null) _endParticles.Stop();

        // Play deactivate sound
        if (_audioSource != null && _deactivateSound != null)
        {
            _audioSource.Stop();
            _audioSource.PlayOneShot(_deactivateSound);
        }
    }

    private IEnumerator OffPhase()
    {
        SetLaserState(LaserState.Off);

        yield return new WaitForSeconds(_offDuration);
    }
    private void SetLaserState(LaserState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case LaserState.Off:
                if (_laserLine != null) _laserLine.enabled = false;
                if (_glowLine != null) _glowLine.enabled = false;
                if (_laserCollider != null) _laserCollider.enabled = false;
                break;

            case LaserState.Warning:
                if (_laserLine != null)
                {
                    _laserLine.enabled = true;
                    _laserLine.startColor = _warningColor;
                    _laserLine.endColor = _warningColor;
                }
                if (_glowLine != null) _glowLine.enabled = false;
                if (_laserCollider != null) _laserCollider.enabled = false;
                break;

            case LaserState.On:
                if (_laserLine != null)
                {
                    _laserLine.enabled = true;
                    _laserLine.startColor = _laserColor;
                    _laserLine.endColor = _laserColor;
                }
                if (_glowLine != null) _glowLine.enabled = true;
                if (_laserCollider != null) _laserCollider.enabled = true;
                break;
        }
    }

    // ===== COLLISION =====

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive || _currentState != LaserState.On) return;

        base.OnTriggerEnter2D(other);
    }

    // ===== DEBUG =====

    private void OnDrawGizmos()
    {
        if (!_showDebugGizmos) return;
        if (_laserStart == null || _laserEnd == null) return;

        // Draw laser line
        Gizmos.color = _currentState == LaserState.On ? Color.red : Color.yellow;
        Gizmos.DrawLine(_laserStart.position, _laserEnd.position);

        // Draw start/end points
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_laserStart.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_laserEnd.position, 0.2f);
    }
}
