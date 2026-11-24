using System;
using System.Linq;
using Microsoft.Win32;

namespace ClockSaver;

internal sealed class Settings
{
    private const string Key = @"Software\Euiyun\Clock Screen Saver";
    private const string Host = "clock.euiyun.com";

    public static string DefaultLink => $"https://{Host}/{(Loc.Korean ? "" : "en")}?f=ciel";

    public string Link { get; set; } = DefaultLink;
    public bool SpanDisplays { get; set; }

    public static bool IsClockPage(Uri uri) =>
        uri.Scheme == Uri.UriSchemeHttps && uri.IsDefaultPort && uri.Host == Host && uri.UserInfo.Length == 0
        && uri.AbsolutePath is "/" or "/en" or "/en/";

    public static Uri? ParseLink(string text) =>
        Uri.TryCreate(text.Trim(), UriKind.Absolute, out var uri) && IsClockPage(uri) ? uri : null;

    public Uri ClockUrl()
    {
        var link = ParseLink(Link) ?? new Uri(DefaultLink);
        var options = link.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(option => Uri.UnescapeDataString(option.Split('=')[0]) != "embed");
        return new UriBuilder(link)
        {
            Query = string.Join("&", new[] { "embed=1" }.Concat(options)),
            Fragment = ""
        }.Uri;
    }

    public static Settings Load()
    {
        var settings = new Settings();
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(Key);
            if (key?.GetValue(nameof(Link)) is string link && ParseLink(link) != null) settings.Link = link;
            settings.SpanDisplays = key?.GetValue(nameof(SpanDisplays)) is 1;
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or System.IO.IOException)
        {
            // A locked or unavailable user registry can still run with defaults.
        }
        return settings;
    }

    public void Save()
    {
        using var key = Registry.CurrentUser.CreateSubKey(Key);
        key.SetValue(nameof(Link), Link);
        key.SetValue(nameof(SpanDisplays), SpanDisplays ? 1 : 0, RegistryValueKind.DWord);
    }
}
