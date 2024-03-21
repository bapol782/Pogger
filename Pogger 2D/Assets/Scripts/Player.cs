using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Create 4 public variables for 4 sprites
    public Sprite playerUp, playerDown, playerLeft, playerRight;

    public int health = 4;

    private Vector2 originalPosition;

    private HUD hud;

    public float gameTime = 30;

    public float gameTimeWarning = 5;

    public float gameTimer;

    public AudioClip hopSound;
    public AudioClip squashSound;
    public AudioClip homeBaySound;
    public AudioClip extraLifeSound;
    public AudioClip introSound;
    public AudioClip levelStartSound;

    private Vector3 startingTimeBandScale;

    private bool gameStarted = false;
    private bool gameStarting = false;
    private bool levelStarted = false;
    private bool levelStarting = false;

    private AudioSource audioSource; // to add as a component to Player class

    private int maxY = 0, currentY = 1; // to keep track of current position and the furthest frog has been 

    private int numFrogsInBays = 0;

    private bool gameWon = false;

    void Start()
    {
        // Grabs AudioSource component and assigns it to audioSource variable
        audioSource = GetComponent<AudioSource>();

        originalPosition = transform.localPosition; // Reference to where player starts

        // To call any public method from HUD.cs
        hud = GameObject.Find("CanvasHUD").GetComponent<HUD>(); // Gets HUD script from CanvasHUD gameObject and stores in hud variable

        startingTimeBandScale = hud.timeband.GetComponent<RectTransform>().localScale; // Gets starting scale

        GameReset();
    }

    void Update()
    {
        UpdatePosition(); // Always updating position
        CheckCollisions(); // Always check if colliding
        CheckGameTimer();
        CheckBypassIntro(); // Press enter to bypass intro sound
        CheckLevelStarted();
        CheckPlayAgain();
    }

    // updates time band
    private void CheckGameTimer()
    {
        // time band shouldn't be updated until player started level, not the game
        // StartSound finishes playing
        if (levelStarted == true)
        {
            gameTimer += Time.deltaTime;

            // Subtracting current game timer from original starting scale and set as new scale
            Vector3 scale = new Vector3(startingTimeBandScale.x - gameTimer, startingTimeBandScale.y, startingTimeBandScale.z);

            hud.timeband.GetComponent<RectTransform>().localScale = scale;

            if (gameTimer >= startingTimeBandScale.x - gameTimeWarning) // startingTimeBandScale.x = 30
                                                                        // gameTimeWarning = 5
            {
                hud.timeband.color = Color.red; // ineffective - cts setting color to red
            }
            else
            {
                hud.timeband.color = Color.black;
            }
            if (gameTimer >= gameTime)
            {
                PlayerDied();
            }
        }
    }

    private void CheckPlayAgain()
    {
        if (gameWon == true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gameWon = false;
                hud.HideWin();
                GameReset();
            }
        }
    }

    private void CheckBypassIntro()
    {
        // only time intro sound is playing is when game is starting
        if (gameStarting == true)
        {
            // if Enter key is pressed, play level start sound
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlayLevelStartSound();
                // AudioSource.PlayClipAtPoint(levelStartSound, Vector3.zero);

                gameStarting = false; // since start has already started
                gameStarted = true;
                levelStarting = true; // level is starting once player presses Enter

                // Reset score to 0 since restarting game
                hud.ResetPlayerScore();

                // show frog and time band
                GetComponent<SpriteRenderer>().enabled = true;
                hud.timeband.enabled = true;
            }
        }
    }

    private void CheckLevelStarted()
    {
        // only time levelStarting is true is when player presses Enter
        if (levelStarting == true)
        {
            // if current audioSource is not playing
            if (!audioSource.isPlaying)
            {
                // this won't run anymore               
                levelStarted = true;
                // level starting sound finished playing
                levelStarting = false;
            }
        }
    }

    private void UpdatePosition()
    {
        // if game is not won yet, allow movement
        if (!gameWon)
        {
            // if level hasn't started, don't check movement updates
            // only time that allows any movement (intro sound and level start sound done playing)
            if (levelStarted == true)
            {
                // Stores current position of GameObject
                Vector2 pos = transform.localPosition; // Get current position of transform, everytime update position, need to know previous position 

                // Incredement position based on movement
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GetComponent<SpriteRenderer>().sprite = playerUp;

                    pos += Vector2.up; // Moving up (0,1)

                    // only give points if currentY > maxY
                    if (currentY > maxY)
                    {
                        hud.UpdatePlayerScore(10);
                        // only increase maxY when player is given points
                        maxY = currentY;
                    }

                    currentY++; // currentY increases by 1

                    PlayHopSound();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    GetComponent<SpriteRenderer>().sprite = playerDown;

                    if (pos.y > -6)
                    {
                        pos += Vector2.down; // Moving down (0,-1)

                        currentY--; // decreases currentY by 1
                        PlayHopSound();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    GetComponent<SpriteRenderer>().sprite = playerLeft;

                    if (pos.x > -8)
                    {
                        pos += Vector2.left; // Moving left (-1,0)

                        PlayHopSound();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    GetComponent<SpriteRenderer>().sprite = playerRight;

                    if (pos.x < 8)
                    {
                        pos += Vector2.right; // Moving right (1,0) OR pos += new Vector2(1,0)

                        PlayHopSound();
                    }
                }

                // Updates pos to the new position
                transform.localPosition = pos;
            }
        }    
    }

    private void CheckCollisions()
    {
        bool isSafe = true;

        // Checks scene to find all GameObjects tagged "CollidableObject" and store them in gameObjects array (of GameObject)
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("CollidableObject");

        foreach (GameObject go in gameObjects) // Creates a GameObject called go for every gameObject in gameObjects
        {
            CollidableObject collidableObject = go.GetComponent<CollidableObject>();

            if (collidableObject.IsColliding(this.gameObject)) // player script is on the Player GameObject
            {
                if(collidableObject.isSafe)
                {
                    isSafe= true;      
                    
                    if (collidableObject.isLog)
                    {
                        Vector2 pos = transform.localPosition;

                        // If log is moving right
                        if (collidableObject.GetComponent<Log>().moveRight)
                        {
                            pos.x += collidableObject.GetComponent<Log>().moveSpeed * Time.deltaTime; // New position for frog

                            if (transform.localPosition.x >= 9.5f) // Position it's currently at
                            // 9.5 is at the edge of play screen
                            {
                                pos.x = transform.localPosition.x - 18f; // moves to the other log
                            }
                        }
                        // If log is moving left
                        else
                        {
                            pos.x -= collidableObject.GetComponent<Log>().moveSpeed * Time.deltaTime;

                            if (transform.localPosition.x <= -9.5f)
                            {
                                pos.x = transform.localPosition.x + 18f;
                            }
                        }

                        transform.localPosition = pos; // Assigns pos to local position
                    }
                    else if (collidableObject.isTurtle)
                    {
                        Vector2 pos = transform.localPosition; // Needed since pos used in if statement
                        if (collidableObject.GetComponent<Turtle>().moveRight) // If turtle moves right
                        {
                            pos.x += collidableObject.GetComponent<Turtle>().moveSpeed * Time.deltaTime;
                            // Shift to the rightmost screen when touch left border
                            if (transform.localPosition.x > 9.5f)
                            {
                                pos.x = transform.localPosition.x - 18f;
                            }
                        }
                        else // If turtle moves left
                        {
                            pos.x -= collidableObject.GetComponent<Turtle>().moveSpeed * Time.deltaTime;
                            
                            if (transform.localPosition.x <= -9.5f)
                            {
                                pos.x = transform.localPosition.x + 18f;
                            }
                        }

                        transform.localPosition = pos; // Need to assign position back to localPosition
                    }

                    if (collidableObject.isHomeBay)
                    {
                        if (!collidableObject.isOccupied) // If not occupied
                        {
                            // Calls instantiate method to create a trophy
                            // Casting a GameObject to instantiate since telling it to instantiate gameObject
                            // Loads the trophy resource in Prefabs folder
                            // What kind of resource it is = GameObject, 1st parameter wants gameObject
                            // collidableObject.transform.localPosition = Gets homebay position and assigns it to position of trophy
                            // Quaternion.identity gives normal rotation
                            GameObject trophy = (GameObject)Instantiate(Resources.Load("Prefabs/trophy", typeof(GameObject)), collidableObject.transform.localPosition, Quaternion.identity);
                            trophy.tag = "Trophy";

                            // Points awarded for reaching homebay
                            hud.UpdatePlayerScore(50);

                            collidableObject.isOccupied = true;

                            // Incrementing number of frogs in bays
                            numFrogsInBays++;

                            if (numFrogsInBays == 5)
                            {
                                GameWon();
                            }

                            // Variable to hold time remaining
                            // Cast int since gameTimer is a float, want int value
                            int timeRemaining = (int)(gameTime - gameTimer);

                            // Bonus score for extra time
                            hud.UpdatePlayerScore(timeRemaining * 10);

                            PlayHomeBaySound();

                            ResetTimer();
                        }
                        ResetPosition();
                    }

                    break; // breaks out of foreach loop to the if not safe statement
                }
                else // set to not safe once everytime it collides with smt not safe 
                {
                    isSafe = false;
                }
            }
        }

        if (!isSafe)
        {
            if (health == 0)
            {
                GameOver();
            }
            else
            {
                PlayerDied();
            }
            
        }
    }

    void PlayerDied()
    {
        DecreaseHealth();
        ResetTimer();
        ResetPosition();
        PlaySquashSound();
    }

    void DecreaseHealth()
    {
        health -= 1;
    }

    void ResetTimer() // Resets timer for next life
    {
        gameTimer = 0;
    }

    void GameOver()
    {
        GameReset();
    }

    void GameReset()
    {
        health = 4;
        ResetPosition();

        levelStarted = false;
        levelStarting = false;
        gameStarted = false;
        gameStarting = true;

        // when game starts, play intro
        PlayIntroSound();

        // disable frog at the start
        // SpriteRenderer component is for drawing the sprite to the screen
        GetComponent<SpriteRenderer>().enabled = false; // unchecks Enabled property box in Unity

        // disable time band at the start
        hud.timeband.enabled = false;

        // reset number of frogs in bays
        numFrogsInBays = 0;

        // remove trophies
        RemoveTrophies();
    }

    void GameWon()
    {
        // Win the game
        hud.ShowWin();
        gameWon = true;

        // frog will disappear
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void RemoveTrophies()
    {
        GameObject[] trophies;
        // fills array with all GameObjects that have tag "Trophy"
        trophies = GameObject.FindGameObjectsWithTag("Trophy");

        // look into the trophies array
        // for each iteration, assign index to trophy GameObject
        foreach (GameObject trophy in trophies)
        {
            // removes all trophies
            Destroy(trophy.gameObject);
        }
    }

    void ResetPosition()
    {
        hud.UpdatePlayerLivesHUD(health);

        transform.localPosition = originalPosition; // Reverts current position to original position
        transform.GetComponent<SpriteRenderer>().sprite = playerUp; // Reset to upwards sprite

        // Resets currentY and maxY
        maxY = 0;
        currentY = 1;
    }

    private void PlayHopSound()
    {
        audioSource.Stop();
        audioSource.clip = hopSound;
        audioSource.Play();
        // AudioSource.PlayClipAtPoint(hopSound, Vector3.zero); // in 3D environment, can tell where to play sound
        // Vector3.zero = reset vector (0, 0, 0)
    }

    void PlaySquashSound()
    {
        audioSource.Stop();
        audioSource.clip = squashSound;
        audioSource.Play();
        // AudioSource.PlayClipAtPoint(squashSound, Vector3.zero);
    }

    void PlayHomeBaySound()
    {
        audioSource.Stop();
        audioSource.clip = homeBaySound;
        audioSource.Play();
        // AudioSource.PlayClipAtPoint(homeBaySound, Vector3.zero);
    }

    void PlayExtraLifeSound()
    {
        audioSource.Stop();
        audioSource.clip = extraLifeSound;
        audioSource.Play();
        // AudioSource.PlayClipAtPoint(extraLifeSound, Vector3.zero);
    }

    void PlayIntroSound()
    {
        audioSource.Stop();
        audioSource.clip = introSound;
        audioSource.Play();
    }

    void PlayLevelStartSound()
    {
        audioSource.Stop();
        audioSource.clip = levelStartSound;
        audioSource.Play();
    }
}
