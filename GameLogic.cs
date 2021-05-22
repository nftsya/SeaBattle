using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle
{
    class GameLogic
    {
        public int[] PlayerShipsAvailableToSet { get; private set; } = new int[4] { 6, 5, 4, 3 };     //доступно кораблей для постановки
        public SuperButton[,] Buttons { get; set; } = new SuperButton[15, 15];                         //массив кнопок(ячейки таблицы)
        private bool waiting = false;                                                                 /*Для случая отсутствия корабля типа ( ждем достройку корабля), 
                                                                                                       true - ждем клика на клетку рядом с кораблем, чтобы изменить его тип на +1 */
        public int LeftShipsToKill;
        // private bool CheckDiagonal = false;                                                           //есть ли сосед на диагонали
        int waiting_i = 0;
        int waiting_j = 0;
        public void InitLogic()
        {
            for (int j = 0; j < 15; j++)
                for (int i = 0; i < 15; i++)
                {
                    Buttons[i, j].IsShip = false;
                    Buttons[i, j].IsNeighbor = false;
                }
            foreach (var e in PlayerShipsAvailableToSet)
                LeftShipsToKill += e;
        }

        public bool CheckAvailableShips(int i)                     //проверяем кол-во кораблей типов выше заданного
        {
            bool result = false;
            for (int k = i + 1; k <= 3; k++)
            {
                if (PlayerShipsAvailableToSet[k] != 0)
                    result = true;
            }
            return result;
        }


        private bool CheckWait(int w_i, int w_j, int[] p)           //Если включен режим ожидания, то проверяем координаты нового корабля, старого и его связующих
        {
            bool result = false;
            if (w_i == p[0] & w_j == p[1])                          //p - координаты соседа, w_i w_j - координаты ожидаемого соседа
                result = true;
            else
            {
                for (int y = 0; y < Buttons[p[0], p[1]].RelativeCells.Count; y = y + 2)

                    if ((w_i == Buttons[p[0], p[1]].RelativeCells[y]) & (w_j == Buttons[p[0], p[1]].RelativeCells[y + 1]))
                        result = true;
            }
            return result;
        }

        private string TypeToString(int type)                        //преобразование типа корабля в строку
        {
            string pristavka = null;
            switch (type)
            {
                case 0:
                    pristavka = "Одно";
                    break;
                case 1:
                    pristavka = "Двух";
                    break;
                case 2:
                    pristavka = "Трех";
                    break;
                case 3:
                    pristavka = "Четырех";
                    break;
            }
            return (pristavka + "палубные ");

        }

        private void ShowMistake(int q, int w)                         //показываем игроку, что он обязан достроить определенный корабль, т.к. корабли данного типа закончились
        {
            Buttons[q, w].BackColor = Color.Yellow;
            for (int i = 0; i < Buttons[q, w].RelativeCells.Count; i = i + 2)
            {
                Buttons[Buttons[q, w].RelativeCells[i], Buttons[q, w].RelativeCells[i + 1]].BackColor = Color.Yellow;
            }
            MessageBox.Show(Convert.ToString(TypeToString(Buttons[q, w].Type)) + "корабли закончились. Достройте или удалите корабль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void MistakeIsSolved(int q, int w)                     //когда игрок устранил ошибку, то желтые клетки красим обратно в черный    
        {
            Buttons[q, w].BackColor = Color.Black;
            for (int i = 0; i < Buttons[q, w].RelativeCells.Count; i = i + 2)
            {
                Buttons[Buttons[q, w].RelativeCells[i], Buttons[q, w].RelativeCells[i + 1]].BackColor = Color.Black;
            }
        }

        public int CheckNeighborhood(ref int[] place, int i, int j, ref bool CheckDiagonal)
        {
            int count = 0;

            if (j + 1 < 15)                                            //проверяем окрестность(квадрат) вокруг корабля              
                if (Buttons[i, j + 1].IsOccupied)
                {
                    count++;
                    place[0] = i;
                    place[1] = j + 1;
                }
            if (j - 1 >= 0)
                if (Buttons[i, j - 1].IsOccupied)
                {
                    count++;
                    place[0] = i;
                    place[1] = j - 1;
                }
            if (j + 1 < 15 & i + 1 < 15)
                if (Buttons[i + 1, j + 1].IsOccupied)
                {
                    count++;
                    place[0] = i + 1;
                    place[1] = j + 1;
                    CheckDiagonal = true;
                }
            if (j - 1 >= 0 & i - 1 >= 0)
                if (Buttons[i - 1, j - 1].IsOccupied)
                {
                    count++;
                    place[0] = i - 1;
                    place[1] = j - 1;
                    CheckDiagonal = true;
                }
            if (i + 1 < 15 & j - 1 >= 0)
                if (Buttons[i + 1, j - 1].IsOccupied)
                {
                    count++;
                    place[0] = i + 1;
                    place[1] = j - 1;
                    CheckDiagonal = true;
                }
            if (i - 1 >= 0 & j + 1 < 15)
                if (Buttons[i - 1, j + 1].IsOccupied)
                {
                    count++;
                    place[0] = i - 1;
                    place[1] = j + 1;
                    CheckDiagonal = true;
                }
            if (i + 1 < 15)
                if (Buttons[i + 1, j].IsOccupied)
                {
                    count++;
                    place[0] = i + 1;
                    place[1] = j;
                }
            if (i - 1 >= 0)
                if (Buttons[i - 1, j].IsOccupied)
                {
                    count++;
                    place[0] = i - 1;
                    place[1] = j;
                }

            return count;
        }

        private void DeleteColorMistake(int[] place)
        {
            Buttons[place[0], place[1]].BackColor = Color.Black;
            for (int i = 0; i < Buttons[place[0], place[1]].RelativeCells.Count; i = i + 2)
            {
                if (Buttons[Buttons[place[0], place[1]].RelativeCells[i], Buttons[place[0], place[1]].RelativeCells[i + 1]].BackColor == Color.Yellow)
                    Buttons[Buttons[place[0], place[1]].RelativeCells[i], Buttons[place[0], place[1]].RelativeCells[i + 1]].BackColor = Color.Black;
            }
        }

        private void SeekCoordinatesToDelete(List<int> list, int q, int w)
        {
            int t = 0;
            int y = 0;
            for (int i = 0; i < list.Count; i = i + 2)
            {
                if ((list[i] == q) & (list[i + 1] == w))
                {
                    t = i;
                    y = i + 1;
                }
            }
            list.RemoveAt(t);                                                           //когда мы удаляем элемент из листа, следующий индекс эелемента уменьшается на 1 ( по сути y-1=t)
            list.RemoveAt(y - 1);
        }

        private bool CheckWaitToDelete(int w_i, int w_j, int i, int j)
        {
            bool result = false;
            if ((w_i == i) & (w_j == j))
                result = true;
            else
            {
                for (int k = 0; k < Buttons[w_i, w_j].RelativeCells.Count; k = k + 2)
                {
                    if ((Buttons[w_i, w_j].RelativeCells[k] == i) & (Buttons[w_i, w_j].RelativeCells[k + 1] == j))
                        result = true;
                }
            }
            return result;
        }

        public bool SetShip(int i, int j)
        {
            bool result = false;
            bool CheckDiagonal = false;
            int count = 0;                                                             //число соседних кораблей
            int[] place = new int[2];                                                  //координаты соседнего корабля

            if (Buttons[i, j].IsOccupied == false)                                         //Если корабль еще не поставле в ячейку
            {
                count = CheckNeighborhood(ref place, i, j, ref CheckDiagonal);         //проверяем окрестность(квадрат) вокруг корабля

                if (count == 0 & CheckAvailableShips(-1) & waiting == false)           //если  нет соседей, свободен хотя бы один корабль 0 типа и выше и не ждем достройки корабля
                {
                    result = true;
                    if (PlayerShipsAvailableToSet[0] == 0 & CheckAvailableShips(0))    //если закончились однопалубные корабли, но есть корабли типов выше то входим в режим ОЖИДАНИЯ
                    {
                        waiting = true;
                        waiting_i = i;
                        waiting_j = j;
                    }
                    PlayerShipsAvailableToSet[0]--;                                    //убираем один однопалубный корабль из возможным для установки
                    Buttons[i, j].Type = 0;                                            //в ячейке однопалубный корабль
                    Buttons[i, j].IsShip = true;
                }


                if (count == 1 & !CheckDiagonal)
                {
                    if (((waiting) & (CheckWait(waiting_i, waiting_j, place))) | (!waiting))
                    {
                        if (CheckAvailableShips(Buttons[place[0], place[1]].Type))                    //если есть доступные корабли на порядок выше уже стоящего
                        {
                            if (waiting)
                                DeleteColorMistake(place);
                            result = true;
                            waiting = false;

                            if (PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type + 1] == 0) //если закончились type+1 корабли, но есть корабли типов выше то входим в режим ОЖИДАНИЯ
                            {
                                waiting = true;
                                waiting_i = i;
                                waiting_j = j;
                            }
                            //добавляем один type корабль из возможных для установки и убираем type + 1
                            PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type]++;
                            PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type + 1]--;

                            Buttons[i, j].IsShip = true;

                            //инкрементируем тип свсязующих корабля-соседки
                            for (int y = 0; y < Buttons[place[0], place[1]].RelativeCells.Count; y = y + 2)
                            {
                                Buttons[Buttons[place[0], place[1]].RelativeCells[y], Buttons[place[0], place[1]].RelativeCells[y + 1]].Type++;
                            }

                            //инкрементируем тип соседки-корабля и приравниваем к самому кораблю
                            Buttons[place[0], place[1]].Type++;
                            Buttons[i, j].Type = Buttons[place[0], place[1]].Type;


                            Buttons[i, j].RelativeCells = new List<int>(Buttons[place[0], place[1]].RelativeCells); //передаем новой части корабля старые части !!!!!!!!

                            Buttons[i, j].RelativeCells.Add(place[0]);                                              //дополняем лист новой части старой-соседкой-частью
                            Buttons[i, j].RelativeCells.Add(place[1]);

                            for (int k = 0; k < Buttons[i, j].RelativeCells.Count; k = k + 2)                       //а старым передаем новую часть корабля
                            {
                                Buttons[Buttons[i, j].RelativeCells[k], Buttons[i, j].RelativeCells[k + 1]].RelativeCells.Add(i);
                                Buttons[Buttons[i, j].RelativeCells[k], Buttons[i, j].RelativeCells[k + 1]].RelativeCells.Add(j);
                            }
                        }
                    }
                }
            }

            if ((waiting) & (!result) & (!CheckDiagonal) & (Buttons[i, j].IsOccupied == false))                       //показываем игроку, что он обязан достроить определенный корабль, т.к. корабли данного типа закончились
                ShowMistake(waiting_i, waiting_j);                              //ожидаемый сосед и все его связующие окрашиваются в желтый и всплывает ошибка
            return result;
        }


        public void DeleteShip(int i, int j)
        {
            int[] place = new int[2];                                                 //координаты соседнего корабля
            int count;
            bool CheckDiagonal = false;                                               //это переменная роли не играет в данном методе, нужна для передачи в другой метод

            if (Buttons[i, j].IsOccupied == true)                                         //Если есть корабль в ячейке
            {
                count = CheckNeighborhood(ref place, i, j, ref CheckDiagonal);

                if ((count == 0 & waiting == false) | (count == 0 & waiting & waiting_i == i & waiting_j == j))
                {
                    PlayerShipsAvailableToSet[0]++;                                    //добавляем один однопалубный корабль из возможным для установки
                    Buttons[i, j].IsShip = false;
                    Buttons[i, j].BackColor = Color.LightBlue;
                    Sound.play_delete();
                    if (waiting)
                        waiting = false;
                }

                if (count == 1)
                {
                    if (((waiting) & (CheckWaitToDelete(waiting_i, waiting_j, i, j))) | (!waiting))
                    {
                        if (waiting)
                        {
                            DeleteColorMistake(place);
                        }

                        waiting = false;

                        if (PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type - 1] <= 0) //если закончились type-1 корабли то входим в режим ОЖИДАНИЯ
                        {
                            waiting = true;
                            waiting_i = place[0];
                            waiting_j = place[1];
                        }
                        //добавляем один type корабль из возможных для установки и убираем type - 1
                        PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type]++;
                        PlayerShipsAvailableToSet[Buttons[place[0], place[1]].Type - 1]--;

                        Buttons[i, j].IsShip = false;
                        Buttons[i, j].BackColor = Color.LightBlue;
                        Sound.play_delete();

                        for (int y = 0; y < Buttons[place[0], place[1]].RelativeCells.Count; y = y + 2)        //декрементируем тип свсязующих корабля-соседки
                        {
                            Buttons[Buttons[place[0], place[1]].RelativeCells[y], Buttons[place[0], place[1]].RelativeCells[y + 1]].Type--;
                        }

                        Buttons[place[0], place[1]].Type--;                                                     //декрементируем тип соседки-корабля

                        for (int k = 0; k < Buttons[i, j].RelativeCells.Count; k = k + 2)                       //у старых частей удаляем клетку, по которой пкм кликнули
                        {
                            SeekCoordinatesToDelete(Buttons[Buttons[i, j].RelativeCells[k], Buttons[i, j].RelativeCells[k + 1]].RelativeCells, i, j);
                        }

                    }
                    if (waiting)                     //показываем игроку, что он обязан достроить определенный корабль, т.к. корабли данного типа закончились
                        ShowMistake(waiting_i, waiting_j);           //ожидаемый сосед и все его связующие окрашиваются в желтый и всплывает ошибка
                }
            }
        }


    }


}


