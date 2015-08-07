using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LynLogger.Views.Contents
{
    /// <summary>
    /// BattleProcess.xaml 的交互逻辑
    /// </summary>
    public partial class BattleProcessPresenter : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty dpBattle = DependencyProperty.Register(nameof(Battle), typeof(BattleProcess), typeof(BattleProcessPresenter), new PropertyMetadata(
            (o, e) => {
                var handler = ((BattleProcessPresenter)o).PropertyChanged;
                if(handler != null) handler(o, new PropertyChangedEventArgs(e.Property.Name));
            }
        ));

        public event PropertyChangedEventHandler PropertyChanged;

        public BattleProcess Battle
        {
            get { return (BattleProcess)GetValue(dpBattle); }
            set { SetValue(dpBattle, value); }
        }

        public BattleProcessPresenter()
        {
            InitializeComponent();
        }
    }
}
