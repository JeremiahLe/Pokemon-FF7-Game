using UnityEngine;
using UnityEngine.UI;

public class UnitHoverIcon : MonoBehaviour
{
   [SerializeField] private Image _unitHoverIconImage;
   public enum UnitHoverIconState { Hovering, Targeting }

   private Animator UnitHoverIconAnimator;
   
   private static readonly int IsHovering = Animator.StringToHash("IsHovering");

   private void Awake()
   {
      UnitHoverIconAnimator = GetComponent<Animator>();
      
      UnitObject.OnUnitObjectHovered += HoverUnitObject;
      UnitObject.OnUnitObjectUnhovered += HideIcon;
   }

   private void HoverUnitObject(UnitObject unitObject)
   {
      ShowIcon();
      transform.position = unitObject.gameObject.transform.position + Vector3.up * (0.005f * unitObject.SpriteRenderer.sprite.pivot.y);
      UnitHoverIconAnimator.SetBool(IsHovering, true);
   }

   private void ShowIcon()
   {
      _unitHoverIconImage.enabled = true;
   }
   
   private void HideIcon()
   {
      _unitHoverIconImage.enabled = false;
   }
}
