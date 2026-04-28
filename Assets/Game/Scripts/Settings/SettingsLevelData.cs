using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = "SettingsLevel", menuName = "Scriptable Objects/SettingsLevel")]
    public class SettingsLevelData : ScriptableObject
    {
        [Header("Start Level Settings")]
        [Range(0,1000)] public int StartBalanceLevel = 200;
        [Range(0,10)] public int StartChickenCount = 2;

        [Header("Coast Animals")] 
        [Range(0, 1000)] public int ChickenCoast = 120;
        
        [Header("Settings")]
        public SettingLevelsWaterWell WaterWellSettings;
        public SettingLevelsStoreHouse StoreHouseSetting;
    }
}
