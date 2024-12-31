using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    // Components
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    public CameraBillboard CameraBillboard { get; private set; }
    public UnitIcon BoundUnitIcon { get; set; }
    
    public bool IsInteractable { get; private set; }

    private Animator Animator;
    
    private static readonly int TakingDamage = Animator.StringToHash("TakingDamage");
    private static readonly int BasicAttack = Animator.StringToHash("BasicAttack");

    public static event Action<UnitObject> OnUnitObjectHovered;
    public static event Action OnUnitObjectUnhovered;
    public static event Action<UnitObject> OnConfirmTarget;
    public static event Action OnAttackAnimationEnd;
    public static event Action<UnitObject, float> OnDamageTaken;

    public static event Action<ResourceBar, UnitData, bool> OnResourceUpdated;
    
    [field: SerializeField, Title("ID")] 
    public UnitOrientation unitSideOfField;
    [field: SerializeField] public UnitData UnitData { get; private set; }

    private void OnValidate()
    {
        ValidateEditorComponents();
    }

    private void ValidateEditorComponents()
    {
        if (!SpriteRenderer) return;

        SpriteRenderer.flipX = UnitOrientationExtension.GetUnitOrientation(unitSideOfField);
            
        if (UnitData != null)
        {
            SpriteRenderer.sprite = UnitData.UnitBaseSprite;
        }
    }

    private void Awake()
    {
        CameraBillboard = GetComponent<CameraBillboard>();
        Animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        DeInitializeEvents();
    }

    private void InitializeEvents()
    {
        CombatManager.OnUnitTargetingBegin += ValidateInteractability;
        CombatManager.OnUnitTargetingEnd += ResetInteractability;
    }
    
    private void DeInitializeEvents()
    {
        CombatManager.OnUnitTargetingBegin -= ValidateInteractability;
        CombatManager.OnUnitTargetingEnd -= ResetInteractability;
    }

    public void InitializeUnitData()
    {
        UnitData = Instantiate(UnitData);

        UnitData.BasicAttack = Instantiate(UnitData.BasicAttack);
        var specialActions = UnitData.SpecialActions.Select(Instantiate).ToList();
        UnitData.SpecialActions = specialActions;
        UnitData.InitializeData();
        
        var allyOrEnemy = unitSideOfField == UnitOrientation.Left ? "Ally" : "Enemy";
        gameObject.name = $"{UnitData.UnitName} - {allyOrEnemy}";
        InitializeEvents();
    }

    public void OnMouseEnter()
    {
        OnUnitObjectHovered?.Invoke(this);
    }

    public void OnMouseExit()
    {
        OnUnitObjectUnhovered?.Invoke();
    }

    public void OnMouseDown()
    {
        CombatManagerSingleton.CombatManager().DebugTargetUnitObject = this;

        if (!IsInteractable) return;

        IsInteractable = false;
        OnConfirmTarget?.Invoke(this);
    }

    private void ValidateInteractability(List<UnitObject> unitObjects)
    {
        if (unitObjects.Contains(this))
        {
            IsInteractable = true;
        }
    }

    private void ResetInteractability()
    {
        IsInteractable = false;
    }

    public float ReceiveDamage(float damagedReceived)
    {
        UnitData.CurrentHealth += damagedReceived;
        
        OnResourceUpdated?.Invoke(BoundUnitIcon.HealthBar, UnitData, false);
        
        BoundUnitIcon.AnimationTextDamageReceivedStart();
        AnimationTakingDamageStart();

        OnDamageTaken?.Invoke(this, damagedReceived);
        
        return UnitData.CurrentHealth;
    }

    public void AnimationBasicAttackStart()
    {
        Animator.SetBool(BasicAttack, true);
    }
    
    private void AnimationBasicAttackEnd()
    {
        Animator.SetBool(BasicAttack, false);
        OnAttackAnimationEnd?.Invoke();
    }

    private void AnimationTakingDamageStart()
    {
        Animator.SetBool(TakingDamage, true);
    }
    
    public void AnimationTakingDamageEnd()
    {
        Animator.SetBool(TakingDamage, false);
    }
}
