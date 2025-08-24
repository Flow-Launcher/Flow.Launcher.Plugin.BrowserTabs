using System.Collections.Generic;
using BrowserTabs;

namespace Flow.Launcher.Plugin.BrowserTabs
{
    public static class ContextMenu
    {
        internal static List<Result> GetBrowserTabContextMenu(BrowserTab tab)
        {
            var results = new List<Result>
            {
                new Result
                {
                    Title = "Switch to tab",
                    IcoPath = Main.IconPath,
                    Action = c =>
                    {
                        tab.ActivateTab();
                        return true;
                    }
                },
                new Result
                {
                    Title = "Close tab",
                    IcoPath = Main.IconPath,
                    Action = c =>
                    {
                        tab.CloseTab();
                        return true;
                    }
                }
            };

            return results;
        }
    }
}