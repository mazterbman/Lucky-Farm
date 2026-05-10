using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Game.Scripts.Items
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Scriptable Objects/Items/ItemType")]
    public class Item : ScriptableObject
    {
        public TypeItem Type;
        public int Coast;
        public LanguageString LocalisationItem;
        public Sprite IcoItem;

        public Item(Item item)
        {
            Type = item.Type;
            Coast = item.Coast;
            LocalisationItem = item.LocalisationItem;
            IcoItem = item.IcoItem;
        }
        
        [System.Serializable]
        public class LanguageString
        {
            public string Key;
            public LocalizationTableCollection TableCollection;
            
            public string GetName(LocaleIdentifier identifier)
            {
                if (TableCollection == null)
                    return GetFallback();
                
                var table = TableCollection.GetTable(identifier) as StringTable;
                if (table == null)
                    return GetFallback();
                
                var entry = table.GetEntry(Key);
                if (entry != null)
                {
                    string localized = entry.GetLocalizedString();
                    if (!string.IsNullOrEmpty(localized))
                        return localized;
                }

                return GetFallback();
            }
            
            private string GetFallback()
            {
                return "Error on Locale";
            }
        }
    }

    public enum TypeItem
    {
        Egg = 0,
        Omelet,
        Chicken,
    }
}
