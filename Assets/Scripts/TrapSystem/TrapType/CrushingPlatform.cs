using NUnit.Framework;
using System.Collections;
using System.Xml.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditorInternal.ReorderableList;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.Rendering.STP;

public class CrushingPlatform : BaseTrap
{
    [Header("Platform References")]
    [SerializeField] private Transform _platform; // Platform sẽ di chuyển
    [SerializeField] private Transform _crushPoint; // Điểm đè xuống (hoặc null = tự tính)

    [Header("Movement Settings")]
    [SerializeField] private CrushDirection _crushDirection = CrushDirection.Down;
    [SerializeField] private float _crushDistance = 5f; // Khoảng cách di chuyển
    [SerializeField] private float _crushSpeed = 10f; // Tốc độ khi đè xuống
    [SerializeField] private float _returnSpeed = 2f; // Tốc độ quay về (chậm hơn)

    [Header("Timing")]
    [SerializeField] private float _warningDelay = 1f; // Thời gian cảnh báo trước khi crush
    [SerializeField] private float _crushHoldTime = 0.5f; // Giữ ở dưới bao lâu
    [SerializeField] private float _cooldownTime = 2f; // Thời gian nghỉ trước chu kỳ mới
    [SerializeField] private bool _autoStart = true;
    [SerializeField] private float _initialDelay = 0f; // Delay lần đầu

    [Header("Trigger Settings")]
    [SerializeField] private bool _playerTrigger = false; // Kích hoạt khi player vào vùng
    [SerializeField] private Collider2D _triggerZone; // Vùng kích hoạt (nếu dùng trigger)

