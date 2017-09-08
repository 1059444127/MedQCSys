﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MedQC.Test
{
    public partial class LiveChartForm : Form
    {
        public LiveChartForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cartesianChart1.Series.Clear();
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisY.Clear();
            cartesianChart1.Series.Add(new ColumnSeries
            {
                Values = new ChartValues<ObservableValue>
                {
                    new ObservableValue(4),
                    new ObservableValue(2),
                    new ObservableValue(8),
                    new ObservableValue(2),
                    new ObservableValue(3),
                    new ObservableValue(0),
                    new ObservableValue(1),
                },
                DataLabels = true,
                LabelPoint = point => point.Y + "K"
            });

            cartesianChart1.AxisX.Add(new Axis
            {
                Labels = new[]
                {
                    "Shea Ferriera",
                    "Maurita Powel",
                    "Scottie Brogdon",
                    "Teresa Kerman",
                    "Nell Venuti",
                    "Anibal Brothers",
                    "Anderson Dillman"
                },
                Separator = new Separator // force the separator step to 1, so it always display all labels
                {
                    Step = 1,
                    IsEnabled = false //disable it to make it invisible.
                },
                LabelsRotation = 15
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                LabelFormatter = value => value + ".00K items",
                Separator = new Separator()
            });

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}
