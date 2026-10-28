using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NabyScape;

internal static class KoreanUiDiagnostics
{
    internal static async Task Run(App app, OverlayWindow overlay, string directory, Action<string, bool> check)
    {
        overlay.Hide();
        app.Settings.UiLanguage = "en";
        Localization.Configure(app.Settings);
        var window = new SettingsWindow(app) { ShowActivated = false };
        try
        {
            window.Show();
            Diagnostics.NavigateSettings(window, "Application");
            await Task.Delay(100);
            var language = Diagnostics.Descendants(window).OfType<ComboBox>()
                .Single(control => UI.Identity(control) == "Interface language");
            check("Korean is offered by the real language selector", language.Items.Cast<string>().Contains("한국어"));
            language.SelectedItem = "한국어";
            var save = Diagnostics.Descendants(window).OfType<Button>()
                .Single(control => UI.Identity(control) == "Save language and accessibility");
            save.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            check("Language selector persists ko using the real save action", app.Store.Load<Settings>("settings").UiLanguage == "ko");
        }
        finally { window.Close(); }

        var reloaded = app.Store.Load<Settings>("settings");
        reloaded.Normalize();
        Localization.Configure(reloaded);
        check("Reloaded Korean setting survives normalization", Localization.Language == "ko" && Localization.Text("Settings") == "설정");
        check("User-owned content is preserved", UI.Text("Settings").Text == "Settings" && Localization.Text("My original note 한글") == "My original note 한글");
        var inventories = new Dictionary<string, string[]>();
        foreach (string category in new[] { "Home", "Appearance", "Ambience", "Motion", "Layout", "Visibility", "Controls", "Content", "Audio", "Clipboard", "Weather", "System", "Launch", "AI", "Notifications", "Application" })
        {
            window = new SettingsWindow(app) { ShowActivated = false, Width = 1100, Height = 780 };
            try
            {
                window.Show();
                Diagnostics.NavigateSettings(window, category);
                await Task.Delay(100);
                window.UpdateLayout();
                var captions = Diagnostics.Descendants(window).OfType<TextBlock>().Where(label => label.IsVisible)
                    .Select(label => label.Text).Where(value => value.Length > 0).Distinct().ToArray();
                inventories[category] = captions;
                check("Korean Settings category renders: " + category, window.Title == "NabyScape · 설정" && captions.Any(ContainsHangul));
                Capture(window, Path.Combine(directory, "ko-settings-" + category + ".png"));
                if (category == "Audio")
                {
                    var exclude = Diagnostics.Descendants(window).OfType<CheckBox>().Single(control => UI.Identity(control) == "Exclude browser media sessions");
                    exclude.BringIntoView();
                    exclude.IsChecked = true;
                    exclude.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    await Task.Delay(400);
                    check("Browser exclusion toggle configures the media service and persists", app.Settings.ExcludeBrowserMedia && app.Media.BrowserMediaExcluded && app.Store.Load<Settings>("settings").ExcludeBrowserMedia);
                    window.UpdateLayout();
                    Capture(window, Path.Combine(directory, "ko-browser-exclusion.png"));
                }
                if (category == "Application")
                {
                    check("Update preferences retain stable IDs in Korean", Diagnostics.Descendants(window).OfType<CheckBox>().Any(control => UI.Identity(control) == "Check automatically once per day") && Diagnostics.Descendants(window).OfType<CheckBox>().Any(control => UI.Identity(control) == "Download updates automatically"));
                    window.Width = 600;
                    window.Height = 650;
                    var language = Diagnostics.Descendants(window).OfType<ComboBox>().Single(control => UI.Identity(control) == "Interface language");
                    language.BringIntoView();
                    await Task.Delay(100);
                    window.UpdateLayout();
                    check("Reopened selector retains Korean", language.SelectedItem?.ToString() == "한국어");
                    check("Korean language selector remains keyboard accessible", language.IsEnabled && language.Focusable && language.IsTabStop);
                    Capture(window, Path.Combine(directory, "ko-language-compact.png"));
                    var search = Diagnostics.Descendants(window).OfType<TextBox>().Single(control => UI.Identity(control) == "Search settings");
                    search.Text = "언어";
                    await Task.Delay(60);
                    var results = Diagnostics.Descendants(window).OfType<Border>().Single(control => UI.Identity(control) == "Settings search results");
                    var matches = Diagnostics.Descendants(results).OfType<Button>().ToArray();
                    check("Korean settings search finds language controls", results.IsVisible && matches.Length > 0);
                    if (matches.Length > 0)
                    {
                        matches[0].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        await Task.Delay(100);
                        check("Korean search result navigates to the General page", Diagnostics.CurrentSettingsCategory(window) == "Application" && search.Text.Length == 0);
                    }
                }
            }
            finally { window.Close(); }
        }
        File.WriteAllText(Path.Combine(directory, "ko-visible-captions.json"), JsonSerializer.Serialize(inventories, new JsonSerializerOptions { WriteIndented = true }));
        app.Settings.UiLanguage = "ko";
        app.ChangeSettings();
        foreach (string tool in new[] { "timer", "focus", "stopwatch", "notes", "calendar", "links", "awake", "clips" })
        {
            overlay.Show();
            overlay.Reveal(false);
            overlay.ShowTools(tool, false);
            await Task.Delay(150);
            overlay.UpdateLayout();
            Capture(overlay, Path.Combine(directory, "ko-widget-" + tool + ".png"));
            check("Korean widget renders: " + tool, overlay.IsVisible && Diagnostics.Descendants(overlay).OfType<TextBlock>().Any(label => label.IsVisible && ContainsHangul(label.Text)));
            if (tool == "notes")
                check("Notes heading is translated on the real widget", Diagnostics.Descendants(overlay).OfType<TextBlock>().Any(label => label.IsVisible && label.Text == "메모"));
        }
        overlay.Hide();
        check("Localization diagnostics do not start voice or screen sharing", !app.Assistant.VoiceConnected && !app.Assistant.Desktop.Sharing);
    }

    private static bool ContainsHangul(string value) => value.Any(character => character >= '\uac00' && character <= '\ud7a3');

    private static void Capture(FrameworkElement element, string path)
    {
        element.UpdateLayout();
        var bitmap = new RenderTargetBitmap((int)Math.Ceiling(element.ActualWidth), (int)Math.Ceiling(element.ActualHeight), 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(path);
        encoder.Save(stream);
    }
}
