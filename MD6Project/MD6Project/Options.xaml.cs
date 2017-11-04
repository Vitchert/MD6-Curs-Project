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
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public static Options Instance { get; private set; }

        static Options()
        {
            Instance = new Options();
        }

        private Options()
        {
            InitializeComponent();
            Lval = 64;
            dVal = 256;
            rVal = 0;
            CustomRounds = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void CustomRoundsCheck_Click(object sender, RoutedEventArgs e)
        {
            NumberOfRoundsText.IsEnabled = (bool)CustomRoundsCheck.IsChecked;
        }

        public int Lval = 64;
        public int dVal = 256;
        public int rVal = 0;
        bool CustomRounds = false;

        private void OptionsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int LvalTemp = 0;
            int dValTemp = 0;
            int rValTemp = 0;



            bool CustomRoundsTemp = (bool)CustomRoundsCheck.IsChecked;
            try
            {
                dValTemp = Convert.ToInt32(MessageDigestLength.Text, 10);
                if ((dValTemp <= 0) || (dValTemp > 512))
                {
                    System.Windows.MessageBox.Show("Wrong message digest length (d)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                    
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Wrong message digest length (d)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                LvalTemp = Convert.ToInt32(ModeControlText.Text, 10);
                if ((LvalTemp < 0) || (LvalTemp > 64))
                {
                    System.Windows.MessageBox.Show("Wrong mode control (L)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Wrong mode control (L)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((bool)CustomRoundsCheck.IsChecked)
            {
                try
                {
                    rValTemp = Convert.ToInt32(NumberOfRoundsText.Text, 10);
                    if ((rValTemp < 1))
                    {
                        System.Windows.MessageBox.Show("Wrong number of rounds (r)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Wrong number of rounds (r)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CustomRoundsTemp = true;
            }
            else
            {
                CustomRoundsTemp = false;
            }

            Lval = LvalTemp;
            dVal = dValTemp;
            if (CustomRoundsTemp)
            {
                rVal = rValTemp;
                CustomRounds = CustomRoundsTemp;
            }
            else
            {
                rVal = 0;
                CustomRounds = CustomRoundsTemp;
            }
        }

        private void OptionsDiscardButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDigestLength.Text = dVal.ToString();
            ModeControlText.Text = Lval.ToString();
            if (CustomRounds)
            {
                CustomRoundsCheck.IsChecked = true;
                NumberOfRoundsText.IsEnabled = true;
                NumberOfRoundsText.Text = rVal.ToString();
            }
            else
            {
                NumberOfRoundsText.IsEnabled = false;
                CustomRoundsCheck.IsChecked = false;
                NumberOfRoundsText.Text = "";
            }
        }

        private void OptionsResetButton_Click(object sender, RoutedEventArgs e)
        {
            Lval = 64;
            dVal = 256;
            rVal = 0;
            CustomRounds = false;
            NumberOfRoundsText.IsEnabled = false;
            CustomRoundsCheck.IsChecked = false;
            MessageDigestLength.Text = "256";
            ModeControlText.Text = "64";
            NumberOfRoundsText.Text = "";
        }

    }
}
