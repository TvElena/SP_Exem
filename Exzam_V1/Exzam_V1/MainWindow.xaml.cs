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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Exzam_V1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static HookProc proc = HookCallback;
        private static IntPtr hook = IntPtr.Zero;


        public MainWindow()
        {
            hook = SetHook(proc);
            InitializeComponent();
            ShowF();
            hook = SetHook(proc);
            UnhookWindowsHookEx(hook);
        }
        public void ShowF()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = drive;
                item.Header = drive.ToString();
                item.Items.Add("*");
                treeFileSystem.Items.Add(item);

            }
        }

        private void item_Expanded(object sender, RoutedEventArgs e)       
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.Items.Clear();
            DirectoryInfo dir;
            if (item.Tag is DriveInfo)
            {
                DriveInfo drive = (DriveInfo)item.Tag;
                dir = drive.RootDirectory;  
            }
            else
            {
                dir = (DirectoryInfo)item.Tag;
            }
            try
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Tag = subDir;
                    newItem.Header = subDir.ToString();
                    newItem.Items.Add("*");
                    item.Items.Add(newItem);
                }
                    //http://axedeos.blogspot.com.by/2012/09/wpf-file-explorer_23.html
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Tag = fi;
                    newItem.Header = fi.ToString();
                        item.Items.Add(newItem);
                    }
                
            }
                catch
            {

            }
        }

        //
        private delegate void AsyncFind();

        public void Ffind ()
        {
            Dispatcher.BeginInvoke( (ThreadStart)delegate () { 
            if (string.IsNullOrEmpty(dirName.Text)) dirName.Text = @"D:\";
            if (string.IsNullOrEmpty(fileName.Text)) fileName.Text = "1";
            DirectoryInfo path = new DirectoryInfo(dirName.Text);
            string strF = fileName.Text;
            List<string> fileFind = new List<string>();
            try
            {
                foreach (FileInfo f in path.GetFiles("*.*"))
                {
                    
                    if (f.ToString().Contains(strF)==true) 
                    fileFind.Add(f.FullName);
                }
            }
            catch (Exception) { }

            fileFindList.ItemsSource = fileFind;
            });
        }
        private static void EndFind(IAsyncResult ar)
        {
            AsyncFind ff = (AsyncFind)ar.AsyncState;
            ff.EndInvoke(ar);
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
           // Ffind();
            AsyncFind f=Ffind;
            f.BeginInvoke(EndFind, f);      
        }

       
        private void treeFileSystem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("123");
        }

        private static IntPtr SetHook(HookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL,
                                        proc,
                                        GetModuleHandle(curModule.ModuleName),
                                        0);
            }
        }
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode == Keys.F1) /*&& ((Keys)vkCode == Keys.F)*/ || ((Keys)vkCode == Keys.F1))
                {
                    var window = System.Windows.Application.Current.MainWindow as MainWindow;
                    window.Ffind();
                    //System.Windows.MessageBox.Show("huck. не вызывается нестатичная функция с  нестатичными полями: Ffind()");
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      HookProc lpfn,
                                                      IntPtr hMod,
                                                      uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk,
                                                    int nCode,
                                                    IntPtr wParam,
                                                    IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}


