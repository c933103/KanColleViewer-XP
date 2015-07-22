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
    class Decimal : StoragePremitive
    {
        private decimal value;

        public override IEnumerable<TypeIdentifier> Type => CollectionsEx.AsEnumerable(TypeIdentifier.Decimal);
        public decimal Value => value;

        public Decimal() { }
        public Decimal(decimal value) { this.value = value; }
        public Decimal(DSReader input)
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
