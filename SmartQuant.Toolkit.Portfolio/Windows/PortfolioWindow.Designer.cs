namespace SmartQuant.Toolkit.Portfolio.Windows
{
    partial class PortfolioWindow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.portfolioControl1 = new SmartQuant.Toolkit.Portfolio.Controls.PortfolioControl();
            this.SuspendLayout();
            // 
            // portfolioControl1
            // 
            this.portfolioControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portfolioControl1.Location = new System.Drawing.Point(0, 0);
            this.portfolioControl1.Name = "portfolioControl1";
            this.portfolioControl1.Size = new System.Drawing.Size(150, 150);
            this.portfolioControl1.TabIndex = 0;
            // 
            // PortfolioWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Control = this.portfolioControl1;
            this.Controls.Add(this.portfolioControl1);
            this.DefaultDockLocation = TD.SandDock.ContainerDockLocation.Center;
            this.Name = "PortfolioWindow";
            this.Text = "Portfolios+";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.PortfolioControl portfolioControl1;
    }
}
