namespace Managers
{
    using UnityEngine;
    
    public static class ApplicationManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeManagers()
        {
            AudioManager.Initialize();
        }
    }
}
