using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.Extensions
{
    static class Streams
    {
        public static void ReadFully(this BinaryReader input, byte[] buffer)
        {
            int offset = 0;
            int remaining = buffer.Length;
            while(remaining > 0) {
                int read = input.Read(buffer, offset, remaining);
                if(read <= 0)
                    throw new EndOfStreamException(string.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }

        public static void ReadFully(this Stream input, byte[] buffer)
        {
            int offset = 0;
            int remaining = buffer.Length;
            while(remaining > 0) {
                int read = input.Read(buffer, offset, remaining);
                if(read <= 0)
                    throw new EndOfStreamException(string.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }
    }
}
