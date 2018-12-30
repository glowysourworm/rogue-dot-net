﻿using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Rogue.NET.Core.Service.Interface;
using System.Windows.Media;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Common.ViewModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class RogueEncyclopediaCategoryViewModel : Image, INotifyPropertyChanged
    {
        const int PAGE_SIZE = 8;

        string _categoryName;
        string _categoryDescription;
        string _categoryDisplayName;

        public string CategoryDescription
        {
            get { return this.IsIdentifiedCategory ? _categoryDescription : "???"; }
            set { this.RaiseAndSetIfChanged(ref _categoryDescription, value); }
        }
        public string CategoryName
        {
            get { return _categoryName; }
            set { this.RaiseAndSetIfChanged(ref _categoryName, value); }
        }
        public string CategoryDisplayName
        {
            get { return this.IsIdentifiedCategory ? _categoryDisplayName : "???"; }
            set { this.RaiseAndSetIfChanged(ref _categoryDisplayName, value); }
        }
        public double PercentComplete
        {
            get { return this.Items.PagedCount(x => x.IsIdentified) / (double)this.Items.PagedCount(x => true); }
        }
        public bool IsIdentifiedCategory
        {
            get { return this.Items.PagedAny(x => x.IsIdentified); }
        }
        public bool IsObjectiveCategory
        {
            get { return this.CategoryName == "Objective"; }
        }

        public PagedObservableCollection<ScenarioMetaDataViewModel> Items { get; set; }

        public RogueEncyclopediaCategoryViewModel(IScenarioResourceService scenarioResourceService)
        {
            this.Height = ModelConstants.CellHeight * 2;
            this.Width = ModelConstants.CellWidth * 2;

            this.Items = new PagedObservableCollection<ScenarioMetaDataViewModel>(PAGE_SIZE);

            // Initialize the category as not known
            this.Source = scenarioResourceService.GetImageSource(new ScenarioImage("", "?", Colors.White.ToString()));
        }

        /// <summary>
        /// Invalidates the calculated parameters and the image - which is a "?" until one of the
        /// items is identified. After that it's set to the first-or-default image in the category
        /// </summary>
        public void Invalidate()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("PercentComplete"));
                PropertyChanged(this, new PropertyChangedEventArgs("CategoryDescription"));
                PropertyChanged(this, new PropertyChangedEventArgs("CategoryDisplayName"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsIdentifiedCategory"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsObjectiveCategory"));
            }

            this.Source = this.Items.PagedFirstOrDefault(x => x.IsIdentified)?.Source ?? this.Source;
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
