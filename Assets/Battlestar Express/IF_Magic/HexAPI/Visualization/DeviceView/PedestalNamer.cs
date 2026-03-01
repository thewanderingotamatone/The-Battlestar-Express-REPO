using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class PedestalNamer : MonoBehaviour
{
    private string moduleName;

    public Material customMaterial;

    void Start()
    {
        // Initial renaming can be done here if needed
        ApplyModuleName();
    }

    private void ApplyModuleName()
    {
        // Update text fields in children with tag "ModuleName"
        UpdateTextFields();

        // Update material for objects with tag "Icon"
        UpdateIcons();
    }

    public void SetModuleName(string name)
    {
        moduleName = name;
        ApplyModuleName();
    }

    private void UpdateTextFields()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == null) continue; // Ensure the child is still valid

            if (child.CompareTag("ModuleName"))
            {
                Text textComponent = child.GetComponent<Text>();
                if (textComponent != null)
                {
                    textComponent.text = moduleName;
                }

                TextMeshProUGUI textMeshProUGUIComponent = child.GetComponent<TextMeshProUGUI>();
                if (textMeshProUGUIComponent != null)
                {
                    textMeshProUGUIComponent.text = moduleName;
                }

                TextMeshPro textMeshProComponent = child.GetComponent<TextMeshPro>();
                if (textMeshProComponent != null)
                {
                    textMeshProComponent.text = moduleName;
                }
            }
        }
    }

    private void UpdateIcons()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == null) continue; // Ensure the child is still valid

            if (child.CompareTag("Icon"))
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material originalMaterial = renderer.sharedMaterial;
                    if (originalMaterial != null)
                    {
                        Material instanceMaterial = new Material(originalMaterial);
                        string iconPath = $"Assets/Magic/Modules/Materials/Icons/{moduleName}.png";
                        Texture2D iconTexture = LoadTexture(iconPath);

                        if (iconTexture != null)
                        {
                            // Try setting the texture using different property names
                            if (instanceMaterial.HasProperty("_MainTex"))
                            {
                                instanceMaterial.SetTexture("_MainTex", iconTexture);
                            }
                            else if (instanceMaterial.HasProperty("_BaseMap"))
                            {
                                instanceMaterial.SetTexture("_BaseMap", iconTexture);
                            }
                            else
                            {
                                Debug.LogWarning($"Material on {child.name} does not have a recognized texture property.");
                            }
                            
                            renderer.material = instanceMaterial;
                            //Debug.Log($"Updated texture for {child.name} using {iconPath}");
                        }
                        else
                        {
                            Debug.LogWarning($"Icon texture not found at path: {iconPath}");
                        }
                    }
                    else{
                        if(customMaterial != null)
                        {
                            renderer.material = new Material(customMaterial);
                        }
                    }                
                }
            }
        }
    }

    private Texture2D LoadTexture(string path)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }
        return null;
    }
}
