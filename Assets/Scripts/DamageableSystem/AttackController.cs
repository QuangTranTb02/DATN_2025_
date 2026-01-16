using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] float OffsetDistance = 1.7f;
    [SerializeField] Vector2 attackAreaSize = new Vector2(1f,1f);

    Rigidbody2D Rigidbody2D;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public void Attack(int damage, Vector2 lastMotionVector)
    {
        Vector2 postion = Rigidbody2D.position + lastMotionVector * OffsetDistance;

        Collider2D[] targets = Physics2D.OverlapBoxAll(postion, attackAreaSize, 0f);

        foreach (var c in targets)
        {
            Damageable damageable = c.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, attackAreaSize);
    }
}
