using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Battling
{
    public class MapNext
    {
        internal string ZwRawData;
        public string RawData { get { return ZwRawData; } }

        internal int ZwMapAreaId;
        public int MapAreaId { get { return ZwMapAreaId; } }

        internal int ZwMapSectionId;
        public int MapSectionId { get { return ZwMapSectionId; } }

        internal int ZwMapLocId;
        public int MapLocId { get { return ZwMapLocId; } }

        internal int ZwMapBossLocId;

        internal EventType ZwEvent;
        public EventType Event { get { return ZwEvent; } }

        internal ItemGetLostInfo ZwItemGetLost;
        public ItemGetLostInfo ItemGetLost { get { return ZwItemGetLost; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}-{1}-{2}", MapAreaId, MapSectionId, (char)(MapLocId - 1 + 'A')));
            switch(Event) {
                case EventType.Battle:
                    sb.Append("来！战个痛！快！");
                    break;
                case EventType.BossBattle:
                    sb.Append("BOSS战突入！");
                    break;
                case EventType.ItemGet:
                    sb.AppendFormat("获得 {0} x{1}", ItemGetLost.ItemName, ItemGetLost.Amount);
                    break;
                case EventType.ItemLost:
                    sb.AppendFormat("损失 {0} x{1}", ItemGetLost.ItemName, ItemGetLost.Amount);
                    break;
                case EventType.Nothing:
                    sb.Append("来！战个……没事");
                    break;
                default:
                    sb.Append(RawData);
                    break;
            }
            return sb.ToString();
        }

        public class ItemGetLostInfo
        {
            internal int ZwItemId;
            public int ItemId { get { return ZwItemId; } }

            public int ZwAmount;
            public int Amount { get { return ZwAmount; } }

            internal string ZwItemName;
            public string ItemName { get { return ZwItemName; } }
        }
        public enum EventType { ItemGet = 2, ItemLost = 3, Battle = 4, BossBattle = 5, Nothing = 6 }
    }
}
