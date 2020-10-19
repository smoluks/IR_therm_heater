using System;
using System.Diagnostics;
using System.Windows.Forms;
using static PreheatingStationControl.Managers.HeaterManager;

namespace PreheatingStationControl
{
    public partial class FormMain : Form
    {
        FormSerialConnect serialConnectForm;

        public FormMain()
        {
            InitializeComponent();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialConnectForm != null)
            {
                serialConnectForm.Close();
            }

            serialConnectForm = new FormSerialConnect(ConnectBySerialAsync);
            serialConnectForm.Show();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        Stopwatch stopwatch = new Stopwatch();

        private async void timerUpdateGraph_Tick(object sender, EventArgs e)
        {
            stopwatch.Restart();

            var state = await heaterManager.GetStateAsync();
            labelSensor0.Text = state.SensorsTemperature[0] + "°";
            labelSensor1.Text = state.SensorsTemperature[1] + "°";
            labelSensor2.Text = state.SensorsTemperature[2] + "°";

            AddToGraph(state);

            stopwatch.Stop();
            toolStripTimeLabel.Text = $"Ping {stopwatch.ElapsedMilliseconds} ms";
        }

        private async void radioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            await heaterManager.SetModeAsync(HeaterMode.Off);
        }

        private async void radioButtonHeat_CheckedChanged(object sender, EventArgs e)
        {
            await heaterManager.SetModeAsync(HeaterMode.Heat);
        }

        private async void radioButtonCool_CheckedChanged(object sender, EventArgs e)
        {
            await heaterManager.SetModeAsync(HeaterMode.Cool);
        }

        private async void numericUpDownA_ValueChanged(object sender, EventArgs e)
        {
            await heaterManager.SetTargetAsync(0, (ushort)numericUpDownA.Value);
        }

        private async void numericUpDownB_ValueChanged(object sender, EventArgs e)
        {
            await heaterManager.SetTargetAsync(1, (ushort)numericUpDownB.Value);
        }

        private async void numericUpDownC_ValueChanged(object sender, EventArgs e)
        {
            await heaterManager.SetTargetAsync(2, (ushort)numericUpDownC.Value);
        }
    }
}
