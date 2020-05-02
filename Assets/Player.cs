using System.Collections;
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
    public NotificationSystem notSystem;
    public Sprite emptyInventoryIcon;
    public Text timeText;

    public AudioSource chopin;
    public AudioSource krendelSound;
    public AudioClip[] krendelClips;
    public Light lampLight;
    public Color someColor;
    public Texture cookie;

    private int taskId;
    private int eventId;
    private bool isUsed = false;
    private int[] itemsId;

    private float timeSpent = 0;
    private float millisecondsSpent = 0;
    private int secondsSpent = 0;
    private int minutesSpent = 0;

    private int[,] movementMatrix =
            {//------     0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12
            /*-0*/      { 1, 3, 4, 1, 1, 1, 1, 0, 5, 1, 1, 1, 1},
            /*-1*/      { 1, 3, 0, 6, 0, 2, 0, 7, 0, 5, 1, 1, 5},
            /*-2*/      { 3, 0, 1, 0, 1, 1, 1, 1, 7, 0, 0, 0, 0},
            /*-3*/      {17, 0, 0, 8, 1, 1, 1, 1, 7, 0, 0, 9, 9},
            /*-4*/      { 1,17, 8, 1, 1,10, 1, 1, 0, 0, 0, 1, 1},
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
    private bool isPlayingPiano = false;
    private int tvBroken = 0;
    private int bookType = 0;
    private int krendelCount = 0;
    private int itemCount = 0;
    private bool isWaterClear = false;
    private bool hasMiska = false;
    private bool hasKuvshin = false;
    private bool hasSecretItem = false;
    private int[] tumba;
    private int tumbaIndex;
    private int tumbaItem;

    private void UpdateTimer()
    {
        timeSpent += Time.deltaTime;
        millisecondsSpent += Time.deltaTime;
        if (millisecondsSpent >= 1)
        {
            secondsSpent++;
            millisecondsSpent--;
        }
        if (secondsSpent >= 60)
        {
            minutesSpent++;
            secondsSpent -= 60;
        }
        timeText.text = minutesSpent.ToString("D2") + ':' + secondsSpent.ToString("D2");
    }

    private void Start()
    {
        transform.position = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
        interactionText_Object.SetActive(false);
        itemsId = new int[4];
        tumba = new int[] { 0, 0, 0, 0 };
        tumbaItem = 13;
    }

    public static IEnumerator ColorChanger(Light component, Color baseColor, Color targetColor, float duration)
    {
        float durationInversed = 1f / duration;
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            component.color = Vector4.Lerp(baseColor, targetColor, f * durationInversed * (2 - f * durationInversed));
            yield return null;
        }
        component.color = targetColor;
    }

    private bool addItemToInventory(int id, string text, string notification = null)
    {
        Sprite newIcon = Resources.Load<Sprite>("Items/" + id);
        if (itemCount < 4)
        {
            itemsId[itemCount] = id;
            inventorySlots[itemCount].sprite = newIcon;
            if (text != null)
            {
                if (notification == null) notSystem.Notify(inventorySlots[itemCount].sprite, text + " теперь в вашем инвентаре");
                else notSystem.Notify(inventorySlots[itemCount].sprite, notification);
            }
            itemCount++;
            return true;
        }
        notSystem.Notify(newIcon, "Не могу положить предмет в инвентарь");
        return false;
    }

    private bool takeItemFromInventory(int id)
    {
        if (itemsId[0] == id)
        {
            itemsId[0] = itemsId[1];
            inventorySlots[0].sprite = inventorySlots[1].sprite;
            itemsId[1] = itemsId[2];
            inventorySlots[1].sprite = inventorySlots[2].sprite;
            itemsId[2] = itemsId[3];
            inventorySlots[2].sprite = inventorySlots[3].sprite;
            itemsId[3] = 0;
            inventorySlots[3].sprite = emptyInventoryIcon;
            itemCount--;
            return true;
        }
        else if (itemsId[1] == id)
        {
            itemsId[1] = itemsId[2];
            inventorySlots[1].sprite = inventorySlots[2].sprite;
            itemsId[2] = itemsId[3];
            inventorySlots[2].sprite = inventorySlots[3].sprite;
            itemsId[3] = 0;
            inventorySlots[3].sprite = emptyInventoryIcon;
            itemCount--;
            return true;
        }
        else if (itemsId[2] == id)
        {
            itemsId[2] = itemsId[3];
            inventorySlots[2].sprite = inventorySlots[3].sprite;
            itemsId[3] = 0;
            inventorySlots[3].sprite = emptyInventoryIcon;
            itemCount--;
            return true;
        }
        else if (itemsId[3] == id)
        {
            itemsId[3] = 0;
            inventorySlots[3].sprite = emptyInventoryIcon;
            itemCount--;
            return true;
        }
        return false;
    }

    private void clearInventory()
    {
        itemsId[0] = 0;
        inventorySlots[0].sprite = emptyInventoryIcon;
        itemsId[1] = 0;
        inventorySlots[1].sprite = emptyInventoryIcon;
        itemsId[2] = 0;
        inventorySlots[2].sprite = emptyInventoryIcon;
        itemsId[3] = 0;
        inventorySlots[3].sprite = emptyInventoryIcon;
        itemCount = 0;

        if (hasMiska) addItemToInventory(3, null, null);
        if (hasKuvshin) addItemToInventory(5, null, null);
        if (hasSecretItem) addItemToInventory(tumbaItem, null, null);
    }

    private void stopPiano()
    {
        if (isPlayingPiano)
        {
            chopin.Stop();
            isPlayingPiano = false;
        }
    }

    private void DoAction()
    {
        switch (eventId)
        {
            case 2:
                {
                    addItemToInventory(12, "Гаечный ключ");
                    break;
                }
            case 3:
                {
                    int cnt = 0;
                    if (takeItemFromInventory(1))
                    {
                        addItemToInventory(10, "Жареные сосиски");
                        cnt++;
                    }
                    if (takeItemFromInventory(2))
                    {
                        addItemToInventory(11, "Жареная рыба");
                        cnt++;
                    }
                    if (cnt == 0) notSystem.Notify(Resources.Load<Sprite>("Items/" + 15), "Нужна еда, чтобы ее приготовить");
                    break;
                }
            case 4:
                {
                    if (takeItemFromInventory(4))
                    {
                        addItemToInventory(3, "Пустая миска", "Цветочек доволен :)");
                    }
                    else if (takeItemFromInventory(6))
                    {
                        addItemToInventory(5, "Пустой кувшин", "Цветочек доволен :)");
                    }
                    else if (takeItemFromInventory(17))
                    {
                        addItemToInventory(3, "Пустая миска", "Цветочек доволен :)");
                    }
                    else notSystem.Notify(Resources.Load<Sprite>("Items/" + 21), "Чтобы полить цветок, нужна вода");
                    break;
                }
            case 5:
                {
                    if (tumba[tumbaIndex] > 0)
                    {
                        addItemToInventory(tumbaItem, "Спрятанный предмет", null);
                        hasSecretItem = true;
                        tumba[tumbaIndex]--;
                    }
                    else notSystem.Notify(Resources.Load<Sprite>("Items/" + 22), "Пусто...");
                    break;
                }
            case 6:
                {
                    addItemToInventory(1, "Сосиски");
                    break;
                }
            case 7:
                {
                    addItemToInventory(2, "Сырая рыба");
                    break;
                }
            case 8:
                {
                    if (takeItemFromInventory(3))
                    {
                        if (isWaterClear) addItemToInventory(4, "Миска с водой");
                        else addItemToInventory(17, "Миска с водой");
                    }
                    else if (takeItemFromInventory(5))
                    {
                        addItemToInventory(6, "Кувшин с водой");
                    }
                    else notSystem.Notify(Resources.Load<Sprite>("Items/" + 5), "Нужен сосуд для воды");
                    break;
                }
            case 9:
                {
                    switch (bookType)
                    {
                        case 0:
                            {
                                addItemToInventory(14, "\"Механика аэрозолей\"");
                                break;
                            }
                        case 1:
                            {
                                addItemToInventory(18, "\"Теория функций комплексных переменных\"");
                                break;
                            }
                        case 2:
                            {
                                addItemToInventory(19, "\"Ядерные энергетические реакции\"");
                                break;
                            }
                        case 3:
                            {
                                addItemToInventory(20, "\"Тайна сибирского леса\"");
                                break;
                            }
                    }
                    break;
                }
            case 10:
                {
                    break;
                }
            case 11:
                {
                    lampLight.enabled = !lampLight.enabled;
                    break;
                }
            case 12:
                {
                    if (krendelCount == 0 || krendelCount > 5) addItemToInventory(7, "Крендель", "Спасибо, что играете в нашу игру :)");
                    else if (krendelCount == 1) addItemToInventory(7, "Крендель", "Правда, большое спасибо :)");
                    else if (krendelCount == 2)
                    {
                        addItemToInventory(7, "Крендель", "TRIPLE KILL!");
                        krendelSound.clip = krendelClips[0];
                        krendelSound.Play();
                    }
                    else if (krendelCount == 3)
                    {
                        addItemToInventory(7, "Крендель", "ULTRA KILL!!!");
                        krendelSound.clip = krendelClips[1];
                        krendelSound.Play();
                    }
                    else if (krendelCount == 4)
                    {
                        addItemToInventory(7, "Крендель", "RAAAMPAAAGEEE!!!!!!!! (1 more)");
                        krendelSound.clip = krendelClips[2];
                        krendelSound.Play();
                    }
                    else if (krendelCount == 5)
                    {
                        addItemToInventory(7, "Крендель", "-PLAY DOOM - AT DOOMS GATE");
                        krendelSound.clip = krendelClips[3];
                        krendelSound.volume = 1f;
                        krendelSound.Play();
                        if (!lampLight.enabled) lampLight.enabled = true;
                        StartCoroutine(ColorChanger(lampLight, Vector4.zero, someColor, 5f));
                        lampLight.intensity = 3;
                        lampLight.spotAngle = 160;
                        lampLight.cookie = cookie;
                    }
                    krendelCount++;
                    break;
                }
            case 13:
                {
                    addItemToInventory(8, "Металлолом");
                    tvBroken++;
                    break;
                }
            case 14:
                {
                    notSystem.Notify(Resources.Load<Sprite>("Items/" + 23), "48°50′ широты и 2°20′ долготы");
                    break;
                }
            case 15:
                {
                    chopin.Play();
                    isPlayingPiano = true;
                    break;
                }
            case 16:
                {
                    addItemToInventory(9, "Тапки");
                    break;
                }
            case 17:
                {
                    Sprite newIcon = Resources.Load<Sprite>("Items/" + 16);
                    if (itemsId[0] > 0)
                    {
                        clearInventory();
                        if (itemCount==0) notSystem.Notify(newIcon, "Вы выбросили все предметы. Что же вы скажете коту?");
                        else notSystem.Notify(newIcon, "Вы не можете выбросить важные предметы");
                    }
                    else
                        notSystem.Notify(newIcon, "Ваши карманы пусты");
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
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                    stopPiano();

                    destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                    direction = -Vector3.right;
                    isMoving = true;
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
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                    stopPiano();

                    destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                    direction = Vector3.right;
                    isMoving = true;
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
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                    stopPiano();

                    destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                    direction = -Vector3.forward;
                    isMoving = true;
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
                if (playerPosition != playerPositionPrev)
                {
                    isUsed = false; interactionText_Object.SetActive(false);
                    stopPiano();

                    destination = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
                    direction = Vector3.forward;
                    isMoving = true;
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
                                if (playerPosition == new Vector2(0, 8) || playerPosition == new Vector2(1, 9))
                                    tumbaIndex = 0;
                                else if (playerPosition == new Vector2(1, 12))
                                    tumbaIndex = 1;
                                else if (playerPosition == new Vector2(5, 8) || playerPosition == new Vector2(6, 9))
                                    tumbaIndex = 2;
                                else tumbaIndex = 3;
                                interactionText_Text.text = "искать предметы";
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
                                if (playerPosition.x == 5) isWaterClear = false;
                                else isWaterClear = true;
                                interactionText_Text.text = "набрать воды";
                                break;
                            }
                        case 9:
                            {
                                interactionText_Object.SetActive(true);
                                bookType = Random.Range(0, 4);
                                switch (bookType)
                                {
                                    case 0:
                                        {
                                            interactionText_Text.text = "взять книгу \"Механика аэрозолей\"";
                                            break;
                                        }
                                    case 1:
                                        {
                                            interactionText_Text.text = "взять книгу \"Теория функций комплексных переменных\"";
                                            break;
                                        }
                                    case 2:
                                        {
                                            interactionText_Text.text = "взять книгу \"Ядерные энергетические реакции\"";
                                            break;
                                        }
                                    case 3:
                                        {
                                            interactionText_Text.text = "взять книгу \"Тайна сибирского леса\"";
                                            break;
                                        }
                                }

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
                                if (!lampLight.enabled) interactionText_Text.text = "включить лампу";
                                else interactionText_Text.text = "выключить лампу";
                                break;
                            }
                        case 12:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "взять крендельки";
                                break;
                            }
                        case 13:
                            {
                                interactionText_Object.SetActive(true);
                                if (tvBroken == 0) interactionText_Text.text = "сломать телевизор";
                                else if (tvBroken < 3) interactionText_Text.text = "взять части телевизора";
                                else interactionText_Text.text = "взять остатки телевизора";
                                break;
                            }
                        case 14:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "найти столицу Парижа (?)";
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
                        case 17:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "выбросить всё";
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
        UpdateTimer();
    }
}
