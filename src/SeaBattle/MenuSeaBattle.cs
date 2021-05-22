using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaBattle
{
    public partial class MenuSeaBattle : Form
    {
        Sound sound = new Sound();

        public MenuSeaBattle(System.Resources.ResourceManager rm)
        {
            InitializeComponent();
        }

        private void box_sound_CheckedChanged(object sender, EventArgs e)
        {
            if (box_sound.Checked)
            {
                sound.sound_on();
                box_sound.Text = "Звук есть";
                sound.play_key();
            }
            else
            {
                sound.sound_off();
                box_sound.Text = "Звука нет";
                sound.StopBackground();
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            
            sound.play_key();
            Hide();
            start_game();
            Show();
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            
            sound.play_key();  
            Close();
        }

        private void start_game()
        {
            FormGame game = new FormGame();
            game.ShowDialog();
        }
    }
}
