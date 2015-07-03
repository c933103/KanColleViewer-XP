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
    class UnsignedInteger : StoragePremitive
    {
        private ulong value;

        public override IEnumerable<TypeIdentifier> Type => Collections.AsEnumerable(TypeIdentifier.UInt);
        public ulong Value => value;

        public UnsignedInteger() { }
        public UnsignedInteger(ulong value) { this.value = value; }
        public UnsignedInteger(DSReader input) { value = input.Read7bUInt(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt(value);
        }
    }
}
