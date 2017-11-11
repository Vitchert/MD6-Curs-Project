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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace MD6Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public BackgroundWorker bw;
        MD6Project.Options opt;
        MD6Project.PasswordOptions PassOpt;
        static MainWindow()
        {
            Instance = new MainWindow();
        }

        private MainWindow()
        {
            InitializeComponent();
            messageFile = "";
            keyFile = "";
            hashFile = "";
            PasswordString = "";
            opt = MD6Project.Options.Instance;
            PassOpt = MD6Project.PasswordOptions.Instance;
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork +=
                new DoWorkEventHandler(bw_DoWork);
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
        }

        private void menuHelpAboutButton_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
            about = null;
            
        }


        private void openFileToHash_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Message File";
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInf = new FileInfo(openFileDialog.FileName);
                if (fileInf.Exists)
                {
                    labelFileToHashname.Content = "Loaded File: " + openFileDialog.FileName;
                    messageFile = openFileDialog.FileName;
                    CalculateHashButton.IsEnabled = true;
                    textFileToHash.IsEnabled = false;
                    CompareHashButton.IsEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("File does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }


        private void openKeyButton_Click(object sender, RoutedEventArgs e)
        {           
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Key File";
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInf = new FileInfo(openFileDialog.FileName);
                if (fileInf.Exists)
                {
                    if(fileInf.Length > 512)
                    {
                        System.Windows.MessageBox.Show("Key file too big.\nExpected length <= 512 bytes.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    textKey.IsEnabled = false;
                    menuFileSaveKeyAsButton.IsEnabled = false;
                    labelKey.Content = "Loaded Key: " + openFileDialog.FileName;
                    keyFile = openFileDialog.FileName;
                    PasswordString = "";
                }
                else
                {
                    System.Windows.MessageBox.Show("File does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }               
            }
        }

        private void InputkeyButton_Click(object sender, RoutedEventArgs e)
        {
            textKey.IsEnabled = true;
            menuFileSaveKeyAsButton.IsEnabled = true;
            labelKey.Content = "User input key selected";
            keyFile = "";
            PasswordString = "";
        }

        private void InputMessageButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateHashButton.IsEnabled = true;
            textFileToHash.IsEnabled = true;
            menuFileSaveTextAsButton.IsEnabled = true;
            labelFileToHashname.Content = "User input message selected";
            messageFile = "";
            if(MD6Hash.IsEnabled || (MD6Hash.Text != ""))
                CompareHashButton.IsEnabled = true;
        }

        private void LoadHashButton_Click(object sender, RoutedEventArgs e)
        {
            MD6ProgressBarLabel.Content = "";
            menuFileSaveHashButton.IsEnabled = false;
            menuFileSaveHashAsButton.IsEnabled = false;
            MD6Hash.IsEnabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open hash file";
            if (openFileDialog.ShowDialog() == true)
            {               
                FileInfo fileInf = new FileInfo(openFileDialog.FileName);
                if (fileInf.Exists)
                {
                    if (fileInf.Length > 512)
                    {
                        System.Windows.MessageBox.Show("Hash file too big.\nExpected length <= 512 bytes.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    MD6HashResultLabel.Content = "Loaded Hash: " + openFileDialog.FileName;
                    MD6Hash.Text = File.ReadAllText(openFileDialog.FileName);
                    hashFile = openFileDialog.FileName;
                    if(CalculateHashButton.IsEnabled)
                        CompareHashButton.IsEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Hash File does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void CalculateHashButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (bw.IsBusy != true)
            {
                System.GC.Collect();
                bw.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                MD6ProgressBarLabel.Content = "Calculating";
                MD6HashResultLabel.Content = "";
                MD6Hash.Text = "";
                MD6ProgressBar.IsIndeterminate = true;

                List<object> arguments = new List<object>();
                arguments.Add(Options.Instance.dVal);
                arguments.Add(Options.Instance.Lval);
                arguments.Add(Options.Instance.rVal);
                arguments.Add(messageFile);
                arguments.Add(textFileToHash.Text);
                arguments.Add(keyFile);
                if (PasswordString != "")
                {
                    arguments.Add(PasswordString);
                    arguments.Add(true);//Flag indicates that string is hex value
                }
                else
                {
                    arguments.Add(textKey.Text);
                    arguments.Add(false);//Flag indicates that string is not hex value
                }

                bw.RunWorkerAsync(arguments);                
            }


        }

        private void EditHashbutton_Click(object sender, RoutedEventArgs e)
        {
            if (hashFile != "")
            {
                menuFileSaveHashButton.IsEnabled = true;
            }
            menuFileSaveHashAsButton.IsEnabled = true;
            MD6ProgressBarLabel.Content = "";
            MD6Hash.IsEnabled = true;
            MD6HashResultLabel.Content = "User edited hash";
            CompareHashButton.IsEnabled = true;
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<object> genericlist = e.Argument as List<object>;
                uint d = (uint)Convert.ToInt32(genericlist[0]);
                uint L = (uint)Convert.ToInt32(genericlist[1]);
                uint r = (uint)Convert.ToInt32(genericlist[2]);
                string messageFilepath = (string)genericlist[3];
                string messageText = (string)genericlist[4];
                string keyFilepath = (string)genericlist[5];
                string keyText = (string)genericlist[6];
                bool isHexString = (bool)genericlist[7];

                MD6 HashFunction = new MD6(d, L, r);
                if (messageFilepath == "")
                {
                    HashFunction.readMessageString(messageText);
                }
                else
                {
                    if (HashFunction.readMessageFile(messageFilepath) != MD6.OK)
                    {
                        //System.Windows.MessageBox.Show("Message File does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        e.Result = "Message File does not exist";
                        return;
                    }
                }

                int errNum;
                if (keyFilepath == "")
                {
                    errNum = HashFunction.readKeyString(keyText, isHexString);
                }
                else
                {
                    errNum = HashFunction.readKeyFile(keyFilepath);
                }

                if (errNum == MD6.NO_FILE)
                {
                    throw new Exception("Key File does not exist");
                    return;
                }
                if (errNum == MD6.WRONG_FILE_SIZE)
                {
                    throw new Exception("Key file too big.\nExpected length <= 512 bytes.");
                    return;
                }
                e.Result = HashFunction.Hash();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bw.RunWorkerCompleted -=
               new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            if ((e.Cancelled == true))
            {
                MD6ProgressBarLabel.Content = "Cancelled";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show("Cancelled", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (!(e.Error == null))
            {
                MD6ProgressBarLabel.Content = "Error";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            else
            {
                MD6ProgressBarLabel.Content = "Done";
                MD6HashResultLabel.Content = "Calculated Hash";
                MD6Hash.Text = e.Result.ToString();
                MD6Hash.IsEnabled = false;
                MD6ProgressBar.IsIndeterminate = false;
                hashFile = "";
                menuFileSaveHashButton.IsEnabled = false;
                menuFileSaveHashAsButton.IsEnabled = true;
                CompareHashButton.IsEnabled = true;
            }
        }

        public void bw_RunWorkerCompletedPassword(object sender, RunWorkerCompletedEventArgs e)
        {
            bw.RunWorkerCompleted -=
               new RunWorkerCompletedEventHandler(bw_RunWorkerCompletedPassword);
            if ((e.Cancelled == true))
            {
                MD6ProgressBarLabel.Content = "Cancelled";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show("Cancelled", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!(e.Error == null))
            {
                MD6ProgressBarLabel.Content = "Error";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            else
            {
                MD6ProgressBarLabel.Content = "Done";
                PasswordString = e.Result.ToString();
                MD6ProgressBar.IsIndeterminate = false;
                hashFile = "";

                textKey.IsEnabled = false;
                menuFileSaveKeyAsButton.IsEnabled = false;
                labelKey.Content = "User key from password selected";
                keyFile = "";

            }
        }

        public void bw_RunWorkerCompletedComparePasswordInput(object sender, RunWorkerCompletedEventArgs e)
        {
            bw.RunWorkerCompleted -=
               new RunWorkerCompletedEventHandler(bw_RunWorkerCompletedComparePasswordInput);
            if ((e.Cancelled == true))
            {
                MD6ProgressBarLabel.Content = "Cancelled";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show("Cancelled", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!(e.Error == null))
            {
                MD6ProgressBarLabel.Content = "Error";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            else
            {
                System.GC.Collect();
                bw.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(bw_RunWorkerCompletedCompare);

                List<object> arguments = new List<object>();
                arguments.Add(Options.Instance.dVal);
                arguments.Add(Options.Instance.Lval);
                arguments.Add(Options.Instance.rVal);
                arguments.Add(messageFile);
                arguments.Add(textFileToHash.Text);
                arguments.Add("");//No keyfile
                arguments.Add(e.Result.ToString());//Key as result of hash function
                arguments.Add(true);//Flag indicates that string is hex value
                bw.RunWorkerAsync(arguments);
            }
        }

        public void bw_RunWorkerCompletedCompare(object sender, RunWorkerCompletedEventArgs e)
        {
            bw.RunWorkerCompleted -=
               new RunWorkerCompletedEventHandler(bw_RunWorkerCompletedCompare);
            if ((e.Cancelled == true))
            {
                MD6ProgressBarLabel.Content = "Cancelled";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show("Cancelled", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!(e.Error == null))
            {
                MD6ProgressBarLabel.Content = "Error";
                MD6ProgressBar.IsIndeterminate = false;
                System.Windows.MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            else
            {
                MD6ProgressBarLabel.Content = "Done";
                MD6ProgressBar.IsIndeterminate = false;

                if(MD6Hash.Text == e.Result.ToString())
                {
                    System.Windows.MessageBox.Show("Hash values match", "Comparison", MessageBoxButton.OK, MessageBoxImage.None);
                }
                else
                {
                    System.Windows.MessageBox.Show("Hash values dont match", "Comparison", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void MenuEditOptionsButton_Click(object sender, RoutedEventArgs e)
        {            
            opt.Show();
        }

        public string messageFile;
        public string keyFile;
        public string hashFile;
        public string PasswordString;

        private void menuFileSaveHashAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Hash File As";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog.OpenFile();
                    byte[] stringArray = Encoding.ASCII.GetBytes(MD6Hash.Text);
                    fs.Write(stringArray, 0, stringArray.Length);
                    fs.Close();
                    menuFileSaveHashButton.IsEnabled = true;
                    hashFile = saveFileDialog.FileName;
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void menuFileSaveHashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(hashFile, System.IO.FileMode.Create);
                byte[] stringArray = Encoding.ASCII.GetBytes(MD6Hash.Text);
                fs.Write(stringArray, 0, stringArray.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void menuFileSaveKeyAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Key File As";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog.OpenFile();
                    byte[] stringArray = Encoding.ASCII.GetBytes(textKey.Text);
                    fs.Write(stringArray, 0, stringArray.Length);
                    fs.Close();
                    menuFileSaveHashButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void menuFileSaveTextAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Message File As";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog.OpenFile();
                    byte[] stringArray = Encoding.ASCII.GetBytes(textFileToHash.Text);
                    fs.Write(stringArray, 0, stringArray.Length);
                    fs.Close();
                    menuFileSaveHashButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void menuFileExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuEditPasswordoptions_Click(object sender, RoutedEventArgs e)
        {
            PassOpt.Show();
        }

        private void PasswordKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                PasswordInput passInt = new PasswordInput();
                passInt.ShowDialog();
                passInt = null;
                return;
            }
        }

        private void CompareHashButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (bw.IsBusy != true)
            {
                ComparePasswordinput passInt = new ComparePasswordinput();
                passInt.ShowDialog();
                passInt = null;
                return;
            }
        }
    }
}
