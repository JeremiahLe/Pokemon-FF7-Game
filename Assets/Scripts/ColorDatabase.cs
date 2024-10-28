using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

// public static class ColorDatabaseSingleton
// {
//     private static ColorDatabase _colorDatabase;
//     
//     public static ColorDatabase ColorDatabase()
//     {
//         return _colorDatabase;
//     }
//     
//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     public static void InitializeSingletonDatabase()
//     {
//         _colorDatabase = AssetDatabase.LoadAssetAtPath<ColorDatabase>();
//     }
// }

[Serializable]
[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Databases")]
public class ColorDatabase : ScriptableSingleton<ColorDatabase>
{
    private static ColorDatabase _instance;
    
    public static ColorDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ColorDatabase>("ColorDatabase");
            }

            return _instance;
        }
    }
    
    [field: SerializeField, Title("UI")] public Color BaseHealthBarColor { get; private set; }
    [field: SerializeField] public Color LowestHealthBarColor { get; private set; }
    [field: SerializeField] public Color StaminaBarColor { get; private set; }
    [field: SerializeField] public Color ManaBarColor { get; private set; }
}