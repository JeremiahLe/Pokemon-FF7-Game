using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

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

    [Button, DisableInEditorMode]
    private void DealDamage()
    {
        DebugTargetUnitObject.ReceiveDamage(-20f);
    }
    
    [Button, DisableInEditorMode]
    private void HealDamage()
    {
        DebugTargetUnitObject.ReceiveDamage(20f);
    }
}
