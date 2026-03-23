using UnityEngine;
using UnityEngine.UI;

public class PopularityMeter : MonoBehaviour
{

    public Slider slider;

    void Update()
    {
        slider.value = GameManager.Instance.popularity;
    }

    public void SetMaxPopularity(int popularity)
    {
        // takes "popularity" from GameManager.cs
        slider.maxValue = popularity;
        slider.value = popularity;
    }

    public void SetPopularity(int popularity)
    {
        slider.value = popularity;
    }
}