    [Header("Warning Effects")]
    [SerializeField] private float _shakeAmount = 0.1f;
    [SerializeField] private float _shakeDuration = 0.8f;
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] private Color _normalColor = Color.white;

    [Header("Crushing Detection")]
    [SerializeField] private LayerMask _crushCheckLayer; // Layer để check crush (Player)
    [SerializeField] private float _crushCheckDistance = 0.2f; // Khoảng cách check phía dưới
    [SerializeField] private Vector2 _crushCheckSize = new Vector2(0.8f, 0.1f); // Kích thước box check

    [Header("Visual")]
    [SerializeField] private SpriteRenderer _platformSprite;
    [SerializeField] private ParticleSystem _crushParticles; // Particles khi đè xuống
    [SerializeField] private ParticleSystem _dustParticles; // Dust khi chạm đất

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _warningSound;
    [SerializeField] private AudioClip _crushSound;
    [SerializeField] private AudioClip _impactSound;
    [SerializeField] private AudioClip _returnSound;

    [Header("Debug")]
    [SerializeField] private bool _showDebugGizmos = true;

    // State
    private Vector3 _startPosition;
    private Vector3 _crushPosition;
    private CrushState _currentState = CrushState.Idle;
    private bool _playerInTrigger = false;
    private Coroutine _crushCoroutine;

    private enum CrushState
    {
        Idle,       // Đứng yên
        Warning,    // Cảnh báo (shake)
        Crushing,   // Đang đè xuống
        Holding,    // Giữ ở dưới
        Returning,  // Quay về vị trí ban đầu
        Cooldown    // Nghỉ
    }

    private enum CrushDirection
    {
        Down,
        Up,
        Left,
        Right
    }

    private void Start()
    {
        InitializePlatform();

        if (_autoStart)
        {
            StartCoroutine(AutoCrushCycle());
        }
    }
    private void InitializePlatform()
    {
        // Validate
        if (_platform == null)
        {
            Debug.LogError($"[CrushingPlatform] {gameObject.name}: Platform not assigned!");
            enabled = false;
            return;
        }

        // Lưu vị trí ban đầu
        _startPosition = _platform.position;
        Debug.Log($"[CrushingPlatform] {gameObject.name}: Start Position: {_startPosition}");
        // Tính vị trí crush
        CalculateCrushPosition();

        // Setup audio
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;

        // Setup sprite
        if (_platformSprite == null)
        {
            _platformSprite = _platform.GetComponent<SpriteRenderer>();
        }

        if (_platformSprite != null)
        {
            _normalColor = _platformSprite.color;
        }

        // Setup trigger zone
        if (_playerTrigger && _triggerZone != null)
        {
            _triggerZone.isTrigger = true;
        }
    }

    private void CalculateCrushPosition()
    {
        if (_crushPoint != null)
        {
            _crushPosition = _crushPoint.position;
        }
        else
        {
            // Tự động tính dựa trên direction
            Vector3 direction = Vector3.zero;

            switch (_crushDirection)
            {
                case CrushDirection.Down:
                    direction = Vector3.down;
                    break;
                case CrushDirection.Up:
                    direction = Vector3.up;
                    break;
                case CrushDirection.Left:
                    direction = Vector3.left;
                    break;
                case CrushDirection.Right:
                    direction = Vector3.right;
                    break;
            }

            _crushPosition = _startPosition + direction * _crushDistance;
        }
    }

    // ===== AUTO CYCLE =====

    private IEnumerator AutoCrushCycle()
    {
        // Initial delay
        if (_initialDelay > 0)
        {
            yield return new WaitForSeconds(_initialDelay);
        }

        while (true)
        {
            if (!_playerTrigger)
            {
                // Auto cycle without trigger
                yield return StartCoroutine(CrushSequence());
            }
            else
            {
                // Wait for player trigger
                yield return new WaitUntil(() => _playerInTrigger);
                yield return StartCoroutine(CrushSequence());
                _playerInTrigger = false; // Reset
            }

            // Cooldown
            _currentState = CrushState.Cooldown;
            yield return new WaitForSeconds(_cooldownTime);
            _currentState = CrushState.Idle;
        }
    }

    // ===== CRUSH SEQUENCE =====

    private IEnumerator CrushSequence()
    {
        // 1. WARNING
        yield return StartCoroutine(WarningPhase());

        // 2. CRUSH DOWN
        yield return StartCoroutine(CrushPhase());

        // 3. HOLD
        yield return StartCoroutine(HoldPhase());

        // 4. RETURN
        yield return StartCoroutine(ReturnPhase());
    }

    private IEnumerator WarningPhase()
    {
        _currentState = CrushState.Warning;

        // Play warning sound
        if (_audioSource != null && _warningSound != null)
        {
            _audioSource.PlayOneShot(_warningSound);
        }

        // Shake và change color
        Vector3 originalPos = _platform.position;
        float elapsed = 0f;

        while (elapsed < _warningDelay)
        {
            // Shake
            float shakeProgress = elapsed / _shakeDuration;
            if (shakeProgress < 1f)
            {
                Vector3 randomOffset = Random.insideUnitCircle * _shakeAmount;
                _platform.position = originalPos + randomOffset;
            }

            // Color blink
            if (_platformSprite != null)
            {
                float t = Mathf.PingPong(elapsed * 5f, 1f);
                _platformSprite.color = Color.Lerp(_normalColor, _warningColor, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position và color
        _platform.position = originalPos;
        if (_platformSprite != null)
        {
            _platformSprite.color = _normalColor;
        }
    }

    private IEnumerator CrushPhase()
    {
        _currentState = CrushState.Crushing;

        // Play crush sound
        if (_audioSource != null && _crushSound != null)
        {
            _audioSource.PlayOneShot(_crushSound);
        }

        // Particles
        if (_crushParticles != null)
        {
            _crushParticles.Play();
        }

        // Move to crush position
        while (Vector3.Distance(_platform.position, _crushPosition) > 0.01f)
        {
            _platform.position = Vector3.MoveTowards(
                _platform.position,
                _crushPosition,
                _crushSpeed * Time.deltaTime
            );

            // Check crush player
            CheckCrushPlayer();

            yield return null;
        }

        _platform.position = _crushPosition;

        // Impact effect
        if (_dustParticles != null)
        {
            _dustParticles.Play();
        }

        if (_audioSource != null && _impactSound != null)
        {
            _audioSource.PlayOneShot(_impactSound);
        }

        // Screen shake (nếu có CameraShake)
        // CameraShake.Instance?.Shake(0.2f, 0.2f);
    }

    private IEnumerator HoldPhase()
    {
        _currentState = CrushState.Holding;

        // Giữ ở dưới và tiếp tục check crush
        float elapsed = 0f;
        while (elapsed < _crushHoldTime)
        {
            CheckCrushPlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ReturnPhase()
    {
        _currentState = CrushState.Returning;

        // Play return sound
        if (_audioSource != null && _returnSound != null)
        {
            _audioSource.PlayOneShot(_returnSound);
        }

        // Move back to start position
        while (Vector3.Distance(_platform.position, _startPosition) > 0.01f)
        {
            _platform.position = Vector3.MoveTowards(
                _platform.position,
                _startPosition,
                _returnSpeed * Time.deltaTime
            );

            yield return null;
        }

        _platform.position = _startPosition;
    }

    // ===== CRUSH DETECTION =====

    private void CheckCrushPlayer()
    {
        if (_crushCheckLayer == 0) return;

        Vector2 checkPosition = GetCrushCheckPosition();

        // BoxCast để check có gì bị đè không
        RaycastHit2D hit = Physics2D.BoxCast(
            checkPosition,
            _crushCheckSize,
            0f,
            GetCrushDirection(),
            _crushCheckDistance,
            _crushCheckLayer
        );

        if (hit.collider != null)
        {
            // Check if it's player
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                if (health != null && !health.IsDead())
                {
                    health.TakeDamage();
                    Debug.Log($"Player crushed by {gameObject.name}!");
                }
            }
        }
    }

    private Vector2 GetCrushCheckPosition()
    {
        Vector2 platformPos = _platform.position;
        Vector2 offset = Vector2.zero;

        // Offset dựa trên hướng crush
        switch (_crushDirection)
        {
            case CrushDirection.Down:
                offset = Vector2.down * (_crushCheckDistance / 2f);
                break;
            case CrushDirection.Up:
                offset = Vector2.up * (_crushCheckDistance / 2f);
                break;
            case CrushDirection.Left:
                offset = Vector2.left * (_crushCheckDistance / 2f);
                break;
            case CrushDirection.Right:
                offset = Vector2.right * (_crushCheckDistance / 2f);
                break;
        }

        return platformPos + offset;
    }

    private Vector2 GetCrushDirection()
    {
        switch (_crushDirection)
        {
            case CrushDirection.Down: return Vector2.down;
            case CrushDirection.Up: return Vector2.up;
            case CrushDirection.Left: return Vector2.left;
            case CrushDirection.Right: return Vector2.right;
            default: return Vector2.down;
        }
    }

    // ===== TRIGGER DETECTION =====

    // Xóa OnTriggerEnter2D và OnTriggerExit2D

    // Thêm public methods cho TriggerZoneDetector gọi

    public void OnPlayerEnterTrigger()
    {
        if (_playerTrigger)
        {
            _playerInTrigger = true;
            Debug.Log($"[CrushingPlatform] {gameObject.name}: Player entered trigger zone.");
        }
    }

    public void OnPlayerExitTrigger()
    {
        if (_playerTrigger)
        {
            _playerInTrigger = false;
            Debug.Log($"[CrushingPlatform] {gameObject.name}: Player exited trigger zone.");
        }
    }

    // ===== PUBLIC METHODS =====

    public void TriggerCrush()
    {
        if (_crushCoroutine != null)
        {
            StopCoroutine(_crushCoroutine);
        }

        _crushCoroutine = StartCoroutine(CrushSequence());
    }

    // ===== DEBUG =====

    private void OnDrawGizmos()
    {
        if (!_showDebugGizmos) return;

        if (_platform != null)
        {
            // Draw start position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_startPosition, Vector3.one * 0.5f);

            // Draw crush position
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_crushPosition, Vector3.one * 0.5f);

                // Draw line
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_startPosition, _crushPosition);
            }
            else
            {
                // In editor, tính crush position preview
                Vector3 direction = Vector3.zero;

                switch (_crushDirection)
                {
                    case CrushDirection.Down: direction = Vector3.down; break;
                    case CrushDirection.Up: direction = Vector3.up; break;
                    case CrushDirection.Left: direction = Vector3.left; break;
                    case CrushDirection.Right: direction = Vector3.right; break;
                }

                Vector3 previewCrushPos = (_crushPoint != null) ? _crushPoint.position : _platform.position + direction * _crushDistance;

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(previewCrushPos, Vector3.one * 0.5f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_platform.position, previewCrushPos);
            }

            // Draw crush check box
            if (Application.isPlaying && (_currentState == CrushState.Crushing || _currentState == CrushState.Holding))
            {
                Gizmos.color = Color.cyan;
                Vector2 checkPos = GetCrushCheckPosition();
                Gizmos.DrawWireCube(checkPos, _crushCheckSize);
            }
        }

        // Draw trigger zone
        if (_playerTrigger && _triggerZone != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawCube(_triggerZone.transform.position, _triggerZone.bounds.size);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showDebugGizmos) return;

        // Draw more detailed info when selected
        if (_platform != null)
        {
            // Label positions
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.Label(_startPosition + Vector3.up * 0.5f, "Start", new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.green },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            });

            Vector3 crushPosPreview = (_crushPoint != null) ? _crushPoint.position : _startPosition + (Vector3)GetCrushDirection() * _crushDistance;

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.Label(crushPosPreview + Vector3.up * 0.5f, "Crush", new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.red },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            });
#endif
        }
    }
}

    