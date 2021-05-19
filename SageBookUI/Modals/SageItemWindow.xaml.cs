using SageBookUI.DataModels;
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
using System.Windows.Shapes;

namespace SageBookUI.Modals
{
    /// <summary>
    /// Interaction logic for AddSageWindow.xaml
    /// </summary>
    public partial class SageItemWindow : Window
    {
        public string SubmitButtonTitle { get; set; }
        public string SageDefaultName { get; set; }
        public string SageDefaultCity { get; set; }
        public string SageDefaultAge { get; set; }
        public int SageId { get; set; }
        private bool IsNewItem { get; set; }
        public SageItemWindow(string buttonText= "Add >>", string sageDefaultName="", string sageCityDefault="", string sageAgeDefault="", bool isNewItem=true, int sageId=-1)
        {
            DataContext = this;
            SubmitButtonTitle = buttonText;
            SageDefaultName = sageDefaultName;
            SageDefaultCity = sageCityDefault;
            SageDefaultAge = sageAgeDefault;
            SageId = sageId;
            IsNewItem = isNewItem;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!sageAge.Text.All(char.IsDigit)) MessageBox.Show($"Age should be a number. But is: {sageAge.Text}");
            else if(sageName.Text == null || sageCity == null) MessageBox.Show("All fields shooould be filled!");
            else
            {
                Sage _sage = new Sage { Name = sageName.Text, City = sageCity.Text, Age = int.Parse(sageAge.Text) };
                using (SgDbContext sbc = new SgDbContext())
                {
                    if (sbc.Sages.Where(s => s.Age == _sage.Age && s.Name == _sage.Name && s.City == _sage.City).ToList().Count() != 0) MessageBox.Show($"Sage {_sage} already exist. Please add new or cancel");
                    else
                    {
                        if (IsNewItem)
                        {
                            if (sbc.Sages.Any(s => s.Name == _sage.Name && s.City == _sage.City && s.Age == _sage.Age)) MessageBox.Show("Sage already exists.Please edit your form!");
                            else sbc.Sages.Add(_sage);
                        }
                        else
                        {
                            Sage editedSage = sbc.Sages.Where(p => p.Id == SageId).FirstOrDefault();
                            sbc.Sages.Attach(editedSage);
                            editedSage.Name = SageDefaultName;
                            editedSage.City = SageDefaultCity;
                            editedSage.Age = int.Parse(SageDefaultAge);
                        }
                        
                        sbc.SaveChanges();                       
                    }

                    Close();
                }
            }           
        }
    }
}
