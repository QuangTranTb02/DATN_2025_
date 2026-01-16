using UnityEngine;
using System.Collections;
using UnityEditor.Tilemaps;

public class ChaseEnemy : MonoBehaviour
{
    private Transform player;
    private Vector3 initialPosition;

    [SerializeField]  float speed = 2.0f;
    [SerializeField]  Vector2 attackSize = Vector2.one;
    [SerializeField]  int damage = 3;
    [SerializeField]  float timeToAttack = 2f;
    [SerializeField]  float detectionRange = 10f;
    [SerializeField]  float roamingRange = 5f;
    public Animator animator;

    private float attackTimer;
    private bool isRoaming = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameManager.Instance.m_PlayerController.transform;
        initialPosition = transform.position;
        attackTimer = Random.Range(0, timeToAttack);
        StartCoroutine(RoamingCoroutine());
    }

    private void Update()
    {
        isRoaming = !CheckDistanceToFollowPlayer(player);
        if (isRoaming == false )
        {
            StopAllCoroutines();
            MoveToPlayer(player);
        }
        Attack();
    }

    IEnumerator RoamingCoroutine()
    {
        while (true)
        {
                // Roaming trong phạm vi xác định nếu không phát hiện người chơi
                Vector3 newRoamingPosition = GetNewRoamingPosition();
                Debug.Log("Roaming to new position: " + newRoamingPosition);
                yield return Roam(newRoamingPosition);
            
                yield return new WaitForSeconds(2.0f);
        }
    }

    private Vector3 GetNewRoamingPosition()
    {
        Vector3 randomOffset;
        Vector3 newRoamingPosition;

        do
        {
            randomOffset = new Vector3(
                Random.Range(-roamingRange, roamingRange),
                Random.Range(-roamingRange, roamingRange),
                0f
            );
            newRoamingPosition = initialPosition + randomOffset;
            RotateAnimation(newRoamingPosition);
        } while (Vector3.Distance(initialPosition, newRoamingPosition) > roamingRange);

        return newRoamingPosition;
    }
    bool CheckDistanceToFollowPlayer(Transform player)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer <= detectionRange;
    }
    private IEnumerator Roam(Vector3 newRoamingPosition)
    {
        while (Vector3.Distance(transform.position, newRoamingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newRoamingPosition, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
    }

    private void MoveToPlayer(Transform playerTransform)
    {
        RotateAnimation(player.position);
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTransform.position,
            speed * Time.deltaTime  
        );
    }

    private void Attack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f)
        {
            return;
        }
        attackTimer = timeToAttack;
        Collider2D[] targets = Physics2D.OverlapBoxAll(transform.position, attackSize, 0f);

        foreach (var c in targets)
        {
            Damageable damageable = c.GetComponent<Damageable>();
            if (damageable != null && c.gameObject.CompareTag("Player"))
            {
                damageable.TakeDamage(damage);
            }
        }
    }
    private void RotateAnimation(Vector3 moveTo)
    {
        Vector3 direction = (moveTo - transform.position).normalized;


        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, attackSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPosition, roamingRange);
    }
}
