using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    PlayerController playerController;
    private Rigidbody2D rb;
    [SerializeField] float OffsetDistance = 1.0f;
    [SerializeField] float SizeOfInteracableArea = 1.2f;
    Player Player;
    [SerializeReference] HighlightController HighlightController;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        Player = GetComponent<Player>();
    }
    private void Update()
    {
       Check();
       if(Input.GetMouseButtonDown(1))
       {
            Interact();
       }
    }

    private void Check()
    {
        Vector2 position = rb.position + playerController.LastMotionVector * OffsetDistance;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, SizeOfInteracableArea);
       
        foreach (Collider2D item in colliders)
        {
            Interacable hit = item.GetComponent<Interacable>();
            if (hit != null)
            {
                HighlightController.Highlight(hit.gameObject);
                return;
            }
        }
        HighlightController.Hide();
    }

    private void Interact()
    {
        Vector2 position = rb.position + playerController.LastMotionVector * OffsetDistance;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, SizeOfInteracableArea);

        foreach (Collider2D item in colliders)
        {
            Interacable hit = item.GetComponent<Interacable>();
            if (hit != null)
            {
                hit.Interact(Player);
                break;
            }

        }
    }
}
