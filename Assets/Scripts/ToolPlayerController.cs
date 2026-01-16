using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolPlayerController : MonoBehaviour
{
    PlayerLevel playerLevel;
    private PlayerController playerController;
    private Player player;
    private Rigidbody2D rb;
    private Animator animator;
    private ToolbarController toolbarController;
    [SerializeField] float OffsetDistance = 1.0f;
    [SerializeField] float SizeOfInteracableArea = 1.5f;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapManager;
    [SerializeField] ToolAction onTilePickUp;
    [SerializeField] IconHighLight iconHighLight;

    [SerializeField] float MaxDistance = 1.5f;
    [SerializeField] int weaponEnegyCost = 1;
    AttackController attackController;
    Vector3Int SelectedTilePosition;
    bool Selectable;

    [SerializeField] float toolTimeOut = 1f;
    float timer;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        toolbarController = GetComponent<ToolbarController>();
        animator = GetComponent<Animator>();
        attackController = GetComponent<AttackController>();
        player = GetComponent<Player>();
        playerLevel = GetComponent<PlayerLevel>();  
    }

    private void Update()
    {
        if(timer > 0f) { timer -= Time.deltaTime; }
        if(Input.GetMouseButtonDown(0))
        {
            WeaponAction();
            //return;
        }
        SelectedTile();
        CanSelectCheck();
        Marker();
        if (Input.GetMouseButtonDown(0))
        {
            if(UseToolworld() == true)
            {
                return;
            }
            
            UseToolGrid();
        }
    }

    

    private void EnegyCost(int enegyCost)
    {
        player.GetTired(enegyCost);
    }

    private void SelectedTile()
    {
        SelectedTilePosition = tileMapManager.GetGridPosition(Input.mousePosition, true);
    }
    private void Marker()
    {
        markerManager.markedCellPosition = SelectedTilePosition;
        iconHighLight.cellPosition = SelectedTilePosition;
    }
    private void CanSelectCheck()
    {
        Vector2 PlayerPosition = transform.position;
        Vector2 CameraPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Selectable = Vector2.Distance(PlayerPosition, CameraPositon) < MaxDistance;
        markerManager.Show(Selectable);
        iconHighLight.CanSelect = Selectable;
    }
    private void WeaponAction()
    {
        Item item = toolbarController.GetItem;
        if (item == null) { return; }
        if (item.isWeapon == false) { return; }

        if (timer > 0f) { return; }
        EnegyCost(weaponEnegyCost);

        Vector2 position = rb.position + playerController.LastMotionVector * OffsetDistance;

        attackController.Attack(item.damage, playerController.LastMotionVector);

        timer = toolTimeOut;
    }
    //dụng cụ tương tác với cây, đá,.. nhưng không phải tile map
    private bool UseToolworld()
    {
        if (timer > 0f) { return false; }
        if (Selectable == true)
        {

            Vector2 position = rb.position + playerController.LastMotionVector * OffsetDistance;
            Item item = toolbarController.GetItem;
            if (item == null) { return false; }
            if (item.OnAction == null) { return false; }

            EnegyCost(GetEnegyCost(item.OnAction));

            animator.SetTrigger("act");
            bool complete = item.OnAction.OnApply(position);
            if (complete == true)
            {
                playerLevel.AddExperience(item.OnAction.SkillType, item.OnAction.skillExpReward);
                if (item.OnItemUsed != null)
                {
                    item.OnItemUsed.OnItemUsed(item, GameManager.Instance.inventoryContainer);
                }

            }
            timer = toolTimeOut;
            return complete;
        }
        else
        {
            return false;
        }
        
    }
    //dụng cụ tương tác với tile map
    private void UseToolGrid()
    {
        if (timer > 0f) { return; }
        if (Selectable == true)
        {
            Item item = toolbarController.GetItem;
            if (item == null)
            {
                PickUpTile();
                return;
            }
            if (item.OnTileMapAction == null) { return; }

            EnegyCost(GetEnegyCost(item.OnTileMapAction));

            bool complete = item.OnTileMapAction.OnApplyOnTileMap(
                SelectedTilePosition,
                tileMapManager,
                item);
            if (complete == true)
            {
                playerLevel.AddExperience(item.OnTileMapAction.SkillType, item.OnTileMapAction.skillExpReward);
                if (item.OnItemUsed != null)
                {
                    item.OnItemUsed.OnItemUsed(item, GameManager.Instance.inventoryContainer);
                }
            }
            timer = toolTimeOut;
        }
        
    }

    private int GetEnegyCost(ToolAction action)
    {
        int enegyCost = action.enegyCost;
        enegyCost -= playerLevel.GetLevel(action.SkillType);

        if(enegyCost < 1)
        {
            enegyCost = 1;
        }
        return enegyCost;
    }
    private void PickUpTile()
    {
        if(onTilePickUp == null) { return; }

        onTilePickUp.OnApplyOnTileMap(SelectedTilePosition, tileMapManager, null);
    }
}
