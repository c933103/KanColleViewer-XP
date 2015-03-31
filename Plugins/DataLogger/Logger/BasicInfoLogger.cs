using LynLogger.DataStore;
namespace LynLogger.Logger
{
    public class BasicInfoLogger
    {
        public BasicInfoLogger()
        {
            Store.OnDataStoreCreate += (sid, store) => store.OnBasicInfoChange += sender => ProcBasicInfoChanged(sender);
        }

        private static void ProcBasicInfoChanged(Store ds)
        {
            var procs = new DataStore.LogBook.BasicInfo[] { ds.Weekbook.BasicInfo, ds.CurrentLogbook.BasicInfo };
            foreach(var hist in procs) {
                hist.Level.Append(ds.BasicInfo.Level, ds.Settings.BasicInfoLoggingInterval);
                hist.Experience.Append(ds.BasicInfo.Experience, ds.Settings.BasicInfoLoggingInterval);
                hist.FurnitureCoin.Append(ds.BasicInfo.FurnitureCoin, ds.Settings.BasicInfoLoggingInterval);
                hist.Fuel.Append(ds.BasicInfo.Fuel, ds.Settings.BasicInfoLoggingInterval);
                hist.Ammo.Append(ds.BasicInfo.Ammo, ds.Settings.BasicInfoLoggingInterval);
                hist.Steel.Append(ds.BasicInfo.Steel, ds.Settings.BasicInfoLoggingInterval);
                hist.Bauxite.Append(ds.BasicInfo.Bauxite, ds.Settings.BasicInfoLoggingInterval);
                hist.HsBuild.Append(ds.BasicInfo.HsBuild, ds.Settings.BasicInfoLoggingInterval);
                hist.HsRepair.Append(ds.BasicInfo.HsRepair, ds.Settings.BasicInfoLoggingInterval);
                hist.DevMaterial.Append(ds.BasicInfo.DevMaterial, ds.Settings.BasicInfoLoggingInterval);
                hist.ModMaterial.Append(ds.BasicInfo.ModMaterial, ds.Settings.BasicInfoLoggingInterval);
                hist.ExerWins.Append(ds.BasicInfo.ExerWins, ds.Settings.BasicInfoLoggingInterval);
                hist.ExerLose.Append(ds.BasicInfo.ExerLose, ds.Settings.BasicInfoLoggingInterval);
                hist.OperWins.Append(ds.BasicInfo.OperWins, ds.Settings.BasicInfoLoggingInterval);
                hist.OperLose.Append(ds.BasicInfo.OperLose, ds.Settings.BasicInfoLoggingInterval);
                hist.ExpeWins.Append(ds.BasicInfo.ExpeWins, ds.Settings.BasicInfoLoggingInterval);
                hist.ExpeLose.Append(ds.BasicInfo.ExpeLose, ds.Settings.BasicInfoLoggingInterval);
            }
        }
    }
}
