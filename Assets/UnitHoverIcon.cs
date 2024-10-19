using UnityEngine;

public class UnitHoverIcon : MonoBehaviour
{
   public enum UnitHoverIconState { Hovering, Targeting }

   private Animator UnitHoverIconAnimator;
   
   private static readonly int IsHovering = Animator.StringToHash("IsHovering");

   private void Awake()
   {
      UnitHoverIconAnimator = GetComponent<Animator>();
      
      UnitObject.OnUnitObjectHovered += HoverUnitObject;
   }

   private void HoverUnitObject(UnitObject unitObject)
   {
      transform.position = unitObject.gameObject.transform.position + Vector3.up * (0.005f * unitObject.SpriteRenderer.sprite.pivot.y);
      UnitHoverIconAnimator.SetBool(IsHovering, true);
   }
}
