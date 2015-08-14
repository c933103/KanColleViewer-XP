using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.Utilities;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class UnsignedInteger : StoragePremitive
    {
        private ulong value;

        public override IEnumerable<TypeIdentifier> Type => EnumerablesEx.AsEnumerable(TypeIdentifier.UInt);
        public ulong Value => value;

        public UnsignedInteger() { }
        public UnsignedInteger(ulong value) { this.value = value; }
        public UnsignedInteger(DSReader input) { value = input.Read7bUInt(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt(value);
        }

        public static implicit operator ulong (UnsignedInteger i) { return (i?.Value ?? 0); }
        public static explicit operator uint (UnsignedInteger i) { return (uint)(i?.Value ?? 0); }
        public static explicit operator ushort (UnsignedInteger i) { return (ushort)(i?.Value ?? 0); }
        public static explicit operator byte (UnsignedInteger i) { return (byte)(i?.Value ?? 0); }

        public static implicit operator UnsignedInteger(byte i) { return new UnsignedInteger(i); }
        public static implicit operator UnsignedInteger(ushort i) { return new UnsignedInteger(i); }
        public static implicit operator UnsignedInteger(uint i) { return new UnsignedInteger(i); }
        public static implicit operator UnsignedInteger(ulong i) { return new UnsignedInteger(i); }
    }
}
