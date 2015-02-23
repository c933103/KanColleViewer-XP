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
            DataStore.OnDataStoreCreate += (_, ds) => {
                ds.BasicInfoChanged += () => ProcBasicInfoChanged(ds);
            };
        }

        private void ProcBasicInfoChanged(DataStore ds)
        {
            ds.BasicInfoHistory.Level.Append(ds.BasicInfo.Level);
            ds.BasicInfoHistory.Experience.Append(ds.BasicInfo.Experience);
            ds.BasicInfoHistory.FurnitureCoin.Append(ds.BasicInfo.FurnitureCoin);
            ds.BasicInfoHistory.Fuel.Append(ds.BasicInfo.Fuel);
            ds.BasicInfoHistory.Ammo.Append(ds.BasicInfo.Ammo);
            ds.BasicInfoHistory.Steel.Append(ds.BasicInfo.Steel);
            ds.BasicInfoHistory.Bauxite.Append(ds.BasicInfo.Bauxite);
            ds.BasicInfoHistory.HsBuild.Append(ds.BasicInfo.HsBuild);
            ds.BasicInfoHistory.HsRepair.Append(ds.BasicInfo.HsRepair);
            ds.BasicInfoHistory.DevMaterial.Append(ds.BasicInfo.DevMaterial);
            ds.BasicInfoHistory.ModMaterial.Append(ds.BasicInfo.ModMaterial);
            ds.BasicInfoHistory.ExerWins.Append(ds.BasicInfo.ExerWins);
            ds.BasicInfoHistory.ExerLose.Append(ds.BasicInfo.ExerLose);
            ds.BasicInfoHistory.OperWins.Append(ds.BasicInfo.OperWins);
            ds.BasicInfoHistory.OperLose.Append(ds.BasicInfo.OperLose);
            ds.BasicInfoHistory.ExpeWins.Append(ds.BasicInfo.ExpeWins);
            ds.BasicInfoHistory.ExpeLose.Append(ds.BasicInfo.ExpeLose);
            ds.BasicInfoHistory.Score.Append(ds.BasicInfo.Score);
        }
    }
}
