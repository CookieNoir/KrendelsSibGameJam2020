using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TalkingCat : MonoBehaviour
{
    public Text textObject;
    public Color mainColor;
    public float timeVisible = 2f;
    public float duration = 1f;

    public string firstPhrace;

    private Color turnOffColor;
    private float durationInversed;

    private IEnumerator refreshVisibility;
    private IEnumerator colorChanger;

    private IEnumerator ColorChanger()
    {
        for (float f = 0; f < duration; f += Time.deltaTime)
        {
            textObject.color = Vector4.Lerp(Color.white, Color.clear, f * durationInversed * (2 - f * durationInversed));
            yield return null;
        }
        textObject.color = turnOffColor;
        textObject.gameObject.SetActive(false);
    }

    private void Start()
    {
        refreshVisibility = RefreshVisibility();
        colorChanger = ColorChanger();
        durationInversed = 1f / duration;
        turnOffColor = mainColor;
        turnOffColor.a = 0;

        Say(firstPhrace);
    }

    public void Say(string textString)
    {
        textObject.gameObject.SetActive(true);
        textObject.text = textString;
        StopCoroutine(refreshVisibility);
        refreshVisibility = RefreshVisibility();
        StartCoroutine(refreshVisibility);
    }

    private IEnumerator RefreshVisibility()
    {
        StopCoroutine(colorChanger);
        textObject.color = mainColor;
        yield return new WaitForSecondsRealtime(timeVisible);
        colorChanger = ColorChanger();
        StartCoroutine(colorChanger);
    }
}
