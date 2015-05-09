using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.LogBook
{
    public class CreateShipLog : AbstractDSSerializable<CreateShipLog>
    {
        [Serialize(0)] public int AdmiralLv { get; private set; }
        [Serialize(1)] public ShipNameType SecretaryShip { get; private set; }
        [Serialize(2)] public int SecretaryShipLv { get; private set; }
        [Serialize(3)] public int SecretaryShipCond { get; private set; }
        [Serialize(4)] public int Fuel { get; private set; }
        [Serialize(5)] public int Ammo { get; private set; }
        [Serialize(6)] public int Steel { get; private set; }
        [Serialize(7)] public int Bauxite { get; private set; }
        [Serialize(8)] public int DevMaterial { get; private set; }
        [Serialize(9)] public int DockId { get; private set; }
        [Serialize(10)] public ShipNameType ResultShip { get; private set; }

        public CreateShipLog(int f, int a, int s, int b, int m, int d, int r)
        {
            Fuel = f; Ammo = a; Steel = s; Bauxite = b; DevMaterial = m; DockId = d; ResultShip = new ShipNameType(r);
            AdmiralLv = Grabacr07.KanColleWrapper.KanColleClient.Current.Homeport.Admiral.Level;

            var secShip = Grabacr07.KanColleWrapper.KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0];
            SecretaryShipLv = secShip.Level;
            SecretaryShipCond = secShip.Condition;
            SecretaryShip = new ShipNameType(secShip.Info.Id);
        }

        public CreateShipLog(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }
    }
}
