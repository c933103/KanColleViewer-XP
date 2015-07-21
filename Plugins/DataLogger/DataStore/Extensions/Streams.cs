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

        public static void WriteVLCI(this Stream output, ulong val)
        {
            for (int i = 0; i < 9; i++) {
                byte thisByte = (byte)(val & 0x7F);
                val >>= 7;
                if (val != 0) {
                    thisByte |= 0x80;
                    output.WriteByte(thisByte);
                } else {
                    output.WriteByte(thisByte);
                    break;
                }
            }
        }

        public static ulong ReadVLCI(this Stream input)
        {
            ulong val = 0;
            for (int i = 0; i < 9; i++) {
                int b = input.ReadByte();
                if (b < 0)
                    throw new EndOfStreamException();

                ulong l = (ulong)(b & 0x7F);
                val |= l << (7 * i);

                if ((b & 0x80) == 0) break;
                if (i == 8) {
                    if ((b & 0x80) != 0) val |= 1UL << 63;
                }
            }

            return val;
        }
    }
}
