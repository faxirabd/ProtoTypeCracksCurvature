namespace ProtoTypeCurvature_V3
{
    partial class StepSizeFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txbx_step = new System.Windows.Forms.TextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Step Size:";
            // 
            // txbx_step
            // 
            this.txbx_step.Location = new System.Drawing.Point(68, 35);
            this.txbx_step.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txbx_step.Name = "txbx_step";
            this.txbx_step.Size = new System.Drawing.Size(42, 20);
            this.txbx_step.TabIndex = 1;
            this.txbx_step.Text = "5";
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(59, 93);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(56, 19);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // StepSizeFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(196, 161);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.txbx_step);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "StepSizeFrm";
            this.Text = "StepSizeFrm";
            this.Load += new System.EventHandler(this.StepSizeFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbx_step;
        private System.Windows.Forms.Button btn_ok;
    }
}