using FontCollection.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FontCollection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel = MainWindowViewModel.Load();

            this.DataContext = this.ViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ViewModel.Save();
        }

        private void lstFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = this.lstFonts.SelectedItem as FontFamily;

            if (selection == null)
            {
                return;
            }

            this.lblPreview.FontFamily = selection;
            this.ViewModel.SelectFont(selection);
        }

        private void mnuAddCollection_Click(object sender, RoutedEventArgs e)
        {
            var diagAdd = new AddCollection();
            
            if (diagAdd.ShowDialog() != true)
            {
                return;
            }

            try
            {
                this.ViewModel.AddCollection(diagAdd.CollectionName);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mnuRemoveCollection_Click(object sender, RoutedEventArgs e)
        {
            var selection = this.cboCollections.SelectedItem as ViewModels.FontCollection;

            if (selection == null || selection.Name == "ALL")
            {
                MessageBox.Show("Choose a collection to delete!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Are you sure to delete that collection?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            {
                return;
            }

            this.ViewModel.DeleteCollection(selection);
        }

        private void cboCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = this.cboCollections.SelectedItem as ViewModels.FontCollection;

            this.ViewModel.FilterFontList(selection);
        }

        private void chkFontCollectionBinding_Checked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            var collectionAndFamilyName = chk.Tag as KeyValuePair<string, string>?;

            if (collectionAndFamilyName == null)
            {
                return;
            }

            this.ViewModel.BindFontAndCollection(collectionAndFamilyName.Value.Value, collectionAndFamilyName.Value.Key, chk.IsChecked == true);
        }
    }
}
