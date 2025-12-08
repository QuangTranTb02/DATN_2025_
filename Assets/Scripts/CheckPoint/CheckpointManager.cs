using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector2 _currentCheckpointPosition;
    private bool _hasCheckpoint = false;

    [Header("Spawn Settings")]
    private Vector2 _levelStartPosition; // Vị trí spawn đầu level

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);    
        }
    }

    private void Start()
    {
        // Set checkpoint ban đầu là vị trí start của level
        _currentCheckpointPosition = _levelStartPosition;
        _hasCheckpoint = true;
    }

    public void SetCheckpoint(Vector2 position)
    {
        _currentCheckpointPosition = position;
        _hasCheckpoint = true;
        Debug.Log($"Checkpoint saved at: {position}");
    }

    public Vector2 GetLastCheckpoint()
    {
        return _hasCheckpoint ? _currentCheckpointPosition : _levelStartPosition;
    }
}