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

        protected override IReadOnlyDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapAreaId = (int)(DataStore.Premitives.SignedInteger)i, true),
                    [2] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapSectId = (int)(DataStore.Premitives.SignedInteger)i, true),
                    [3] = new HandlerInfo(
                        (x, p) => null,
                        (o, i, p) => o.ZwMapLocation.MapLocId = (int)(DataStore.Premitives.SignedInteger)i, true),
                    [4] = HandlerInfo.NoOp,
                };
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(NodeId);
            switch(Event) {
                case EventType.Battle:
                case EventType._Battle:
                    sb.Append("来！战个痛！快！");
                    break;
                case EventType.BossBattle:
                case EventType._BossBattle:
                    sb.Append("BOSS战突入！");
                    break;
                case EventType.NightBattle:
                case EventType._NightBattle:
                    sb.Append("夜战突入！");
                    break;
                case EventType.AirBattle:
                    sb.Append("航空战准备！");
                    break;
                case EventType.ItemGet:
                case EventType._ItemGet:
                case EventType.ResourceGet:
                    sb.AppendFormat("获得 {0} x{1}", ItemGetLost.ItemName, ItemGetLost.Amount);
                    break;
                case EventType.ItemLost:
                case EventType._ItemLost:
                    sb.AppendFormat("损失 {0} x{1}", ItemGetLost.ItemName, ItemGetLost.Amount);
                    break;
                case EventType.Nothing:
                case EventType._Nothing:
                    sb.Append("来！战个……没事");
                    break;
                default:
                    sb.Append("Ev: 0x");
                    sb.AppendLine(((int)Event).ToString("X8"));
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

        public enum EventType
        {
            _ItemGet = 2, _ItemLost = 3, _Battle = 4, _BossBattle = 5, _NightBattle = -4, _Nothing = 6,

            ItemGet = 0x00020000,
            ItemLost = 0x00030000,
            Nothing = 0x00060000,
            ResourceGet = 0x00080000,

            Battle = 0x00040001,
            NightBattle = 0x00040002,
            AirBattle = 0x00040004,
            BossBattle = 0x00050001,
        }
    }
}
