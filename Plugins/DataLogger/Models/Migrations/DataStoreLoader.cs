using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace LynLogger.Models.Migrations
{
    public static class DataStoreLoader
    {
        private static Lazy<SerializationBinder>[] migrationBinders = new Lazy<SerializationBinder>[] {
            new Lazy<SerializationBinder>(() => new Versions.V0Binder())
        };

        public static DataStore LoadFromStream(Stream input)
        {
            if(input.ReadByte() != 0x48) throw new InvalidDataException();
            if(input.ReadByte() != 0x41) throw new InvalidDataException();
            if(input.ReadByte() != 0x49) throw new InvalidDataException();
            int b = input.ReadByte();
            if(b == 0x32) {
                MemoryStream buf = new MemoryStream();
                Helpers.DecompressData(input, null, buf);
                buf.Position = 0;
                input = buf;
            } else if(b != 0x49) throw new InvalidDataException();

            ulong structureVersion = input.ReadVLCI();
            BinaryFormatter formatter = new BinaryFormatter();
            if(structureVersion != DataStore.StructureVersionNumber) {
                if(structureVersion >= (ulong)migrationBinders.Length) throw new InvalidDataException();
                formatter.Binder = migrationBinders[structureVersion].Value;
                return ((IMigratableDataStore)formatter.Deserialize(input)).Migrate();
            } else {
                return (DataStore)formatter.Deserialize(input);
            }
        }
    }

    interface IMigratableDataStore
    {
        DataStore Migrate();
    }
}
