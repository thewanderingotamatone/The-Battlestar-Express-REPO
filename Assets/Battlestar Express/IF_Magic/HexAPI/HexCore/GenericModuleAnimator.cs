using UnityEngine;
using Magic;
using Magic.Modules;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

public class GenericModuleAnimator : MonoBehaviour
{
    public ModuleTypeSVR module;
    private PlayableDirector anim;
    private Module moduleData;
    public float progress = 0f;
    public bool reverse = false;

    void Start()
    {
        anim = this.GetComponent<PlayableDirector>();

        
        // Dynamically determine the module type and find the corresponding module
        Type moduleClassType = Type.GetType($"Magic.Modules.{module}Module");


        if (moduleClassType != null)
        {
            moduleData = (Module)FindObjectOfType(moduleClassType, true); //gets that dynamic type for the module
        }
        else
        {
            Debug.LogWarning("Module type not found: " + module);
        }

    }

    void SetAnimProgress()
    {
        progress = moduleData.svr;
            
        progress = reverse ? 1f - progress : progress;

        anim.time = progress;
    }

    private void Update() 
    {
        SetAnimProgress();
    }
}