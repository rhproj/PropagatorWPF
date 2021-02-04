using System;
using System.Collections.Generic;
using System.IO;
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
        IList<string> resourseList = new List<string>();
        int countResItems;

        DirectoryInfo copyTo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)); //Environment.GetFolderPath() !! don;t forget otherwise it roots from Debug..

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

            lvArm.ItemsSource = resourseList;
            tBlCount.Text = countResItems.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (countResItems > 0)
            {
                CopyAll(resoursePath, copyTo);
                MessageBox.Show("Готово!");
            }
            else
            {
                MessageBox.Show("Папка ресурсов пуста!");
                Application.Current.Shutdown();
            }
        }

        private void CopyAll(DirectoryInfo fromD, DirectoryInfo toD)
        {
            //Directory.CreateDirectory(toD.FullName); //don't need it in case of Desktop
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

        private int ListAll(DirectoryInfo fromD, IList<string> resourseList)
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
    }
}
