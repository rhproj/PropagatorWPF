using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PropagatorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DirectoryInfo resoursePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Resources");
        //DirectoryInfo copyTo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        List<string> resourseList = new List<string>();
        int countResItems;
        string pathArm = AppDomain.CurrentDomain.BaseDirectory + @"Arm.csv";
        List<string> armsList = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (resoursePath.Exists)
            {
                countResItems = ListAll(resoursePath, resourseList);              
                //lvArm.Items.Add(resoursePath);
            }
            else
            {
                resourseList.Add("Проверьте наличие папки 'Resources'");
            }

            //lvArm.ItemsSource = resourseList;
            foreach (var r in resourseList)
            {
                tBArm.Text += r + Environment.NewLine;
            }

            tBlCount.Text = countResItems.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {Mouse.OverrideCursor = Cursors.Wait;});

            tBArm.Text = string.Empty;

            if (countResItems > 0)
            {
                if (File.Exists(pathArm))
                {
                    List<string> armsOn = new List<string>();
                    List<string> armsOff = new List<string>();
                    armsOff.Add("Отсутствует соединение с:");

                    using (StreamReader sR = new StreamReader(pathArm, Encoding.Default))
                    {
                        while (!sR.EndOfStream)
                        {
                            string[] armLine = sR.ReadLine().Split(';');
                            armsList.Add(armLine[0]);
                            //armsList.Add($@"\\{armLine[0]}\c$\Users");
                        }
                    }

                    foreach (var arm in armsList)
                    {
                        Ping ping = new Ping();
                        PingReply pingReply = await ping.SendPingAsync(arm); //Send(arm);

                        if (pingReply.Status == IPStatus.Success)
                        {
                            armsOn.Add($@"\\{arm}\c$\Users");
                        }
                        else
                        {
                            armsOff.Add(arm);
                        }
                    }

                    foreach (var arm in armsOn)
                    {
                        if (Directory.Exists(arm))
                        {
                            var dirsArm = Directory.GetDirectories(arm).Where(d => Regex.IsMatch(d, @"\.[A-Za-z]+\.")); //"rh"));//   //(d, @"[.]{1}")); //([\w+\.{1}\w+\.{1}\w+])

                            //Regex rx = new Regex(@"\.[A-Za-z]+\.");

                            foreach (var dir in dirsArm)
                            {
                                var userDirs = Directory.GetDirectories(dir).Where(d => Regex.IsMatch(d, @"Desktop")); // @"Documents"));

                                foreach (var u in userDirs)
                                {
                                    //tBArm.Text += u + Environment.NewLine;
                                    CopyAll(resoursePath, new DirectoryInfo(u));
                                }

                            }
                        }
                        else
                        {
                            MessageBox.Show($"{arm} не является АРМом");
                        }
                    }
                    
                    foreach (var a in armsOff)
                    {
                        tBArm.Text += a + Environment.NewLine;
                    }

                    MessageBox.Show("Готово!");
                }
                else
                {
                    MessageBox.Show("Нет информации по целевым АРМ");
                }
            }
            else
            {
                MessageBox.Show("Папка ресурсов пуста!");
                Application.Current.Shutdown();
            }

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }

        private void CopyAll(DirectoryInfo fromD, DirectoryInfo toD)
        {
            //Directory.CreateDirectory(toD.FullName); //don't need this in case of Desktop
            //copy files
            foreach (FileInfo fI in fromD.GetFiles())
            {
                fI.CopyTo(System.IO.Path.Combine(toD.FullName, fI.Name), true);
            }
            //copy sub-dirs recursively
            foreach (DirectoryInfo sourceDirs in fromD.GetDirectories())
            {
                DirectoryInfo targDirs = toD.CreateSubdirectory(sourceDirs.Name);
                CopyAll(sourceDirs, targDirs);
            }
        }


        #region TEST method
        private int ListAll(DirectoryInfo fromD, List<string> resourseList)
        {
            foreach (FileInfo fI in fromD.GetFiles())
            {
                resourseList.Add(fI.Name);
            }

            //copy sub-dirs recursively
            foreach (DirectoryInfo sourceDirs in fromD.GetDirectories())
            {
                resourseList.Add(sourceDirs.Name);
                //ListAll(sourceDirs, resourseList);
            }

            return resourseList.Count();
        } 
        #endregion
    }
}
