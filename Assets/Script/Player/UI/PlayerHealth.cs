using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar;
    public RegainBar RegainBar;
    public ConcentrateBar ConcentrateBar;
    private PlayerController playercontroller;
    ScreenFlash flash;

    [SerializeField] private int MaxConcentrate = 500;
    [SerializeField] private int maxHealth = 500;
    [SerializeField] private int prevHealth;
    [SerializeField] private int loseHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int concentrate;

    private bool OnDamaged;


    private float regainTimer = 0;

    private void Awake()
    {
        playercontroller = GetComponent<PlayerController>();
    }
    private void Start()
    {
        flash = playercontroller.flash;
    }
    public void PlayerUISet()
    {
        currentHealth = maxHealth;
        prevHealth = maxHealth;
        loseHealth = maxHealth;
        concentrate = MaxConcentrate;
    }

    public int GetConcentrate()
    {
        return concentrate;
    }

    public void ChargeConcentrate()
    {
        if (concentrate >= MaxConcentrate)
        {
            return;
        }
        concentrate++;
    }

    public void SetHealth()
    {
        if (currentHealth > prevHealth)
        {
            prevHealth = currentHealth;
            loseHealth = currentHealth;
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        RegainBar.SetMaxRegainHealth(maxHealth);
        RegainBar.SetRegainHealth(loseHealth);

        ConcentrateBar.SetMax(MaxConcentrate);
        ConcentrateBar.SetGage(concentrate);
    }


    public void TakeDamage(int damage)
    {
        flash.Hurt();
        if (prevHealth != currentHealth)
        {
            loseHealth = prevHealth;
        }

        prevHealth = currentHealth;
        currentHealth -= damage;
        OnDamaged = true;
        regainTimer = 0;
        if (currentHealth <= 0)
        {
            playercontroller.PlayerDead();
        }
    }

    public void Regain()
    {
        if (OnDamaged)
        {
            if (prevHealth <= loseHealth)
            {
                loseHealth--;
            }

            regainTimer += Time.deltaTime;
            if (regainTimer > 3f)
            {
                prevHealth--;
                if (prevHealth <= currentHealth)
                {
                    OnDamaged = false;
                    regainTimer = 0;
                    prevHealth = currentHealth;
                }
            }
        }
    }

    public void RegainHealth()
    {
        Debug.Log("RegainHealth");
        int count = 0;
        while (currentHealth < prevHealth)
        {
            Debug.Log("Health+1");
            currentHealth++;
            count++;
            if (count == 10)
            {
                break;
            }
        }
    }

}
