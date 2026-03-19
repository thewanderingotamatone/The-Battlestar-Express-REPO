using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Popularity Settings")]
    // Starts out with "X" amount of pts (120 = max)
    public float popularity = 120f;
    // Rate of point decay
    public float decayRateMultiplier = 1f;

    void Awake()
    {
        // Ensures only one GameManager exists at a time (future planning) ----------------------------------------
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // POPULARITY METER INFORMATION ---------------------------------------------------------------------------
        popularity -= decayRateMultiplier * Time.deltaTime;
        popularity = Mathf.Clamp(popularity, 0f, 120f);
    }
}