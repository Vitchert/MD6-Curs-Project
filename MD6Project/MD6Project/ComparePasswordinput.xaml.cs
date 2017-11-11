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
    /// Interaction logic for ComparePasswordinput.xaml
    /// </summary>
    public partial class ComparePasswordinput : Window
    {
        public ComparePasswordinput()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {            
            MainWindow.Instance.bw.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(MainWindow.Instance.bw_RunWorkerCompletedComparePasswordInput);
            MainWindow.Instance.MD6ProgressBarLabel.Content = "Calculating";
            MainWindow.Instance.MD6ProgressBar.IsIndeterminate = true;

            if (passwordBox.Password.Length == 0)
            {
                RunWorkerCompletedEventArgs fakeComplition = new RunWorkerCompletedEventArgs("",null,false);

                MainWindow.Instance.bw_RunWorkerCompletedComparePasswordInput(this, fakeComplition);
            }
            else
            {
                List<object> arguments = new List<object>();
                arguments.Add(Options.Instance.dVal);
                arguments.Add(Options.Instance.Lval);
                arguments.Add(Options.Instance.rVal);
                arguments.Add("");
                arguments.Add(passwordBox.Password);
                arguments.Add("");
                arguments.Add("");
                arguments.Add(false);//Flag indicates that string is not hex value

                MainWindow.Instance.bw.RunWorkerAsync(arguments);
            }
            this.Close();
        }
    }
}
