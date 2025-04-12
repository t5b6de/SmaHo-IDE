using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_Debugger
{
    public enum ShcCommand : byte
    {
        ST = 0xff,
        INFO = 0xfe,
        PING = 0x01, // sendt nur ST+OK
        OUT = 0x02, // portnum, state
        OUT_CHG = 0xa2,// portnum, state
        VOUT_CHG = 0xb2, // portnum, state
        IN_CHG = 0x03,

        STATE_REQ = 0x04, // nothing or expander index
        MCP_STATE_RES = 0xa4,
        VIO_STATE_RES = 0xb4,

        INFO_REQ = 0x05,
        INFO_RES = 0xa5,

        CFG_RNAME = 0x06,// flag i/o, num
        CFG_ANAME = 0xa6, // Response -> flag i/o, num, n name-bytes...
        CFG_WNAME = 0xb6,// flag i/o, num, data

        // Input-Trigger, simple handle_io, ohne Übermittelung der inputs.
        VINP_SET = 0x0d, // index, state

        LED_CTR = 0x0e, // index, state

        // TODO Befehle implementieren zwecks "Konfigurationsmodus"
        // in diesem Modus alles so belassen, keine Ausführungen. alles statisch.
        // beim "commit" dann quasi-Reboot durchführen.
        // dieses betrifft nur die Konfiguration der Logik, nicht die der Bezeichner

        ENTER_CFG = 0xcc, // aktiviert den Programmiermodus.
        READ_EE = 0x0a,// 2 byte page. 128Byte Data, 1 byte CRC
        DATA_EE = 0xaa,// 2 byte page, 128Byte Data.
        WRITE_EE = 0xba, // write Block, 2 byte Block ID, 128b data.

        CFG_COMMIT = 0x0f, // reload config.


    }
}
