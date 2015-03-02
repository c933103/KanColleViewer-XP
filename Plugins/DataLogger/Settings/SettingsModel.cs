using LynLogger.Models;
using LynLogger.Models.Scavenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LynLogger.Settings
{
    public class SettingsModel : NotificationSourceObject
    {
        private CleanupDataByCountCommand cleanCountCmd;
        public ICommand CleanupByCount { get { return cleanCountCmd ?? (cleanCountCmd = new CleanupDataByCountCommand()); } }

        private CleanupDataByTimeCommand cleanTimeCmd;
        public ICommand CleanupByTime { get { return cleanTimeCmd ?? (cleanTimeCmd = new CleanupDataByTimeCommand()); } }

        private CleanupDataRemoveNonExistenceCommand cleanDeadCmd;
        public ICommand CleanupDead { get { return cleanDeadCmd ?? (cleanDeadCmd = new CleanupDataRemoveNonExistenceCommand()); } }

        private SaveDataCommand saveDataCmd;
        public ICommand SaveData { get { return saveDataCmd ?? (saveDataCmd = new SaveDataCommand()); } }

        private BrowseFileCommand browseFileCmd;
        public ICommand BrowseFile { get { return browseFileCmd ?? (browseFileCmd = new BrowseFileCommand()); } }

        private MergeDataCommand mergeDataCmd;
        public ICommand MergeData { get { return mergeDataCmd ?? (mergeDataCmd = new MergeDataCommand()); } }

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

        private int _cleanupKeepDatapointCount = 2000;
        public int CleanupKeepDatapointCount {
            get { return _cleanupKeepDatapointCount; }
            set
            {
                if(value == _cleanupKeepDatapointCount) return;
                _cleanupKeepDatapointCount = value;
                RaisePropertyChanged();
            }
        }

        private int _cleanupKeepDatapointDays = 90;
        public int CleanupKeepDatapointDays
        {
            get { return _cleanupKeepDatapointDays; }
            set
            {
                if(value == _cleanupKeepDatapointDays) return;
                _cleanupKeepDatapointDays = value;
                RaisePropertyChanged();
            }
        }

        private string _mergeFileName = "";
        public string MergeFileName
        {
            get { return _mergeFileName; }
            set
            {
                if(value == _mergeFileName) return;
                _mergeFileName = value;
                RaisePropertyChanged();
            }
        }

        public SettingsModel()
        {
            DataStore.OnDataStoreSwitch += (_, __) => {
                RaiseMultiPropertyChanged();
            };
        }

        public class CleanupDataByCountCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Instance.Cleanup(new Scavenger((int)parameter));
            }

            [ScavengeTargetType(typeof(long), typeof(int))]
            [ScavengeTargetType(typeof(long), typeof(double))]
            [ScavengeTargetType(typeof(long), typeof(string))]
            private class Scavenger : IRampUpScavenger
            {
                private LinkedList<long> seenKeys;
                private long seenObjectCount;
                private long thredsholdAmount;
                private bool keep;

                public Scavenger(int keepAfter)
                {
                    thredsholdAmount = keepAfter;
                }

                public void Reset() { seenKeys = new LinkedList<long>(); seenObjectCount = 0; keep = false; }

                public void RampUp(object key, object value)
                {
                    if(!(key is long)) return;
                    seenKeys.AddLast((long)key);
                    if(++seenObjectCount > thredsholdAmount) {
                        seenKeys.RemoveFirst();
                    }
                }

                public bool ShouldKeep(object key, object value)
                {
                    if(!(key is long)) return true;
                    if((long)key == seenKeys.First.Value) {
                        keep = true;
                    }
                    return keep;
                }
            }
        }

        public class CleanupDataByTimeCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Instance.Cleanup(new Scavenger(Helpers.UnixTimestamp - ((int)parameter) * 86400L));
            }

            [ScavengeTargetType(typeof(long), typeof(int))]
            [ScavengeTargetType(typeof(long), typeof(double))]
            [ScavengeTargetType(typeof(long), typeof(string))]
            private class Scavenger : IScavenger
            {
                private long thredsholdTs;

                public Scavenger(long keepAfter)
                {
                    thredsholdTs = keepAfter;
                }

                public void Reset() { }

                public bool ShouldKeep(object key, object value)
                {
                    if(!(key is long)) return true;
                    return (long)key >= thredsholdTs;
                }
            }
        }

        public class CleanupDataRemoveNonExistenceCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Instance.Cleanup(new Scavenger());
            }

            [ScavengeTargetType(typeof(int), typeof(Models.ShipHistory))]
            private class Scavenger : IScavenger
            {
                public void Reset() { }

                public bool ShouldKeep(object key, object value)
                {
                    var val = value as Models.ShipHistory;
                    if(val == null) return true;

                    return !(val.EnhancedAntiAir.Count == 1 && val.EnhancedDefense.Count == 1 && val.EnhancedLuck.Count == 1
                        && val.EnhancedPower.Count == 1 && val.EnhancedTorpedo.Count == 1 && val.Exp.Count == 1
                        && val.Level.Count == 1 && val.ShipId.Count == 1 && val.SRate.Count == 1
                        && val.ExistenceLog.Last().Value == ShipExistenceStatus.NonExistence);
                }
            }
        }

        public class SaveDataCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.SaveData();
            }
        }

        public class BrowseFileCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;
                ofd.ShowReadOnly = false;
                ofd.ValidateNames = true;
                if(ofd.ShowDialog() != true) return;

                ((SettingsModel)parameter).MergeFileName = ofd.FileName;
            }
        }

        public class MergeDataCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                string fn = parameter as string;
                if(string.IsNullOrWhiteSpace(fn)) return;
                if(!System.IO.File.Exists(fn)) return;
                try {
                    using(System.IO.Stream input = System.IO.File.OpenRead(fn)) {
                        DataStore.Instance.Merge(LynLogger.Models.Migrations.DataStoreLoader.LoadFromStream(input));
                    }
                } catch(Exception e) {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }
}
