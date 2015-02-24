using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Settings
{
    public class SettingsModel : NotificationSourceObject
    {
        public int BasicInfoLoggingInterval
        {
            get { return DataStore.Instance == null ? 0 : DataStore.Instance.Settings.BasicInfoLoggingInterval; }
            set
            {
                if(DataStore.Instance == null) return;
                if(DataStore.Instance.Settings.BasicInfoLoggingInterval == value) return;
                DataStore.Instance.Settings.BasicInfoLoggingInterval = value;
                RaisePropertyChanged();
            }
        }

        public int ShipDataLoggingInterval
        {
            get { return DataStore.Instance == null ? 0 : DataStore.Instance.Settings.ShipDataLoggingInterval; }
            set
            {
                if(DataStore.Instance == null) return;
                if(DataStore.Instance.Settings.ShipDataLoggingInterval == value) return;
                DataStore.Instance.Settings.ShipDataLoggingInterval = value;
                RaisePropertyChanged();
            }
        }

        public SettingsModel()
        {
            DataStore.OnDataStoreSwitch += (_, __) => {
                RaiseMultiPropertyChanged();
            };
        }
    }
}
