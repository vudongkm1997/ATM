﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FITHAUI.ATMSystem.UI
{
    public partial class frmInputAccountValid : Form
    {
        public frmInputAccountValid()
        {
            InitializeComponent();
        }

        private void btnChooseAccept_Click(object sender, EventArgs e)
        {
            frmChoosePrintReceipt frmChoosePrintReceipt = new frmChoosePrintReceipt();
            frmChoosePrintReceipt.Show();
            this.Hide();
        }
    }
}
