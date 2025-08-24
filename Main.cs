using BrowserTabs;
using Flow.Launcher.Plugin.BrowserTabs.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.BrowserTabs
{
    public class Main : IAsyncPlugin, IPluginI18n, IContextMenu, ISettingProvider
    {
        public const string IconPath = "Images/icon.png";

        private SettingWindow? SettingWindow;

        public static PluginInitContext Context { get; private set; } = null!;
        
        public static Settings Settings { get; private set; } = null!;

        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            var results = new List<Result>();

            foreach (var tab in BrowserTabManager.GetAllChromiumTabs(token))
            {
                var titleMatch = Context.API.FuzzySearch(query.Search, tab.Title);
                var browserNameMatch = Context.API.FuzzySearch(query.Search, tab.BrowserName);

                if (string.IsNullOrWhiteSpace(query.Search) ||
                    titleMatch.IsSearchPrecisionScoreMet() ||
                    browserNameMatch.IsSearchPrecisionScoreMet())
                {
                    results.Add(new Result()
                    {
                        Title = tab.Title,
                        IcoPath = GetBrowserIcoPath(tab.BrowserName),
                        Score = titleMatch.Score + browserNameMatch.Score,
                        TitleHighlightData = titleMatch.MatchData,
                        SubTitle = tab.BrowserName,
                        ContextData = tab,
                        Action = c =>
                        {
                            if (c.SpecialKeyState.CtrlPressed)
                            {
                                tab.CloseTab();
                                // Re-query to remove closed tab from the results
                                Context.API.ChangeQuery(query.RawQuery, true);
                            }
                            else
                                tab.ActivateTab();

                            return true;
                        },
                        ShowBadge = true,
                        BadgeIcoPath = IconPath
                    });
                }
            }
            
            return Task.FromResult(results.OrderBy(x => x.Title).ToList());
        }

        private string GetBrowserIcoPath(string browserName)
        {
            // Defined at
            // https://github.com/jjw24/BrowserTabs/blob/6d95bc467c58eb89c8e2f707d72d4cf180c48976/BrowserTabs/BrowserTabManager.cs#L18
            return browserName.ToLower() switch
            {
                "chrome" => "Images/chrome.png",
                "msedge" => "Images/msedge.png",
                _ => "Images/chromium.png",
            };
        }

        public Task InitAsync(PluginInitContext context)
        {
            Context = context;
            Settings = Context.API.LoadSettingJsonStorage<Settings>();

            return Task.CompletedTask;
        }

        public string GetTranslatedPluginTitle()
        {

            return "Browser Tabs";
        }

        public string GetTranslatedPluginDescription()
        {
            return "Search, activate, or close browser tabs. A centralized plugin for managing all open tabs in your browser";
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult == null)
                return new List<Result>();

            if (selectedResult.ContextData is BrowserTab tab)
                return ContextMenu.GetBrowserTabContextMenu(tab);

            return new List<Result>();
        }

        public Control CreateSettingPanel()
        {
            return SettingWindow;
            //return SettingWindow ??= new SettingWindow(Settings);
        }
    }
}
