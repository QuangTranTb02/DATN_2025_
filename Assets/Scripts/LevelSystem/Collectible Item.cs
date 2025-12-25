using NUnit.Framework;
using System.Xml.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _value = 1;

    [Header("Visual")]
    [SerializeField] private ParticleSystem _collectEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip _collectSound;

    [Header("Animation")]
    [SerializeField] private bool _rotate = true;
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private bool _bobUpDown = true;
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 0.3f;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        // Rotation
        if (_rotate)
        {
            transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        }

        // Bob up-down
        if (_bobUpDown)
        {
            float newY = _startPosition.y + Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (LevelManager.Instance == null)
            {
            Debug.LogError("LevelManager instance not found!");
            return;
        }
        Debug.Log("Collected!");

        // Register với LevelManager
        LevelManager.Instance.RegisterCollectible();

        // Visual effect
        if (_collectEffect != null)
        {
            Instantiate(_collectEffect, transform.position, Quaternion.identity);
        }

        // Audio
        if (_collectSound != null)
        {
            AudioSource.PlayClipAtPoint(_collectSound, transform.position);
        }

        // Destroy
        Destroy(gameObject);
    }
}
