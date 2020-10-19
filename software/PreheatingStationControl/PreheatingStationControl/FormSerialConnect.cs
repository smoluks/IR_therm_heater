using PreheatingStationControl.Managers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreheatingStationControl
{
    public partial class FormSerialConnect : Form
    {
        private readonly Func<string, Task> _connectCallback;

        public FormSerialConnect(Func<string, Task> connectCallback)
        {
            _connectCallback = connectCallback;
            InitializeComponent();
        }

        private async void buttonRefresh_Click(object sender, System.EventArgs e)
        {
            await RefreshPorts();
        }

        private async void FormSerialConnect_Load(object sender, EventArgs e)
        {
            await RefreshPorts();
        }

        private async Task RefreshPorts()
        {
            listBoxPorts.Enabled = false;
            buttonRefresh.Enabled = false;

            var ports = await ComPortManager.GetAllPortsAsync();

            listBoxPorts.Invoke(new Action(
    () =>
    {
        listBoxPorts.Items.Clear();
        listBoxPorts.Items.AddRange(ports.ToArray());
        listBoxPorts.Enabled = true;
        buttonRefresh.Enabled = true;
    }));
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if(listBoxPorts.SelectedIndex != -1)
            {
                buttonConnect.Enabled = false;

                try
                {
                    await _connectCallback(((ComPort)listBoxPorts.SelectedItem).Name);
                    this.Close();
                }
                catch(Exception)
                {
                    buttonConnect.Enabled = true;
                }
            }
        }
    }
}
