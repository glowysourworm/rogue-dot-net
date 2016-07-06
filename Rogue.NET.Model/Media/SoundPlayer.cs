using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Model.Media
{
    public static class RogueSoundPlayer
    {
        private static MediaPlayer _player = new MediaPlayer();
        public enum SoundClips
        {
            AWitch,
            Tim,
            FleshWound,
            RunAway,
            Apology,
            WindChime
        }
        public static void PlaySound(SoundClips clip)
        {
            _player.Volume = 0;
            _player.IsMuted = false;

            switch (clip)
            {
                case SoundClips.AWitch:
                    _player.Open(new Uri("Sounds/witch.wav", UriKind.Relative));
                    _player.Play();
                    break;
                case SoundClips.FleshWound:
                    _player.Open(new Uri("Sounds/wound.wav", UriKind.Relative));
                    _player.Play();
                    break;
                case SoundClips.RunAway:
                    _player.Open(new Uri("Sounds/runaway.wav", UriKind.Relative));
                    _player.Play();
                    break;
                case SoundClips.Tim:
                    _player.Open(new Uri("Sounds/tim.wav", UriKind.Relative));
                    _player.Play();
                    break;
                case SoundClips.Apology:
                    _player.Open(new Uri("Sounds/apology.wav", UriKind.Relative));
                    _player.Play();
                    break;
                case SoundClips.WindChime:
                    _player.Open(new Uri("Sounds/windchime.wav", UriKind.Relative));
                    _player.Play();
                    break;
            }
        }
    }
}
