using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LynLogger.Models.Battling
{
    class BattleResult
    {
        public IReadOnlyList<ShipInfo> EnemyShips { get; private set; }
        public Ranking Rank { get; private set; }
        public int AdmiralGetExp { get; private set; }
        public int AdmiralLv { get; private set; }
        public int AdmiralExp { get; private set; }

        public BattleResult(kcsapi_battleresult raw)
        {
            EnemyShips = raw.api_ship_id.Skip(1).TakeWhile(x => x != -1).Select(x => KanColleClient.Current.Master.Ships[x]).ToList();
            Rank = (Ranking)Enum.Parse(typeof(Ranking), raw.api_win_rank, true);
            AdmiralGetExp = raw.api_get_exp;
            AdmiralExp = raw.api_member_exp;
            AdmiralLv = raw.api_member_lv;
        }

        public class ShipStatus
        {

        }
    }
}
