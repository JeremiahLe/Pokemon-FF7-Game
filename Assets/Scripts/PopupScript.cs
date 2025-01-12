using TMPro;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetTextFromAmount(float amount)
    {
        if (amount > 0)
        {
            SetText($"+{amount}", ColorDatabase.Instance.BaseHealthBarColor);
        }
        else if (amount < 0)
        {
            SetText($"{amount}", ColorDatabase.Instance.LowestHealthBarColor);
        }
        else if (amount == 0)
        {
            SetText($"{amount}", Color.white);
        }
    }

    public void SetText(string text, Color color = default)
    {
        if (color != default) _text.color = color;
        _text.text = text;
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
