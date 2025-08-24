namespace Flow.Launcher.Plugin.BrowserTabs.Views
{
    public partial class SettingWindow
    {
        public SettingWindow(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
        }

        public Settings Settings { get; }
    }
}
