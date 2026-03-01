using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    // The names of the bool fields that will be in control
    public string[] ConditionalSourceFields;
    // The required values of the corresponding bool fields
    public bool[] ConditionalSourceValues;
    // TRUE = Hide in inspector / FALSE = Disable in inspector
    public bool HideInInspector = false;

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false)
    {
        this.ConditionalSourceFields = new string[] { conditionalSourceField };
        this.ConditionalSourceValues = new bool[] { true }; // Default to true
        this.HideInInspector = hideInInspector;
    }

    public ConditionalHideAttribute(string[] conditionalSourceFields, bool[] conditionalSourceValues, bool hideInInspector = false)
    {
        this.ConditionalSourceFields = conditionalSourceFields;
        this.ConditionalSourceValues = conditionalSourceValues;
        this.HideInInspector = hideInInspector;
    }
}
