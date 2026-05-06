using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingLevelsStoreHouse", menuName = "Scriptable Objects/BuildingsSetting/SettingLevelsStoreHouse")]
    public class SettingLevelsStoreHouse : ScriptableObject
    {
        [SerializeField] [Range(0, 200)] private int _standardMaxItems = 100;
        [SerializeField] [Range(0, 360)] private int _standardSecondsOnSale = 100;

        [Space]
        public List<LevelSettings> LevelSettingsList;

        public int GetMaxItem(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.MaxMod * _standardMaxItems);
        }

        public int GetSecondsOnSale(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.SecondsMod * _standardSecondsOnSale);
        }
        
        [Serializable]
        public class LevelSettings
        {
            [Range(1, 3)] public float MaxMod;
            [Range(0, 1)] public float SecondsMod;
        }
    }
}