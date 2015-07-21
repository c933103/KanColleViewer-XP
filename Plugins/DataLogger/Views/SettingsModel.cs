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
        
        private class SaveDataCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged { add { } remove { } }

            public void Execute(object parameter)
            {
                DataStore.Store.Current?.SaveData();
            }
        }

        private class SwitchMemberCommand : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged { add { } remove { } }

            private readonly SettingsModel vm;
            public SwitchMemberCommand(SettingsModel vm) { this.vm = vm; }

            public void Execute(object parameter)
            {
                DataStore.Store.SwitchMember(vm.SwitchMemberId);
            }
        }
    }
}
