using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class AdmiralViewModel : ViewModel
    {
        #region Model 変更通知プロパティ

		public Admiral Model => KanColleClient.Current.Homeport.Admiral;

        #endregion

        public bool IsShipCountAboveWarningLine
        {
            get
            {
                if (Model == null) return false;
                return Application.Instance.MainWindowViewModel.Ships.Count >= Model.MaxShipCount - 10;
            }
        }

        public bool IsShipCountAboveHighWaterLine
        {
            get
            {
                if (Model == null) return false;
                return Application.Instance.MainWindowViewModel.Ships.Count >= Model.MaxShipCount - 5;
            }
        }

        public bool IsItemCountAboveWarningLine
        {
            get
            {
                if (Model == null) return false;
                return Application.Instance.MainWindowViewModel.SlotItems.Count >= Model.MaxSlotItemCount - 40;
            }
        }

        public bool IsItemCountAboveHighWaterLine
        {
            get
            {
                if (Model == null) return false;
                return Application.Instance.MainWindowViewModel.SlotItems.Count >= Model.MaxSlotItemCount - 20;
            }
        }

        public AdmiralViewModel(MainWindowViewModel vmRoot)
        {
            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
            {
				{ nameof(Homeport.Admiral), (sender, args) => this.Update() },
            });
            this.CompositeDisposable.Add(new PropertyChangedEventListener(vmRoot.SlotItems)
            {
                { "Count", (sender, args) => this.Update() },
            });
            this.CompositeDisposable.Add(new PropertyChangedEventListener(vmRoot.Ships)
            {
                { "Count", (sender, args) => this.Update() },
            });
        }

        private void Update()
        {
            this.RaisePropertyChanged(nameof(Model));
            this.RaisePropertyChanged(nameof(IsShipCountAboveWarningLine));
            this.RaisePropertyChanged(nameof(IsShipCountAboveHighWaterLine));
            this.RaisePropertyChanged(nameof(IsItemCountAboveWarningLine));
            this.RaisePropertyChanged(nameof(IsItemCountAboveHighWaterLine));
        }
    }
}
