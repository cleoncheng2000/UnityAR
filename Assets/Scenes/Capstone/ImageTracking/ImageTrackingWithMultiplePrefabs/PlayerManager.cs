using System.Drawing.Printing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Player p1 = new Player(); // Player 1
    public Player p2 = new Player(); // Player 2
    public int prevBombs = 2;
    public int prevBullet = 6;


    public Button p1Button;
    public Button p2Button;

    public TMP_Text p1Text;
    public TMP_Text p2Text;

    private Player currentPlayer; // Currently selected player

    void Start()
    {
        // Default to Player 1
        currentPlayer = p1;
        HighlightPlayer();
        if (p1Button != null)
        {
            p1Button.onClick.AddListener(SelectPlayer1);
        }
        if (p2Button != null)
        {
            p2Button.onClick.AddListener(SelectPlayer2);
        }
    }

    // Method to switch to Player 1
    public void SelectPlayer1()
    {
        currentPlayer = p1;
        HighlightPlayer();  
        Debug.Log("Player 1 selected.");
    }

    // Method to switch to Player 2
    public void SelectPlayer2()
    {
        currentPlayer = p2;
        HighlightPlayer();
        Debug.Log("Player 2 selected.");
    }

    // Method to get the currently selected player
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void setPlayer1(int hp, int bombs, int bullets, int shields, int shield_hp, int deaths) // Set the current player and their bombs
    {
        p1.hp = hp;
        p1.bombs = bombs;
        p1.bullets = bullets;
        p1.shields = shields;
        p1.shield_hp = shield_hp;
        p1.deaths = deaths;
    }

    public void setPlayer2(int hp, int bombs, int bullets, int shields, int shield_hp, int deaths) // Set the current player and their bombs
    {
        p2.hp = hp;
        p2.bombs = bombs;
        p2.bullets = bullets;
        p2.shields = shields;
        p2.shield_hp = shield_hp;
        p2.deaths = deaths;
    }

    public void HighlightPlayer() {
        if (currentPlayer == p1) {
            p1Text.color = Color.green;
            p2Text.color = Color.white;
        } else {
            p1Text.color = Color.white;
            p2Text.color = Color.green;
        }
    }
}