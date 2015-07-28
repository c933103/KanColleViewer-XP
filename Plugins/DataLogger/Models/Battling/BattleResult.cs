using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Collections.Generic;
using System.Linq;
using System;
using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;

namespace LynLogger.Models.Battling
{
    public class BattleResult : AbstractDSSerializable<BattleResult>
    {
        [Serialize(0)] public Ranking Rank { get; private set; }
        [Serialize(1)] public int AdmiralExp { get; private set; }
        [Serialize(2)] public int BaseExp { get; private set; }
        [Serialize(3)] public ShipNameType DropShip { get; private set; }
        [Serialize(4)] public string FleetName { get; private set; }
        [Serialize(5)] public string MapName { get; private set; }
        [Serialize(6)] public int MvpId { get; private set; }
        
        public BattleResult(kcsapi_battleresult raw)
        {
            Rank = (Ranking)Enum.Parse(typeof(Ranking), raw.api_win_rank, true);
            AdmiralExp = raw.api_get_exp;
            BaseExp = raw.api_get_base_exp;
            MapName = raw.api_quest_name ?? "";
            FleetName = raw.api_enemy_info?.api_deck_name ?? "";
            MvpId = raw.api_mvp;
            if (raw.api_get_ship != null) {
                DropShip = new ShipNameType(raw.api_get_ship.api_ship_id);
            }
        }

        internal BattleResult(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }
    }
}
