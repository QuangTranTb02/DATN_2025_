using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AttackState : IState
{
    private AIController ai;
    private Animator animator;


    private float attackTimer;
    public AttackState(AIController aiController, Animator anim)
    {
        ai = aiController;
        animator = anim;
    }

    public void Enter()
    {
        
        Debug.Log("Enter Attack State");
        
        Execute();
    }

    public void Execute()
    {

        if (Vector3.Distance(ai.transform.position, ai.player.transform.position) < ai.detectionRange)
        {
            if (Vector3.Distance(ai.transform.position, ai.player.transform.position) < 0.1f)
            {
                Attack();
            }
            else
            {
                ai.ChangeState(new ChaseState(ai,animator));
            }

        }
        else
        {
            ai.ChangeState(new EnemePatrolState(ai, animator));
        }
    }

    public void Exit()
    {
        Debug.Log("Exit Attack State");
    }
    private void Attack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f)
        {
            return;
        }
        attackTimer = ai.AttackCoolDown;
        Collider2D[] targets = Physics2D.OverlapBoxAll(ai.transform.position, ai.attackSize, 0f);

        foreach (var c in targets)
        {
            Damageable damageable = c.GetComponent<Damageable>();
            if (damageable != null && c.gameObject.CompareTag("Player"))
            {
                damageable.TakeDamage(ai.damage);
            }
        }
    }
}
