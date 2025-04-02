using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Player p1 = new Player(); // Player 1
    public Player p2 = new Player(); // Player 2

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