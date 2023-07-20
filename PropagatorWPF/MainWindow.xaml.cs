using PropagatorWPF.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PropagatorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkspaceService workspace = new WorkspaceService();
        private DirectoryInfo resoursePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Resources");
        private int countResItems;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (resoursePath.Exists)
            {
                countResItems = ResourceCopier.CountAllResItems(resoursePath, workspace.ResourseList);              
            }
            else
            {
                workspace.ResourseList.Add("Проверьте наличие папки 'Resources'");
            }

            WriteItemsToTextBox(workspace.ResourseList);

            tBlCount.Text = countResItems.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(workspace.pathArm))
            {
                MessageBox.Show("Нет информации по целевым АРМ");
                return;
            }
            if (countResItems <= 0)
            {
                MessageBox.Show("Папка ресурсов пуста!");
                return;
            }

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

            await SendItemsAsync();

            MessageBox.Show("Готово!");

            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
        }

        private async Task SendItemsAsync()
        {
            tBArm.Text = string.Empty;

            workspace.FillArmsList();

            await workspace.FillUsersList();

            CopyResourses();

            WriteItemsToTextBox(workspace.ArmsOff);
        }

        private void CopyResourses()
        {
            foreach (var arm in workspace.ArmsOn)
            {
                if (!Directory.Exists(arm))
                {
                    MessageBox.Show($"{arm} не является АРМом");
                    return;
                }

                var dirsArm = PickArmDirectories(arm,"\\.[A-Za-z]+\\.");

                foreach (var dir in dirsArm)
                {
                    var userDirs = PickArmDirectories(dir, "Desktop");

                    foreach (var u in userDirs)
                    {
                        ResourceCopier.CopyAll(resoursePath, new DirectoryInfo(u));
                    }
                }
            }
        }

        private IEnumerable<string> PickArmDirectories(string arm, string pattern)
        {
            return Directory.GetDirectories(arm).Where(d => Regex.IsMatch(d, $"{pattern}"));
        }

        private void WriteItemsToTextBox(List<string> armsOff)
        {
            foreach (var a in armsOff)
            {
                tBArm.Text += a + Environment.NewLine;
            }
        }
    }
}
