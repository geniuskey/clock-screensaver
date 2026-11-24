# CLOCK Screen Saver privacy policy

The desktop screen saver has no user accounts or built-in telemetry. It stores your clock link and display preference in the Windows user registry. It stores the WebView2 profile, runtime loader and diagnostic log under `%LOCALAPPDATA%\Euiyun\Clock Screen Saver`. The diagnostic log may contain the URL used to load a clock. These files are stored locally.

In online mode the screen saver requests the selected clock page from [CLOCK](https://clock.euiyun.com). The website and its hosting provider receive ordinary web request data such as the IP address and requested URL. The URL can include a selected face, time zone and any custom text included in an imported share link. Clock faces may make further requests for time synchronization, weather, news or market data. The desktop host blocks Google Tag Manager and denies browser permission prompts, including geolocation. Opening a clock in your regular browser uses that browser's settings.

The embedded browser uses the Microsoft Edge WebView2 Runtime installed on your device. Its handling of diagnostic data is described in the [Microsoft Privacy Statement](https://privacy.microsoft.com/privacystatement) and [WebView2 privacy documentation](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/privacy).

To remove local screen saver data after uninstalling, delete the folder above and the registry key `HKCU\Software\Euiyun\Clock Screen Saver`. For privacy questions, contact the [repository maintainer](https://github.com/geniuskey).
