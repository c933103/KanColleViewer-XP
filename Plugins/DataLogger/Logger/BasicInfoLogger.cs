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
            ds.BasicInfoHistory.Level[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Level;
            ds.BasicInfoHistory.Experience[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Experience;
            ds.BasicInfoHistory.FurnitureCoin[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.FurnitureCoin;
            ds.BasicInfoHistory.Fuel[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Fuel;
            ds.BasicInfoHistory.Ammo[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Ammo;
            ds.BasicInfoHistory.Steel[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Steel;
            ds.BasicInfoHistory.Bauxite[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Bauxite;
            ds.BasicInfoHistory.HsBuild[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.HsBuild;
            ds.BasicInfoHistory.HsRepair[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.HsRepair;
            ds.BasicInfoHistory.DevMaterial[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.DevMaterial;
            ds.BasicInfoHistory.ModMaterial[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.ModMaterial;
            ds.BasicInfoHistory.ExerWins[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.ExerWins;
            ds.BasicInfoHistory.ExerLose[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.ExerLose;
            ds.BasicInfoHistory.OperWins[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.OperWins;
            ds.BasicInfoHistory.OperLose[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.OperLose;
            ds.BasicInfoHistory.ExpeWins[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.ExpeWins;
            ds.BasicInfoHistory.ExpeLose[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.ExpeLose;
            ds.BasicInfoHistory.Score[Helpers.UnixTimestamp / 3600] = ds.BasicInfo.Score;
        }
    }
}
