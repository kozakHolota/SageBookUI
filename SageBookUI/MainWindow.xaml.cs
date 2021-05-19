using SageBookUI.DataModels;
using SageBookUI.Modals;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SageBookUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Sage> Sages { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public MainWindow()
        {
            Sages = new ObservableCollection<Sage>();
            Books = new ObservableCollection<Book>();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SageItemWindow addSageDialog = new SageItemWindow();
            _ = addSageDialog.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BookItemWindow addBookWindowDialog = new BookItemWindow();
            _ = addBookWindowDialog.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string sageName = sageNameField.Text;
            string sageCity = sageCityField.Text;
            Sages.Clear();
            using (SgDbContext sbc = new SgDbContext())
            {
                Sages.Clear();
               if (sageName != "" && sageCity != "") foreach (var sage in sbc.Sages.Where(p => p.Name == sageName).Where(p => p.City == sageCity).ToList()) Sages.Add(sage);
               else if(sageName != "" && sageCity == "") foreach (var sage in sbc.Sages.Where(p => p.Name == sageName).ToList()) Sages.Add(sage);
               else if(sageName == "" && sageCity != "") foreach (var sage in sbc.Sages.Where(p => p.City == sageCity).ToList()) Sages.Add(sage);
               else foreach (var sage in sbc.Sages.Select(p=>p).ToList()) Sages.Add(sage);
                resultGrid.ItemsSource = Sages;
                resultGrid.Items.Refresh();
            }
                
        }

        private void resultGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((Button)FindName("editButton")).IsEnabled = true;
            ((Button)FindName("deleteButton")).IsEnabled = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Books.Clear();
            string bookTitle = bookTitleField.Text;
            string bookAuthor = bookAuthorField.Text;
            using (SgDbContext sbc = new SgDbContext())
            {
                if (bookTitle != "" && bookAuthor != "") foreach (var book in sbc.Books.Where(p => p.Sages.Any(s => s.Name == bookAuthor)).ToList().Where(p => p.Title == bookTitle).ToList()) Books.Add(book);
                else if (bookTitle == "" && bookAuthor != "") foreach (var book in sbc.Books.Where(p => p.Title == bookTitle).ToList()) Books.Add(book);
                else if (bookTitle != "" && bookAuthor == "") foreach (var book in sbc.Books.Where(p => p.Sages.Any(s => s.Name == bookAuthor)).ToList()) Books.Add(book);
                else foreach (var book in sbc.Books.Select(p=>p).ToList()) Books.Add(book);
                resultGrid.ItemsSource = Books;
                resultGrid.Items.Refresh();
            }               
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Would you like to delete the entry?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            using (SgDbContext sbc = new SgDbContext()) {
                if (resultGrid.ItemsSource.GetType() == typeof(ObservableCollection<Sage>))
                {
                    Sage s = sbc.Sages.Where(p => p.Id == ((Sage)resultGrid.SelectedItem).Id).FirstOrDefault();
                    sbc.Sages.Remove(s);
                    Sages.Remove(s);
                    resultGrid.ItemsSource = Sages;
                }
                else {
                    Book b = sbc.Books.Where(p=>p.Id == ((Book)resultGrid.SelectedItem).Id).FirstOrDefault();
                    sbc.Books.Remove(b); 
                    Books.Remove(b);
                    resultGrid.ItemsSource = Books;
                }
                sbc.SaveChanges();
             }
            resultGrid.Items.Refresh();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            using (SgDbContext sbc = new SgDbContext())
            {
                if (resultGrid.ItemsSource.GetType() == typeof(ObservableCollection<Sage>))
                {
                    Sage selectedSage = resultGrid.SelectedItem as Sage;
                    SageItemWindow addSageDialog = new SageItemWindow(buttonText: "Edit", sageDefaultName: selectedSage.Name, sageCityDefault: selectedSage.City, sageAgeDefault: selectedSage.Age.ToString(), isNewItem: false, sageId: selectedSage.Id);
                    _ = addSageDialog.ShowDialog();
                }
                else if(resultGrid.ItemsSource.GetType() == typeof(ObservableCollection<Book>)) {
                    Book selectedBook = resultGrid.SelectedItem as Book;
                    BookItemWindow bookItemWindow = new BookItemWindow(buttonText: "Edit Book", bookName: selectedBook.Title, bookDescription: selectedBook.Description, isNewItem: false, bookId: selectedBook.Id);
                    _ = bookItemWindow.ShowDialog();
                }
            }
        }
    }
}
