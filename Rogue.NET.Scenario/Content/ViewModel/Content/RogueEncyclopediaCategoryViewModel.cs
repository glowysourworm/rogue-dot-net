using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class RogueEncyclopediaCategoryViewModel : Image, INotifyPropertyChanged
    {
        string _displayName;
        string _categoryName;
        private DungeonMetaDataObjectTypes _type;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string CategoryName
        {
            get { return _categoryName; }
            set { this.RaiseAndSetIfChanged(ref _categoryName, value); }
        }

        public ObservableCollection<ScenarioMetaDataViewModel> Items { get; set; }

        public RogueEncyclopediaCategoryViewModel()
        {
            this.Height = ModelConstants.CELLHEIGHT * 2;
            this.Width = ModelConstants.CELLWIDTH * 2;

            this.Items = new ObservableCollection<ScenarioMetaDataViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}
