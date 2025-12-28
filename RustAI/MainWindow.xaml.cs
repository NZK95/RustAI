using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace RustAI
{
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon { get; set; }
        public static TelegramBot TelegramBot { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeAssets();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            await TelegramBot.ShutdownAsync();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBot();
        }

        private async Task InitializeBot()
        {
            TelegramBot = new TelegramBot();
            await TelegramBot.InitAsync();
        }

        private void InitializeAssets()
        {
            try
            {
                _notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    Text = "RustAI is running in the background.",
                    Icon = new Icon("assets/icons/icon.ico"),
                    ContextMenuStrip = new ContextMenuStrip()
                };

                _notifyIcon.ContextMenuStrip.Items.Add("GitHub", null, (s, e) =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(Builders.BuildRustAIProjectLink())
                        {
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error while opening link: {ex.Message}");
                    }
                });

                _notifyIcon.ContextMenuStrip.Items.Add("Close", null, (s, e) =>
                {
                    System.Windows.Application.Current.Shutdown();
                });

                _notifyIcon.DoubleClick += (sender, args) =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                    ShowInTaskbar = true;
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
            base.OnClosing(e);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });

            e.Handled = true;
        }
    }
}
