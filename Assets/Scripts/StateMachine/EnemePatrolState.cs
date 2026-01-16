using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemePatrolState : PatrolState
{

    public EnemePatrolState(AIController aiController, Animator anim) : base(aiController, anim)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Execute()
    {
        if (Vector3.Distance(ai.transform.position, ai.player.position) <= ai.detectionRange)
        {
            ai.ChangeState(new ChaseState(ai, animator));
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
    
}
