using System;
using Game.Scripts.Building.StoreHouse;

namespace Game.Scripts.Building
{
    [Serializable]
    public class BuildingData
    {
        public WaterWellController.WaterWellController WellController;
        public StoreHouseController StoreHouseController;
        public StoreHouseUiController StoreHouseUiController;
    }
}