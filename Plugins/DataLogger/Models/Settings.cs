using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class Settings : NotificationSourceObject
    {
        private int _basicInfoLoggingInterval = 7200;
        public int BasicInfoLoggingInterval
        {
            get { return _basicInfoLoggingInterval; }
            set
            {
                if(_basicInfoLoggingInterval == value) return;
                _basicInfoLoggingInterval = value;
                RaisePropertyChanged();
            }
        }

        private int _shipDataLoggingInterval = 1800;
        public int ShipDataLoggingInterval
        {
            get { return _shipDataLoggingInterval; }
            set
            {
                if(_shipDataLoggingInterval == value) return;
                _shipDataLoggingInterval = value;
                RaisePropertyChanged();
            }
        }
    }
}
