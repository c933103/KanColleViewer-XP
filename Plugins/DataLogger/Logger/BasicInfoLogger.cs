using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Logger
{
    public class BasicInfoLogger
    {
        public BasicInfoLogger()
        {
            Models.DataStore.OnDataStoreCreate += (_, ds) => {
                ds.BasicInfoChanged += () => ProcBasicInfoChanged(ds);
            };
        }

        private void ProcBasicInfoChanged(Models.DataStore ds)
        {
            ds.BasicInfoHistory.Level.Append(ds.BasicInfo.Level, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Experience.Append(ds.BasicInfo.Experience, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.FurnitureCoin.Append(ds.BasicInfo.FurnitureCoin, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Fuel.Append(ds.BasicInfo.Fuel, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Ammo.Append(ds.BasicInfo.Ammo, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Steel.Append(ds.BasicInfo.Steel, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Bauxite.Append(ds.BasicInfo.Bauxite, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.HsBuild.Append(ds.BasicInfo.HsBuild, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.HsRepair.Append(ds.BasicInfo.HsRepair, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.DevMaterial.Append(ds.BasicInfo.DevMaterial, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.ModMaterial.Append(ds.BasicInfo.ModMaterial, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.ExerWins.Append(ds.BasicInfo.ExerWins, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.ExerLose.Append(ds.BasicInfo.ExerLose, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.OperWins.Append(ds.BasicInfo.OperWins, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.OperLose.Append(ds.BasicInfo.OperLose, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.ExpeWins.Append(ds.BasicInfo.ExpeWins, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.ExpeLose.Append(ds.BasicInfo.ExpeLose, ds.Settings.BasicInfoLoggingInterval);
            ds.BasicInfoHistory.Score.Append(ds.BasicInfo.Score, ds.Settings.BasicInfoLoggingInterval);
        }
    }
}
