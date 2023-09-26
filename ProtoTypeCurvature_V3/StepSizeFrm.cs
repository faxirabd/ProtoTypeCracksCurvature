using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoTypeCurvature_V3
{
    public partial class StepSizeFrm : Form
    {
        public StepSizeFrm()
        {
            InitializeComponent();
        }

        public int stepsize = 0;
        private void btn_ok_Click(object sender, EventArgs e)
        {
            stepsize = int.Parse(txbx_step.Text);
            this.Close();
        }

        private void StepSizeFrm_Load(object sender, EventArgs e)
        {
            btn_ok.Focus();
        }
    }
}
