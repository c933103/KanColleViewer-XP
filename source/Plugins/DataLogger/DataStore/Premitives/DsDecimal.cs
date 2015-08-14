using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;
using LynLogger.DataStore.Extensions;
using LynLogger.Utilities;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class DsDecimal : StoragePremitive
    {
        private decimal value;

        public override IEnumerable<TypeIdentifier> Type => EnumerablesEx.AsEnumerable(TypeIdentifier.Decimal);
        public decimal Value => value;

        public DsDecimal() { }
        public DsDecimal(decimal value) { this.value = value; }
        public DsDecimal(DSReader input)
        {
            value = input.ReadDecimal();
        }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }
    }
}
