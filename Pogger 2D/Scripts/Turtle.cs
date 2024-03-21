using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{

    public enum TurtleType
    {
        TurtleTypeFloating,
        TurtleTypeDiving
    };

    public TurtleType turtleType = TurtleType.TurtleTypeFloating;

    // Need to change sprites
    public Sprite turtleDiveSprite;
    public Sprite turtleFloatSprite;

    public float moveSpeed = 5.0f;
    public bool moveRight = true;

    // Reference to number of units in playing field
    private readonly float playAreaWidth = 19.0f; // 19 squares, entire distance of playing field

    // Appear to dive first (blue)
    public bool shouldDive = false, shouldSurface = true, didDive = false, didSurface = false;
    private float surfaceTime = 5f;
    private float diveTime = 5f;
    private float surfaceTimer;
    private float diveTimer;
    private float transitionTime = 5f;
    private float transitionTimer;

    void Update()
    {
        UpdateTurtlePosition();
        UpdateDiveTurtleStatus();
    }

    void UpdateTurtlePosition()
    {
        Vector2 pos = transform.localPosition; // Gets current position of turtle and stores it as pos

        // Moving right
        if (moveRight)
        {
            pos.x += moveSpeed * Time.deltaTime;
            // If outside of play screen
            if (pos.x >= ((playAreaWidth / 2) - 1) + (playAreaWidth - 1) - GetComponent<SpriteRenderer>().size.x / 2) // 2 columns on the sides are 1 unit wide, to know if log gets to edge of 1 column
                                                                                                                      // playAreaWidth - 1 = length of travel
            {
                pos.x = -playAreaWidth / 2 - GetComponent<SpriteRenderer>().size.x / 2; // Outer leftmost edge of playing field
            }
        }
        // Moving left
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;

            if (pos.x <= ((-playAreaWidth / 2) + 1) - (playAreaWidth - 1) + GetComponent<SpriteRenderer>().size.x / 2)
            {
                pos.x = playAreaWidth / 2 + GetComponent<SpriteRenderer>().size.x / 2;
            }
        }

        transform.localPosition = pos; // Updates pos
    }

    void UpdateDiveTurtleStatus()
    {
        if (turtleType == TurtleType.TurtleTypeDiving)
        {
            if (shouldSurface == true) // Going to surface
            {
                transitionTimer += Time.deltaTime; // Starts transition timer

                if (transitionTimer >= transitionTime) // Once timer hits time
                {
                    shouldSurface = false;
                    transitionTimer = 0; // Resets to 0
                    didSurface = true;
                    GetComponent<SpriteRenderer>().sprite = turtleFloatSprite; // Change to float sprite
                }
            }

            if (didSurface == true)
            {
                surfaceTimer += Time.deltaTime; // Starts transition timer

                if (surfaceTimer >= surfaceTime)
                {
                    didSurface = false;
                    surfaceTimer = 0; // Resets surface timer to 0
                    GetComponent<SpriteRenderer>().sprite = turtleDiveSprite; // Change to dive sprite
                    shouldDive = true;
                }
            }

            if (shouldDive == true)
            {
                transitionTimer += Time.deltaTime;

                if (transitionTimer >= transitionTime)
                {
                    shouldDive = false;
                    didDive = true;
                    transitionTimer = 0;
                    GetComponent<SpriteRenderer>().enabled = false; // Won't render the sprite
                    GetComponent<CollidableObject>().isSafe = false;
                }
            }

            if (didDive == true)
            {
                diveTimer += Time.deltaTime; // Spends time in dive state

                if (diveTimer >= diveTime) // Run out of dive time
                {
                    didDive = false;
                    shouldSurface = true;
                    diveTimer = 0;
                    GetComponent<SpriteRenderer>().sprite = turtleDiveSprite;
                    GetComponent<CollidableObject>().isSafe = true;
                    GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }
}
