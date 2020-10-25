using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Media;
using static PreheatingStationControl.Managers.HeaterManager;

namespace PreheatingStationControl
{
    public partial class FormMain : Form
    {
        List<LineSeries> sensorCharts;
        LineSeries ambientChart;
        List<LineSeries> outputCharts;

        private void FormMain_Load(object sender, EventArgs e)
        {
            sensorCharts = new List<LineSeries>()
            {
                new LineSeries
                {
                    Title = "A",
                    Values = new ChartValues<double>(),
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "B",
                    Values = new ChartValues<double>(),
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "C",
                    Values = new ChartValues<double>(),
                    Fill = Brushes.Transparent,
                    PointGeometry = null
                }
            };

            ambientChart = new LineSeries()
            {
                Title = "Amb.",
                Values = new ChartValues<double>(),
                //Visibility = Visibility.Hidden
            };

            outputCharts = new List<LineSeries>()
            {
                new LineSeries
                {
                    Title = "Ch. 1",
                    Values = new ChartValues<int>(),
                    //Visibility = Visibility.Hidden
                }
            };

            cartesianChart.Series = new SeriesCollection();
            cartesianChart.Series.AddRange(sensorCharts);
            //cartesianChart.Series.Add(ambientChart);
            //cartesianChart.Series.AddRange(outputCharts);

            cartesianChart.AxisX.Add(new Axis
            {
                Title = "Time",
                Labels = new List<string>()
            });

            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.DisableAnimations = true;
            cartesianChart.Zoom = ZoomingOptions.X;
        }

        void AddToGraph(HeaterState state)
        {
            cartesianChart.AxisX[0].Labels.Add(DateTime.Now.ToString("T", DateTimeFormatInfo.InvariantInfo));
            for (int i = 0; i < 3; i++)
            {
                sensorCharts[i].Values.Add(state.SensorsTemperature[i]);
            }

            ambientChart.Values.Add(state.AmbientTemperature);

            outputCharts[0].Values.Add((int)state.OutputValues[0]);

        }
    }
}
