using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingLevelsSweetShop", menuName = "Scriptable Objects/BuildingsSetting/SettingLevelsSweetShop")]
    public class SettingLevelsSweetShop : ScriptableObject
    {
        [SerializeField] [Range(0, 3)] private int _standardMaxItems = 1;
        [SerializeField] [Range(0, 360)] private int _standardTimeWaite = 120;

        [Space]
        public List<LevelSettings> LevelSettingsList;

        public int GetMaxItem(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.MaxMod * _standardMaxItems);
        }

        public int GetTimeWaite(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.TimeWaiteMod * _standardTimeWaite);
        }
        
        [Serializable]
        public class LevelSettings
        {
            [Range(1, 3)] public int MaxMod;
            [Range(0, 1)] public float TimeWaiteMod;
        }
    }
}