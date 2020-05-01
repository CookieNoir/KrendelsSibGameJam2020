using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private int[,] movementMatrix =
            {//------0,1,2,3,4,5,6,7,8,9,10,11,12
            /*-0*/      { 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
            /*-1*/      { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            /*-2*/      { 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0},
            /*-3*/      { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0},
            /*-4*/      { 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0},
            /*-5*/      { 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1},
            /*-6*/      { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1},
            /*-7*/      { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1},
            /*-8*/      { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
            /*-9*/      { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1},
        };
    private Vector2Int playerPosition = new Vector2Int(6, 10);
    private bool isMoving = false;
    private Vector3 destination;
    private Vector2Int pointer;
    private void Start()
    {
        Debug.Log(movementMatrix[6,10]);
        transform.position = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
    }

    private void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.x > 0 && movementMatrix[pointer.x-1, pointer.y] != 1)
                {
                    pointer.x--;
                }
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.x< 9 && movementMatrix[pointer.x+1, pointer.y] != 1)
                {
                    pointer.x++;
                }
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.y > 0 && movementMatrix[pointer.x, pointer.y-1] != 1)
                {
                    pointer.y--;
                }
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.y < 11 && movementMatrix[pointer.x, pointer.y+1] != 1)
                {
                    pointer.y++;
                }
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                isMoving = true;
            }
        }
        else
        {
            transform.position = destination;
            isMoving = false;
        }
    }
}
