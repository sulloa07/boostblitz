using System.Collections;
using UnityEngine;
using TMPro;

public class TextBouncer : MonoBehaviour
{
    public TextMeshProUGUI textToBounce;

    void Start()
    {
        // Check if the TextMeshProUGUI component is assigned
        if (textToBounce != null)
        {
            // Start the BounceText coroutine
            StartCoroutine(BounceText(textToBounce));
        }
    }

    // Coroutine to create a bouncing text animation effect
    IEnumerator BounceText(TextMeshProUGUI text)
    {
        Vector3 initialPosition = text.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, 20f, 0);

        while (true)
        {
            // Move text up
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                text.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
                yield return null;
            }

            // Move text back down
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                text.transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, t);
                yield return null;
            }
        }
    }

}// I initially did this in rocketcontroller and wanted something to apply everywhere
// but then I got lazy and didnt replace it in the rocketcontroller script, sorry!