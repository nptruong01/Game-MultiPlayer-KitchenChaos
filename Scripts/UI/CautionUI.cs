using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CautionUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI cautionText;
    [SerializeField] private Image cautionscreen;

    private void Start() {
        StartCoroutine(BlinkText());
        StartCoroutine(ScaleImage());
    }

    private IEnumerator BlinkText() {
        while (true) {
            cautionText.color = Color.red;
            yield return new WaitForSeconds(0.2f); // Adjust the WaitForSeconds value for faster blinking
            cautionText.color = Color.black;
            yield return new WaitForSeconds(0.2f); // Adjust the WaitForSeconds value for faster blinking
        }
    }

    private IEnumerator ScaleImage() {
        while (true) {
            float scaleAmount = Mathf.PingPong(Time.time * 2.0f, 1.0f) + 1.0f; // Adjust the multiplier for faster scaling
            cautionscreen.transform.localScale = new Vector3(scaleAmount, scaleAmount, 1f);
            yield return null;
        }
    }
}
