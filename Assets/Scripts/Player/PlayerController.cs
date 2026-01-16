using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Movement m_movement;
    private PlayerControll m_Input;
    private Vector2 m_MoveInputValue;

    public Vector2 LastMotionVector;

    private Animator m_Animator;

    public bool running = false;
    public float speed = 5f;
    public float runSpeed = 10f;
    private void OnEnable()
    {
        m_Input = new PlayerControll();
        m_Input.Enable();
    }
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_movement = GetComponent<Movement>();
    }
    private void OnDisable()
    {
        m_Input.Disable();
        m_movement.m_rb.velocity = Vector2.zero;
    }
    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            running = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }
        m_MoveInputValue = m_Input.Player.Move.ReadValue<Vector2>();

        m_movement.Move(m_MoveInputValue, running == true? runSpeed: speed);

        AnimatorMovement(m_MoveInputValue);
    }
    private void AnimatorMovement(Vector2 direction)
    {
        if (m_Animator != null)
        {
            if (direction.magnitude > 0)
            {
                m_Animator.SetFloat("horizontal", direction.x);
                m_Animator.SetFloat("vertical", direction.y);
                m_Animator.SetBool("isWalking", true);

                if(direction.x !=0 || direction.y != 0) { LastMotionVector = new Vector2(direction.x, direction.y).normalized;}
            }
            else
            {
                m_Animator.SetBool("isWalking", false);
            }
        }
    }
}
