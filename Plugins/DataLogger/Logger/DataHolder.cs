using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DataLogger.Logger
{
    class DataHolder
    {
        public static DataHolder Instance { get; private set; }

        public static void Init()
        {
            Instance = new DataHolder();
        }

        public event Action<int> ShipDataChanged;
        public IReadOnlyDictionary<int, Models.Ship> Ships { get; private set; }
        internal IDictionary<int, Models.Ship> i_Ships { get; private set; }
        internal void RaiseShipDataChange(int id) { ShipDataChanged(id); }

        public event Action ResourceChanged;
        private int _fuel;
        private int _ammo;
        private int _steel;
        private int _bauxite;
        private int _repair;
        private int _build;
        private int _dev;
        private int _mod;

        public int Fuel { 
            get { return _fuel; }
            internal set
            {
                if(_fuel == value) return;
                _fuel = value;
                ResourceChanged();
            }
        }

        public int Ammo
        {
            get { return _ammo; }
            internal set
            {
                if(_ammo == value) return;
                _ammo = value;
                
            }
        }

        private DataHolder()
        {
            Ships = (IReadOnlyDictionary<int, Models.Ship>)(i_Ships = new Dictionary<int, Models.Ship>());
        }
    }
}
