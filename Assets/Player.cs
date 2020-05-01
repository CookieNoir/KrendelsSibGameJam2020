using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 3f;

    [HideInInspector]
    public Vector2Int playerPosition = new Vector2Int(6, 10);
    private Vector2Int playerPositionPrev;
    [HideInInspector]
    public bool isMoving = false;

    public Image[] inventorySlots;
    public GameObject interactionText_Object;
    public Text interactionText_Text;

    private int taskId;
    private int eventId;
    private bool isUsed = false;
    private int[] itemsId;

    private int[,] movementMatrix =
            {//------     0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12
            /*-0*/      { 1, 3, 4, 1, 1, 1, 1, 0, 5, 1, 1, 1, 1},
            /*-1*/      { 1, 3, 0, 6, 0, 2, 0, 7, 0, 5, 1, 1, 5},
            /*-2*/      { 3, 0, 1, 0, 1, 1, 1, 1, 7, 0, 0, 0, 0},
            /*-3*/      { 0, 0, 0, 8, 1, 1, 1, 1, 7, 0, 0, 9, 9},
            /*-4*/      { 1, 0, 8, 1, 1,10, 1, 1, 0, 0, 0, 1, 1},
            /*-5*/      { 1, 1, 0, 0, 1, 8, 0, 0, 5, 1, 1, 1, 1},
            /*-6*/      { 4, 0, 0,11, 1, 1, 1, 0, 0, 5, 0, 1, 1},
            /*-7*/      { 1, 4, 0,12, 0, 0, 1, 0, 0, 0, 0, 1, 1},
            /*-8*/      { 4, 0, 1, 1, 1, 0, 0, 5, 0, 0,16, 1, 1},
            /*-9*/      { 1,15, 0, 0, 0, 0,14, 1, 5,16, 1, 1, 1},
            /*10*/      { 1,15,13, 1,13,14, 1, 1, 1, 1, 1, 1, 1},
        };
    private Vector2 borders = new Vector2(10, 12);
    private Vector3 destination;
    private Vector3 direction;
    private Vector2Int pointer;
    private void Start()
    {
        transform.position = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
        interactionText_Object.SetActive(false);
        itemsId = new int[4];
    }

    private bool addItemToInventory(int id)
    {
        return false;
    }

    private bool takeItemFromInventory(int id)
    {
        return false;
    }

    private void DoAction()
    {
        switch (eventId)
        {
            case 2:
                {
                    break;
                }
            case 3:
                {
                    break;
                }
            case 4:
                {
                    break;
                }
            case 5:
                {
                    break;
                }
            case 6:
                {
                    //if (addItemToInventory(1))
                    break;
                }
            case 7:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "взять рыбу";
                    break;
                }
            case 8:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "налить воды";
                    break;
                }
            case 9:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "взять книгу";
                    break;
                }
            case 10:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "пообщаться с котом";
                    break;
                }
            case 11:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "включить лампу";
                    break;
                }
            case 12:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "взять конфеты";
                    break;
                }
            case 13:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "сломать телевизор";
                    break;
                }
            case 14:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "найти столицу Франции";
                    break;
                }
            case 15:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "играть на пианино";
                    break;
                }
            case 16:
                {
                    interactionText_Object.SetActive(true);
                    interactionText_Text.text = "взять тапки";
                    break;
                }
            default:
                {
                    break;
                }
        }
        interactionText_Object.SetActive(false);
        isUsed = true;
    }

    private void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.x > 0 && movementMatrix[pointer.x - 1, pointer.y] != 1)
                {
                    pointer.x--;
                }
                playerPositionPrev = playerPosition;
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                direction = -Vector3.right;
                isMoving = true;
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                while (pointer.x < borders.x && movementMatrix[pointer.x + 1, pointer.y] != 1)
                {
                    pointer.x++;
                }
                playerPositionPrev = playerPosition;
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                direction = Vector3.right;
                isMoving = true;
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                if (movementMatrix[pointer.x, pointer.y] == 2) pointer.y--;
                while (pointer.y > 0 && movementMatrix[pointer.x, pointer.y - 1] != 1 && movementMatrix[pointer.x, pointer.y] != 2)
                {
                    pointer.y--;
                }
                playerPositionPrev = playerPosition;
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                direction = -Vector3.forward;
                isMoving = true;
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {               
                pointer = new Vector2Int(playerPosition.x, playerPosition.y);
                if (movementMatrix[pointer.x, pointer.y] == 2) pointer.y++;
                while (pointer.y < borders.y && movementMatrix[pointer.x, pointer.y + 1] != 1 && movementMatrix[pointer.x, pointer.y] != 2)
                {
                    pointer.y++;
                }
                playerPositionPrev = playerPosition;
                playerPosition = pointer;
                destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                direction = Vector3.forward;
                isMoving = true;
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                }
            }
            else if (!isUsed && Input.GetKey(KeyCode.F))
            {
                DoAction();
            }
        }
        else
        {
            if (Vector3.Magnitude(destination - transform.position) > Time.deltaTime * speed)
                transform.position += direction * Time.deltaTime * speed;
            else
            {
                transform.position = destination;

                eventId = movementMatrix[playerPosition.x, playerPosition.y];
                if (!isUsed)
                    switch (eventId)
                    {
                        case 2:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "осмотреть кладовую";
                                break;
                            }
                        case 3:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "приготовить еду";
                                break;
                            }
                        case 4:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "полить цветок";
                                break;
                            }
                        case 5:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "искать скотч";
                                break;
                            }
                        case 6:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять сосиски";
                                break;
                            }
                        case 7:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять рыбу";
                                break;
                            }
                        case 8:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "налить воды";
                                break;
                            }
                        case 9:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять книгу";
                                break;
                            }
                        case 10:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "пообщаться с котом";
                                break;
                            }
                        case 11:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "включить лампу";
                                break;
                            }
                        case 12:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять конфеты";
                                break;
                            }
                        case 13:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "сломать телевизор";
                                break;
                            }
                        case 14:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "найти столицу Франции";
                                break;
                            }
                        case 15:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "играть на пианино";
                                break;
                            }
                        case 16:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять тапки";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                isMoving = false;
            }
        }
    }
}
