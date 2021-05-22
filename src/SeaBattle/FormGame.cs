using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle
{
    public partial class FormGame : Form
    {
        GameLogic _gl = new GameLogic();     //класс для постановки или удаления кораблей
        Bot _bot = new Bot();
        Sound snd = new Sound();
        private bool game_started = false;
        private int dk = 225;                          //кол-во всех клеток минус кол-во клеток, в которые бот уже выбирал
        bool game_end = false;
        bool win = true;
        int last_x;
        int last_y;
        int direction = -1;
        int x1;
        int y1;
        private static Color DAMAGED_COLOR = Color.Yellow;

        public FormGame()
        {
            InitializeComponent();
            snd.Init();
            if (Sound.sound_enabled)
            {
                checkBox1.Checked = true;
                checkBox1.Text = "Звук есть";
                snd.PlayBackground();
            }
            else
            {
                checkBox1.Checked = false;
                checkBox1.Text = "Звука нет";
                snd.StopBackground();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                snd.sound_on();
                checkBox1.Text = "Звук есть";
                snd.play_key();
                snd.PlayBackground();
            }
            else
            {
                snd.sound_off();
                checkBox1.Text = "Звука нет";
                snd.StopBackground();
            }

        }

        private void FormGame_Load(object sender, EventArgs e)
        {

            label_ship1.Text = "1-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[0]);
            label_ship2.Text = "2-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[1]);
            label_ship3.Text = "3-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[2]);
            label_ship4.Text = "4-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[3]);

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    SuperButton baton = new SuperButton(i, j) { Margin = new Padding(0) };
                    baton.MouseUp += Baton_Click;

                    tableLayoutPanel1.Controls.Add(baton, i, j);
                    baton.BackColor = Color.LightBlue;
                    _gl.Buttons[i, j] = baton;
                }
            }

            _gl.InitLogic();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    SuperButton baton_2 = new SuperButton(i, j) { Margin = new Padding(0) };
                    baton_2.Click += Baton_2_Click;

                    tableLayoutPanel2.Controls.Add(baton_2, i, j);
                    baton_2.BackColor = Color.LightBlue;
                    _bot.Buttons[i, j] = baton_2;
                }
            }
            _bot.InitLogic();
            _bot.GenerateShips();
        }

        private void Baton_Click(object sender, MouseEventArgs e)
        {
            if (!game_started)
            {
                var button = (SuperButton)sender;
                if (e.Button == MouseButtons.Left)
                {

                    if (_gl.SetShip(button.X, button.Y))
                    {
                        snd.play_key();
                        if (!(button.BackColor == Color.Yellow))
                            button.BackColor = Color.Black;
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    _gl.DeleteShip(button.X, button.Y);
                }

                if (!_gl.CheckAvailableShips(-1))                                           //Если все корабли поставлены, то кнопка начать доступна для нажатия
                    button_start_game.Enabled = true;
                else
                    button_start_game.Enabled = false;

                if (_gl.PlayerShipsAvailableToSet[0] < 0)
                    label_ship1.Text = "1-палубные Х" + Convert.ToString(0);
                else
                    label_ship1.Text = "1-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[0]);
                if (_gl.PlayerShipsAvailableToSet[1] < 0)
                    label_ship2.Text = "2-палубные Х" + Convert.ToString(0);
                else
                    label_ship2.Text = "2-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[1]);
                if (_gl.PlayerShipsAvailableToSet[2] < 0)
                    label_ship3.Text = "3-палубные Х" + Convert.ToString(0);
                else
                    label_ship3.Text = "3-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[2]);
                if (_gl.PlayerShipsAvailableToSet[3] < 0)
                    label_ship4.Text = "4-палубные Х" + Convert.ToString(0);
                else
                    label_ship4.Text = "4-палубные Х" + Convert.ToString(_gl.PlayerShipsAvailableToSet[3]);
            }
        }

        private void button_start_game_Click(object sender, EventArgs e)
        {
            snd.play_key();
            button_start_game.Enabled = false;
            game_started = true;
        }

        private void DeleteList(int i, int j)             //находим объект в листе и удаляем его
        {
            for (int g = 0; g < _bot.AvailableCellsToAttack.Count; g++)
            {
                if ((_bot.AvailableCellsToAttack[g][0] == i) & (_bot.AvailableCellsToAttack[g][1] == j))
                {
                    dk--;
                    _bot.AvailableCellsToAttack.RemoveAt(g);
                }
            }
        }

        private bool CheckList(int i, int j)
        {
            bool result = false;
            for (int g = 0; g < _bot.AvailableCellsToAttack.Count; g++)
            {
                if ((_bot.AvailableCellsToAttack[g][0] == i) & (_bot.AvailableCellsToAttack[g][1] == j))
                {
                    result = true;
                }
            }
            return result;
        }


        private void DeleteOkrestnost(int i, int j)                //делает недоступными для бота клетки вокруг убитого корабля игрока и саму эту клетку.
        {
            DeleteList(i, j);
            if (j + 1 < 15)
                DeleteList(i, j + 1);
            if (j - 1 >= 0)
                DeleteList(i, j - 1);
            if (j + 1 < 15 & i + 1 < 15)
                DeleteList(i + 1, j + 1);
            if (j - 1 >= 0 & i - 1 >= 0)
                DeleteList(i - 1, j - 1);
            if (i + 1 < 15 & j - 1 >= 0)
                DeleteList(i + 1, j - 1);
            if (i - 1 >= 0 & j + 1 < 15)
                DeleteList(i - 1, j + 1);
            if (i + 1 < 15)
                DeleteList(i + 1, j);
            if (i - 1 >= 0)
                DeleteList(i - 1, j);
        }

        private bool ProverkaDirect(int direction, int x, int y)
        {
            bool result = false;

            if ((direction == 0) & (CheckList(x + 1, y)) & (x + 1 < 15))
                result = true;
            if ((direction == 1) & (CheckList(x, y + 1)) & (y + 1 < 15))
                result = true;
            if ((direction == 2) & (CheckList(x - 1, y)) & (x - 1 >= 0))
                result = true;
            if ((direction == 3) & (CheckList(x, y - 1)) & (y - 1 >= 0))
                result = true;

            return result;
        }

        private void ChangeColor(int x, int y)
        {
            for (int i = 0; i < _gl.Buttons[x, y].RelativeCells.Count; i = i + 2)
            {
                _gl.Buttons[_gl.Buttons[x, y].RelativeCells[i], _gl.Buttons[x, y].RelativeCells[i + 1]].BackColor = Color.Red;
            }
            _gl.Buttons[x, y].BackColor = Color.Red;
        }

        private void Baton_2_Click(object sender, EventArgs e)
        {
            var button_2 = (SuperButton)sender;

            if ((game_started) & (!(_bot.Buttons[button_2.X, button_2.Y].BackColor == Color.Red)))
            {
                snd.play_key();
                if (!game_end)
                {

                    //ход игрока
                        var shotResult = _bot.PlayerShot(button_2.X, button_2.Y);
                        switch (shotResult)
                        {
                            case 0:
                                button_2.BackColor = DAMAGED_COLOR;
                                break;
                            case 1:
                                button_2.BackColor = Color.Gray;
                                break;
                            case 2:
                            _gl.LeftShipsToKill--;
                            _bot.ChangeColor(button_2.X, button_2.Y);
                                break;
                            default:
                                break;
                    }
                    //ход бота
                    if (!_bot.popal)                    //если в прошлый раз бот не попал
                    {
                        Random random = new Random();
                        int koord = random.Next(dk);
                        int x = _bot.AvailableCellsToAttack[koord][0];
                        int y = _bot.AvailableCellsToAttack[koord][1];
                        if (_gl.Buttons[x, y].IsOccupied)  //если бот попал
                        {
                            if (_gl.Buttons[x, y].Type == 0)                     //добил
                            {
                                _gl.Buttons[x, y].BackColor = Color.Red;
                                _bot.left_ships--;
                                DeleteOkrestnost(x, y);
                            }
                            else                                     //не добил
                            {
                                _gl.Buttons[x, y].BackColor = Color.Yellow;
                                _bot.popal = true;
                                last_x = x;
                                last_y = y;
                                x1 = x;
                                y1 = y;
                                _bot.type_ship_now = _gl.Buttons[x, y].Type;
                                _bot.type_ship_now--;
                            }
                        }
                        else
                        {
                            _gl.Buttons[x, y].BackColor = Color.Gray;
                            DeleteList(x, y);
                        }


                    }
                    else                                   //бот в прошлый раз попал
                    {
                        if (direction == -1)              //если направление следующего хода не задано
                        {
                            do
                            {
                                Random r = new Random();
                                direction = r.Next(4);         //выбираем следующий ход, но с условием незанятости клетки: 0 - вправо, 1 - вниз, 2 - влево, 3 - вверх
                            }
                            while (!ProverkaDirect(direction, last_x, last_y));
                        }
                        if (direction > -1)
                        {
                            int koefx = 0;
                            int koefy = 0;
                            int smenadirect = 0;
                            if (_gl.Buttons[last_x, last_y].Type - _bot.type_ship_now == 1)    //если подбита всего 1 клетка корабля, то направление изменяется по часовой стрелке
                            {
                                while (!ProverkaDirect(direction, last_x, last_y))
                                {
                                    direction++;
                                    if (direction == 4)
                                        direction = 0;
                                }
                                smenadirect = direction + 1;
                                if (smenadirect == 4)
                                    smenadirect = 0;
                            }
                            else                                   //иначе направление изменяется зеркально ( лево - право )
                            {
                                if (direction == 0)
                                {
                                    smenadirect = 2;
                                }
                                if (direction == 1)
                                {
                                    smenadirect = 3;
                                }
                                if (direction == 2)
                                {
                                    smenadirect = 0;
                                }
                                if (direction == 3)
                                {
                                    smenadirect = 1;
                                }
                            }

                            if (direction == 0)
                                koefx = 1;
                            if (direction == 1)
                                koefy = 1;
                            if (direction == 2)
                                koefx = -1;
                            if (direction == 3)
                                koefy = -1;

                            if (!ProverkaDirect(direction, last_x, last_y))
                            {

                                direction = smenadirect;
                                last_x = x1;
                                last_y = y1;

                            }

                            if (_gl.Buttons[last_x + koefx, last_y + koefy].IsOccupied)
                            {
                                if (!(_bot.type_ship_now == 0))                   //попал но не добил
                                {
                                    _gl.Buttons[last_x + koefx, last_y + koefy].BackColor = Color.Yellow;

                                    _bot.type_ship_now--;

                                    last_x = last_x + koefx;
                                    last_y = last_y + koefy;


                                }
                                else                                     //если добил
                                {
                                    _bot.popal = false;

                                    ChangeColor(last_x, last_y);           //Окрашивание всех клеток в красный    

                                    _bot.left_ships--;

                                    _bot.type_ship_now = 0;

                                    DeleteOkrestnost(last_x + koefx, last_y + koefy);                 //удаляем из доступных для атаки все клетки вокург убитого корабля и сам корабль
                                    for (int k = 0; k < _gl.Buttons[last_x + koefx, last_y + koefy].RelativeCells.Count; k = k + 2)
                                    {
                                        DeleteOkrestnost(_gl.Buttons[last_x + koefx, last_y + koefy].RelativeCells[k], _gl.Buttons[last_x + koefx, last_y + koefy].RelativeCells[k + 1]);
                                    }
                                }
                            }
                            else                                         //не попал, идем в другую сторону от первого попаданияв данный корабль
                            {
                                _gl.Buttons[last_x + koefx, last_y + koefy].BackColor = Color.Gray;

                                DeleteList(last_x + koefx, last_y + koefy);

                                direction = smenadirect;
                                last_x = x1;
                                last_y = y1;
                            }
                        }

                    }
                    if (_bot.left_ships == 0)
                    {
                        game_end = true;
                        win = false;
                    }
                    if(_gl.LeftShipsToKill == 0)
                        game_end = true;

                }
                if ((win) & (game_end))
                {
                    MessageBox.Show("ПОБЕДА!");
                    snd.play_win();
                }
                else if (game_end)
                {
                    MessageBox.Show("ВЫ ПРОИГРАЛИ!");
                    snd.play_fail();
                }
            }



        }

    }
}
