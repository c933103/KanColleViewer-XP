using LynLogger.DataStore.LogBook;
using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views.BattleLog
{
    class DrillHistoryModel : NotificationSourceObject
    {
        private DrillInfo _drill;
        public DrillInfo Drill
        {
            get { return _drill; }
            set { _drill = value;RaisePropertyChanged(); }
        }

        public KeyValuePair<long, DrillInfo> KvDrill { set { Drill = value.Value; } }
    }
}
