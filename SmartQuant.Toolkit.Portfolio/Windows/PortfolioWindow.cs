using System;
using System.Windows.Forms;
using SmartQuant.Shared;

namespace SmartQuant.Toolkit.Portfolio.Windows
{
    public partial class PortfolioWindow : DockWindow
    {
        public PortfolioWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("error: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }
}
