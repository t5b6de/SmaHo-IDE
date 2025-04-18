using SmaHo_C_IDE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Helper
{
    internal class IdManager
    {
        // Arrays beinhalten nur bools, index = ID, true = verwendet, false = frei
        private bool[] _DefaultGateIDs, _DelayGateIDs, _MdsGateIds;
        private bool[] _DOut, _DIn, _VOut, _VIn;
        public IdManager()
        {
            // TODO: dynamisch gestalten, je nach Mikrocontroller. Hier erstmal noch "Standardmengen"
            /*
             * 
                #define GATE_COUNT_STD 250
                #define GATE_COUNT_DELAY 30
                #define GATE_COUNT_MDS 20
                #define GATE_COUNT_LIST 30
                #define GATE_LIST_ENTRIES 16 
             */
            // Nach Initialisierung sind erst einmal alle False.
            _DIn = new bool[256];
            _DOut = new bool[256];
            _VIn = new bool[64];
            _VOut = new bool[64];

            _DefaultGateIDs = new bool[250];
            _DelayGateIDs = new bool[30];
            _MdsGateIds = new bool[20];
        }

        /// <summary>
        /// Gibt eine freie ID für ein Gatter der angegebenen Art zurück
        /// </summary>
        /// <param name="id"> Größer gleich 0: Wunsch-ID, beim laden, kleiner 0 nächste freie.</param>
        /// <param name="type">GateTyp, für die diese ID ermittelt und reserviert werden soll</param>
        /// <returns>Nächste/Gewünschte Gate-ID, -1 wenn nichts frei.</returns>
        /// <exception cref="NotImplementedException">Wird geworfen, wenn GateType ungültig ist.</exception>
        public int GetId(int id, GateType type)
        {
            bool[] dst;

            switch (type)
            {
                case GateType.DIn:
                    dst = _DOut;
                    break;

                case GateType.DOut:
                    dst = _DIn;
                    break;

                case GateType.VIn:
                    dst = _VIn;
                    break;

                case GateType.VOut:
                    dst = _VOut;
                    break;

                case GateType.And:
                case GateType.Or:
                case GateType.Xor:
                case GateType.Not:
                case GateType.Nand:
                case GateType.Nor:
                case GateType.RSFlipFlop:
                case GateType.TFlipFlop:
                    dst = _DefaultGateIDs;
                    break;

                case GateType.Mds:
                    dst = _MdsGateIds;
                    break;

                case GateType.DelayOn:
                case GateType.DelayOff:
                case GateType.DelayOnOff:
                case GateType.DelayStoreOn:
                case GateType.DelayPulse:
                case GateType.DelayTriggerPulse:
                case GateType.DelayStairLight:
                    dst = _DelayGateIDs;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (id < 0) // nächste Freie erhalten
            {
                for (int i = 0; i < dst.Length; i++)
                {
                    if (dst[i])
                        continue;

                    dst[i] = true;
                    return i;
                }

                return -1;
            }
            else // Spezifische ID erhalten
            {
                if (id >= dst.Length)
                    return -1;

                if (dst[id] == true)
                    return -1;

                dst[id] = true;
                return id;
            }
        }
    }
}
