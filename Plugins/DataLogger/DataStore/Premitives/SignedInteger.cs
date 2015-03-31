using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class SignedInteger : StoragePremitive
    {
        private long value;

        public override IEnumerable<TypeIdentifier> Type { get { return Collections.AsEnumerable(TypeIdentifier.Int); } }
        public long Value { get { return value; } }

        public SignedInteger() { }
        public SignedInteger(long value) { this.value = value; }
        public SignedInteger(DSReader input) { value = input.Read7bInt(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bInt(value);
        }
    }
}
