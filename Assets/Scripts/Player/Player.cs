using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]    
public class Stat
{
    public int maxVal;
    public int currVal;

    public Stat(int curr, int max)
    {
        this.currVal = curr;
        this.maxVal = max;
    }

    internal void Subtract(int amount)
    {
        currVal -= amount;
    }

    internal void Add(int amount)
    {
        currVal += amount;
        if(currVal > maxVal) { currVal = maxVal; }
    }

    internal void SetToMax()
    {
        currVal = maxVal;
    }
}



public class Player : MonoBehaviour, IDamageable
{
    public Stat hp;
    public Stat stamina;

    [SerializeField] StatusBarPanel hpBar;
    [SerializeField] StatusBarPanel staminaBar;

    public bool isDead;
    public bool isExhausted;

    DisableControls disableControls;
    PlayerRepawn playerRespawn;
    private void Awake()
    {
        disableControls = GetComponent<DisableControls>();
        playerRespawn = GetComponent<PlayerRepawn>();
    }
    private void Start()
    {
        UpdateHpBar();
        UpdateStaminaBar();
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Heal(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FullHeal();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GetTired(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Rest(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            FullRest(0);
        }


    }
    private void UpdateHpBar()
    {
        hpBar.Set(hp.currVal, hp.maxVal);
    }
    private void UpdateStaminaBar()
    {
        staminaBar.Set(stamina.currVal, stamina.maxVal);
    }
    public void TakeDamage(int amount)
    {
        if(isDead) return;
        hp.Subtract(amount);
        if(hp.currVal <= 0 && isDead != true)
        {
            Dead();
        }
        UpdateHpBar();
    }
    private void Dead()
    {
        isDead = true;
        disableControls.DisableControl();
        playerRespawn.StartRespawn();
    }
    public void Heal(int amount)
    {
        hp.Add(amount);
        UpdateHpBar();
    }
    public void FullHeal()
    {
        hp.SetToMax();
        UpdateHpBar();
    }
    public  void GetTired(int amount)
    {
        stamina.Subtract(amount);
        if(stamina.currVal < 0)
        {
            Echausted();
        }
        UpdateStaminaBar();
    }

    private void Echausted()
    {
        isExhausted = true;
        disableControls.DisableControl();
        playerRespawn.StartRespawn();
    }

    public void Rest(int amount)
    {
        stamina.Add(amount);
        UpdateStaminaBar();
    }
    public void FullRest(int amount)
    {
        stamina.SetToMax();
        UpdateStaminaBar();
    }

    public void ApplyDamage(int damage)
    {
        TakeDamage(damage);
    }

    public void CalculateDamage(ref int damage)
    {
        damage = 4;
    }

    public void CheckState()
    {
       
    }
}
