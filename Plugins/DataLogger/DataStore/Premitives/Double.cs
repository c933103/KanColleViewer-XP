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
    class Double : StoragePremitive
    {
        private double value;

        public override IEnumerable<TypeIdentifier> Type { get { return Collections.AsEnumerable(TypeIdentifier.Double); } }
        public double Value { get { return value; } }

        public Double() { }
        public Double(double value) { this.value = value; }
        public Double(DSReader input) { value = input.ReadDouble(); }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write(value);
        }
    }
}
