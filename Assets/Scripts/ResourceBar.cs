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

    [Header("Settings")] 
    [SerializeField] private float _sliderLerpSpeed = 5f;
    
    public void InitializeResourceBar(float currentValue, float maxValue)
    {
        _resourcebarSlider.maxValue = maxValue;
        _resourcebarBackgroundSlider.maxValue = maxValue;
        
        _resourcebarBackgroundSlider.value = currentValue;
        _resourcebarSlider.value = currentValue;

        var resourcePercentage = currentValue / maxValue;
        _resourcebarImageFill.color = TryLerpResourceFillColor(resourcePercentage);
        
        UpdateResourceBarText(currentValue, maxValue);
    }
    
    public void UpdateResourceBar(float currentAmount, float maxAmount)
    {
        Debug.Log("Update resource bar!");
        StopAllCoroutines();

        var resourcePercentage = currentAmount / maxAmount;
        
        UpdateResourceBarText(currentAmount, maxAmount);
        
        var color = TryLerpResourceFillColor(resourcePercentage);
        
        // Resource is going up
        if (_resourcebarSlider.value < currentAmount)
        {
            StartCoroutine(FadeHealthBarForegroundFill(color, currentAmount));
            
            return;
        }
        
        // Resource is going down
        StartCoroutine(FadeHealthBarBackgroundFill(color, currentAmount));
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

        return Color.Lerp(lowColor, baseColor, resourcePercentage);
    }

    private void UpdateResourceBarText(float amount, float maxAmount)
    {
        _resourceValueText.text = $"{Mathf.Max(0, amount)}/{maxAmount}";
    }

    private IEnumerator FadeHealthBarBackgroundFill(Color color, float currentAmount)
    {
        var lerpTime = GetLerpTime(currentAmount, _resourcebarSlider.value);
        
        // Lerp the colored bar to the target amount of the resource
        while (Mathf.Abs(_resourcebarSlider.value - currentAmount) > 0.1f)
        {
            _resourcebarSlider.value = Mathf.Lerp(_resourcebarSlider.value, currentAmount,  lerpTime * Time.deltaTime);
            
            _resourcebarImageFill.color = Color.Lerp(_resourcebarImageFill.color, color, lerpTime * Time.deltaTime);
            
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        
        // Lerp the white bar to the fill of the colored bar
        while (Mathf.Abs(_resourcebarBackgroundSlider.value - _resourcebarSlider.value) > 0.1f)
        {
            _resourcebarBackgroundSlider.value = Mathf.Lerp(_resourcebarBackgroundSlider.value, _resourcebarSlider.value,  lerpTime * Time.deltaTime);
            
            yield return null;
        }
    }
    
    private IEnumerator FadeHealthBarForegroundFill(Color color, float currentAmount)
    {
        var lerpTime = GetLerpTime(currentAmount, _resourcebarSlider.value);

        // Lerp the white bar to the target amount of the resource
        while (Math.Abs(_resourcebarBackgroundSlider.value - currentAmount) > 0.1f)
        {
            _resourcebarBackgroundSlider.value = Mathf.Lerp(_resourcebarBackgroundSlider.value, currentAmount,  lerpTime * Time.deltaTime);
            
            yield return null;
        }
        
        yield return new WaitForSeconds(0.15f);
        
        // Lerp the fill of the colored bar to the white background bar
        while (Mathf.Abs(_resourcebarSlider.value - _resourcebarBackgroundSlider.value) > 0.1f)
        {
            _resourcebarSlider.value = Mathf.Lerp(_resourcebarSlider.value, _resourcebarBackgroundSlider.value,  lerpTime * Time.deltaTime);

            _resourcebarImageFill.color = Color.Lerp(_resourcebarImageFill.color, color, lerpTime * Time.deltaTime);

            yield return null;
        }
    }

    private float GetLerpTime(float currentAmount, float oldAmount)
    {
        var finalLerpSpeed = Mathf.Clamp(Mathf.Abs((currentAmount - oldAmount) / oldAmount * 10f), _sliderLerpSpeed, 10f);

        return finalLerpSpeed;
    }
}
