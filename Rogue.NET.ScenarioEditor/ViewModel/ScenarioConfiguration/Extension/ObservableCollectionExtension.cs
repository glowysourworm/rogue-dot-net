using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Extension
{
    public static class ObservableCollectionExtension
    {
        public static ICollectionView CreateDefaultView<T>(this ObservableCollection<T> collection) where T : TemplateViewModel
        {
            var view = CollectionViewSource.GetDefaultView(collection);

            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            return view;
        }

        public static ICollectionView CreateView<T>(this ObservableCollection<T> collection, Func<T, bool> selector) where T : TemplateViewModel
        {
            var view = CollectionViewSource.GetDefaultView(collection);

            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            view.Filter = new Predicate<object>(obj =>
            {
                return selector(obj as T);
            });

            return view;
        }
    }
}
