using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class SwapPrefab : MonoBehaviour
{
    public GameObject proxyPrefab;
    public GameObject realPrefab;

    private GameObject currentInstance;
    private bool usingProxy = true;

    void OnEnable()
    {
        SwapToProxy();
    }

    public void Swap()
    {
        if (usingProxy)
        {
            SwapToReal();
        }
        else
        {
            SwapToProxy();
        }
    }

    private void SwapToProxy()
    {
        EnsureSingleChild();

        if (proxyPrefab != null)
        {
            #if UNITY_EDITOR
            currentInstance = (GameObject)PrefabUtility.InstantiatePrefab(proxyPrefab, transform);
            #else
            currentInstance = Instantiate(proxyPrefab, transform);
            #endif
            usingProxy = true;
        }
    }

    private void SwapToReal()
    {
        EnsureSingleChild();

        if (realPrefab != null)
        {
            #if UNITY_EDITOR
            currentInstance = (GameObject)PrefabUtility.InstantiatePrefab(realPrefab, transform);
            #else
            currentInstance = Instantiate(realPrefab, transform);
            #endif
            usingProxy = false;
        }
    }

    private void EnsureSingleChild()
    {
        // Destroy all existing children
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        currentInstance = null;  // Reset currentInstance as it will be set in SwapToProxy or SwapToReal
    }

    void OnDisable()
    {
        if (currentInstance != null)
        {
            DestroyImmediate(currentInstance);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SwapPrefab))]
public class SwapPrefabEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SwapPrefab script = (SwapPrefab)target;

        if (GUILayout.Button("Swap Prefab"))
        {
            script.Swap();
        }
    }
}
#endif
