using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmaHo_C_Debugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts = new();
        private List<Packet> _receivedPackets = new();
        private bool _isConnected => _client?.Connected ?? false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Connect(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
            StartReading(_cts.Token);
        }
        private void SendPacket(Packet packet)
        {
            var bytes = packet.ToBytes();
            _stream.Write(bytes, 0, bytes.Length);
        }

        private async void StartReading(CancellationToken token)
        {
            var buffer = new byte[256];
            int len = 0;
            int p = 0;
            byte cmd = 0;
            int sync = 0;

            try
            {
                while (_isConnected && !token.IsCancellationRequested)
                {
                    int d = _stream.ReadByte(); // Blocking: wartet auf ein Byte

                    if (d <= -1)
                    {
                        break;
                    }

                    byte b = (byte)d;

                    if (sync == 0) // kein Sync
                    {
                        if (b == 0x5a)
                            sync++;
                    }
                    else if (sync == 1) // dann ist nächste Byte länge
                    {
                        len = b;
                        p = 0;
                        if (len == 0 || len > 150) // Pakete niemals 0 oder > 150 (buffersize im µC)
                            sync = 0;
                        sync++;
                    }
                    else if (sync == 2) // dann ist nächste Byte cmd.
                    {
                        cmd = b;
                        len--;
                        sync++;
                        // theoretisch hier noch befehlsprüfung einbauen, falls vorhanden, dann ok, sonst sync = 0 - könnte probleme beim debuggen neuer Befehle machen.
                    }
                    else
                    {
                        // Restliches Paket sammeln
                        if (p < len)
                        {
                            buffer[p++] = b;
                        }

                        if (p == len)
                        {

                            Dispatcher.Invoke(() => AddPacketToList(new Packet(cmd, buffer, len), false));

                            sync = 0; // auf nächstes Sync Byte warten.
                        }
                    }

                }
            }
            catch { }

            Dispatcher.Invoke(() =>
            {
                SetUiConnectionState(false);
            });
        }

        private void PortTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumericInput(e.Text);
        }

        private static bool IsNumericInput(string text)
        {
            return int.TryParse(text, out int result) && result <= 65535;
        }

        // Alles markieren beim Tab/Keyboard-Fokus
        private void SelectAllText(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.SelectAll();
        }

        // Alles markieren beim Klick
        private void SelectAllText_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb && !tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
            }
        }

        private void AddPacketToList(Packet packet, bool sent)
        {
            _receivedPackets.Add(packet);
            string add = sent ? "<<" : ">>";

            if (Enum.IsDefined(typeof(ShcCommand), packet.Command))
            {
                PacketList.Items.Add($"{add} Cmd: SHC_CMD_{Enum.GetName(typeof(ShcCommand), packet.Command)}, Len: {packet.Data.Length}");
            }
            else
            {
                PacketList.Items.Add($"{add} Cmd: 0x{packet.Command:x2}, Len: {packet.Data.Length}");
            }

        }

        private void PacketList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PacketList.SelectedIndex;
            if (index < 0 || index >= _receivedPackets.Count) return;

            var packet = _receivedPackets[index];
            CommandTextBox.Text = $"0x{packet.Command:X2}";
            HexView.Text = ToHexAsciiView(packet.Data);
        }

        private string ToHexAsciiView(byte[] data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i += 16)
            {
                var line = data.Skip(i).Take(16).ToArray();
                var hex = string.Join(" ", line.Select(b => $"{b:x2}")).PadRight(48);
                var ascii = new string(line.Select(b => b >= 32 && b <= 126 ? (char)b : '.').ToArray());
                sb.AppendLine($"{hex}  {ascii}");
            }
            return sb.ToString();
        }

        private byte crc8(byte[] buf, int ofs, byte len)
        {
            byte tmp, bitCtr, fb, res = 0;
            int ptr = ofs;

            while (len-- != 0)
            {
                tmp = buf[ptr++]; // *buf++;

                for (bitCtr = 8; bitCtr != 0; bitCtr--)
                {
                    fb = (byte)(res & 0x01);
                    res >>= 1;
                    if ((fb ^ (tmp & 0x01)) != 0)
                    {
                        res ^= 0x8c;
                    }
                    tmp >>= 1;
                }
            }
            return res;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isConnected) return;

                if (CommandComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Bitte ein Command auswählen.");
                    return;
                }

                byte commandByte = (byte)CommandComboBox.SelectedValue;
                byte[] payload = ParseHexString(SendHexBox.Text); // Nur Daten, kein Command hier!

                if(commandByte == (byte)ShcCommand.WRITE_EE) // CRC8 geradebiegen für Datenblock.
                {
                    payload[0x82] = crc8(payload, 2, 0x80);
                }

                // Paket aufbauen: Sync, Length, Command, Data
                byte[] packet = new byte[2 + 1 + payload.Length];
                packet[0] = 0x5A;
                packet[1] = (byte)(1 + payload.Length); // Command + Data
                packet[2] = commandByte;
                Array.Copy(payload, 0, packet, 3, payload.Length);

                _stream.Write(packet, 0, packet.Length);
                AddPacketToList(new Packet(commandByte, payload, payload.Length), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden: {ex.Message}");
            }
        }

        private byte[] ParseHexString(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            // 1. Entferne alle definierten Trennzeichen (Leerzeichen, Tabs, Umbrüche)
            //    und füge die Teile wieder zusammen. Das ergibt einen String ohne Trennzeichen.
            var cleanedHex = string.Concat(hex.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries));

            // Handle leeren String nach Bereinigung
            if (cleanedHex.Length == 0)
            {
                return Array.Empty<byte>(); // Leeres Array für leere Eingabe nach Bereinigung
            }

            // 2. Überprüfe, ob die resultierende Länge gerade ist (jedes Byte braucht 2 Hex-Zeichen)
            if (cleanedHex.Length % 2 != 0)
            {
                throw new ArgumentException("Die Hexadezimal-Zeichenkette muss nach Entfernung von Trennzeichen eine gerade Anzahl von Zeichen haben.", nameof(hex));
            }

            // 3. Iteriere durch den bereinigten String in 2-Zeichen-Schritten
            int byteCount = cleanedHex.Length / 2;
            byte[] resultBytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                // Extrahiere das Zeichenpaar für ein Byte
                string byteString = cleanedHex.Substring(i * 2, 2);
                try
                {
                    // Konvertiere das Zeichenpaar (Basis 16) in ein Byte
                    resultBytes[i] = Convert.ToByte(byteString, 16);
                }
                catch (FormatException ex)
                {
                    // Gib eine spezifischere Fehlermeldung, falls ungültige Zeichen vorkommen
                    throw new FormatException($"Ungültiges Hexadezimal-Zeichenpaar '{byteString}' an Index {i * 2} gefunden.", ex);
                }
                // OverflowException sollte bei 2 Hex-Zeichen nicht auftreten, aber zur Sicherheit erwähnt.
            }

            return resultBytes;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = IpTextBox.Text.Trim();
                int port = int.Parse(PortTextBox.Text.Trim());

                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();

                _cts = new CancellationTokenSource();
                Task.Run(() => StartReading(_cts.Token));

                SetUiConnectionState(true);

                _receivedPackets.Clear();
                PacketList.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Verbindung fehlgeschlagen: {ex.Message}");
                SetUiConnectionState(false);
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();

            _stream?.Close();
            _client?.Close();

            SetUiConnectionState(false);
        }

        private void SetUiConnectionState(bool connected)
        {
            PacketList.IsEnabled = connected;
            CommandTextBox.IsEnabled = connected;
            HexView.IsEnabled = connected;
            SendHexBox.IsEnabled = connected;

            IpTextBox.IsEnabled = !connected;
            PortTextBox.IsEnabled = !connected;

            // Optional: Buttons ein/aus
            /* foreach (var ctrl in new[] { SendHexBox, PacketList })
                 ctrl.IsEnabled = connected;*/
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Command-Dropdown befüllen
            var commandList = Enum.GetValues(typeof(ShcCommand))
                                  .Cast<ShcCommand>()
                                  .ToDictionary(cmd => cmd.ToString(), cmd => (byte)cmd);

            CommandComboBox.ItemsSource = commandList;
            CommandComboBox.SelectedIndex = 0;
        }
    }
}