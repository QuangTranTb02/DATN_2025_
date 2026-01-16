using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class AIController : MonoBehaviour
{
    public Transform player;
    private IState currentState;
    public float speed = 10f;
    public float detectionRange = 3f;
    public float patrolRange = 5f;
    public Vector2 attackSize = Vector2.one;
    public int damage = 5;
    public float AttackCoolDown = 2f;
    public Vector3 initialPosition;

    private void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    Dictionary<Type, IState> pairs;
    

    public IState GetState<TState>() where TState : IState
    {
        pairs.TryGetValue(typeof(TState), out IState state);

        return state;
    }
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPosition,patrolRange);
    }
}

