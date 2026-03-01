using System.Collections;
using UnityEngine;

public class DeleteChildren : MonoBehaviour
{
    // Delay between deleting each child in seconds
    public float deleteSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to delete children
        StartCoroutine(DeleteChildrenCoroutine());
    }

    // Coroutine to delete children with delay
    IEnumerator DeleteChildrenCoroutine()
    {
        yield return new WaitForSeconds(10f);

        // Loop through all the children of the GameObject
        while (transform.childCount > 0)
        {
            // Get the first child
            Transform child = transform.GetChild(0);

            // Destroy the child GameObject
            Destroy(child.gameObject);

            // Wait for the specified delay before continuing
            yield return new WaitForSeconds(deleteSpeed);
        }
    }
}
