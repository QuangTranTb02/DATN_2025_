using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   

    public Rigidbody2D m_rb;
    private Vector2 m_direction;
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }
    /*private void FixedUpdate()
    {
        Move(m_direction);
    }*/
    public void Move(Vector2 direction, float speed)
    {
        //_rb.velocity = direction ;
            m_rb.MovePosition(m_rb.position + direction * speed * Time.deltaTime);
    }
    public void MoveTo(Vector2 direction)
    {
        this.m_direction = direction ;
    }
}
