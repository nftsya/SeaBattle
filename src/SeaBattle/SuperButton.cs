using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle
{
    internal class SuperButton: Button
    {
        internal SuperButton(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }                                //Координаты клетки(кнопки)
        public int Y { get; set; }
        public bool IsOccupied { get {
                return IsShip || IsNeighbor;
            } }                       //Можно ли ставить в клетку? (Она не окресность и не корабль)
        public bool IsShip { get; set; }                          //Это корабль
        public bool IsNeighbor { get; set; }                      //Это окресность
        public int Type { get; set; }                             //тип корабля 0 -однопалубный 1 - двупалубный и т.д.
        public List<int> RelativeCells = new List<int>();         //связанные с ним корабли
    }
}
