using LynLogger.DataStore.Serialization;
using System.Collections.Generic;
using System.Text;

namespace LynLogger.Models.Battling
{
    public class MapNext : AbstractDSSerializable<MapNext>
    {
        [Serialize(0, DepthLimit = 3)] internal string ZwRawData;
        public string RawData => ZwRawData;

        [Serialize(5)] internal EventType ZwEvent;
        public EventType Event => ZwEvent;

        [Serialize(6)] internal ItemGetLostInfo ZwItemGetLost;
        public ItemGetLostInfo ItemGetLost => ZwItemGetLost;

        [Serialize(7)] internal int ZwNextNodeCount;
        public int NextNodeCount => ZwNextNodeCount;

        [Serialize(8)] internal MapLocInfo ZwMapLocation;
        public MapLocInfo MapLocation => ZwMapLocation;

        public string NodeId => MapLocation.NodeId;

        internal MapNext() { }
        internal MapNext(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }

        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapAreaId = (int)(DataStore.Premitives.SignedInteger)i),
                    [2] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapSectId = (int)(DataStore.Premitives.SignedInteger)i),
                    [3] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapLocId = (int)(DataStore.Premitives.SignedInteger)i),
                    [4] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.EnemyId = (int)(DataStore.Premitives.SignedInteger)i),
                };
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(NodeId);
            switch(Event) {
                case EventType.Battle:
                    sb.Append("来！战个痛！快！");
                    break;
                case EventType.BossBattle:
                    sb.Append("BOSS战突入！");
                    break;
                case EventType.NightBattle:
                    sb.Append("夜战突入！");
                    break;
                /*case EventType.NightBossBattle:
                    sb.Append("夜战 BOSS！");
                    break;*/
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

        public class ItemGetLostInfo : AbstractDSSerializable<ItemGetLostInfo>
        {
            [Serialize(0)] internal int ZwItemId;
            public int ItemId => ZwItemId;

            [Serialize(1)] public int ZwAmount;
            public int Amount => ZwAmount;

            [Serialize(2)] internal string ZwItemName;
            public string ItemName => ZwItemName;

            internal ItemGetLostInfo() { }
            internal ItemGetLostInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }
        }

        public enum EventType { ItemGet = 2, ItemLost = 3, Battle = 4, BossBattle = 5, NightBattle = -4, NightBossBattle = -5, Nothing = 6 }
    }
}
