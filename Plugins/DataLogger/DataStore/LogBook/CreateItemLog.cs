using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.LogBook
{
    public class CreateItemLog : AbstractDSSerializable<CreateItemLog>
    {
        [Serialize(0)] public int AdmiralLv { get; private set; }
        [Serialize(1)] public ShipNameType SecretaryShip { get; private set; }
        [Serialize(2)] public int SecretaryShipLv { get; private set; }
        [Serialize(3)] public int SecretaryShipCond { get; private set; }
        [Serialize(4)] public int Fuel { get; private set; }
        [Serialize(5)] public int Ammo { get; private set; }
        [Serialize(6)] public int Steel { get; private set; }
        [Serialize(7)] public int Bauxite { get; private set; }
        [Serialize(8)] public EquiptInfo ResultItem { get; private set; }

        public CreateItemLog(int f, int a, int s, int b, int r, int c)
        {
            Fuel = f; Ammo = a; Steel = s; Bauxite = b; ResultItem = new EquiptInfo(r, c);
            AdmiralLv = Grabacr07.KanColleWrapper.KanColleClient.Current.Homeport.Admiral.Level;

            var secShip = Grabacr07.KanColleWrapper.KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0];
            SecretaryShipLv = secShip.Level;
            SecretaryShipCond = secShip.Condition;
            SecretaryShip = new ShipNameType(secShip.Info.Id);
        }

        public CreateItemLog(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }
    }
}
