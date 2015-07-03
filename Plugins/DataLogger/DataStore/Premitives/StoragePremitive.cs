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
    public class StoragePremitive
    {
        public virtual IEnumerable<TypeIdentifier> Type => Collections.AsEnumerable(TypeIdentifier.Undefined);
        public virtual void SerializeNonNull(DSWriter output) { throw new NotImplementedException(); }

        public static StoragePremitive Parse(DSReader input)
        {
            return MapIdentifierToConstructor(input)(input);
        }

        private static Type MapIdentifierToType(DSReader reader)
        {
            var type = (TypeIdentifier)reader.Read7bUInt();
            switch(type) {
                case TypeIdentifier.Blob:
                    return typeof(Blob);
                case TypeIdentifier.Compound:
                    return typeof(Compound);
                case TypeIdentifier.Decimal:
                    return typeof(Decimal);
                case TypeIdentifier.Dictionary:
                    return typeof(Dictionary<,>).MakeGenericType(MapIdentifierToType(reader), MapIdentifierToType(reader));
                case TypeIdentifier.Double:
                    return typeof(Double);
                case TypeIdentifier.Int:
                    return typeof(SignedInteger);
                case TypeIdentifier.List:
                    return typeof(List<>).MakeGenericType(MapIdentifierToType(reader));
                case TypeIdentifier.String:
                    return typeof(String);
                case TypeIdentifier.UInt:
                    return typeof(UnsignedInteger);
                case TypeIdentifier.Undefined:
                    return typeof(StoragePremitive);
                default:
                    throw new ArgumentException();
            }
        }

        private static Func<DSReader, StoragePremitive> MapIdentifierToConstructor(DSReader reader)
        {
            var type = (TypeIdentifier)reader.Read7bUInt();
            switch(type) {
                case TypeIdentifier.Blob:
                    return x => new Blob(x);
                case TypeIdentifier.Compound:
                    return x => new Compound(x);
                case TypeIdentifier.Decimal:
                    return x => new Decimal(x);
                case TypeIdentifier.Double:
                    return x => new Double(x);
                case TypeIdentifier.Int:
                    return x => new SignedInteger(x);
                case TypeIdentifier.List:
                    return x => (StoragePremitive)typeof(List<>).MakeGenericType(MapIdentifierToType(x)).GetConstructor(new Type[] { typeof(DSReader) }).Invoke(new object[] { x });
                case TypeIdentifier.String:
                    return x => new String(x);
                case TypeIdentifier.UInt:
                    return x => new UnsignedInteger(x);
                case TypeIdentifier.Null:
                    return x => null;
                case TypeIdentifier.Dictionary:
                    return x => {
                        var kt = MapIdentifierToType(reader);
                        var vt = MapIdentifierToType(reader);
                        return (StoragePremitive)typeof(Dictionary<,>).MakeGenericType(kt, vt).GetConstructor(new Type[] { typeof(DSReader) }).Invoke(new object[] { x });
                    };
                default:
                    throw new ArgumentException();
            }
        }

        /*public static implicit operator StoragePremitive(sbyte i) { return new SignedInteger(i); }
        public static implicit operator StoragePremitive(short i) { return new SignedInteger(i); }
        public static implicit operator StoragePremitive(int i) { return new SignedInteger(i); }
        public static implicit operator StoragePremitive(long i) { return new SignedInteger(i); }

        public static implicit operator StoragePremitive(byte i) { return new UnsignedInteger(i); }
        public static implicit operator StoragePremitive(ushort i) { return new UnsignedInteger(i); }
        public static implicit operator StoragePremitive(uint i) { return new UnsignedInteger(i); }
        public static implicit operator StoragePremitive(ulong i) { return new UnsignedInteger(i); }

        public static implicit operator StoragePremitive(string i) { return new String(i); }

        public static implicit operator StoragePremitive(double i) { return new Double(i); }
        public static implicit operator StoragePremitive(float i) { return new Double(i); }

        public static implicit operator StoragePremitive(decimal i) { return new Decimal(i); }

        public static implicit operator StoragePremitive(byte[] i) { return new Blob(i); }*/
    }
}
