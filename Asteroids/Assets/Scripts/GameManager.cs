using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static GameManager instance;
    /// <summary>
    /// Singleton Accessor.
    /// </summary>
    /// <returns>Instance of class</returns>
    public static GameManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Different Game States.
    /// </summary>
    public enum GameStates
    {
        WaitToStart,
        Playing,
        GameOver,
    }

    /// <summary>
    /// Reference to Player Controller in the scene.
    /// </summary>
    [Tooltip("Player Controller in the scene")]
    [SerializeField]
    private PlayerController player;

    /// <summary>
    /// Reference to the Text UI field for the score.
    /// </summary>
    [Tooltip("UI Text element for the score")]
    [SerializeField]
    private Text score;
    /// <summary>
    /// Reference to the Text UI field for the game message.
    /// </summary>
    [Tooltip("UI Text element for the game message")]
    [SerializeField]
    private Text gameMessage;

    /// <summary>
    /// Stores the current state of the game.
    /// </summary>
    private GameStates currentState;

    /// <summary>
    /// Stores the current score for the level.
    /// </summary>
    private int currentScore;

    /// <summary>
    /// Called when GameObject is woken.
    /// </summary>
    private void Awake()
    {
        instance = this;
        SetGameState(GameStates.WaitToStart);
        GameRestart();
    }

    /// <summary>
    /// Start the game.
    /// </summary>
    public void StartGame()
    {
        SetGameState(GameStates.Playing);
    }

    /// <summary>
    /// Restart the game and call other objects GameRestart().
    /// </summary>
    public void Restart()
    {
        GameRestart();
        GetPlayer().GameRestart();
        AsteroidManager.GetInstance().GameRestart();
        BulletManager.GetInstance().GameRestart();

        SetGameState(GameStates.Playing);
    }

    /// <summary>
    /// Called when game is restarted - reset.
    /// </summary>
    public void GameRestart()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    /// <summary>
    /// Update the manager.
    /// </summary>
    void Update()
    {
        switch (currentState)
        {
            case GameStates.WaitToStart:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StartGame();
                }
                break;
            case GameStates.Playing:
                break;
            case GameStates.GameOver:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Restart();
                }
                break;
        }
    }

    /// <summary>
    /// Get the current Game State.
    /// </summary>
    /// <returns>Current Game State</returns>
    public GameStates GetGameState()
    {
        return currentState;
    }
    /// <summary>
    /// Sets the current Game State.
    /// </summary>
    /// <param name="state">New Game State</param>
    public void SetGameState(GameStates state)
    {
        currentState = state;

        switch (state)
        {
            case GameStates.WaitToStart:
                gameMessage.text = "Press Enter to Start";
                break;
            case GameStates.Playing:
                gameMessage.text = "";
                break;
            case GameStates.GameOver:
                gameMessage.text = "GAME OVER!\nPress Enter to Restart";
                break;
        }
    }

    /// <summary>
    /// Update the score Text field with the current score.
    /// </summary>
    public void UpdateScoreText()
    {
        score.text = "Score: " + currentScore;
    }
    /// <summary>
    /// Increase the current score by an amount.
    /// </summary>
    /// <param name="amount">Amount to increase score by</param>
    public void IncreaseScore(int amount)
    {
        currentScore += amount;
        UpdateScoreText();
    }
    /// <summary>
    /// Get the current score.
    /// </summary>
    /// <returns>Current score</returns>
    public int GetScore()
    {
        return currentScore;
    }

    /// <summary>
    /// Get reference to the Player Controller.
    /// </summary>
    /// <returns>Player Controller</returns>
    public PlayerController GetPlayer()
    {
        return player;
    }
}
