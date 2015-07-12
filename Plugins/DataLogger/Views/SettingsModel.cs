using Grabacr07.KanColleViewer.Models.Data.Xml;
using LynLogger.DataStore.Extensions;
using LynLogger.Models;
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

        private ExportEnemyInfoCommand expEnemyInfoCmd;
        public ICommand ExportEnemyInfo => expEnemyInfoCmd ?? (expEnemyInfoCmd = new ExportEnemyInfoCommand());

        private ImportEnemyInfoCommand impEnemyInfoCmd;
        public ICommand ImportEnemyInfo => impEnemyInfoCmd ?? (impEnemyInfoCmd = new ImportEnemyInfoCommand());

        private SwitchMemberCommand switchMemberCmd;
        public ICommand SwitchMember => switchMemberCmd ?? (switchMemberCmd = new SwitchMemberCommand(this));

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

        private string _switchMemberId;
        public string SwitchMemberId
        {
            get { return _switchMemberId; }
            set
            {
                if (value == _switchMemberId) return;
                _switchMemberId = value;
                RaisePropertyChanged();
            }
        }

        public SettingsModel()
        {
            DataStore.Store.OnDataStoreSwitch += ds => {
                _switchMemberId = ds.MemberId;
                RaiseMultiPropertyChanged();
            };
        }

        public DataStore.Store CurrentActiveDs
        {
            get { return DataStore.Store.Current; }
        }

#pragma warning disable 0067
        private class SaveDataCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                DataStore.Store.Current?.SaveData();
            }
        }

        private class ExportEnemyInfoCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                var info = DataStore.Store.Current?.EnemyInfo;
                if (info == null) return;

                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.OverwritePrompt = true;
                sfd.CheckPathExists = true;
                sfd.ValidateNames = true;
                if (sfd.ShowDialog() != true) return;

                var dummy = new LinkedList<object>();
                using (System.IO.Stream s = sfd.OpenFile())
                using (DataStore.IO.DSWriter w = new DataStore.IO.DSWriter(s))
                    info.GetSerializationInfo(dummy).Serialize(w);
            }
        }

        private class ImportEnemyInfoCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                var info = DataStore.Store.Current?.EnemyInfo;
                if (info == null) return;

                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;
                ofd.ValidateNames = true;
                if (ofd.ShowDialog() != true) return;

                var dummy = new LinkedList<object>();
                using (System.IO.Stream s = ofd.OpenFile()) {
                    using (DataStore.IO.DSReader r = new DataStore.IO.DSReader(s)) {
                        var dict = (DataStore.Premitives.Dictionary<DataStore.Premitives.StoragePremitive, DataStore.Premitives.StoragePremitive>)DataStore.Premitives.StoragePremitive.Parse(r);
                        foreach (var kv in dict.Convert((k, v) => new { Key = new MapLocInfo(k, dummy), Value = new BattleInfo(v, dummy) })) {
                            info[kv.Key] = kv.Value;
                        }
                    }
                }
            }
        }

        private class SwitchMemberCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            private readonly SettingsModel vm;
            public SwitchMemberCommand(SettingsModel vm) { this.vm = vm; }

            public void Execute(object parameter)
            {
                DataStore.Store.SwitchMember(vm.SwitchMemberId);
            }
        }
    }
}
