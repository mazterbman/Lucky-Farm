using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingLevelsStoreHouse", menuName = "Scriptable Objects/BuildingsSetting/SettingLevelsStoreHouse")]
    public class SettingLevelsStoreHouse : ScriptableObject
    {
        [SerializeField] [Range(0, 200)] private int _standardMaxItems = 100;

        [Space]
        public List<LevelSettings> LevelSettingsList;

        public int GetMaxItem(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.MaxMod * _standardMaxItems);
        }
        
        [Serializable]
        public class LevelSettings
        {
            [Range(1, 3)] public float MaxMod;
        }
    }
}