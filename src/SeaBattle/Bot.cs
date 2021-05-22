using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Определяем, что корабли типа type влезают в строку n раз.
 * n = 15 - type - 1. Далее рандомно определяем ориентацию корабля и нос корабля ( координаты ), который свободен. Затем проверяем, свободны ли клетки для тела корабля и 
 * либо выходим из цикла, поставив корабль, либо переопределяем нос корабля и выполняем последние дейтсвия заново.
 */
namespace SeaBattle
{
    class Bot
    {
        public List<int[]> AvailableCellsToAttack= new List<int[]>(); //удаляем отсюда клетки, в которых точно нет корабля противника - человека

        readonly int[] available_ships = new int[4] { 6, 5, 4, 3 };

        public SuperButton[,] Buttons = new SuperButton[15, 15];

        public bool popal = false;
        public int left_ships;              //осталось убить столько кораблей.
        public int type_ship_now;

        public void InitLogic()
        {
            
            for (int j = 0; j < 15; j++)
                for (int i = 0; i < 15; i++)
                {
                    Buttons[i, j].IsShip = false;
                    Buttons[i, j].IsNeighbor = false;
                    int[] mas = new int[2];
                    mas[0] = i;
                    mas[1] = j;
                    AvailableCellsToAttack.Add(mas);
                }
            foreach (var e in available_ships)
                left_ships += e;
        }

        private void OkrestnostKletkiZanyata(int i, int j)
        {
            if (j + 1 < 15)                                            //делаем недоступной окрестность(квадрат) вокруг клетки            
                Buttons[i, j + 1].IsNeighbor = true;
            if (j - 1 >= 0)
                Buttons[i, j - 1].IsNeighbor = true;
            if (j + 1 < 15 & i + 1 < 15)
                Buttons[i + 1, j + 1].IsNeighbor = true;
            if (j - 1 >= 0 & i - 1 >= 0)
                Buttons[i - 1, j - 1].IsNeighbor = true;
            if (i + 1 < 15 & j - 1 >= 0)
                Buttons[i + 1, j - 1].IsNeighbor = true;
            if (i - 1 >= 0 & j + 1 < 15)
                Buttons[i - 1, j + 1].IsNeighbor = true;
            if (i + 1 < 15)
                Buttons[i + 1, j].IsNeighbor = true;
            if (i - 1 >= 0)
                Buttons[i - 1, j].IsNeighbor = true;
        }

        private void RelateShips(int x, int y, int mode)
        {
            if (mode == 1)   //горизонтальный или вертикальный корабль
            {
                Buttons[x, y].RelativeCells = new List<int>(Buttons[x - 1, y].RelativeCells); //передаем новой части корабля старые части

                Buttons[x, y].RelativeCells.Add(x - 1);                                              //дополняем лист новой части старой-соседкой-частью
                Buttons[x, y].RelativeCells.Add(y);

                for (int k = 0; k < Buttons[x, y].RelativeCells.Count; k = k + 2)                       //а старым передаем новую часть корабля
                {
                    Buttons[Buttons[x, y].RelativeCells[k], Buttons[x, y].RelativeCells[k + 1]].RelativeCells.Add(x);
                    Buttons[Buttons[x, y].RelativeCells[k], Buttons[x, y].RelativeCells[k + 1]].RelativeCells.Add(y);
                }
            }
            else
            {
                Buttons[y, x].RelativeCells = new List<int>(Buttons[y, x - 1].RelativeCells); //передаем новой части корабля старые части

                Buttons[y, x].RelativeCells.Add(y);                                              //дополняем лист новой части старой-соседкой-частью
                Buttons[y, x].RelativeCells.Add(x - 1);

                for (int k = 0; k < Buttons[y, x].RelativeCells.Count; k = k + 2)                       //а старым передаем новую часть корабля
                {
                    Buttons[Buttons[y, x].RelativeCells[k], Buttons[y, x].RelativeCells[k + 1]].RelativeCells.Add(y);
                    Buttons[Buttons[y, x].RelativeCells[k], Buttons[y, x].RelativeCells[k + 1]].RelativeCells.Add(x);
                }
            }

        }

