using UnityEngine;
using UnityEngine.UI;
using TMPro; // remove if not using TextMeshPro

public class TextToggle : MonoBehaviour
{
    // Drag your text objects here in the Inspector
    public GameObject[] textElements;

    private bool isVisible = true;

    public void ToggleText()
    {
        // Flip the state
        isVisible = !isVisible;

        // Apply to all text elements
        foreach (GameObject text in textElements)
        {
            text.SetActive(isVisible);
        }
    }
}