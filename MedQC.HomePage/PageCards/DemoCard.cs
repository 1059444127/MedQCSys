﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Heren.MedQC.HomePage.PageCards
{
    public partial class DemoCard : BaseCard,ICardControl
    {
        public DemoCard()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
        }
        public override  bool RefreshCard()
        {
            Random rand = new Random();
            for (int index = 1; index < 100; index++)
            {
                chart1.Series["Series1"].Points.AddY(rand.Next(1, 1000));
            }
            return true;
        }
        private void cardPanel1_Enter(object sender, EventArgs e)
        {

        }
    }
}
