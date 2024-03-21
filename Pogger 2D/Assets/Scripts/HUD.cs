using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Image life1, life2, life3, life4, timeband;

    // Create a reference/property for text object
    public Text scoreText;

    public Text winText;
    public Text winTextSub;
    void Start()
    {
        HideWin();
    }

    public void UpdatePlayerLivesHUD(int playerHealth)
    {
        switch (playerHealth)
        {
            case 4:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = true;
                life4.enabled = true;
                break;
            case 3:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = true;
                life4.enabled = false;
                break;
            case 2:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = false;
                life4.enabled = false;
                break;
            case 1:
                life1.enabled = true;
                life2.enabled = false;
                life3.enabled = false;
                life4.enabled = false;
                break;
            case 0:
                life1.enabled = false;
                life2.enabled = false;
                life3.enabled = false;
                life4.enabled = false;
                break;
        }
    }

    public void UpdatePlayerScore (int score) // Parameter is the score to update player score with
    {
        // Gets current score
        // scoreText has the text property to hold score
        // Score is stored as string -> Need int.Parse() to convert to integer
        int currentScore = int.Parse(scoreText.text); // Parse string of the scoreText.text

        // Increases currentScore by parameter
        currentScore += score;

        // Updates scoreText to currentScore
        // Converts back to string from integer
        scoreText.text = currentScore.ToString();
    }

    public void ResetPlayerScore()
    {
        scoreText.text = "0";
    }

    public void ShowWin()
    {
        winText.enabled = true;
        winTextSub.enabled = true;
    }

    public void HideWin()
    {
        winText.enabled = false;
        winTextSub.enabled = false;
    }
}
