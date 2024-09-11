namespace Databases
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "ChimericDatabase", menuName = "Databases")]
    public class ChimericDatabase : ScriptableSingleton<ChimericDatabase>
    {
        private static ChimericDatabase _instance;
        public static ChimericDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ChimericDatabase>("ChimericDatabase");
                }

                return _instance;
            }
        }

        public Monster[] Chimerics => _chimerics;
    
        [SerializeField] private Monster[] _chimerics;
        
        private void OnValidate()
        {
            _chimerics = _chimerics.Distinct().ToArray();
            SortByChimericIndex();
        }

        [ContextMenu("Sort By Chimeric Index")]
        public void SortByChimericIndex()
        {
            _chimerics = _chimerics.OrderBy(chimeric => chimeric.ChimericIndex).ToArray();
        }
    }
}
