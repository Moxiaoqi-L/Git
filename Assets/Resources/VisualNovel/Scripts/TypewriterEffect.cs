using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public float typingSpeed = Constants.DEFAULT_TYPING_SPEED;

    private Coroutine typingCoroutine;
    private bool isTyping;

    public void StartTyping(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        textDisplay.text = text;
        textDisplay.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++)
        {
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        textDisplay.maxVisibleCharacters = textDisplay.text.Length;
        isTyping = false;
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}
