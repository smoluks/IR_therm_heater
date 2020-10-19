using NModbus;
using NModbus.Serial;
using PreheatingStationControl.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PreheatingStationControl.Managers
{
    internal class HeaterManager: IDisposable
    {
        const int ADDRESS = 1;

        SerialPort serialPort;
        TcpClient tcpClient;
        IModbusMaster master;

        internal async Task ConnectBySerialAsync(string comPort)
        {
            serialPort = new SerialPort()
            {
                PortName = comPort,
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.Odd,
                StopBits = StopBits.One,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            serialPort.Open();

            var factory = new ModbusFactory();
            master = factory.CreateRtuMaster(serialPort);

            await master.ReadInputRegistersAsync(ADDRESS, 0, 1);
        }

        internal async Task ConnectByTCPAsync(string address, int port)
        {
            tcpClient = new TcpClient(address, port);

            var factory = new ModbusFactory();
            master = factory.CreateMaster(tcpClient);

            await master.ReadInputRegistersAsync(ADDRESS, 0, 1);
        }

        public void Dispose()
        {
            master?.Dispose();
            serialPort?.Close();
            tcpClient?.Close();
        }
        
        internal async Task<HeaterState> GetStateAsync()
        {          
            var regs = await master.ReadInputRegistersAsync(ADDRESS, 0, 10);

            return new HeaterState()
            {
                SensorsTemperature = regs.SubArray(0, 8).Select(x => (double)x / 16).ToList(),
                AmbientTemperature = (double)regs[8] / 16,
                OutputValues = regs.SubArray(9, 1).Select(x => (byte)x).ToList(),
            };
        }

        internal async Task<HeaterTarget> GetCurrentModeAsync()
        {
            var regs = await master.ReadHoldingRegistersAsync(ADDRESS, 0, 9);

            return new HeaterTarget()
            {
                TargetsTemperature = regs.SubArray(0, 8).Select(x => (double)x / 16).ToList(),
                Mode = (HeaterMode)regs[8]
            };
        }

        internal class HeaterState
        {
            public List<double> SensorsTemperature { get; internal set; }
            public double AmbientTemperature { get; internal set; }
            public List<byte> OutputValues { get; internal set; }
        }

        internal class HeaterTarget
        {
            public List<double> TargetsTemperature { get; internal set; }
            public HeaterMode Mode { get; internal set; }
        }

        internal enum HeaterMode
        {
            Off,
            Heat,
            Cool
        }
    }
}
