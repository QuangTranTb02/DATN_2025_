using UnityEngine;

public class MultiPointMovingTrap : BaseTrap
{
    [Header("Movement Settings")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private bool _loop = true;
    [SerializeField] private bool _reverseOnEnd = false;

    [SerializeField] private bool _isRotating = false;

    private int _currentWaypointIndex = 0;
    private int _direction = 1; // 1 = forward, -1 = backward

    private void Start()
    {
        if (_wayPoints == null || _wayPoints.Length < 2)
        {
            Debug.LogError($"[MultiPointMovingTrap] {gameObject.name}: Cần ít nhất 2 waypoints!", this);
            enabled = false;
            return;
        }

        // Kiểm tra null waypoints
        foreach (var waypoint in _wayPoints)
        {
            if (waypoint == null)
            {
                Debug.LogError($"[MultiPointMovingTrap] {gameObject.name}: Có waypoint bị null!", this);
                enabled = false;
                return;
            }
        }

        // Start tại waypoint đầu tiên
        transform.position = _wayPoints[0].position;
        _currentWaypointIndex = 0;
    }

    private void Update()
    {
        if (_wayPoints == null || _wayPoints.Length == 0) return;

        MoveTrap();
    }

    private void MoveTrap()
    {
        Transform targetWaypoint = _wayPoints[_currentWaypointIndex];

        // Di chuyển về waypoint hiện tại
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, _speed * Time.deltaTime);

        // Check đã đến waypoint chưa
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.01f)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        if (_reverseOnEnd)
        {
            // Đi qua lại (ping-pong)
            _currentWaypointIndex += _direction;

            // Đảo chiều khi đến đầu/cuối
            if (_currentWaypointIndex >= _wayPoints.Length)
            {
                _currentWaypointIndex = _wayPoints.Length - 2;
                _direction = -1;
            }
            else if (_currentWaypointIndex < 0)
            {
                _currentWaypointIndex = 1;
                _direction = 1;
            }
        }
        else
        {
            // Đi vòng (loop)
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _wayPoints.Length)
            {
                if (_loop)
                {
                    _currentWaypointIndex = 0;
                }
                else
                {
                    _currentWaypointIndex = _wayPoints.Length - 1;
                    enabled = false; // Dừng khi đến điểm cuối
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_wayPoints == null || _wayPoints.Length < 2) return;

        // Vẽ đường nối các waypoint
        Gizmos.color = Color.red;
        for (int i = 0; i < _wayPoints.Length - 1; i++)
        {
            if (_wayPoints[i] != null && _wayPoints[i + 1] != null)
            {
                Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
            }
        }

        // Vẽ đường nối điểm cuối về điểm đầu nếu loop
        if (_loop && _wayPoints[_wayPoints.Length - 1] != null && _wayPoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_wayPoints[_wayPoints.Length - 1].position, _wayPoints[0].position);
        }

        // Vẽ các waypoint
        for (int i = 0; i < _wayPoints.Length; i++)
        {
            if (_wayPoints[i] != null)
            {
                // Màu khác nhau cho điểm đầu và cuối
                if (i == 0)
                    Gizmos.color = Color.green;
                else if (i == _wayPoints.Length - 1)
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.white;

                Gizmos.DrawWireSphere(_wayPoints[i].position, 0.25f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_wayPoints == null || _wayPoints.Length == 0) return;

#if UNITY_EDITOR
        for (int i = 0; i < _wayPoints.Length; i++)
        {
            if (_wayPoints[i] != null)
            {
                Color labelColor = (i == 0) ? Color.green : (i == _wayPoints.Length - 1) ? Color.blue : Color.white;

                UnityEditor.Handles.color = labelColor;
                UnityEditor.Handles.Label(_wayPoints[i].position + Vector3.up * 0.3f, $"Point {i}", new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = labelColor },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                });
            }
        }
#endif
    }
}