        private void SetOneShip(int direction, int koord1, int koord2, int type)
        {
            if (direction == 1)
                for (int i = 0; i <= type; i++)
                {
                    Buttons[koord1 + i, koord2].IsShip = true;
                    OkrestnostKletkiZanyata(koord1 + i, koord2);
                  //  Buttons[koord1 + i, koord2].BackColor = Color.Red;
                    if (i>0)
                    RelateShips(koord1 + i, koord2, 1);
                }
            else
            {
                for (int i = 0; i <= type; i++)
                {
                    Buttons[koord2, koord1 + i].IsShip = true;
                    OkrestnostKletkiZanyata(koord2, koord1 + i);
                //    Buttons[koord2, koord1 + i].BackColor = Color.Red;
                    if(i > 0)
                    RelateShips(koord1 + i, koord2, 0);
                }
            }

        }

        private bool CheckNeighbourhood(int i, int j)                  //занята ли хотя бы одна клетка в окрестности
        {
            bool result = false;
            if (j + 1 < 15)
                if (Buttons[i, j + 1].IsOccupied == true)
                    result = true;
            if (j - 1 >= 0)
                if (Buttons[i, j - 1].IsOccupied == true)
                    result = true;
            if (j + 1 < 15 & i + 1 < 15)
                if (Buttons[i + 1, j + 1].IsOccupied == true)
                    result = true;
            if (j - 1 >= 0 & i - 1 >= 0)
                if (Buttons[i - 1, j - 1].IsOccupied == true)
                    result = true;
            if (i + 1 < 15 & j - 1 >= 0)
                if (Buttons[i + 1, j - 1].IsOccupied == true)
                    result = true;
            if (i - 1 >= 0 & j + 1 < 15)
                if (Buttons[i - 1, j + 1].IsOccupied == true)
                    result = true;
            if (i + 1 < 15)
                if (Buttons[i + 1, j].IsOccupied == true)
                    result = true;
            if (i - 1 >= 0)
                if (Buttons[i - 1, j].IsOccupied == true)
                    result = true;
            return result;
        }

        private void SetShip(int type)
        {
            bool result = false;
            Random r = new Random();
            int direction = 0;
            int nos_1 = 0;
            int nos_2 = 0;
            while (!result)
            {
                direction = r.Next(0, 2);                                                         //ориентация корабля ( 0 - вертикаль, 1 - горизонталь)
                nos_1 = r.Next(15 - type - 1);                                                 //координаты корабля
                nos_2 = r.Next(15);
                bool q = true;
                if (direction == 1)
                    for (int i = 0; i <= type; i++)
                    {
                        if ((Buttons[nos_1 + i, nos_2].IsOccupied) & (CheckNeighbourhood(nos_1 + i, nos_2)))
                            q = false;
                    }
                if (direction == 0)
                {
                    for (int i = 0; i <= type; i++)
                    {
                        if ((Buttons[nos_2, nos_1 + i].IsOccupied) & CheckNeighbourhood(nos_2, nos_1 + i))
                            q = false;
                    }
                }
                result = q;
            }
            SetOneShip(direction, nos_1, nos_2, type);

        }

        public void GenerateShips()
        {
            for (int type = 3; type >= 0; type--)
                for (int i = 0; i < available_ships[type]; i++)       //кол-во кораблей
                    SetShip(type);

        }

        public void ChangeColor(int x, int y)
        {
            for (int i = 0; i < Buttons[x, y].RelativeCells.Count; i = i + 2)
            {
                Buttons[Buttons[x, y].RelativeCells[i], Buttons[x, y].RelativeCells[i + 1]].BackColor = Color.Red;
            }
            Buttons[x, y].BackColor = Color.Red;
        }

        private bool CheckRelativeShips(int q, int w)
        {
            bool result = true;
            for (int i = 0; i < Buttons[q, w].RelativeCells.Count; i = i + 2)
            {
                if (!(Buttons[Buttons[q, w].RelativeCells[i], Buttons[q, w].RelativeCells[i + 1]].BackColor == Color.Yellow))
                    result = false;
            }
            return result;
        }

        public int PlayerShot(int x, int y)
        {
            if ((CheckRelativeShips(x, y)) & (Buttons[x, y].IsShip))
                return 2;
            else
              if (Buttons[x, y].IsShip)
                return 0;
            else
                return 1;
        }
    }
}
