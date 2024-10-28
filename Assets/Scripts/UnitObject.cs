using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    [field: SerializeField, Title("ID")] 
    public UnitOrientation unitSideOfField;
    [field: SerializeField] public UnitData UnitData { get; private set; }

    // Components
    public SpriteRenderer SpriteRenderer { get; private set; }
    public SpriteShadow SpriteShadow { get; private set; }
    public CameraBillboard CameraBillboard { get; private set; }
    public UnitIcon BoundUnitIcon { get; set; }

    public static event Action<UnitObject> OnUnitObjectHovered;

    public static event Action<ResourceBar, UnitData, bool> OnResourceUpdated;
    
    private void OnValidate()
    {
        ValidateEditorComponents();
    }
    
    private void ValidateEditorComponents()
    {
        if (!TryGetComponent<SpriteRenderer>(out var spriteRenderer)) return;
        
        SpriteRenderer = spriteRenderer;
            
        SpriteRenderer.flipX = UnitOrientationExtension.GetUnitOrientation(unitSideOfField);
            
        if (UnitData != null)
        {
            SpriteRenderer.sprite = UnitData.UnitBaseSprite;
        }
    }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteShadow = GetComponent<SpriteShadow>();
        CameraBillboard = GetComponent<CameraBillboard>();
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

        return UnitData.CurrentHealth;
    }
}
