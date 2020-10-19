using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace PreheatingStationControl.Managers
{
    internal static class ComPortManager
    {
        internal static Task<List<ComPort>> GetAllPortsAsync()
        {
            return Task.Run(() =>
            {
                var result = new List<ComPort>();

                //using (var searcher = new ManagementObjectSearcher ("SELECT * FROM WIN32_SerialPort"))
                //{
                    var ports = SerialPort.GetPortNames();
                    //var portManagements = searcher.Get().Cast<ManagementBaseObject>().ToList();

                    foreach (var port in ports)
                    {
                        //var description = portManagements
                        //    .Where(x => x["DeviceID"].ToString() == port)
                        //    .Select(x => $"{port} ({x["Caption"]})")
                        //    .FirstOrDefault();

                        result.Add(new ComPort(port, port));
                    }
                //}

                return result;
            });
        }
    }

    public class ComPort
    {
        public ComPort(string port, string description)
        {
            this.Name = port;
            this.Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public override string ToString()
        {
            return Description;
        }
    }
}
