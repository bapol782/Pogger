using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public bool moveRight = true;

    // Reference to number of units in playing field
    private readonly float playAreaWidth = 19.0f; // 19 squares, entire distance of playing field

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.localPosition;

        if (moveRight)
        {
            pos.x += moveSpeed * Time.deltaTime;

            if (pos.x >= ((playAreaWidth / 2) -1) + (playAreaWidth - 1) - GetComponent<SpriteRenderer>().size.x / 2) // 2 columns on the sides are 1 unit wide, to know if log gets to edge of 1 column
                    // playAreaWidth - 1 = length of travel
            {
                pos.x = -playAreaWidth / 2 - GetComponent<SpriteRenderer>().size.x / 2; // Outer leftmost edge of playing field
            }
        }
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;

            if (pos.x <= ((-playAreaWidth / 2) + 1) - (playAreaWidth - 1) + GetComponent<SpriteRenderer>().size.x / 2)
            {
                pos.x = playAreaWidth / 2 + GetComponent<SpriteRenderer>().size.x / 2;
            }
        }

        transform.localPosition = pos;
    }
}
