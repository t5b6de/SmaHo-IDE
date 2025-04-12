using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_Debugger
{
    public class Packet
    {
        public byte Command { get; set; }
        public byte[] Data { get; set; }

        public Packet(byte cmd, byte[] src, int len)
        {
            Command = cmd;
            Data = new byte[len];

            Array.Copy(src, 0, Data, 0, len);
        }


        public byte[] ToBytes()
        {
            var length = (byte)(1 + Data.Length); // 1 für Befehl
            return new[] { (byte)0x5A, length, Command }.Concat(Data).ToArray();
        }
    }
}
