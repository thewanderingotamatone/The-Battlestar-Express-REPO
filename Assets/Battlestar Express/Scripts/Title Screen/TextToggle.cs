using UnityEngine;
using TMPro; // remove if not using TextMeshPro

public class TextToggle : MonoBehaviour
{
    [Header("Group A (Default Visible)")]
    public GameObject[] groupA;

    [Header("Group B (Default Hidden)")]
    public GameObject[] groupB;

    private bool showingGroupA = true;

    void Start()
    {
        // Ensure correct starting state
        SetGroupState(groupA, true);
        SetGroupState(groupB, false);
    }

    public void ToggleText()
    {
        showingGroupA = !showingGroupA;

        SetGroupState(groupA, showingGroupA);
        SetGroupState(groupB, !showingGroupA);
    }

    // Helper function to enable/disable groups
    void SetGroupState(GameObject[] group, bool state)
    {
        foreach (GameObject obj in group)
        {
            obj.SetActive(state);
        }
    }
}