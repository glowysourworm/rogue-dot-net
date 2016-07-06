using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common
{
    /// <summary>
    /// Stores unlocked difficulties
    /// </summary>
    [Serializable]
    public class DifficultyKey : INotifyPropertyChanged
    {
        public static readonly string KEY_FILE_NAME = "id.r2k";

        public bool Medium
        {
            get { return _medium; }
            set
            {
                _medium = value;
                OnPropertyChanged("Medium");
            }
        }
        public bool Hard
        {
            get { return _hard; }
            set
            {
                _hard = value;
                OnPropertyChanged("Hard");
            }
        }
        public bool Brutal
        {
            get { return _brutal; }
            set
            {
                _brutal = value;
                OnPropertyChanged("Brutal");
            }
        }
        public bool Custom
        {
            get { return _custom; }
            set
            {
                _custom = value;
                OnPropertyChanged("Custom");
            }
        }
        public string FingerPrint { get; set; }

        bool _medium = false;
        bool _hard = false;
        bool _brutal = false;
        bool _custom = false;

        /// <summary>
        /// Initializes the contents of the key file
        /// </summary>
        /// <param name="cpuId">Unique ID of cpu</param>
        public DifficultyKey()
        {
            this.FingerPrint = Rogue.NET.Common.FingerPrint.CreateComputerFingerPrint();
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
