using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using FieldStat.DataCollection;
using FieldStat;

namespace FieldStatsGUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Plugins.InitPlugins();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FieldStatFrm());
        }
    }
}
