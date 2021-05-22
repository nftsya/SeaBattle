using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Media;

namespace SeaBattle
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ResourceManager rm = new ResourceManager("Resources", Assembly.GetExecutingAssembly());
            Application.Run(new MenuSeaBattle(rm));

        }
    }
}
