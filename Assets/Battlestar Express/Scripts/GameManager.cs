using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton (global access)
    public static GameManager Instance;

    [Header("Popularity Settings")]
    public float popularity = 120f;
    public float decayRateMultiplier = 1f;
    [SerializeField] private int currentStarLevel; //  visible in Inspector (DO NOT EDIT)
    [Header("Game State Outcomes")]


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentStarLevel = GetStarLevel();
        Debug.Log("Starting Star Level: " + currentStarLevel);
    }

    void Update()
    {
        popularity -= decayRateMultiplier * Time.deltaTime;
        popularity = Mathf.Clamp(popularity, 0f, 120f);

        int newStarLevel = GetStarLevel();

        if (newStarLevel != currentStarLevel)
        {
            currentStarLevel = newStarLevel;
            Debug.Log("Star Level Changed: " + currentStarLevel);
        }
    }

    public int GetStarLevel()
    {
        int stars = Mathf.FloorToInt(popularity / 20f);
        return Mathf.Clamp(stars, 0, 6);
    }
}