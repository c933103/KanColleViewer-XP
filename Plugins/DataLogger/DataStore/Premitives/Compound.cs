using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;
using LynLogger.DataStore.Extensions;
using System.Collections;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    public class Compound : StoragePremitive, IEnumerable<KeyValuePair<ulong, StoragePremitive>>
    {
        public StoragePremitive this[ulong id]
        {
            get { return fields[id]; }
            set { fields[id] = value; }
        }

        private IDictionary<ulong, StoragePremitive> fields = new System.Collections.Generic.Dictionary<ulong, StoragePremitive>();

        public override IEnumerable<TypeIdentifier> Type { get { return Collections.AsEnumerable(TypeIdentifier.Compound); } }

        public Compound() { }
        public Compound(DSReader input)
        {
            var fcount = input.Read7bUInt();
            for(uint i = 0; i < fcount; i++) {
                var id = input.Read7bUInt();
                var value = Parse(input);
                fields[id] = value;
            }
        }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt((uint)fields.Count);
            foreach(var flist in fields) {
                output.Write7bUInt(flist.Key);
                flist.Value.Serialize(output);
            }
        }

        public IEnumerator<KeyValuePair<ulong, StoragePremitive>> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }
    }
}
