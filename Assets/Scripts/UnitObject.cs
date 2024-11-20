using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    // Components
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    public CameraBillboard CameraBillboard { get; private set; }
    public UnitIcon BoundUnitIcon { get; set; }

    private Animator Animator;
    
    private static readonly int TakingDamage = Animator.StringToHash("TakingDamage");

    public static event Action<UnitObject> OnUnitObjectHovered;

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

    public void InitializeUnitData()
    {
        UnitData = Instantiate(UnitData);
        UnitData.MaxHealth = UnitData.BaseHealth;
        UnitData.CurrentHealth = UnitData.MaxHealth;
    }

    public void OnMouseEnter()
    {
        OnUnitObjectHovered?.Invoke(this);
    }

    public void OnMouseDown()
    {
        CombatManagerSingleton.CombatManager().DebugTargetUnitObject = this;
    }

    public float ReceiveDamage(float damagedReceived)
    {
        UnitData.CurrentHealth += damagedReceived;
        
        OnResourceUpdated?.Invoke(BoundUnitIcon.HealthBar, UnitData, false);
        
        BoundUnitIcon.AnimationTextDamageReceivedStart();
        AnimationTakingDamageStart();

        return UnitData.CurrentHealth;
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
