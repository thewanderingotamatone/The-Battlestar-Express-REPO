using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Magic;
[System.Serializable]
public class ModuleEvent : UnityEvent<int>{}
public class ModuleStreamerPro : MonoBehaviour
{
    [SerializeField]
    public InputStreamEvent[] streamEvents;

    void Update()
    {
        foreach(InputStreamEvent streamEvent in streamEvents){ModuleBump(streamEvent);}
    }

    //void ModuleBump(Module module){moduleEvents[modEventNum++].Invoke(module.raw);}
    void ModuleBump(InputStreamEvent streamEvent){streamEvent.moduleEvent.Invoke(streamEvent.module.raw);}


}
[System.Serializable]
public class InputStreamEvent{
    [SerializeField]
    public Magic.Modules.DialModule module;
   // [SerializeField]
    //public Magic.Modules.DistanceModule module;
    [SerializeField]
    public ModuleEvent moduleEvent; 

}
