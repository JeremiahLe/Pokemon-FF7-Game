using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Unit Action", menuName = "Damage Sources")]
public class UnitSpecialAction : DamageSource
{
    [field: SerializeField, Title("Action Identifier")]
    public string ActionName { get; private set; }
    
    [field: SerializeField, TextArea]
    public string ActionDescription { get; private set; }

    public AffinityClass.AffinityType ActionAffinityType { get; private set; }
    public AffinityClass ActionAffinityClass { get; private set; }
}
