using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public enum ResourceBarType { Health, Mana, Stamina, Ammo, Shield }
    [field: SerializeField] public ResourceBarType CurrentResourceBarType { get; private set; }
    [field: SerializeField] public bool ShouldLerpColor { get; private set; }
    
    [SerializeField] private Slider _resourcebarSlider;
    [SerializeField] private Slider _resourcebarBackgroundSlider;
    [SerializeField] private Image _resourcebarImageFill;
    [SerializeField] private Image _resourcebarBackgroundFill;
    [SerializeField] private TextMeshProUGUI _resourceValueText;
    
    public void InitializeResourceBar(float maxValue)
    {
        _resourcebarSlider.maxValue = maxValue;
        _resourcebarBackgroundSlider.maxValue = maxValue;
        _resourcebarBackgroundSlider.value = maxValue;
    }
    
    public void UpdateResourceBar(float currentAmount, float maxAmount, bool isInit = false)
    {
        var resourcePercentage = currentAmount / maxAmount;
        
        if (_resourcebarSlider.value < currentAmount && !isInit)
        {
            _resourcebarBackgroundSlider.value = currentAmount;
            
            UpdateResourceBarText(currentAmount, maxAmount);
            
            var color = TryLerpResourceFillColor(resourcePercentage);
            
            StartCoroutine(FadeHealthBarForegroundFill(color));
            
            return;
        }

        _resourcebarSlider.value = currentAmount;
        
        TryLerpResourceFillColor(resourcePercentage);
            
        UpdateResourceBarText(currentAmount, maxAmount);

        StartCoroutine(FadeHealthBarBackgroundFill());
    }

    private Color TryLerpResourceFillColor(float resourcePercentage)
    {
        if (!ShouldLerpColor) return Color.white;

        var lowColor = Color.white;
        var baseColor = Color.white;
        
        switch (CurrentResourceBarType)
        {
            case ResourceBarType.Health:
                lowColor = ColorDatabase.Instance.LowestHealthBarColor;
                baseColor = ColorDatabase.Instance.BaseHealthBarColor;
                break;
            
            case ResourceBarType.Mana:
                break;
            
            case ResourceBarType.Stamina:
                break;
            
            case ResourceBarType.Ammo:
                break;
            
            case ResourceBarType.Shield:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        _resourcebarImageFill.color = Color.Lerp(lowColor, baseColor, resourcePercentage);

        return _resourcebarImageFill.color;
    }

    private void UpdateResourceBarText(float amount, float maxAmount)
    {
        _resourceValueText.text = $"{Mathf.Max(0, amount)}/{maxAmount}";
    }

    private IEnumerator FadeHealthBarBackgroundFill()
    {
        _resourcebarBackgroundFill.CrossFadeAlpha(0, .75f, false);

        yield return new WaitForSeconds(0.75f);

        _resourcebarBackgroundSlider.value = _resourcebarSlider.value;
        _resourcebarBackgroundFill.CrossFadeAlpha(1, 0.1f, false);
    }
    
    private IEnumerator FadeHealthBarForegroundFill(Color color)
    {
        _resourcebarBackgroundFill.color = Color.white;

        _resourcebarBackgroundFill.CrossFadeColor(color, 0.75f, false, false);
        
        yield return new WaitForSeconds(0.75f);

        _resourcebarSlider.value = _resourcebarBackgroundSlider.value;
        //_resourcebarBackgroundFill.CrossFadeAlpha(1, 0.1f, false);
    }
}
