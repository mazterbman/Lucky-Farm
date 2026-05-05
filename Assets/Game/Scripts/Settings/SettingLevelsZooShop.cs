using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingLevelsZooShop", menuName = "Scriptable Objects/BuildingsSetting/SettingLevelsZooShop")]
    public class SettingLevelsZooShop : ScriptableObject
    {
        [SerializeField] private int _standardItemEnabled = 1;
        
        [Space]
        public List<LevelSettings> Levels;

        public int GetItemsEnabled(LevelSettings levelSettings)
        {
            return levelSettings.MaxMod * _standardItemEnabled;
        }
        
        [Serializable]
        public class LevelSettings
        {
            [Range(1, 5)] public int MaxMod;
        }
    }
}