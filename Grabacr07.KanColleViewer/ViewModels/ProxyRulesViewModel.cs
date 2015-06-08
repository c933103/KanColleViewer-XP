using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Grabacr07.KanColleViewer.ViewModels
{
    public class ProxyRulesViewModel : ViewModelEx
    {
        public IEnumerable<KeyValuePair<int, ProxyRule>> Rules => Settings.Current.ProxySettings.Rules.OrderBy(x => x.Key);
        
        public KeyValuePair<int, ProxyRule> SelectedRule
        {
            set
            {
                RuleId = value.Key;
                RuleName = value.Value.Name;
                RuleEnabled = value.Value.Enabled;
                RuleMatchItem = value.Value.MatchIn;
                RuleNegateMatch = value.Value.Negative;
                RuleMatchType = value.Value.Type;
                RuleMatchPattern = value.Value.Pattern;
                RuleAction = value.Value.Action;
                RuleActionString = value.Value.ActionString;
            }
        }

        public ProxyRulesViewModel()
        {
            if (Helper.IsInDesignMode) return;
            Settings.Current.ProxySettings.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Settings.Current.ProxySettings.Rules)) RaisePropertyChanged(nameof(Rules));
            };
        }

        private int _ruleId;
        private string _ruleName;
        private bool _ruleEnabled;
        private ProxyRule.MatchPortion _ruleMatchItem;
        private bool _ruleNegateMatch;
        private ProxyRule.MatchType _ruleMatchType;
        private string _ruleMatchPattern;
        private ProxyRule.MatchAction _ruleAction;
        private string _ruleActionString;

        public int RuleId
        {
            get { return _ruleId; }
            set { _ruleId = value; RaisePropertyChanged(); }
        }

        public string RuleName
        {
            get { return _ruleName; }
            set { _ruleName = value; RaisePropertyChanged(); }
        }

        public bool RuleEnabled
        {
            get { return _ruleEnabled; }
            set { _ruleEnabled = value; RaisePropertyChanged(); }
        }

        public ProxyRule.MatchPortion RuleMatchItem
        {
            get { return _ruleMatchItem; }
            set { _ruleMatchItem = value; RaisePropertyChanged(); }
        }

        public bool RuleNegateMatch
        {
            get { return _ruleNegateMatch; }
            set { _ruleNegateMatch = value; RaisePropertyChanged(); }
        }

        public ProxyRule.MatchType RuleMatchType
        {
            get { return _ruleMatchType; }
            set { _ruleMatchType = value; RaisePropertyChanged(); }
        }

        public string RuleMatchPattern
        {
            get { return _ruleMatchPattern; }
            set { _ruleMatchPattern = value; RaisePropertyChanged(); }
        }

        public ProxyRule.MatchAction RuleAction
        {
            get { return _ruleAction; }
            set { _ruleAction = value; RaisePropertyChanged(); }
        }

        public string RuleActionString
        {
            get { return _ruleActionString; }
            set { _ruleActionString = value; RaisePropertyChanged(); }
        }

        public Array MatchPositions => Enum.GetValues(typeof(ProxyRule.MatchPortion));
        public Array MatchTypes => Enum.GetValues(typeof(ProxyRule.MatchType));
        public Array Actions => Enum.GetValues(typeof(ProxyRule.MatchAction));
        public ICommand InsertRuleCommand => new CmdInsertRule(this);
        public ICommand DeleteRuleCommand => new CmdDeleteRule(this);
        public ICommand ExportRuleCommand => new CmdExportRule(this);
        public ICommand ImportRuleCommand => new CmdImportRule(this);

        class CmdInsertRule : ICommand
        {
            private readonly ProxyRulesViewModel vm;

            public CmdInsertRule(ProxyRulesViewModel vm)
            {
                this.vm = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Settings.Current.ProxySettings.Rules[vm.RuleId] = new ProxyRule() {
                    Enabled = vm.RuleEnabled,
                    Action = vm.RuleAction,
                    ActionString = vm.RuleActionString,
                    MatchIn = vm.RuleMatchItem,
                    Name = vm.RuleName,
                    Negative = vm.RuleNegateMatch,
                    Pattern = vm.RuleMatchPattern,
                    Type = vm.RuleMatchType
                };
                ((IProxySettings)Settings.Current.ProxySettings).CompiledRules = null;
                vm.RaisePropertyChanged(nameof(vm.Rules));
            }
        }

        class CmdDeleteRule : ICommand
        {
            private readonly ProxyRulesViewModel vm;

            public CmdDeleteRule(ProxyRulesViewModel vm)
            {
                this.vm = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Settings.Current.ProxySettings.Rules.Remove(vm.RuleId);
                ((IProxySettings)Settings.Current.ProxySettings).CompiledRules = null;
                vm.RaisePropertyChanged(nameof(vm.Rules));
            }
        }

        class CmdExportRule : ICommand
        {
            private readonly ProxyRulesViewModel vm;

            public CmdExportRule(ProxyRulesViewModel vm)
            {
                this.vm = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.OverwritePrompt = true;
                sfd.CheckPathExists = true;
                sfd.ValidateNames = true;
                if (sfd.ShowDialog() != true) return;

                using (System.IO.Stream s = sfd.OpenFile()) {
                    var list = vm.Rules.Where(x => x.Key < int.MaxValue - 1).Select(x => new Models.Data.Xml.XmlSerializableDictionary<int, ProxyRule>.LocalKeyValuePair(x.Key, x.Value)).ToList();
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(list.GetType());
                    xs.Serialize(s, list);
                }
            }
        }

        class CmdImportRule : ICommand
        {
            private readonly ProxyRulesViewModel vm;

            public CmdImportRule(ProxyRulesViewModel vm)
            {
                this.vm = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.ValidateNames = true;
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != true) return;

                using (System.IO.Stream s = ofd.OpenFile()) {
                    List<Models.Data.Xml.XmlSerializableDictionary<int, ProxyRule>.LocalKeyValuePair> list = new List<Models.Data.Xml.XmlSerializableDictionary<int, ProxyRule>.LocalKeyValuePair>();
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(list.GetType());
                    list = (List<Models.Data.Xml.XmlSerializableDictionary<int, ProxyRule>.LocalKeyValuePair>)xs.Deserialize(s);

                    var rules = Settings.Current.ProxySettings.Rules;
                    list.ForEach(kv => rules[kv.Key] = kv.Value);

                    ((IProxySettings)Settings.Current.ProxySettings).CompiledRules = null;
                    vm.RaisePropertyChanged(nameof(vm.Rules));
                }
            }
        }
    }
}
