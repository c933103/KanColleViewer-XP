﻿using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace LynLogger.Views
{
    public class SettingsModel : NotificationSourceObject
    {
        private SaveDataCommand saveDataCmd;
        public ICommand SaveData => saveDataCmd ?? (saveDataCmd = new SaveDataCommand());

        public int BasicInfoLoggingInterval
        {
            get { return CurrentActiveDs?.Settings.BasicInfoLoggingInterval ?? 0; }
            set
            {
                if(CurrentActiveDs == null) return;
                if(CurrentActiveDs.Settings.BasicInfoLoggingInterval == value) return;
                CurrentActiveDs.Settings.BasicInfoLoggingInterval = value;
                RaisePropertyChanged();
            }
        }

        public int ShipDataLoggingInterval
        {
            get { return CurrentActiveDs?.Settings.ShipDataLoggingInterval ?? 0; }
            set
            {
                if(CurrentActiveDs == null) return;
                if(CurrentActiveDs.Settings.ShipDataLoggingInterval == value) return;
                CurrentActiveDs.Settings.ShipDataLoggingInterval = value;
                RaisePropertyChanged();
            }
        }

        /*private int _cleanupKeepDatapointCount = 2000;
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
        }*/

        public SettingsModel()
        {
            DataStore.Store.OnDataStoreSwitch += (_, ds) => RaiseMultiPropertyChanged();
        }

        public DataStore.Store CurrentActiveDs
        {
            get { return DataStore.Store.Current; }
        }

#pragma warning disable 0067
/*        public class CleanupDataByCountCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Store.Current.Cleanup(new Scavenger((int)parameter));
                DataStore.Refresh();
            }

            [ScavengeTargetType(typeof(long), typeof(int))]
            [ScavengeTargetType(typeof(long), typeof(double))]
            [ScavengeTargetType(typeof(long), typeof(ShipNameType))]
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
                DataStore.Store.Current.Cleanup(new Scavenger(Helpers.UnixTimestamp - ((int)parameter) * 86400L));
                DataStore.Refresh();
            }

            [ScavengeTargetType(typeof(long), typeof(int))]
            [ScavengeTargetType(typeof(long), typeof(double))]
            [ScavengeTargetType(typeof(long), typeof(ShipNameType))]
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
                DataStore.Store.Current.Cleanup(new Scavenger());
                DataStore.Refresh();
            }

            [ScavengeTargetType(typeof(int), typeof(ShipHistory))]
            private class Scavenger : IScavenger
            {
                public void Reset() { }

                public bool ShouldKeep(object key, object value)
                {
                    var val = value as Models.ShipHistory;
                    if(val == null) return true;

                    return !(val.EnhancedAntiAir.Count == 1 && val.EnhancedDefense.Count == 1 && val.EnhancedLuck.Count == 1
                        && val.EnhancedPower.Count == 1 && val.EnhancedTorpedo.Count == 1 && val.Exp.Count == 1
                        && val.Level.Count == 1 && val.ShipNameType.Count == 1 && val.SRate.Count == 1
                        && val.ExistenceLog.Last().Value == ShipExistenceStatus.NonExistence);
                }
            }
        }*/

        public class SaveDataCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Store.Current?.SaveData();
            }
        }

        /*public class BrowseFileCommand : ICommand
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
        }*/
    }
}