using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Unit Action", menuName = "Damage Sources/UnitSpecialActions")]
public class UnitSpecialAction : ScriptableObject, ISpecialAction
{
    [field: SerializeField] public string ActionName { get; private set; }
    [field: SerializeField, TextArea] public string ActionDescription { get; private set;}
    [field: SerializeField] public DamageType DamageType { get; private set;}
    [field: SerializeField] public List<DamageScalar> DamageScalars { get; private set;}
    [field: SerializeField] public TargetingData TargetingData { get; private set;}
    [field: SerializeField] public AffinityClass ActionAffinityClass { get; private set;}
    [field: SerializeField] public int ActionPointCost { get; private set; }
    
    [Title("Helpers"), Button("Autofill description")]
    public void Button()
    {
        ActionDescription = DealsDamageStaticHelpers.GetAutofilledDescription(this);
        EditorUtility.SetDirty(this);
    }
}
