using LynLogger.Models;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Views
{
    class ResourceHistoryModel : NotificationSourceObject
    {
        public IEnumerable<KeyValuePair<long, double>> FuelHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.Fuel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> AmmoHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.Ammo.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> SteelHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.Steel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> BauxiteHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.Bauxite.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> HSRepairHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.HsRepair.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> HSBuildHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.HsBuild.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> ModMaterialHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.ModMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<long, double>> DevMaterialHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.DevMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public ResourceHistoryModel()
        {
            DataStore.OnDataStoreCreate += (_, ds) => {
                ds.BasicInfoChanged += () => {
                    RaiseMultiPropertyChanged();
                };
            };
        }
    }
}
