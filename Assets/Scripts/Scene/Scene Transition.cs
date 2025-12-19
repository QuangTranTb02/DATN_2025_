using NUnit.Framework.Internal;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _loadingText; // Optional

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Debug")]
    [SerializeField] private bool _showDebugLogs = false;

    // Thêm vào SceneTransition.cs

    [Header("Loading Animation")]
    [SerializeField] private RectTransform _loadingIcon; // Icon xoay
    [SerializeField] private float _loadingRotationSpeed = 360f;

    private void Update()
    {
        if (_loadingIcon != null && _loadingText.activeSelf)
        {
            _loadingIcon.Rotate(0, 0, -_loadingRotationSpeed * Time.deltaTime);
        }
    }

    private bool _isFading = false;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure fade panel bắt đầu trong suốt
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        // Hide loading text
        if (_loadingText != null)
        {
            _loadingText.SetActive(false);
        }

        Log("SceneTransition initialized");
    }

    /// <summary>
    /// Fade từ trong suốt → đen (che màn hình)
    /// </summary>
    public IEnumerator FadeOut()
    {
        if (_isFading)
        {
            LogWarning("Already fading! Skipping FadeOut.");
            yield break;
        }

        _isFading = true;
        Log("Fading out...");

        // Show loading text
        if (_loadingText != null)
        {
            _loadingText.SetActive(true);
        }

        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để không bị ảnh hưởng bởi Time.timeScale
            float t = elapsed / _fadeDuration;
            float curveValue = _fadeCurve.Evaluate(t);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);
            }

            yield return null;
        }

        // Ensure alpha = 1 (hoàn toàn đen)
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }

        _isFading = false;
        Log("Fade out complete");
    }

    /// <summary>
    /// Fade từ đen → trong suốt (hiện màn hình)
    /// </summary>
    public IEnumerator FadeIn()
    {
        if (_isFading)
        {
            LogWarning("Already fading! Skipping FadeIn.");
            yield break;
        }

        _isFading = true;
        Log("Fading in...");

        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / _fadeDuration;
            float curveValue = _fadeCurve.Evaluate(t);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveValue);
            }

            yield return null;
        }

        // Ensure alpha = 0 (hoàn toàn trong suốt)
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        // Hide loading text
        if (_loadingText != null)
        {
            _loadingText.SetActive(false);
        }

        _isFading = false;
        Log("Fade in complete");
    }

    /// <summary>
    /// Fade out → callback → fade in
    /// </summary>
    public IEnumerator FadeTransition(System.Action onFadedOut = null)
    {
        yield return FadeOut();

        onFadedOut?.Invoke();

        yield return FadeIn();
    }

    // ===== INSTANT FADE (không có animation) =====

    public void SetFadeInstant(float alpha)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = Mathf.Clamp01(alpha);
        }
    }

    public void SetBlackInstant()
    {
        SetFadeInstant(1f);
    }

    public void SetClearInstant()
    {
        SetFadeInstant(0f);
    }

    // ===== GETTERS =====

    public bool IsFading() => _isFading;

    // ===== DEBUG =====

    private void Log(string message)
    {
        if (_showDebugLogs)
            Debug.Log($"[SceneTransition] {message}");
    }

    private void LogWarning(string message)
    {
        if (_showDebugLogs)
            Debug.LogWarning($"[SceneTransition] {message}");
    }
}