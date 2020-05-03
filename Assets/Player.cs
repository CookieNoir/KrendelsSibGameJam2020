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
    public TalkingCat talkingCat;
    public Text questText;
    public Text questHeader;

    public UiMovement oldTask;
    public Text oldQuestText;
    public Text oldQuestHeader;

    public AudioSource chopin;
    public AudioSource krendelSound;
    public AudioSource mainSound;
    public AudioClip[] krendelClips;
    public Light lampLight;
    public Color someColor;
    public Texture cookie;
    public Renderer flower1;
    public Renderer flower2;
    public Color flowerGreen;
    public Color flowerYellow;
    public Text scoreText;
    public Text starvingText;

    private int eventId;
    private bool isUsed = false;
    private int[] itemsId;

    private float timeSpent = 0;
    private float millisecondsSpent = 0;
    private int secondsSpent = 0;
    private int minutesSpent = 0;

    private float timeLeft = 30f;

    private int[,] movementMatrix =
            {//------     0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12
            /*-0*/      { 1, 3, 4, 1, 1, 1, 1, 0, 5, 1, 1, 1, 1},
            /*-1*/      { 1, 3, 0, 6, 0, 2, 0, 7, 0, 5, 1, 1, 5},
            /*-2*/      { 3, 0, 1, 0, 1, 1, 1, 1, 7, 0, 0, 0, 0},
            /*-3*/      {17, 0, 0, 8, 1, 1, 1, 1, 7, 0, 0, 9, 9},
            /*-4*/      { 1,17, 8, 1, 1,10, 1, 1, 0, 0, 0, 1, 1},
            /*-5*/      { 1, 1, 0, 0, 1, 8, 0, 0, 5, 1, 1, 1, 1},
            /*-6*/      { 4, 0, 0,11, 1, 1, 1, 0, 0, 5,18, 1, 1},
            /*-7*/      { 1, 4, 0,12, 0,19, 1, 0, 0, 0, 0, 1, 1},
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
    private float pianoTimeSpent = 0;

    private bool hasMiska = false;
    private bool hasKuvshin = false;
    private bool hasRoulette = false;

    private int[] tumba;
    private int tumbaIndex;
    private int[] books;

    private int questId = 0;
    private int questProgress = 0;
    private int[] questItems;
    private int questAmount = 10;

    private bool objective1 = false;
    private bool objective2 = false;

    private int score = 0;

    private IEnumerator questEnumerator;
    private IEnumerator flowerColor;

    private IEnumerator ReturnNormalSound()
    {
        mainSound.Stop();
        yield return new WaitForSecondsRealtime(71f);
        mainSound.Play();
    }

    private void FlowersState(bool state)
    {
        StopCoroutine(flowerColor);
        if (state)
        {
            flowerColor = FlowerColor(flowerGreen);
        }
        else
        {
            flowerColor = FlowerColor(flowerYellow);
        }
        StartCoroutine(flowerColor);
    }

    private void UpdateScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    private IEnumerator FlowerColor(Color targetColor)
    {
        Color baseColor1 = flower1.material.color;
        Color baseColor2 = flower2.material.color;
        for (float f = 0; f < 1; f += Time.deltaTime)
        {
            flower1.material.color = Vector4.Lerp(baseColor1, targetColor, f * (2 - f));
            flower2.material.color = Vector4.Lerp(baseColor2, targetColor, f * (2 - f));
            yield return null;
        }
        flower1.material.color = targetColor;
        flower2.material.color = targetColor;
    }

    private void ChangeQuest(int amount)
    {
        UpdateScore(amount);
        oldTask.TranslateInstantly();
        oldQuestText.text = questText.text;
        oldQuestHeader.text = questHeader.text;

        StopCoroutine(questEnumerator);
        questEnumerator = GiveNewQuestYield();
        StartCoroutine(questEnumerator);
    }

    private IEnumerator GiveNewQuestYield()
    {
        yield return new WaitForSeconds(3f);
        giveNewQuest();
        oldTask.Translate();
    }

    private void giveNewQuest()
    {
        questProgress = 0;
        int prevQuestId = questId;
        questId = Random.Range(1, questAmount);
        if (questId > questAmount - 3 && itemCount == 4)
        {
            questId = Random.Range(1, questAmount - 2);
            if (prevQuestId == questId) questId = (questId + 1) % (questAmount - 2);
        }
        else
        {
            if (prevQuestId == questId) questId = (questId + 1) % questAmount;

        }
        if (questId == 0) questId = 1;
        switch (questId)
        {
            case 1:
                {
                    questHeader.text = "Создание сосискомёта";
                    questText.text = "○ Принести \"Механику аэрозолей\"";
                    talkingCat.Say("Не хватает огневой мощи против повстанцев, нужно оружие");
                    break;
                }
            case 2:
                {
                    questHeader.text = "Что-то интересное";
                    questText.text = "○ Найти кейс";
                    talkingCat.Say("В одном из моих кейсов есть интересные бумаги. Принеси их");
                    break;
                }
            case 3:
                {
                    questHeader.text = "Книжный кот";
                    questText.text = "○ Принести книги:" + '\n' +
                        "\"Теория функций комплексных переменных\"" + '\n' +
                        "\"Ядерные энергетические реакции\"" + '\n' +
                        "\"Тайна сибирского леса\"";
                    addItemToQuestInventory(18, 19, 20);
                    talkingCat.Say("Человек, мне нужно больше литературы");
                    break;
                }
            case 4:
                {
                    questHeader.text = "Подготовка к сборке";
                    questText.text = "○ Принести рулетку и гаечный ключ";
                    addItemToQuestInventory(13, 12);
                    tumba[Random.Range(0, 4)] = 1;
                    talkingCat.Say("Человек, срочно принеси мне инструменты");
                    break;
                }
            case 5:
                {
                    questHeader.text = "Сорванная башня";
                    questText.text = "○ Узнать координаты столицы Парижа";
                    talkingCat.Say("Узнай, где находится Париж. Думаю, скоро он станет неПарижем...");
                    objective1 = false;
                    break;
                }
            case 6:
                {
                    questHeader.text = "Истинная ненависть";
                    questText.text = "○ Принести 3 кренделя";
                    addItemToQuestInventory(7, 7, 7);
                    talkingCat.Say("Проверь несколько кренделей. Они не внушают доверия");
                    break;
                }
            case 7:
                {
                    questHeader.text = "Большая трапеза";
                    questText.text = "○ Принести жареные сосиски и жареную рыбу";
                    addItemToQuestInventory(10, 11);
                    talkingCat.Say("Раб, я голоден. Принеси притовленную еду");
                    break;
                }
            case 8:
                {
                    questHeader.text = "Напоить кота";
                    questText.text = "○ Принести воды коту";
                    addItemToInventory(3, "Пустая миска", null);
                    talkingCat.Say("Я хочу пить, принеси воды");
                    hasMiska = true;
                    break;
                }
            case 9:
                {
                    questHeader.text = "Полить цветы";
                    questText.text = "○ Полить каждый цветок в доме";
                    addItemToInventory(5, "Пустой кувшин", null);
                    talkingCat.Say("Цветы сегодня желтоваты, сосредоточься на этом");
                    hasKuvshin = true;
                    objective1 = false;
                    objective2 = false;
                    FlowersState(false);
                    break;
                }
        }
    }

    private void checkQuestProgress()
    {
        switch (questId)
        {
            case 1:
                {
                    if (questProgress == 0)
                    {
                        if (takeItemFromInventory(14))
                        {
                            questProgress++;
                            questText.text = "● Принести \"Механику аэрозолей\"" + '\n' + "○ Достать гаечный ключ и металлолом";
                            books[0] = 1;
                            timeLeft += 10f;
                            talkingCat.Say("Пикатная книженция. Неси инструменты");
                            addItemToQuestInventory(12, 8);
                        }
                    }
                    else if (questProgress == 1)
                    {
                        if (takeItemFromInventory(12))
                        {
                            takeItemFromQuestInventory(12);
                        }
                        if (takeItemFromInventory(8))
                        {
                            takeItemFromQuestInventory(8);
                        }
                        if (questItems[0] == 0)
                        {
                            questProgress++;
                            questText.text = "● Принести \"Механику аэрозолей\"" + '\n' + "● Достать гаечный ключ и металлолом" + '\n' + "○ Зарядить пушку сосисками";
                            timeLeft += 15f;
                            talkingCat.Say("Нет патронов...");
                        }
                    }
                    else
                    {
                        if (takeItemFromInventory(1) || takeItemFromInventory(10))
                        {
                            questText.text = "● Принести \"Механику аэрозолей\"" + '\n' + "● Достать гаечный ключ и металлолом" + '\n' + "● Зарядить пушку сосисками";
                            timeLeft += 8f;
                            talkingCat.Say("Теперь у оппозиции нет шансов");
                            ChangeQuest(350);
                        }
                    }
                    break;
                }
            case 2:
                {
                    if (questProgress == 0)
                    {
                        if (takeItemFromInventory(24))
                        {
                            questProgress++;
                            objective1 = false;
                            questText.text = "● Найти кейс" + '\n' + "○ Оплатить счета";
                            timeLeft += 8f;
                            talkingCat.Say("Чёрт! Это были счета. Оплати их");
                        }
                    }
                    else if (questProgress == 1)
                    {
                        if (objective1)
                        {
                            questProgress++;
                            questText.text = "● Найти кейс" + '\n' + "● Оплатить счета" + '\n' + "○ Принести крендель";
                            timeLeft += 12f;
                            talkingCat.Say("Как же я зол. Проверь крендели");
                        }
                    }
                    else
                    {
                        if (takeItemFromInventory(7))
                        {
                            questText.text = "● Найти кейс" + '\n' + "● Оплатить счета" + '\n' + "● Принести крендель";
                            timeLeft += 8f;
                            talkingCat.Say("Проверил? Рад, что они все еще мертвы");
                            ChangeQuest(150);
                        }
                    }
                    break;
                }
            case 3:
                {
                    if (takeItemFromInventory(18))
                    {
                        takeItemFromQuestInventory(18);
                        books[1] = 1;
                    }
                    if (takeItemFromInventory(19))
                    {
                        takeItemFromQuestInventory(19);
                        books[2] = 1;
                    }
                    if (takeItemFromInventory(20))
                    {
                        takeItemFromQuestInventory(20);
                        books[3] = 1;
                    }
                    if (questItems[0] == 0)
                    {
                        questText.text = "● Принести книги:" + '\n' +
                            "\"Теория функций комплексных переменных\"" + '\n' +
                            "\"Ядерные энергетические реакции\"" + '\n' +
                            "\"Тайна сибирского леса\"";
                        timeLeft += 12f;
                        talkingCat.Say("Ты хороший раб, человек");
                        ChangeQuest(150);
                    }
                    break;
                }
            case 4:
                {
                    if (questProgress == 0)
                    {
                        if (takeItemFromInventory(12))
                        {
                            takeItemFromQuestInventory(12);
                        }
                        if (takeItemFromInventory(13))
                        {
                            takeItemFromQuestInventory(13);
                            hasRoulette = false;
                        }
                        if (questItems[0] == 0)
                        {
                            questProgress++;
                            questText.text = "● Принести рулетку и гаечный ключ" + '\n' + "○ Принести \"Механику аэрозолей\"";
                            timeLeft += 18f;
                            talkingCat.Say("Ничего не понимаю... Сгоняй за \"Механикой аэрозолей\"");
                        }
                    }
                    else
                    {
                        if (takeItemFromInventory(14))
                        {
                            books[0] = 1;
                            questText.text = "● Принести рулетку и гаечный ключ" + '\n' + "● Принести \"Механику аэрозолей\"";
                            timeLeft += 10f;
                            talkingCat.Say("Превосходно, человек, служи мне и дальше");
                            ChangeQuest(450);
                        }
                    }
                    break;
                }
            case 5:
                {
                    if (questProgress == 0)
                    {
                        if (objective1)
                        {
                            questProgress++;
                            questText.text = "● Узнать координаты столицы Парижа" + '\n' + "○ Принести тапок";
                            timeLeft += 8f;
                            talkingCat.Say("Отлично. Очень хочу его прихлопнуть");
                        }
                    }
                    else if (questProgress == 1)
                    {
                        if (takeItemFromInventory(9))
                        {
                            questProgress++;
                            questText.text = "● Узнать координаты столицы Парижа" + '\n' + "● Принести тапок" + '\n' + "○ Найти и принести металлолом";
                            timeLeft += 8f;
                            talkingCat.Say("Да не так! Неси металлолом");
                        }
                    }
                    else
                    {
                        if (takeItemFromInventory(8))
                        {
                            questText.text = "● Узнать координаты столицы Парижа" + '\n' + "● Принести тапок" + '\n' + "● Найти и принести металлолом";
                            timeLeft += 8f;
                            talkingCat.Say("Ты принял верное решение служить мне, человек");
                            ChangeQuest(150);
                        }
                    }
                    break;
                }
            case 6:
                {
                    if (takeItemFromInventory(7))
                    {
                        takeItemFromQuestInventory(7);
                    }
                    if (takeItemFromInventory(7))
                    {
                        takeItemFromQuestInventory(7);
                    }
                    if (takeItemFromInventory(7))
                    {
                        takeItemFromQuestInventory(7);
                    }
                    if (questItems[0] == 0)
                    {
                        questText.text = "● Принести 3 кренделя";
                        timeLeft += 12f;
                        talkingCat.Say("Хмм... Вроде ничего подозрительного");
                        ChangeQuest(150);
                    }
                    break;
                }
            case 7:
                {
                    if (takeItemFromInventory(10))
                    {
                        takeItemFromQuestInventory(10);
                    }
                    if (takeItemFromInventory(11))
                    {
                        takeItemFromQuestInventory(11);
                    }
                    if (questItems[0] == 0)
                    {
                        questText.text = "● Принести жареные сосиски и жареную рыбу";
                        timeLeft += 15f;
                        talkingCat.Say("Ты сделал правильный выбор, человек");
                        ChangeQuest(200);
                    }
                    break;
                }
            case 8: // Напоить кота
                {
                    if (takeItemFromInventory(4))
                    {
                        hasMiska = false;
                        questText.text = "● Принести воды коту";
                        timeLeft += 10f;
                        talkingCat.Say("Успех, я пью за себя!");
                        ChangeQuest(50);
                    }
                    else if (takeItemFromInventory(17))
                    {
                        addItemToInventory(3, null, null);
                        timeLeft -= 2f;
                        talkingCat.Say("Что за кошачью мочу ты мне подсунул?");
                    }
                    break;
                }
            case 9: // Полить цветы
                {
                    if (questProgress == 0)
                    {
                        if (objective1 && objective2)
                        {
                            hasKuvshin = false;
                            takeItemFromInventory(5);
                            takeItemFromInventory(6);
                            objective1 = false;
                            questProgress++;
                            questText.text = "● Полить каждый цветок в доме" + '\n' + "○ Играть на пианино, пока не позеленеют цветы";
                            timeLeft += 12f;
                            talkingCat.Say("Поиграй на пианино, пока они не позеленеют");
                        }
                    }
                    else
                    {
                        if (objective1)
                        {
                            timeLeft += 15f;
                            talkingCat.Say("Хоть что-то тебе можно доверить");
                            ChangeQuest(350);
                        }
                    }
                    break;
                }
        }
    }

    private void GameOver()
    {
        Debug.Log("GO");
    }

    private void UpdateTimer()
    {
        timeSpent += Time.deltaTime;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            GameOver();
            timeLeft = 0;
        }
        if (isPlayingPiano)
        {
            pianoTimeSpent += Time.deltaTime;
            if (questId == questAmount - 1 && questProgress == 1 && pianoTimeSpent >= 10 && objective1 == false)
            {
                objective1 = true;
                FlowersState(true);
                questText.text = "● Полить каждый цветок в доме" + '\n' + "● Играть на пианино, пока не позеленеют цветы";
            }
        }
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

        int sec = Mathf.FloorToInt(timeLeft);
        starvingText.text = (sec / 60).ToString() + ':' + (sec % 60).ToString("D2");
    }

    private void Start()
    {
        transform.position = new Vector3(playerPosition.x + 0.5f, 0, playerPosition.y + 0.5f);
        interactionText_Object.SetActive(false);
        itemsId = new int[4];
        tumba = new int[] { 0, 0, 0, 0 };
        books = new int[] { 1, 1, 1, 1 };

        oldTask.TranslateInstantly();
        oldQuestHeader.text = "Поговорить с котом";
        oldQuestText.text = "○ Найти кота в ванной и поговорить с ним.";
        questItems = new int[] { 0, 0, 0 };

        questEnumerator = GiveNewQuestYield();
        flowerColor = FlowerColor(flowerGreen);
    }

    public IEnumerator ColorChanger(Light component, Color baseColor, Color targetColor, float duration)
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

    private void addItemToQuestInventory(int id1, int id2 = 0, int id3 = 0)
    {
        questItems[0] = id1;
        questItems[1] = id2;
        questItems[2] = id3;
    }

    private void takeItemFromQuestInventory(int id)
    {
        if (questItems[0] == id)
        {
            questItems[0] = questItems[1];
            questItems[1] = questItems[2];
            questItems[2] = 0;
        }
        else if (questItems[1] == id)
        {
            questItems[1] = questItems[2];
            questItems[2] = 0;
        }
        else if (questItems[2] == id)
        {
            questItems[3] = 0;
        }
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
        if (hasRoulette) addItemToInventory(13, null, null);

        for (int i = 0; i < 4; ++i) books[i] = 1;
    }

    private void stopPiano()
    {
        if (isPlayingPiano)
        {
            chopin.Stop();
            isPlayingPiano = false;
            pianoTimeSpent = 0f;
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
                    if (questId == questAmount - 1 && questProgress == 0)
                    {
                        if (playerPosition == new Vector2Int(0, 2)) objective1 = true;
                        else objective2 = true;
                        if (objective1 && objective2)
                            questText.text = "● Полить каждый цветок в доме";
                    }
                    break;
                }
            case 5:
                {
                    if (tumba[tumbaIndex] > 0)
                    {
                        addItemToInventory(13, "Рулетка", null);
                        hasRoulette = true;
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
                        case 4:
                            {
                                notSystem.Notify(Resources.Load<Sprite>("Items/" + 22), "Пусто...");
                                break;
                            }
                    }
                    if (bookType != 4) books[bookType] = 0;
                    break;
                }
            case 10:
                {
                    switch (questId)
                    {
                        case 0: // Пришествие
                            {
                                giveNewQuest();
                                oldTask.Translate();
                                break;
                            }
                        default:
                            {
                                checkQuestProgress();
                                break;
                            }
                    }
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
                        lampLight.spotAngle = 120;
                        lampLight.cookie = cookie;
                        StartCoroutine(ReturnNormalSound());
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
                    if (questId == 5 && questProgress == 0)
                    {
                        objective1 = true;
                        questText.text = "● Узнать координаты столицы Парижа";
                    }
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
                        if (itemCount == 0) notSystem.Notify(newIcon, "Вы выбросили все предметы. Что же вы скажете коту?");
                        else notSystem.Notify(newIcon, "Вы не можете выбросить важные предметы");
                    }
                    else
                        notSystem.Notify(newIcon, "Ваши карманы пусты");
                    break;
                }
            case 18:
                {
                    addItemToInventory(24, "Документы");
                    if (questId == 2 && questProgress == 0) questText.text = "● Найти кейс";
                    break;
                }
            case 19:
                {
                    if (questId == 2 && questProgress == 1 && objective1 == false)
                    {
                        objective1 = true;
                        notSystem.Notify(Resources.Load<Sprite>("Items/" + 26), "Счета успешно оплачены");
                        questText.text = "● Найти кейс" + '\n' + "● Оплатить счета";
                    }
                    else
                        notSystem.Notify(Resources.Load<Sprite>("Items/" + 25), "Все счета уже оплачены");
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
                                interactionText_Text.text = "искать рулетку";
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
                                int bookCnt = 0;
                                while (books[bookType] == 0 && bookCnt < 4)
                                {
                                    bookType = (bookType + 1) % 4;
                                    bookCnt++;
                                }
                                if (bookCnt < 4)
                                {
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
                                }
                                else
                                {
                                    bookType = 4;
                                    interactionText_Text.text = "Искать что-либо на полке";
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
                        case 18:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "искать документы";
                                break;
                            }
                        case 19:
                            {
                                interactionText_Object.SetActive(true);
                                interactionText_Text.text = "Оплатить счета";
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
