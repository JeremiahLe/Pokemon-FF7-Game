using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHoverIcon : MonoBehaviour
{
   [SerializeField] private Image _unitHoverIconImage;
   [SerializeField] private TextMeshProUGUI _unitHoverIconText;
   public enum UnitHoverIconState { Hovering, Targeting }

   public UnitHoverIconState UnitHoverState;

   private Animator UnitHoverIconAnimator;
   
   private static readonly int IsHovering = Animator.StringToHash("IsHovering");

   private void Awake()
   {
      UnitHoverIconAnimator = GetComponent<Animator>();
      _unitHoverIconText.text = "";

      UnitObject.OnUnitObjectHovered += HoverUnitObject;
      UnitObject.OnUnitObjectUnhovered += UnHoverUnitObject;
   }

   private void OnDestroy()
   {
      UnitObject.OnUnitObjectHovered -= HoverUnitObject;
      UnitObject.OnUnitObjectUnhovered -= UnHoverUnitObject;
   }
   
   public void HoverUnitObject(UnitObject unitObject)
   {
      transform.position = unitObject.gameObject.transform.position + Vector3.up * (0.005f * unitObject.SpriteRenderer.sprite.pivot.y);
      
      if (UnitHoverState == UnitHoverIconState.Targeting)
      {
         if (!unitObject.IsInteractable)
         {
            HideIcon();
            ShowText("Invalid Target");
            return;
         }
      }
      
      ShowIcon();
      ShowText("");
      UnitHoverIconAnimator.SetBool(IsHovering, true);
   }

   private void UnHoverUnitObject()
   {
      HideIcon();
      ShowText("");
   }

   private void ShowIcon()
   {
      _unitHoverIconImage.enabled = true;
   }
   
   private void HideIcon()
   {
      _unitHoverIconImage.enabled = false;
   }

   private void ShowText(string text)
   {
      _unitHoverIconText.text = text;
   }
}
