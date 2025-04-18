using System;
using System.Runtime.InteropServices;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class HUDManager : MonoBehaviour
{
    //UI Texts
    public TMP_Text healthText;
    public TMP_Text shieldText;
    public TMP_Text shieldHealthText;
    public TMP_Text ammoText;
    public TMP_Text bombText;
    public TMP_Text teamText;
    public TMP_Text scoreText;

    //Border Images
    public Q_Vignette_Single shieldImage;
    public Image damageImage;
    public Image missImage;
    public Image P1HealthBarFront;
    public Image P1HealthBarBack;
    public Image P1ShieldBarFront;
    public Image P1ShieldBarBack;
    public Image P2HealthBarFront;
    public Image P2HealthBarBack;
    public Image P2ShieldBarFront;
    public Image P2ShieldBarBack;

    //UI Buttons
    public Button damageSmallButton;
    public Button damageLargeButton;
    public Button shieldButton;
    public Button changeTeamButton;
    public Button shootButton;
    public Button bombButton;
    public Button reloadButton;
    public Button selfShieldButton;
    public Button VRButton;

    //Max values of stats
    public int maxHealth = 100;
    public int maxShield = 3;
    public int maxShieldHealth = 30;
    public int maxAmmo = 6;
    public int maxBomb = 2;
    public string score;

    //Player stats
    public PlayerManager playerManager;
    Player p1;
    Player p2;

    //VR
    public RenderTexture VRRenderTexture;
    public Canvas VRCanvas;
    private bool isVR = false;

    private float lerpTimer;
    public float chipSpeed = 5f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        shieldImage.mainScale = 0;
        p1 = playerManager.p1;
        p2 = playerManager.p2;
        p1.hp = p2.hp = maxHealth;
        p1.shields = p2.shields = maxShield;
        p1.shield_hp = p2.shield_hp = 0;
        p1.bullets = p2.bullets = maxAmmo;
        p1.bombs = p2.bombs = maxBomb;
        scoreText.text = "0 : 0";
        lerpTimer = 0f;
        UpdateHUD();
        if (damageSmallButton != null)
        {
            damageSmallButton.onClick.AddListener(() => TakeDamage(5));
        }
        if (damageLargeButton != null)
        {
            damageLargeButton.onClick.AddListener(() => TakeDamage(10));
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
        if (selfShieldButton != null)
        {
            selfShieldButton.onClick.AddListener(SelfShield);
        }
        // if (VRButton != null)
        // {
        //     VRButton.onClick.AddListener(EnterVR);
        // }
    }

    void Update()
    {
        // Update the timer display
        UpdateBarUI(P1HealthBarFront, P1HealthBarBack, p1.hp, maxHealth);
        UpdateBarUI(P1ShieldBarFront, P1ShieldBarBack, p1.shield_hp, maxShieldHealth);
        UpdateBarUI(P2HealthBarFront, P2HealthBarBack, p2.hp, maxHealth);
        UpdateBarUI(P2ShieldBarFront, P2ShieldBarBack, p2.shield_hp, maxShieldHealth);
        if (isVR && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ExitVR();
        }
    }

    public void TakeDamage(int damage)
    {
        if (p1.shield_hp > 0)
        {
            if (damage < p1.shield_hp)
            {
                p1.shield_hp -= damage;
                damage = 0;
            }
            else
            {
                damage -= p1.shield_hp;
                p1.shield_hp = 0;
                shieldImage.mainScale = 0;
            }
        }
        if (damage > 0)
        {
            p1.hp -= damage;
        }
        if (p1.hp < 0)
        {
            p1.hp = 0;
        }
        Debug.Log("current hp = " + p1.hp);
        UpdateHUD();
    }

    public void Shield() //toggles opponent shield
    {
    }

    public void SelfShield() //toggles self shield
    {
        if (p1.shield_hp == 0 && p1.shields > 0)
        {
            shieldImage.mainScale = 1f;
            p1.shield_hp = maxShieldHealth;
            p1.shields--;
        }
        UpdateHUD();
    }

    private void ChangeTeam()
    {
        // CS1010: teamText.text = teamText.text == "A" ? "B" : "A"; 
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
        if (playerManager.GetCurrentPlayer().bullets > 0)
        {
            playerManager.GetCurrentPlayer().bullets--;
            UpdateHUD();
        }
    }

    public void UseBomb()
    {
        if (playerManager.GetCurrentPlayer().bombs > 0)
        {
            playerManager.GetCurrentPlayer().bombs--;
            UpdateHUD();
        }
    }

    public void Reload()
    {
        playerManager.GetCurrentPlayer().bullets = maxAmmo;
        UpdateHUD();
    }

    public void UpdatePlayer(Player player, int hp, int shields, int shield_hp, int bullets, int bombs, int p1deaths, int p2deaths)
    {
        player.hp = hp;
        player.shields = shields;
        player.shield_hp = shield_hp;
        player.bullets = bullets;
        player.bombs = bombs;
        scoreText.text = p2deaths.ToString() + " : " + p1deaths.ToString();
        UpdateHUD();
        UpdateShield();
    }


    void UpdateHUD()
    {
        healthText.text = playerManager.GetCurrentPlayer().hp.ToString();
        shieldText.text = playerManager.GetCurrentPlayer().shields.ToString();
        ammoText.text = playerManager.GetCurrentPlayer().bullets.ToString();
        bombText.text = playerManager.GetCurrentPlayer().bombs.ToString();
        shieldHealthText.text = playerManager.GetCurrentPlayer().shield_hp.ToString();
    }

    public void UpdateShield()
    {
        if (playerManager.GetCurrentPlayer().shield_hp > 0)
        {
            shieldImage.mainScale = 1f;
        }
        else
        {
            shieldImage.mainScale = 0;
        }
    }



    public void UpdateBarUI(Image frontBar, Image backBar, int currentAmount, int maxAmount)
    {
        float fillF = frontBar.fillAmount;
        float fillB = backBar.fillAmount;
        float hfraction = (float)currentAmount / (float)maxAmount;

        if (fillB > hfraction)
        {
            frontBar.fillAmount = hfraction;
            backBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backBar.fillAmount = Mathf.Lerp(fillB, hfraction, percentComplete);
            if (backBar.fillAmount == hfraction)
            {
                lerpTimer = 0f; // Reset lerpTimer after animation completes
            }
        }
        if (fillF < hfraction)
        {
            backBar.color = Color.green;
            backBar.fillAmount = hfraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontBar.fillAmount = Mathf.Lerp(fillF, backBar.fillAmount, percentComplete);
            if (frontBar.fillAmount == hfraction)
            {
                lerpTimer = 0f; // Reset lerpTimer after animation completes
            }
        }
    }

    public void EnterVR()
    {
        Camera.main.targetTexture = VRRenderTexture;
        VRCanvas.gameObject.SetActive(true);
        isVR = true;
    }

    public void ExitVR()
    {
        if (isVR)
        {
            Camera.main.targetTexture = null;
            VRCanvas.gameObject.SetActive(false);
            isVR = false;
        }

    }

    public void ShowDamage()
    {
        StartCoroutine(FadeImage(damageImage));
    }

    public void ShowMiss()
    {
        StartCoroutine(FadeImage(missImage));
    }

    private IEnumerator FadeImage(Image image)
    {
        // Ensure the image is fully visible at the start
        if (image == missImage) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.03f);
            Debug.Log("Miss image color: " + image.color);
        }
        else {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
        

        // Gradually fade out the image over 1 second
        float fadeDuration = 1f; // Duration of the fade-out effect
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }

        // Ensure the image is fully transparent at the end
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
    }
}
