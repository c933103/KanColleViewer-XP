using LynLogger.DataStore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class List<T> : StoragePremitive
        where T : StoragePremitive, new()
    {
        private IList<T> data = new System.Collections.Generic.List<T>();

        public override IEnumerable<TypeIdentifier> Type { get { return Collections.AsEnumerable(TypeIdentifier.List).Concat(new T().Type); } }

        public List(IEnumerable<T> s) { foreach(var v in s) data.Add(v); }

        public List(DSReader input)
        {
            var fcount = input.Read7bUInt();
            for(uint i = 0; i < fcount; i++) {
                var v = Parse(input);
                data.Add((T)v);
            }
        }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt((uint)data.Count);
            foreach(var value in data) {
                value.Serialize(output);
            }
        }

        public IEnumerable<TOut> Convert<TOut>(Func<T, TOut> converter)
        {
            return data.Select(x => converter(x)).ToList();
        }
    }
}
