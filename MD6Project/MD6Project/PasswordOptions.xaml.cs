using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MD6Project
{
    /// <summary>
    /// Interaction logic for PasswordOptions.xaml
    /// </summary>
    public partial class PasswordOptions : Window
    {
        public static PasswordOptions Instance { get; private set; }

        static PasswordOptions()
        {
            Instance = new PasswordOptions();
        }

        private PasswordOptions()
        {
            InitializeComponent();
        }

        public bool Upper = false;
        public bool Lower = false;
        public bool Number = false;
        public bool Letter = false;

        public int minLength = 0;

        private void PasswordApplyButton_Click(object sender, RoutedEventArgs e)
        {
            bool TempUpper = (bool)UpperCaseCheckBox.IsChecked;
            bool TempLower = (bool)LowerCaseCheckBox.IsChecked;
            bool TempNumber = (bool)NumbersCheckBox.IsChecked;
            bool TempLetter = (bool)LettersCheckBox.IsChecked;
            int TempminLength;
            try
            {
                TempminLength = Convert.ToInt32(PasswordLengthTextBox.Text, 10);
                if (TempminLength < 0) 
                {
                    System.Windows.MessageBox.Show("Wrong minimum password length", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Wrong minimum password length", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Upper = TempUpper;
            Lower = TempLower;
            Number = TempNumber;
            Letter = TempLetter;
            minLength = TempminLength; 

        }

        private void PasswordDiscardButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordLengthTextBox.Text = minLength.ToString();           
            UpperCaseCheckBox.IsChecked = Upper;
            LowerCaseCheckBox.IsChecked = Lower;
            NumbersCheckBox.IsChecked = Number;
            LettersCheckBox.IsChecked = Letter;
        }

        private void UpperCaseCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)UpperCaseCheckBox.IsChecked)
            {
                LettersCheckBox.IsChecked = true;
            }
        }

        private void LowerCaseCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)LowerCaseCheckBox.IsChecked)
            {
                LettersCheckBox.IsChecked = true;
            }
        }

        private void LettersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)LettersCheckBox.IsChecked)
            {
                UpperCaseCheckBox.IsChecked = false;
                LowerCaseCheckBox.IsChecked = false;
            }
        }
    }
}
