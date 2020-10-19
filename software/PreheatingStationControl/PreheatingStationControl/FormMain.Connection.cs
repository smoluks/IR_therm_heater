using PreheatingStationControl.Managers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreheatingStationControl
{
    public partial class FormMain : Form
    {
        private HeaterManager heaterManager = new HeaterManager();

        async Task ConnectBySerialAsync(string portName)
        {
            toolStripStatusLabel.Text = $"Connecting to {portName}";

            try
            {
                await heaterManager.ConnectBySerialAsync(portName);
            }
            catch(Exception e)
            {
                toolStripStatusLabel.Text = $"{portName}: {e.Message}";
                throw;
            }

            connectToolStripMenuItem.Enabled = false;
            connectByTCPToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = true;
            timerUpdateGraph.Start();

            toolStripStatusLabel.Text = $"Connected to {portName}";

            await UpdateModeAsync();
        }

        async Task ConnectByTcpAsync(string address, int port)
        {
            toolStripStatusLabel.Text = $"Connecting to {address}:{port}";

            try
            {
                await heaterManager.ConnectByTCPAsync(address, port);
            }
            catch (Exception e)
            {
                toolStripStatusLabel.Text = $"{address}:{port}: {e.Message}";
                throw;
            }

            connectToolStripMenuItem.Enabled = false;
            connectByTCPToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = true;
            timerUpdateGraph.Start();

            toolStripStatusLabel.Text = $"Connected to {address}:{port}";

            await UpdateModeAsync();
        }

        private async Task UpdateModeAsync()
        {
            var mode = await heaterManager.GetCurrentModeAsync();


        }

        void Disconnect()
        {
            toolStripStatusLabel.Text = $"Disconnecting";

            timerUpdateGraph.Stop();

            heaterManager.Dispose();

            connectToolStripMenuItem.Enabled = true;
            connectByTCPToolStripMenuItem.Enabled = true;
            disconnectToolStripMenuItem.Enabled = false;

            toolStripStatusLabel.Text = $"Disconnected";
        }
    }
}
