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
    class DsDouble : StoragePremitive
    {
        private double value;

        public override IEnumerable<TypeIdentifier> Type => EnumerablesEx.AsEnumerable(TypeIdentifier.Double);
        public double Value => value;

        public DsDouble() { }
        public DsDouble(double value) { this.value = value; }
        public DsDouble(DSReader input) { value = input.ReadDouble(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }
    }
}
