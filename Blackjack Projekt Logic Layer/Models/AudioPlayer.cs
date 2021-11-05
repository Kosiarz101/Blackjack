using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public class AudioPlayer
    {
        public string FileName { get; set; }
        public SoundPlayer Player { get; set; }
        public bool IsPlaying { get; set; } = false;
        public AudioPlayer(string fileName)
        {
            FileName = fileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Sounds");
            string finalPath = Path.Combine(path, fileName);
            Player = new SoundPlayer(finalPath);
        }
        public void StartMusic()
        {           
            Player.Play();
            IsPlaying = true;
        }
        public void StartMusicLooping()
        {
            Player.PlayLooping();
            IsPlaying = true;
        }
        public void StartDifferentMusic(string fileName)
        {
            FileName = fileName;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Sounds");
            string finalPath = Path.Combine(path, fileName);
            Player.SoundLocation = finalPath;
            Player.Play();
        }
        public void StopMusic()
        {
            Player.Stop();
            IsPlaying = false;
        }
    }
}
