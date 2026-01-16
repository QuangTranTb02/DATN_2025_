using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : IState
{
    private AIController ai;
    private Animator animator;

    public ChaseState(AIController aiController, Animator anim)
    {
        ai = aiController;
        animator = anim;
        ai.player = GameManager.Instance.m_PlayerController.transform;
    }

    public void Enter()
    {
        Debug.Log("Enter Chase State");
    }

    public void Execute()
    {
        if(Vector3.Distance(ai.transform.position, ai.player.transform.position) < ai.detectionRange)
        {
            if(Vector3.Distance(ai.transform.position, ai.player.transform.position) < 0.1f)
            {
                ai.ChangeState( new AttackState(ai, animator) );
            }
            else
            {
                MoveToPlayer(ai.player);
            }
        }
        else
        {
            ai.ChangeState(new EnemePatrolState(ai, animator));
        }
    }
    public void Exit()
    {
        Debug.Log("Exit Chase State");
    }
    private void MoveToPlayer(UnityEngine.Transform playerTransform)
    {
        RotateAnimation(ai.player.position);
        ai.transform.position = Vector3.MoveTowards(
            ai.transform.position,
            playerTransform.position,
            ai.speed * Time.deltaTime
        );
    }
    public void RotateAnimation(Vector3 moveTo)
    {
        Vector3 direction = (moveTo - ai.transform.position).normalized;
        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
    }
}
