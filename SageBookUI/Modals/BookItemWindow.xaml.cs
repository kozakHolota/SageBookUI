using SageBookUI.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace SageBookUI.Modals
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class BookItemWindow : Window
    {
        public ObservableCollection<Sage> Sages { get; set; }
        public ObservableCollection<Sage> SelectedSages { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        private int BookId { get; set; }
        public string ButtonText {get; set;}
        private bool IsNewItem { get; set; }
        public BookItemWindow(string buttonText="Add Book >>", string bookName="", string bookDescription="", bool isNewItem=true, int bookId=-1)
        {
            InitializeComponent();
            DataContext = this;
            Sages = new ObservableCollection<Sage>();
            SelectedSages = new ObservableCollection<Sage>();
            BookName = bookName;
            BookDescription = bookDescription;
            BookId = bookId;            
            foundSages.ItemsSource = Sages;
            sagesList.ItemsSource = SelectedSages;
            ButtonText = buttonText;
            IsNewItem = isNewItem;
            TextRange textRange = new TextRange(bookDescriptionField.Document.ContentStart, bookDescriptionField.Document.ContentEnd);
            textRange.Text = BookDescription;
            if (!IsNewItem)
            {
                using (SgDbContext sbc = new SgDbContext())
                {
                    foreach (var sage in sbc.Sages.Where(p => p.Books.Any(b=>b.Id == bookId))) SelectedSages.Add(sage);
                    sagesList.ItemsSource = SelectedSages;
                    sagesList.Items.Refresh();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Sages.Clear();
            foundSages.Items.Refresh();

            using (SgDbContext sbc = new SgDbContext())
            {
                string sName = sageName.Text;
                string sCity = sageCity.Text;

                ICollection<Sage> sages = new List<Sage>();
                if (sName != "" && sCity != "") sages = sbc.Sages.Where(p => p.Name == sName).Where(p => p.City == sCity).ToList();
                else if (sName != "" && sCity == "") sages = sbc.Sages.Where(p => p.Name == sName).ToList();
                else if (sName == "" && sCity != "") sages = sbc.Sages.Where(p => p.City == sCity).ToList();
                else sages = sbc.Sages.Select(p => p).ToList();
                foreach (var sg in sages) Sages.Add(sg);
            }

            foundSages.Items.Refresh();
        }

        private void foundSages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedSages.Add(((DataGrid)sender).SelectedItem as Sage);
            sagesList.Items.Refresh();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string bookName = ((TextBox)FindName("bookNameField")).Text;
            string bookDescription = ((RichTextBox)FindName("bookDescriptionField")).Document.Blocks.FirstOrDefault().ToString();

            if(bookName == "") MessageBox.Show("Please enter the book name");
            if (sagesList.Items.Count == 0) MessageBox.Show("The book has no author. Please add at least one author!");

            using(SgDbContext sbc = new SgDbContext())
            {
                if (IsNewItem) {
                    Book book = new Book { Title = bookName, Description = bookDescription, Sages = SelectedSages };
                    if (sbc.Books.Any(b => b.Title == book.Title && b.Description == book.Description && b.Sages == book.Sages)) MessageBox.Show("Book already exists");
                    else sbc.Books.Add(book); 
                }
                else
                {
                    Book book = sbc.Books.Where(b => b.Id == BookId).FirstOrDefault();
                    sbc.Books.Attach(book);
                    book.Title = bookNameField.Text;
                    book.Description = bookDescriptionField.Document.Blocks.FirstOrDefault().ToString();
                    book.Sages = SelectedSages;
                }
                sbc.SaveChanges();
            }

            DialogResult = true;
            Close();
        }
    }
}
