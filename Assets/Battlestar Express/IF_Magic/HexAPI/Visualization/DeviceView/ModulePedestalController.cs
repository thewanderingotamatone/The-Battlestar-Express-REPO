using UnityEngine;
using Magic;

[ExecuteAlways]
public class ModulePedestalController : MonoBehaviour
{
    public ModuleType module;
    private PedestalNamer pedestalNamer;

    void Start()
    {
        PedestalRename();
    }

    public void PedestalRename()
    {
        if (pedestalNamer == null)
        {
            // Find the PedestalNamer component in the children
            pedestalNamer = GetComponentInChildren<PedestalNamer>();
        }

        if (pedestalNamer != null)
        {
            // Set the module name in the PedestalNamer
            pedestalNamer.SetModuleName(module.ToString());

            // Change the name of the GameObject
            gameObject.name = $"{module.ToString()} [Foundation]";
        }
        else
        {
            Debug.LogWarning("PedestalNamer component not found in children.");
        }
    }

    void OnValidate()
    {
        PedestalRename();
    }
}
