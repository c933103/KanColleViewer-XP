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
    class SignedInteger : StoragePremitive
    {
        private long value;

        public override IEnumerable<TypeIdentifier> Type => CollectionsEx.AsEnumerable(TypeIdentifier.Int);
        public long Value => value;

        public SignedInteger() { }
        public SignedInteger(long value) { this.value = value; }
        public SignedInteger(DSReader input) { value = input.Read7bInt(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bInt(value);
        }

        public static implicit operator long (SignedInteger i) { return (i?.Value ?? 0); }
        public static explicit operator int (SignedInteger i) { return (int)(i?.Value ?? 0); }
        public static explicit operator short (SignedInteger i) { return (short)(i?.Value ?? 0); }
        public static explicit operator sbyte (SignedInteger i) { return (sbyte)(i?.Value ?? 0); }

        public static implicit operator SignedInteger(sbyte i) { return new SignedInteger(i); }
        public static implicit operator SignedInteger(short i) { return new SignedInteger(i); }
        public static implicit operator SignedInteger(int i) { return new SignedInteger(i); }
        public static implicit operator SignedInteger(long i) { return new SignedInteger(i); }
    }
}
