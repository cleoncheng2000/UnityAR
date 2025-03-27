using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Player p1 = new Player(); // Player 1
    public Player p2 = new Player(); // Player 2

    public Button p1Button;
    public Button p2Button;

    private Player currentPlayer; // Currently selected player

    void Start()
    {
        // Default to Player 1
        currentPlayer = p1;
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
        Debug.Log("Player 1 selected.");
    }

    // Method to switch to Player 2
    public void SelectPlayer2()
    {
        currentPlayer = p2;
        Debug.Log("Player 2 selected.");
    }

    // Method to get the currently selected player
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
}