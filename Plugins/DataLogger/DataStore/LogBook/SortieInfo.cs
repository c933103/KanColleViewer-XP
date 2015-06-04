using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.Serialization;
using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.LogBook
{
    public class SortieInfo : AbstractDSSerializable<SortieInfo>
    {
        [Serialize(0)] public string MapId { get; set; }
        /*[Serialize(1)]*/ public Node[] Nodes { get; set; }
        
        public SortieInfo() { }
        public SortieInfo(Premitives.StoragePremitive _info, LinkedList<object> _path) : base(_info, _path) { }

        public class Node : AbstractDSSerializable<Node>
        {
            [Serialize(0)] public MapNext Route { get; set; }
            [Serialize(1)] public BattleProcess Battle { get; set; }
            [Serialize(2)] public BattleResult Result { get; set; }
            public Node() { }
            public Node(Premitives.StoragePremitive _info, LinkedList<object> _path) : base(_info, _path) { }
        }

        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = new HandlerInfo(
                        (x, p) => x.Nodes.GetSerializationInfo(p),
                        (o, i, p) => o.Nodes = ((Premitives.List<Premitives.StoragePremitive>)i).Convert(x => new Node(x, p)).ToArray()),
                };
            }
        }
    }
}
