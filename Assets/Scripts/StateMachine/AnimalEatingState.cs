using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEatingState : IState
{
    protected AIController ai;
    protected Animator animator;
    float timeToEat = 3f;
    float timer;

    public AnimalEatingState(AIController ai, Animator animator)
    {
        this.ai = ai;
        this.animator = animator;
    }
    public void Enter()
    {
        animator.SetBool("Eating", true);
        timer = timeToEat;
    }

    public void Execute()
    {
        timer -= Time.deltaTime;
        if(timer <=  0f)
        {
            ai.ChangeState(new AnimalPatrolState(ai, animator));
            timer = timeToEat;
        }
    }

    public void Exit()
    {
        animator.SetBool("Eating", false);
    }
}
