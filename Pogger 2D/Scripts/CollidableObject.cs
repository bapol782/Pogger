using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{

    public bool isSafe;
    public bool isLog;
    public bool isTurtle;
    public bool isHomeBay;
    public bool isOccupied = false;

    Rect playerRect;

    // So that no need playerRect.Size.width or playerRect.Position.height
    Vector2 playerSize;
    Vector2 playerPosition;

    Rect collidableObjectRect;
    Vector2 collidableObjectSize;
    Vector2 collidableObjectPosition;

    // Only playerGameObject calls this function
    public bool IsColliding (GameObject playerGameObject)
    {
        // Any object can call this object to see if it's colliding with itself

        // Gets width & height of player stored in Vector2
        playerSize = playerGameObject.transform.GetComponent<SpriteRenderer>().size; // SpriteRenderer contains size property
        playerPosition = playerGameObject.transform.localPosition;

        // Stores size & position of collidable (player & sprite that's part of GameObject to test collision)
        collidableObjectSize = GetComponent<SpriteRenderer>().size;
        collidableObjectPosition = transform.localPosition;

        // Rect for player
        // playerPosition is always in the middle of sprite
        // x position - subtract half of player's width (playerSize.x)
        // y position - subtract half of player's height (playerSize.y)
        playerRect = new Rect(playerPosition.x - playerSize.x / 2, playerPosition.y - playerSize.y / 2, playerSize.x, playerSize.y);

        // Rect for Object to check to see if player is colliding with
        collidableObjectRect = new Rect(collidableObjectPosition.x - collidableObjectSize.x / 2, collidableObjectPosition.y - collidableObjectSize.y / 2, collidableObjectSize.x, collidableObjectSize.y);

        // Need to test for negative direction
        if (collidableObjectRect.Overlaps(playerRect, true)) // true to allowInverse
        {
            return true;
        }

        return false;
    }
}
