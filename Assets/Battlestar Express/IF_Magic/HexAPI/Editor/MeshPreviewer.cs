using UnityEngine;

[ExecuteInEditMode]
public class MeshPreviewer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    // private void OnDrawGizmos()
    // {
    //     if (mesh != null && material != null)
    //     {
    //         Gizmos.color = Color.gray;
    //         Gizmos.DrawMesh(mesh, transform.position, transform.rotation, transform.localScale);
    //     }
    // }
}
