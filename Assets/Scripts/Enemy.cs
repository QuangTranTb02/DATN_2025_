using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AIController aiController;
    private Animator animator;

    void Start()
    {
        aiController = GetComponent<AIController>();
        animator = GetComponent<Animator>();
        aiController.ChangeState(new EnemePatrolState(aiController, animator));
    }
}
