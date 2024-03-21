using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool moveRight = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.localPosition;

        if (moveRight)
        {
            // x value of Vector2
            // Moving right
            pos.x += Vector2.right.x * moveSpeed * Time.deltaTime; // (1,0)

            if (pos.x >= 10)
            {
                pos.x = -10;
            }
        }
        else
        {
            // Moving left
            pos.x += Vector2.left.x * moveSpeed * Time.deltaTime; //(-1,0)

            if (pos.x <= -10)
            {
                pos.x = 10;
            }
        }

        

        transform.localPosition = pos; // Updates localPosition to new position since car moved
    }
}
