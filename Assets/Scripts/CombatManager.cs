using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public static class CombatManagerSingleton
{
    private static CombatManager _combatManager;
    public static CombatManager CombatManager()
    {
        return _combatManager;
    }

    public static void InitializeCombatManagerSingleton(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }
}

public class CombatManager : MonoBehaviour
{
    public List<UnitObject> UnitObjectsInScene;

    public UnitObject DebugTargetUnitObject;
    
    private HUDManager _HUDManager;
    
    #region Debug
    public float debugAttributeValue = 1f;
    
    [Button, DisableInEditorMode]
    private void ApplyOrHealDamage()
    {
        DebugTargetUnitObject.ReceiveDamage(debugAttributeValue);
    }
    #endregion
    
    private void Awake()
    {
        InitializeComponents();
        GetAllUnitObjects();
    }

    private void InitializeComponents()
    {
        if (!TryGetComponent(out _HUDManager))
        {
            throw new Exception($"No {nameof(_HUDManager)} found!");
        }
        
        CombatManagerSingleton.InitializeCombatManagerSingleton(this);
    }

    private void GetAllUnitObjects()
    {
        UnitObjectsInScene = FindObjectsOfType<UnitObject>().ToList();

        foreach (var unit in UnitObjectsInScene)
        {
            unit.GetComponent<UnitObject>().InitializeUnitData();
        }
        
        _HUDManager.InitializeComponents();
    }
}
