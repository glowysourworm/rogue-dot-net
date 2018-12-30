using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.Windows.Threading;
using System.Windows.Input;
using Prism.Commands;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class DungeonEncyclopedia : UserControl
    {
        public static readonly DependencyProperty CategoryPageNumberProperty =
            DependencyProperty.Register("CategoryPageNumber", typeof(int), typeof(DungeonEncyclopedia));

        public static readonly DependencyProperty EntryPageNumberProperty =
            DependencyProperty.Register("EntryPageNumber", typeof(int), typeof(DungeonEncyclopedia));

        public static readonly DependencyProperty CategoryPageCountProperty =
            DependencyProperty.Register("CategoryPageCount", typeof(int), typeof(DungeonEncyclopedia));

        public static readonly DependencyProperty EntryPageCountProperty =
            DependencyProperty.Register("EntryPageCount", typeof(int), typeof(DungeonEncyclopedia));

        public int CategoryPageNumber
        {
            get { return (int)GetValue(CategoryPageNumberProperty); }
            set { SetValue(CategoryPageNumberProperty, value); }
        }
        public int EntryPageNumber
        {
            get { return (int)GetValue(EntryPageNumberProperty); }
            set { SetValue(EntryPageNumberProperty, value); }
        }
        public int CategoryPageCount
        {
            get { return (int)GetValue(CategoryPageCountProperty); }
            set { SetValue(CategoryPageCountProperty, value); }
        }
        public int EntryPageCount
        {
            get { return (int)GetValue(EntryPageCountProperty); }
            set { SetValue(EntryPageCountProperty, value); }
        }

        public ICommand CategoryNextPageCommand { get; set; }
        public ICommand EntryNextPageCommand { get; set; }
        public ICommand CategoryPreviousPageCommand { get; set; }
        public ICommand EntryPreviousPageCommand { get; set; }

        private RogueEncyclopediaCategoryViewModel _selectedCategory;

        [ImportingConstructor]
        public DungeonEncyclopedia(RogueEncyclopediaViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();

            this.CategoryLB.SelectionChanged += CategoryLB_SelectionChanged;

            // Paging for categories and entries
            this.CategoryNextPageCommand = new DelegateCommand(() =>
            {
                viewModel.Categories.NextPage();

                // Update page number
                this.CategoryPageNumber = viewModel.Categories.PageNumber;

                // Select a category -> Select Entry
                DispatcherInvoke(SelectFirstCategory);
            });
            this.CategoryPreviousPageCommand = new DelegateCommand(() =>
            {
                viewModel.Categories.PreviousPage();

                // Update page number
                this.CategoryPageNumber = viewModel.Categories.PageNumber;

                // Select a category -> Select Entry
                DispatcherInvoke(SelectFirstCategory);
            });
            this.EntryNextPageCommand = new DelegateCommand(() =>
            {
                if (_selectedCategory != null)
                {
                    _selectedCategory.Items.NextPage();

                    // Update page number
                    this.EntryPageNumber = _selectedCategory.Items.PageNumber;

                    // Select an Entry
                    DispatcherInvoke(SelectFirstEntry);
                }
            });
            this.EntryPreviousPageCommand = new DelegateCommand(() =>
            {
                if (_selectedCategory != null)
                {
                    _selectedCategory.Items.PreviousPage();

                    // Update page number
                    this.EntryPageNumber = _selectedCategory.Items.PageNumber;

                    // Select an Entry
                    DispatcherInvoke(SelectFirstEntry);
                }
            });

            // FORCE WAIT TO APPLICATION IDLE (DON'T LIKE THIS...)
            //
            // **Issue was that the UI wasn't animating the ellipse panel to center the
            //   selected item. Never figured out how to force timing for this.
            this.Loaded += (sender, e) =>
            {
                DispatcherInvoke(() =>
                {
                    // Reset the page number in case it was previously used
                    viewModel.Categories.ResetPageNumber();

                    // Initialize paging data
                    this.CategoryPageCount = viewModel.Categories.PageCount;
                    this.CategoryPageNumber = viewModel.Categories.PageNumber;

                    SelectFirstCategory();
                });
            };
        }

        private void SelectFirstCategory()
        {
            var viewModel = this.DataContext as RogueEncyclopediaViewModel;
            if (viewModel == null)
                return;

            // Select Initial Category
            _selectedCategory = viewModel.Categories.GetCurrentPage().FirstOrDefault();
            if (_selectedCategory != null)
            {
                //  Reset the page number
                _selectedCategory.Items.ResetPageNumber();

                this.CategoryLB.SelectedItem = _selectedCategory;
            }
        }

        private void SelectFirstEntry()
        {
            // Select Initial Entry
            if (_selectedCategory != null)
            {
                this.CategoryEntryLB.SelectedItem = _selectedCategory.Items.GetCurrentPage().FirstOrDefault(x => x.IsIdentified) ??
                                                    _selectedCategory.Items.GetCurrentPage().FirstOrDefault();

                this.EntryPageNumber = _selectedCategory.Items.PageNumber;
                this.EntryPageCount = _selectedCategory.Items.PageCount;
            }
        }

        // NOTE*** Wrapping all calls to select entry or category in a Dispatcher.Invoke on the
        //         Application Idle event. This seems to resolve timing problems for animation.
        private void DispatcherInvoke(Action action)
        {
            Dispatcher.Invoke(action, DispatcherPriority.ApplicationIdle);
        }

        // Select first or default identified item
        private void CategoryLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _selectedCategory = e.AddedItems[0] as RogueEncyclopediaCategoryViewModel;
                _selectedCategory.Items.ResetPageNumber();

                // () -> Select the first entry that is identified
                DispatcherInvoke(SelectFirstEntry);
            }
        }
    }
}
