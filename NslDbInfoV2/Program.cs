using System;
using System.Configuration;
using System.Windows.Forms;

namespace NslDbInfoV2
{
    static class Program
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                new NslDbLogin().ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}