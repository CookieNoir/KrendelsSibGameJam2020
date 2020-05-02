using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSystem : UiMovement
{
    public Image iconObject;
    public Sprite emptyIcon;
    public Text textObject;

    private IEnumerator refreshVisibility;

    private void Start()
    {
        refreshVisibility = RefreshVisibility();
    }

    public void Notify(Sprite icon, string textString)
    {
        if (icon) iconObject.sprite = icon;
        else iconObject.sprite = emptyIcon;
        textObject.text = textString;
        StopCoroutine(refreshVisibility);
        refreshVisibility = RefreshVisibility();
        StartCoroutine(refreshVisibility);
    }

    private IEnumerator RefreshVisibility()
    {
        StopCoroutine(activeMovement);
        activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, newPosition, movingFunction);
        StartCoroutine(activeMovement);
        yield return new WaitForSecondsRealtime(2f);
        StopCoroutine(activeMovement);
        activeMovement = SmoothMove(GetComponent<RectTransform>().anchoredPosition, defaultPosition, movingFunction);
        StartCoroutine(activeMovement);
    }
}
