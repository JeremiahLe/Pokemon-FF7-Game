using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private HUDManager _HUDManager;

    #region Debug
    public UnitObject DebugTargetUnitObject;
    
    public float debugAttributeValue = 1f;
    
    [Button, DisableInEditorMode]
    private void ApplyOrHealDamage()
    {
        DebugTargetUnitObject.ReceiveDamage(debugAttributeValue);
    }
    #endregion
    
    public List<UnitObject> UnitObjectsInScene;
    public Dictionary<UnitData, float> CurrentActionOrder;
    public UnitData CurrentUnitAction;
    
    private void Awake()
    {
        InitializeComponents();
        GetAllUnitObjects();
        InitializeCombat();
    }

    private void Update()
    {
        CheckInputs();
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (CurrentUnitAction)
            {
                UnitEndAction();
                UpdateActionOrder();
            }
        }
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

    private void InitializeCombat()
    {
        GetInitialRoundActionOrder();
        UpdateActionOrder();
    }

    private IEnumerator AdvanceActionValues()
    {
        CurrentUnitAction = null;
        while (!CurrentUnitAction)
        {
            foreach (var unit in CurrentActionOrder.ToList())
            {
                var newActionValue = unit.Key.CurrentActionValue -= 1f;

                CurrentActionOrder[unit.Key] = newActionValue;

                if (unit.Key.CurrentActionValue <= 0f && !CurrentUnitAction)
                {
                    CurrentUnitAction = unit.Key;
                }
            }

            yield return null;
        }
        
        _HUDManager.AdjustActionOrderIcons(CurrentActionOrder);
    }

    private void GetInitialRoundActionOrder()
    {
        var initialUnitActionOrder = UnitObjectsInScene.OrderBy(unit => Mathf.Round(100 / unit.UnitData.BaseSpeed)).ToList();

        foreach (var unit in initialUnitActionOrder)
        {
            unit.UnitData.CurrentActionValue = Mathf.Round(100 / unit.UnitData.BaseSpeed);
        }

        var actionOrder = initialUnitActionOrder.ToDictionary(unit => unit.UnitData, unit => unit.UnitData.CurrentActionValue);

        CurrentActionOrder = actionOrder;
        
        _HUDManager.AdjustActionOrderIcons(actionOrder);
    }

    private void UpdateActionOrder()
    {
        var newActionOrder = UnitObjectsInScene.OrderBy(unit => unit.UnitData.CurrentActionValue).ToList();

        CurrentActionOrder = newActionOrder.ToDictionary(unit => unit.UnitData, unit => unit.UnitData.CurrentActionValue);
        
        StartCoroutine(AdvanceActionValues());
    }

    private void UnitEndAction()
    {
        if (CurrentUnitAction)
        {
            CurrentUnitAction.CurrentActionValue = Mathf.Round(100 / CurrentUnitAction.BaseSpeed);
        }
    }
}
