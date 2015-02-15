using Grabacr07.KanColleViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    class GradeEvaluationModel : TabItemViewModel
    {
        public override string Name { get { return "肝度"; } protected set { throw new NotImplementedException(); } }

        public double Score
        {
            get
            {
                var info = DataStore.Instance.BasicInfo;
                if(info.Level > 99) {
                    return (1.0 * info.OperWins + info.OperLose + (info.ExpeLose + info.ExpeWins) / 4.0) / (info.Level / 100.0 * (info.ExerLose + info.ExerWins));
                } else {
                    return (1.0 * info.OperWins + info.OperLose + (info.ExpeLose + info.ExpeWins) / 4.0) / (Math.Sqrt(info.Level) / 10 * (info.ExerLose + info.ExerWins));
                }
            }
            set { }
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
            set { }
        }

        public DataStore CurrentActiveDs
        {
            get { return DataStore.Instance; }
        }

        public GradeEvaluationModel()
        {
            DataStore.OnDataStoreCreate += (_, ds) => {
                ds.BasicInfoChanged += () => {
                    RaisePropertyChanged("Score");
                    RaisePropertyChanged("Grade");
                };
            };
        }
    }
}
