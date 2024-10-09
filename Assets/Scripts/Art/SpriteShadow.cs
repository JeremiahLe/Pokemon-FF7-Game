using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
