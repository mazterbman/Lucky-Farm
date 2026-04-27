using System;
using Game.Scripts.Building.StoreHouse;

namespace Game.Scripts.Building
{
    [Serializable]
    public class BuildingData
    {
        public WaterWellController WellController;
        public StoreHouseController StoreHouseController;
        public StoreHouseUiController StoreHouseUiController;
    }
}