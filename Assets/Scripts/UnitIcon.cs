using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    public UnitData UnitData;

    [SerializeField] private Image _unitIconSprite;
    [SerializeField] private Slider _healthbarSlider;
    [SerializeField] private Slider _healthbarBackgroundSlider;
    
    [SerializeField] private Image _healthbarImageFill;
    [SerializeField] private Image _healthbarBackgroundFill;
    
    [SerializeField] TextMeshProUGUI _healthValueText;

    [SerializeField] private Color _baseHealthbarColor;
    [SerializeField] private Color _lowestHealthBarColor;
    
    private static readonly int TextDamageReceived = Animator.StringToHash("TextDamageReceived");
    
    public void InitializeData(UnitData unitData)
    {
        UnitData = unitData;
        
        _unitIconSprite.sprite = UnitData.UnitBaseSprite;
        
        InitializeHealthbar();
    }

    public void InitializeHealthbar()
    {
        _healthbarSlider.maxValue = UnitData.MaxHealth;
        _healthbarBackgroundSlider.maxValue = UnitData.MaxHealth;
        _healthbarBackgroundSlider.value = _healthbarBackgroundSlider.maxValue;
        
        UpdateHealthbar();
    }

    public void UpdateHealthbar()
    {
        var healthPercentage = UnitData.CurrentHealth / UnitData.MaxHealth;

        _healthbarSlider.value = UnitData.CurrentHealth;

        _healthbarImageFill.color = Color.Lerp(_lowestHealthBarColor, _baseHealthbarColor, healthPercentage);
        
        UpdateHealthText();

        StartCoroutine(FadeHealthbarBackgroundFill());
    }

    public void UpdateHealthText()
    {
        _healthValueText.text = $"{Mathf.Max(0, UnitData.CurrentHealth)}/{UnitData.MaxHealth}";
    }

    private IEnumerator FadeHealthbarBackgroundFill()
    {
        _healthbarBackgroundFill.CrossFadeAlpha(0, .75f, false);

        yield return new WaitForSeconds(0.75f);
        
        AdjustHealthbarBackgroundFill();
    }

    private void AdjustHealthbarBackgroundFill()
    {
        _healthbarBackgroundSlider.value = UnitData.CurrentHealth;
        _healthbarBackgroundFill.CrossFadeAlpha(1, 0.1f, false);
    }

    public void AnimationTextDamageReceivedStart()
    {
        GetComponent<Animator>().SetBool(TextDamageReceived, true);  
    }
    
    public void AnimationTextDamageReceivedEnd()
    {
        GetComponent<Animator>().SetBool(TextDamageReceived, false);  
    }
}
