using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class FadeAnimatorEditor : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    private void OnValidate()
    {
        if (!Application.isPlaying) _fadeImage.enabled = false;
    }
}
#endif