using LynLogger.DataStore.LogBook;
using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views.BattleLog
{
    class SortieHistoryModel : NotificationSourceObject
    {
        private SortieInfo _sortie;
        public SortieInfo Sortie
        {
            get { return _sortie; }
            set { _sortie = value; Node = value?.Nodes.First(); RaisePropertyChanged(); }
        }

        public KeyValuePair<long, SortieInfo> KvSortie { set { Sortie = value.Value; } }

        private SortieInfo.Node _node;
        public SortieInfo.Node Node
        {
            get { return _node; }
            set { _node = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(HasBattle)); }
        }

        public bool HasBattle => _node?.Battle != null;
    }
}
