using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    //UI Texts
    public TMP_Text healthText;
    public TMP_Text shieldText;
    public TMP_Text shieldHealthText;
    public TMP_Text ammoText;
    public TMP_Text bombText;
    public TMP_Text timerText;
    public TMP_Text teamText;

    //UI Buttons
    public Button damageButton;
    public Button shieldButton;
    public Button changeTeamButton;
    public Button shootButton;
    public Button bombButton;
    public Button reloadButton;

    //Max values of stats
    public int maxHealth = 100;
    public int maxShield = 3;
    public int maxShieldHealth = 30;
    public int maxAmmo = 6;
    public int maxBomb = 2;

    //Timer value
    private float timeRemaining = 300f;

    //Current values of stats
    private int currentHealth;
    private int currentShield;
    private int currentShieldHealth;
    private int currentAmmo;
    private int currentBomb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentAmmo = maxAmmo;
        currentBomb = maxBomb;
        currentShieldHealth = 0;
        UpdateHUD();
        UpdateTimer();
        if (damageButton != null)
        {
            damageButton.onClick.AddListener(() => TakeDamage(5));
        }
        if (changeTeamButton != null)
        {
            changeTeamButton.onClick.AddListener(ChangeTeam);
        }
        if (shootButton != null)
        {
            shootButton.onClick.AddListener(Shoot);
        }
        if (bombButton != null)
        {
            bombButton.onClick.AddListener(UseBomb);
        }
        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(Reload);
        }
        if (shieldButton != null)
        {
            shieldButton.onClick.AddListener(Shield);
        }
    }

    void Update()
    {
        // Decrease the timer each frame
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0; // Ensure the timer doesn't go below zero
        }

        // Update the timer display
        UpdateTimer();
    }

    public void TakeDamage(int damage)
    {
        if (currentShieldHealth > 0)
        {
            if (damage < currentShieldHealth)
            {
                currentShieldHealth -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentShieldHealth;
                currentShieldHealth = 0;
            }
        }
        if (damage > 0)
        {
            currentHealth -= damage;
        }
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHUD();
    }

    public void Shield() {
        if (currentShield > 0) {
            currentShieldHealth = maxShieldHealth;
            currentShield--;
            UpdateHUD();
        }
    }

    private void ChangeTeam()
    {
        if (teamText.text == "B")
        {
            teamText.text = "A";
        }
        else
        {
            teamText.text = "B";
        }
    }

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateHUD();
        }
    }

    public void UseBomb()
    {
        if (currentBomb > 0)
        {
            currentBomb--;
            UpdateHUD();
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        UpdateHUD();
    }

    void UpdateHUD()
    {
        healthText.text = currentHealth.ToString();
        shieldText.text = currentShield.ToString();
        ammoText.text = currentAmmo.ToString();
        bombText.text = currentBomb.ToString();
        shieldHealthText.text = currentShieldHealth.ToString();
    }

    void UpdateTimer()
    {
        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Format the time to MM:SS format
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
