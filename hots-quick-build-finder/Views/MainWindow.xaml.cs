using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using static hots_quick_build_finder.Classes.WindowsApi;

namespace hots_quick_build_finder.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            if (AlreadyOpen)
            {
                FocusBuildFinder();
                Close();
            }

            else
                InitializeComponent();
        }

        private static bool AlreadyOpen
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
                return Process.GetProcessesByName(name).Length > 1;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox) sender;
            if (e.Key == Key.Enter)
            {
                textbox.CaretIndex = textbox.Text.Length;
                textbox.SelectionStart = textbox.Text.Length;
            }
            else if (e.Key == Key.Escape)
            {
                textbox.Text = "";
            }
        }
    }
}
