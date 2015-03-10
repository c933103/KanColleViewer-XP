using LynLogger.Models;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Views
{
    class GradeEvaluationModel : NotificationSourceObject
    {
        public double Score
        {
            get
            {
                if(DataStore.Instance == null) return 0;
                return DataStore.Instance.BasicInfo.Score;
            }
        }

        public string Grade
        {
            get
            {
                var s = Score;
                if(s < 5) return "很休闲";
                if(s < 12) return "正常";
                if(s < 15) return "甘地";
                return "超神";
            }
        }

        public IEnumerable<KeyValuePair<long, double>> ScoreHistogram
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.BasicInfoHistory.Score.Select(x => x);
            }
        }

        public GradeEvaluationModel()
        {
            DataStore.BasicInfoChanged += x => {
                RaiseMultiPropertyChanged();
            };
            DataStore.OnDataStoreSwitch += (_, ds) => RaiseMultiPropertyChanged();
        }
    }
}
