﻿using FindReplace;
using FModel.Forms;
using FModel.Methods;
using FModel.Methods.AESManager;
using FModel.Methods.Assets;
using FModel.Methods.BackupsManager;
using FModel.Methods.PAKs;
using FModel.Methods.TreeViewModel;
using FModel.Methods.Utilities;
using ICSharpCode.AvalonEdit.Folding;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FModel
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SortedTreeViewWindowViewModel ViewModel { get { return DataContext as SortedTreeViewWindowViewModel; } set { DataContext = value; } }

        public MainWindow()
        {
            InitializeComponent();
            FWindow.FMain = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FModelVersionLabel.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);

            await Task.Run(() => 
            {
                FoldersUtility.LoadFolders();
                RegisterFromPath.FilterPAKs();
                DynamicKeysChecker.SetDynamicKeys();
                RegisterDownloadedBackups.LoadBackupFiles();
            }).ContinueWith(TheTask =>
            {
                TasksUtility.TaskCompleted(TheTask.Exception);
            });

            AvalonEdit.SetAEConfig();
        }

        #region BUTTON EVENTS
        private void Button_AESManager_Click(object sender, RoutedEventArgs e)
        {
            if (!FormsUtility.IsWindowOpen<Window>("AES Manager"))
            {
                new AESManager().Show();
            }
            else { FormsUtility.GetOpenedWindow<Window>("AES Manager").Focus(); }
        }
        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageBox_Main.Source != null)
            {
                if (!FormsUtility.IsWindowOpen<Window>(FWindow.FCurrentAsset))
                {
                    Window win = new Window();
                    win.Title = FWindow.FCurrentAsset;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    win.Width = ImageBox_Main.Source.Width;
                    win.Height = ImageBox_Main.Source.Height;

                    DockPanel dockPanel = new DockPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    Image img = new Image();
                    img.Source = ImageBox_Main.Source;
                    dockPanel.Children.Add(img);

                    win.Content = dockPanel;
                    win.Show();
                }
                else { FormsUtility.GetOpenedWindow<Window>(FWindow.FCurrentAsset).Focus(); }
            }
        }
        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (TasksUtility.CancellableTaskTokenSource != null)
            {
                TasksUtility.CancellableTaskTokenSource.Cancel();
                if (TasksUtility.CancellableTaskTokenSource.IsCancellationRequested)
                {
                    new UpdateMyProcessEvents("Canceled!", "Yikes").Update();
                }
                else { new UpdateMyProcessEvents("This is odd!\tCanceled but not requested. You should never see this tbh", "Yikes").Update(); }
            }
        }
        private void Button_Extract_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                //FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString(); <-- already in the 'Load' loop
                AssetsLoader.LoadSelectedAsset();
            }
        }
        #endregion

        #region MENU ITEM EVENTS
        public void MI_Pak_Click(object sender, RoutedEventArgs e)
        {
            FWindow.FCurrentPAK = (sender as MenuItem).Header.ToString();
            PAKsLoader.LoadOnePAK();
        }
        private void MI_LoadAllPAKs_Click(object sender, RoutedEventArgs e)
        {
            FWindow.FCurrentPAK = string.Empty;
            PAKsLoader.LoadAllPAKs();
        }
        private void MI_BackupPAKs_Click(object sender, RoutedEventArgs e)
        {
            BackupPAKs.CreateBackupFile();
        }
        private void MI_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!FormsUtility.IsWindowOpen<Window>("Settings"))
            {
                new FModel_Settings().Show();
            }
            else { FormsUtility.GetOpenedWindow<Window>("Settings").Focus(); }
        }
        private void MI_About_Click(object sender, RoutedEventArgs e)
        {
            if (!FormsUtility.IsWindowOpen<Window>("About"))
            {
                new FModel_About().Show();
            }
            else { FormsUtility.GetOpenedWindow<Window>("About").Focus(); }
        }
        private void MI_OpenOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            FoldersUtility.OpenOutputFolder();
        }
        #endregion

        #region TREEVIEW EVENTS
        private void NodeSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem currContainer = e.OriginalSource as TreeViewItem;
            if (currContainer != null)
            {
                FWindow.TVItem = currContainer;
                ListBoxUtility.PopulateListBox(currContainer);
            }

        }
        #endregion

        #region LISTBOX EVENTS
        private void ListBox_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Extract.IsEnabled = ListBox_Main.SelectedIndex >= 0;
        }
        private void ListBox_Main_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {   
            if (ListBox_Main.SelectedIndex >= 0)
            {
                //FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString(); <-- already in the 'Load' loop
                AssetsLoader.LoadSelectedAsset();
            }
        }
        private void FilterTextBox_Main_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBoxUtility.FilterListBox();
        }
        private void RC_Extract_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                //FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString(); <-- already in the 'Load' loop
                AssetsLoader.LoadSelectedAsset();
            }
        }
        private void RC_Copy_FPath_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString();
                Clipboard.SetText(AssetsUtility.GetAssetPathToCopy());
            }
        }
        private void RC_Copy_FName_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString();
                Clipboard.SetText(AssetsUtility.GetAssetPathToCopy(true));
            }
        }
        private void RC_Copy_FPath_NoExt_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString();
                Clipboard.SetText(AssetsUtility.GetAssetPathToCopy(false, false));
            }
        }
        private void RC_Copy_FName_NoExt_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString();
                Clipboard.SetText(AssetsUtility.GetAssetPathToCopy(true, false));
            }
        }
        private void RC_Properties_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Main.SelectedIndex >= 0)
            {
                FWindow.FCurrentAsset = ListBox_Main.SelectedItem.ToString();
                AssetInformations.OpenAssetInfos();
            }
        }
        #endregion
    }
}
