using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodexIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _codexIconImage;

    private Action CodexDelegate;
    private Monster _monster;
    
    public void SetIconData(Sprite iconImage)
    {
        _codexIconImage.sprite = iconImage;
    }
    
    public void SetMonsterData(Monster monster, Action codexDelegate)
    {
        _monster = monster;
        _codexIconImage.sprite = monster.baseSprite;
        CodexDelegate = codexDelegate;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        CodexDelegate.Invoke();
    }
}
