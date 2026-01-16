using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    Transform player;
    [SerializeField] float Speed = 5f;
    [SerializeField] float TimeToLive = 10f;
    [SerializeField] float PickupDistance = 1.5f;
    private float DistanceToPlayer;

    public Item item;
    public int Count = 1;

    private void Start()
    {
        player = GameManager.Instance.m_PlayerController.transform;
    }
    public void set(Item item, int count)
    {
        this.item = item;
        this.Count = count;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.Icon;
    }
    private void Update()
    {
        TimeToLive -= Time.deltaTime;
        if (TimeToLive < 0)
        {
            Destroy(gameObject);
        }
        DistanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (DistanceToPlayer > PickupDistance)
        {
            return;
        }
        transform.position = Vector3.MoveTowards(
            transform.position, player.position,
            Speed * Time.deltaTime);

        if (DistanceToPlayer < 0.1f)
        {
            if(GameManager.Instance.inventoryContainer != null)
            {
                GameManager.Instance.inventoryContainer.Add(item,Count);
            }
            else
            {
                Debug.LogWarning("Chưa có inventoty được gắn vào GameManager");
            }
            Destroy(gameObject);    
        }
    }
}

