using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPatrolState : PatrolState
{
    float timeToRoam = 3f;
    float timer = 0f;
    public AnimalPatrolState(AIController aiController, Animator anim) : base(aiController, anim)
    {
    }
    public override void Enter()
    {
        timer = timeToRoam;
        base.Enter();
    }
    public override void Execute()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ai.ChangeState(new AnimalEatingState(ai, animator));
        }
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}
