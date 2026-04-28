using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingLevelsWaterWell", menuName = "Scriptable Objects/BuildingsSetting/SettingLevelsWaterWell")]
    public class SettingLevelsWaterWell : ScriptableObject
    {
        [SerializeField] [Range(0, 1000)] private int _standardMaxWater = 120;
        [SerializeField] [Range(0, 100)] private int _standardCoastBuyWater = 15;
        [SerializeField] [Range(0, 1)] private float _standardCoastUseWater = 0.15f;
        [SerializeField] [Range(0,1)] private float _standardAddWaterOnClk = 0.35f;

        [Space] 
        public List<LevelSettings> SettingsList;

        public int GetMaxWater(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.MaxWaterMod * _standardMaxWater);
        }

        public int GetCoastBuyWater(LevelSettings levelSettings)
        {
            return Mathf.RoundToInt(levelSettings.CoastBuyMod * _standardCoastBuyWater);
        }

        public float GetCoastUseWater(LevelSettings levelSettings)
        {
            return levelSettings.CoastUseMod * _standardCoastUseWater;
        }

        public float GetAddWaterOnClk(LevelSettings levelSettings)
        {
            return levelSettings.AddWaterOnClickMod * _standardAddWaterOnClk;
        }
        
        [Serializable]
        public class LevelSettings
        {
            [Range(1, 3)] public float MaxWaterMod = 1;
            [Range(0, 1)] public float CoastBuyMod = 1;
            [Range(0, 1)] public float CoastUseMod = 1;
            [Range(0, 1)] public float AddWaterOnClickMod = 1f;
        }
    }
}