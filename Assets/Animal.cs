using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    private AIController aiController;
    private Animator animator;

    void Start()
    {
        aiController = GetComponent<AIController>();
        animator = GetComponent<Animator>();
        aiController.ChangeState(new AnimalPatrolState(aiController, animator));
    }
}
