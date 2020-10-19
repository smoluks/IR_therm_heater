using NModbus;
using NModbus.Serial;
using PreheatingStationControl.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PreheatingStationControl.Managers
{
    internal class HeaterManager: IDisposable
    {
        const byte SENSORS_COUNT = 3;
        const byte CHANNELS_COUNT = 1;

        const byte SLAVE_ADDRESS = 1;

        //input regs addresses
        //0-7 A,B,C...
        const byte INPUT_REG_AMBIENTTEMP = 0x08;
        const byte INPUT_REG_OUT1 = 0x09;

        //holding regs addresses
        //0-7 A,B,C... target
        const byte HOLDING_REG_MODE = 0x08;
        const byte HOLDING_REG_BEEP = 0x09;
        const byte HOLDING_REG_GOTOBOOTLOADER = 0x0F;
        //0x10 - 0x1F dT/dt
        //0x20 - 0x31 corrections

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

            await master.ReadInputRegistersAsync(SLAVE_ADDRESS, 0, 1);
        }

        internal async Task ConnectByTCPAsync(string address, int port)
        {
            tcpClient = new TcpClient(address, port);

            var factory = new ModbusFactory();
            master = factory.CreateMaster(tcpClient);

            await master.ReadInputRegistersAsync(SLAVE_ADDRESS, 0, 1);
        }

        public void Dispose()
        {
            master?.Dispose();
            serialPort?.Close();
            tcpClient?.Close();
        }
        
        internal async Task<HeaterState> GetStateAsync()
        {          
            var regs = await master.ReadInputRegistersAsync(SLAVE_ADDRESS, 0, 10);

            return new HeaterState()
            {
                SensorsTemperature = regs.SubArray(0, SENSORS_COUNT).Select(x => (double)x / 16).ToList(),
                AmbientTemperature = (double)regs[INPUT_REG_AMBIENTTEMP] / 16,
                OutputValues = regs.SubArray(INPUT_REG_OUT1, CHANNELS_COUNT).Select(x => (byte)x).ToList(),
            };
        }

        internal async Task<HeaterTarget> GetCurrentModeAsync()
        {
            var regs = await master.ReadHoldingRegistersAsync(SLAVE_ADDRESS, 0, 9);

            return new HeaterTarget()
            {
                TargetsTemperature = regs.SubArray(0, SENSORS_COUNT).Select(x => x >> 4).ToList(),
                Mode = (HeaterMode)regs[8]
            };
        }

        internal async Task SetModeAsync(HeaterMode mode)
        {
            if (master?.Transport == null)
                return;

            await master.WriteSingleRegisterAsync(SLAVE_ADDRESS, HOLDING_REG_MODE, (ushort)mode);
        }

        internal async Task SetTargetAsync(ushort number, ushort value)
        {
            if (master?.Transport == null)
                return;

            await master.WriteSingleRegisterAsync(SLAVE_ADDRESS, number, (ushort)(value << 4));
        }

        internal class HeaterState
        {
            public List<double> SensorsTemperature { get; internal set; }
            public double AmbientTemperature { get; internal set; }
            public List<byte> OutputValues { get; internal set; }
        }

        internal class HeaterTarget
        {
            public List<int> TargetsTemperature { get; internal set; }
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
