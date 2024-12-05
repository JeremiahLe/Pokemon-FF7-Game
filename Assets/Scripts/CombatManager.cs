using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
    
    public static event Action<UnitObject> OnUnitActionStart;
    public static event Action<CommandType> OnCommandTypeStart;
    public static event Action<List<UnitObject>> OnUnitTargetingBegin;
    public static event Action OnUnitTargetingEnd;
    public List<UnitObject> UnitObjectsInScene { get; private set; }
    public Dictionary<UnitData, float> CurrentActionOrder { get; private set; }
    public UnitData CurrentUnitAction { get; private set; }
    public Command CurrentCommand { get; private set; }
    public DamageSource CurrentDamageSource { get; private set; }

    [Tooltip("Determines a unit's place in the action order.")] private int _defaultActionGauge = 250;
    [Tooltip("Determines the the action value allowed before a round passes.")] private int _initialRoundActionValue = 150;
    public int CurrentRound { get; private set; }
    public int CurrentRoundActionValue { get; private set; }
    public List<UnitObject> CurrentTargetedUnits { get; private set; } = new List<UnitObject>();
    
    #region Debug
    public UnitObject DebugTargetUnitObject;
    
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
        InitializeEvents();
        GetAllUnitObjects();
        InitializeCombat();
    }

    private void Update()
    {
        CheckInputs();
    }

    private void OnDestroy()
    {
        LogUnitActions();
        UninitializeEvents();
    }

    private void InitializeEvents()
    {
        CommandWindow.OnCommandButtonClicked += AssignCommandType;
        UnitObject.OnAttackAnimationEnd += HandleAttack;
        UnitObject.OnConfirmTarget += HandleTargeting;
    }

    private void UninitializeEvents()
    {
        CommandWindow.OnCommandButtonClicked -= AssignCommandType;
        UnitObject.OnAttackAnimationEnd -= HandleAttack;
        UnitObject.OnConfirmTarget -= HandleTargeting;
    }

    private void LogUnitActions()
    {
        var unitActions = new StringBuilder();
        unitActions.Append(CurrentRound).Append(" Rounds").AppendLine();
        
        foreach (var unit in UnitObjectsInScene)
        {
            unitActions.Append(unit.UnitData.UnitName).Append(" - ")
                .Append(unit.UnitData.ActionsTaken)
                .Append(" Actions")
                .AppendLine();
        }
        
        Debug.Log(unitActions);
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
        InitializeRound();
        UpdateActionOrder();
    }

    private void InitializeRound()
    {
        CurrentRound++;
        CurrentRoundActionValue = _initialRoundActionValue;
    }

    private IEnumerator AdvanceActionValues()
    {
        CurrentUnitAction = null;
        
        while (!CurrentUnitAction)
        {
            foreach (var unit in CurrentActionOrder.ToList())
            {
                unit.Key.CurrentActionValue -= 1f;
                
                CurrentActionOrder[unit.Key] = unit.Key.CurrentActionValue;

                if (unit.Key.CurrentActionValue <= 0f && !CurrentUnitAction)
                {
                    CurrentUnitAction = unit.Key;
                }
            }
            
            CurrentRoundActionValue -= 1;

            yield return null;
        }

        if (CurrentRoundActionValue <= 0)
        {
            InitializeRound();
        }
        
        _HUDManager.AdjustActionOrderIcons(CurrentActionOrder);

        InitiateUnitActionBegin();
    }

    private void GetInitialRoundActionOrder()
    {
        var initialUnitActionOrder = UnitObjectsInScene.OrderBy(unit => Mathf.Round(_defaultActionGauge / unit.UnitData.BaseSpeedStat.CurrentTotalStat)).ToList();
        
        foreach (var unit in initialUnitActionOrder)
        {
            unit.UnitData.CurrentActionValue = Mathf.Round(_defaultActionGauge / unit.UnitData.BaseSpeedStat.CurrentTotalStat);
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

    private void InitiateUnitActionBegin()
    {
        if (!CurrentUnitAction) return;

        var unitObject = UnitObjectsInScene.Find(unit => unit.UnitData == CurrentUnitAction);

        if (!unitObject)
        {
            Debug.LogError("Can't find {0}", CurrentUnitAction);
            return;
        }
        
        OnUnitActionStart?.Invoke(unitObject);
    }

    private void UnitEndAction()
    {
        if (!CurrentUnitAction) return;
        CurrentUnitAction.ActionsTaken++;
        
        CurrentUnitAction.CurrentActionValue = Mathf.Round(_defaultActionGauge / CurrentUnitAction.BaseSpeedStat.CurrentTotalStat);
    }

    private void AssignCommandType(Command command)
    {
        if (command.CommandType == CommandType.Back)
        {
            CurrentCommand = null;
            OnUnitTargetingEnd?.Invoke();
            return;
        }
        
        CurrentCommand = Command.ReturnCommandType(command.CommandType);
        OnCommandTypeStart?.Invoke(CurrentCommand.CommandType);
        
        switch (CurrentCommand.CommandType)
        {
            case CommandType.Attack:
                BeginTargetingPhase(CurrentUnitAction.BasicAttack);
                break;
            
            case CommandType.Action:
                break;
            
            case CommandType.Defend:
                PassAction();
                break;
            
            case CommandType.Item:
                break;
            
            case CommandType.Pass:
                PassAction();
                break;

            case CommandType.Back:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public UnitObject GetUnitObject(UnitData unitData)
    {
        return UnitObjectsInScene.Find(x => x.UnitData == unitData);
    }

    private void BeginTargetingPhase(DamageSource damageSource = null)
    {
        var currentUnitObject = GetUnitObject(CurrentUnitAction);

        if (!currentUnitObject) throw new NullReferenceException($"Cannot find UnitObject for {CurrentUnitAction}");

        if (damageSource == null) return; // TODO: TEMP

        CurrentDamageSource = damageSource;
        
        var targetRestrictions = damageSource.GetTargetingData().TargetRestrictions;
        var potentialTargets = GetTargetList(targetRestrictions, currentUnitObject);
        var initialHoveredTargets = GetTargetCount(damageSource.GetTargetingData(), potentialTargets, currentUnitObject);

        var targetLog = new StringBuilder().Append("Targets:").AppendLine();
        foreach (var target in initialHoveredTargets)
        {
            targetLog.Append(target.UnitData.UnitName).AppendLine();
        }
        Debug.Log(targetLog);

        OnUnitTargetingBegin?.Invoke(potentialTargets.ToList());
    }

    private List<UnitObject> GetTargetList(TargetRestrictions targetRestrictions, UnitObject unitObject = null)
    {
        switch (targetRestrictions)
        {
            case TargetRestrictions.Any:
                return UnitObjectsInScene;
            
            case TargetRestrictions.Self:
                if (unitObject) return new List<UnitObject> { unitObject };
                break;
            
            case TargetRestrictions.Allies:
                return unitObject ? 
                    UnitObjectsInScene.Where(unit => unit.unitSideOfField != unitObject.unitSideOfField).ToList() 
                    : UnitObjectsInScene.Where(unit => unit.unitSideOfField == UnitOrientation.Left).ToList();
            
            case TargetRestrictions.Enemies:
                return unitObject ? 
                    UnitObjectsInScene.Where(unit => unit.unitSideOfField != unitObject.unitSideOfField).ToList() 
                    : UnitObjectsInScene.Where(unit => unit.unitSideOfField == UnitOrientation.Right).ToList();
            
            default:
                throw new ArgumentOutOfRangeException(nameof(targetRestrictions), targetRestrictions, null);
        }

        return null;
    }

    private IEnumerable<UnitObject> GetTargetCount(TargetingData targetingData, IEnumerable<UnitObject> potentialTargets, UnitObject unitObject = null)
    {
        switch (targetingData.TargetingType)
        {
            case TargetingType.Single:
                return targetingData.TargetRestrictions == TargetRestrictions.Any 
                    ? GetTargetList(TargetRestrictions.Enemies, unitObject).GetRange(0, 1)
                    : new List<UnitObject> { potentialTargets.FirstOrDefault() };

            case TargetingType.MultiSelect:
                break;
            
            case TargetingType.Adjacent:
                break;
            
            case TargetingType.All:
                return potentialTargets;
            
            case TargetingType.Random:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(targetingData), targetingData, null);
        }

        return null;
    }

    private void HandleTargeting(UnitObject unitObject)
    {
        CurrentTargetedUnits.Add(unitObject);
        
        OnUnitTargetingEnd?.Invoke();
        
        GetUnitObject(CurrentUnitAction).AnimationBasicAttackStart();
    }

    private void HandleAttack()
    {
        foreach (var target in CurrentTargetedUnits)
        {
            target.ReceiveDamage(CalculateDamage(target));
        }

        ResetAttack();
        UnitEndAction();
        UpdateActionOrder();
    }

    private float CalculateDamage(UnitObject target)
    {
        if (!CurrentUnitAction) return 0f;
        if (CurrentDamageSource == null) return 0f;

        var finalDamage = CurrentUnitAction.GetScaledDamageAmount(CurrentDamageSource) - target.UnitData.GetScaledDefensiveAmount(CurrentDamageSource);

        if (finalDamage <= 0) return -1f; // TODO: Do not return 1 if immune
        
        return Mathf.Abs(finalDamage) * -1f;
    }

    private void ResetAttack()
    {
        CurrentTargetedUnits.Clear();
    }

    private void PassAction()
    {
        UnitEndAction();
        UpdateActionOrder();
    }
}
