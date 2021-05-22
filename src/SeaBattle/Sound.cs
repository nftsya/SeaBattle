using System.Media;
using System.IO;
using WMPLib;

namespace SeaBattle
{
    public class Sound
    {
        private static WindowsMediaPlayer _player = new WindowsMediaPlayer();

        static SoundPlayer sound_fail = new SoundPlayer(Properties.Resources.SoundFail);
        static SoundPlayer sound_key = new SoundPlayer(Properties.Resources.SoundKey);
        static SoundPlayer sound_win = new SoundPlayer(Properties.Resources.win);
        static SoundPlayer sound_delete = new SoundPlayer(Properties.Resources.delete);

        static public bool sound_enabled = true;



        public void Init()
        {
           _player.settings.setMode("loop", true);

            IWMPPlaylist playlist = _player.playlistCollection.newPlaylist("backgroundPlaylist");
           
            playlist.appendItem(_player.newMedia(".\\Music\\background1.wav"));
            playlist.appendItem(_player.newMedia(".\\Music\\background3.wav"));
            playlist.appendItem(_player.newMedia(".\\Music\\background2.wav"));

            _player.currentPlaylist = playlist;
        }

        public void PlayBackground()
        {
            if (sound_enabled)
            _player.controls.play();
        }

        public void StopBackground()
        {
            _player.controls.stop();
        }

        public void sound_on()
        {
            sound_enabled = true;
        }

        public void sound_off()
        {
            sound_enabled = false;
        }

        public void play_fail()
        {
            if (sound_enabled)
                sound_fail.Play();
        }

        public void play_key()
        {
            if (sound_enabled)
                sound_key.Play();
        }

        public void play_win()
        {
            if (sound_enabled)
                sound_win.Play();
        }

        public static void play_delete()
        {
            if (sound_enabled)
                sound_delete.Play();
        }

    }
}
