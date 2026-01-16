using System.Collections;
using UnityEngine;

public class PatrolState : IState
{
    protected AIController ai;
    protected Animator animator;


    protected Coroutine roamingCoroutine;
        
    public PatrolState(AIController aiController, Animator anim)
    {
        ai = aiController;
        animator = anim;
        ai.initialPosition = ai.transform.position;
        ai.player = GameManager.Instance.m_PlayerController.transform;
    }

    public virtual void Enter()
    {
       Debug.Log("Enter Patrol State");

       roamingCoroutine = ai.StartCoroutine(RoamingCoroutine());
       Execute();
    }

    public virtual void Execute()
    {
        
    }

    public virtual void Exit()
    {
        Debug.Log("Exit Patrol State");
        if(roamingCoroutine != null)
        {
            ai.StopCoroutine(roamingCoroutine);
        }
    }

    private IEnumerator RoamingCoroutine()
    {
        while (true)
        {
            Vector3 newRoamingPosition = GetNewRoamingPosition();
            yield return Roam(newRoamingPosition);

            yield return new WaitForSeconds(2f);
        }
    }

    private Vector3 GetNewRoamingPosition()
    {
        Vector3 randomOffset;
        Vector3 newRoamingPosition;

        do
        {
            randomOffset = new Vector3(
                Random.Range(-ai.patrolRange, ai.patrolRange),
                Random.Range(-ai.patrolRange, ai.patrolRange),
                0f
            );
            newRoamingPosition = ai.initialPosition + randomOffset;
            RotateAnimation(newRoamingPosition);
        } while (Vector3.Distance(ai.initialPosition, newRoamingPosition) > ai.patrolRange);

        return newRoamingPosition;
    }

    public void RotateAnimation(Vector3 moveTo)
    {
        Vector3 direction = (moveTo - ai.transform.position).normalized;
        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
    }

    private IEnumerator Roam(Vector3 newRoamingPosition)
    {
        while (Vector3.Distance(ai.transform.position, newRoamingPosition) > 0.1f)
        {
            ai.transform.position = Vector3.MoveTowards(ai.transform.position, newRoamingPosition, ai.speed * Time.deltaTime);
            yield return null;
        }
    }
}
