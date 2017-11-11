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
using System.ComponentModel;

namespace MD6Project
{
    /// <summary>
    /// Interaction logic for PasswordInput.xaml
    /// </summary>
    public partial class PasswordInput : Window
    {
        public PasswordInput()
        {
            InitializeComponent();
        }

        private void PasswordinputButton_Click(object sender, RoutedEventArgs e)
        {
            bool Upper = PasswordOptions.Instance.Upper;
            bool Lower = PasswordOptions.Instance.Lower;
            bool Number = PasswordOptions.Instance.Number;
            bool Letter = PasswordOptions.Instance.Letter;
            int minLength = PasswordOptions.Instance.minLength;

            bool CurUpper = false;
            bool CurLower = false;
            bool CurNumber = false;
            bool CurLetter = false;

            if (FirstpasswordBox.Password.Length == 0)
            {
                System.Windows.MessageBox.Show("Password can't be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (FirstpasswordBox.Password.Length < minLength)
            {
                System.Windows.MessageBox.Show("Password too short.\nMust be at least " + minLength +" symbols.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int i = 0;
            while ((i < FirstpasswordBox.Password.Length))
            {
                CurUpper |= Char.IsUpper(FirstpasswordBox.Password[i]);
                CurLower |= Char.IsLower(FirstpasswordBox.Password[i]);
                CurLetter |= Char.IsLetter(FirstpasswordBox.Password[i]);
                CurNumber |= Char.IsNumber(FirstpasswordBox.Password[i]);
                ++i;
            }
            if (Upper && !CurUpper)
            {
                System.Windows.MessageBox.Show("Password must contain Upper Case letters", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Lower && !CurLower)
            {
                System.Windows.MessageBox.Show("Password must contain Lower Case letters", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Letter && !CurLetter)
            {
                System.Windows.MessageBox.Show("Password must contain letters", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Number && !CurNumber)
            {
                System.Windows.MessageBox.Show("Password must contain numbers", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (FirstpasswordBox.Password != SecondpasswordBox.Password)
            {
                System.Windows.MessageBox.Show("Passwords dont match", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainWindow.Instance.bw.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(MainWindow.Instance.bw_RunWorkerCompletedPassword);
            MainWindow.Instance.MD6ProgressBarLabel.Content = "Calculating";
            MainWindow.Instance.MD6ProgressBar.IsIndeterminate = true;
            
            List<object> arguments = new List<object>();
            arguments.Add(Options.Instance.dVal);
            arguments.Add(Options.Instance.Lval);
            arguments.Add(Options.Instance.rVal);
            arguments.Add("");
            arguments.Add(FirstpasswordBox.Password);
            arguments.Add("");
            arguments.Add("");
            arguments.Add(false);//Flag indicates that string is not hex value

            MainWindow.Instance.bw.RunWorkerAsync(arguments);
            this.Close();
        }
    }
}
