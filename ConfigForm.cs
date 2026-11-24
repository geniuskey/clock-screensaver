using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Windows.Forms;

namespace ClockSaver;

internal sealed class ConfigForm : Form
{
    private sealed class Option(string value, string label)
    {
        public string Value { get; } = value;
        public override string ToString() => label;
    }

    private static readonly Option[] Faces =
    {
        new("ciel", Loc.L("지금의 하늘 · Ciel", "Ciel")),
        new("classique", Loc.L("클래식 · Classique", "Classique")),
        new("monde", Loc.L("월드 타이머 · Fuseaux", "Fuseaux")),
        new("typographe", Loc.L("타이포그래프 · Typographe", "Typographe")),
        new("palette", Loc.L("플립 · Palette", "Palette")),
        new("lueur", Loc.L("닉시 · Lueur", "Lueur")),
        new("segments", Loc.L("세그먼트 · Segments", "Segments")),
        new("affiche", Loc.L("포스터 · Affiche", "Affiche")),
        new("epure", Loc.L("작도 · Épure", "Épure"))
    };

    private readonly ComboBox face = Choice(Faces);
    private readonly ComboBox zone = Choice(
        new("auto", Loc.L("Windows 시간대 사용", "Use Windows time zone")),
        new("Asia/Seoul", Loc.L("서울", "Seoul")),
        new("Asia/Tokyo", Loc.L("도쿄", "Tokyo")),
        new("Asia/Singapore", Loc.L("싱가포르", "Singapore")),
        new("Europe/London", Loc.L("런던", "London")),
        new("Europe/Paris", Loc.L("파리", "Paris")),
        new("America/New_York", Loc.L("뉴욕", "New York")),
        new("America/Los_Angeles", Loc.L("로스앤젤레스", "Los Angeles")),
        new("UTC", "UTC"));
    private readonly ComboBox timeFormat = Choice(
        new("1", Loc.L("24시간제", "24-hour")),
        new("0", Loc.L("12시간제", "12-hour")));
    private readonly ComboBox language = Choice(new("ko", "한국어"), new("en", "English"));
    private readonly ComboBox displays = Choice(
        new("primary", Loc.L("주 모니터에만 표시", "Show on primary display")),
        new("span", Loc.L("모든 모니터에 걸쳐 표시", "Span all displays")));
    private readonly PictureBox picture = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, Margin = Padding.Empty };
    private readonly Label noPicture = new()
    {
        Dock = DockStyle.Fill,
        ForeColor = Color.Gainsboro,
        TextAlign = ContentAlignment.MiddleCenter,
        Text = Loc.L("전체 화면에서 이 페이스를 미리 보세요.", "Use full-screen preview to see this face.")
    };
    private Uri source;

    public Settings? PreviewSettings { get; private set; }

    public ConfigForm(Settings settings)
    {
        Text = Loc.L("CLOCK 화면 보호기 설정", "CLOCK Screen Saver Settings");
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(744, 414);
        Font = SystemFonts.MessageBoxFont;

        source = Settings.ParseLink(settings.Link) ?? new Uri(Settings.DefaultLink);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(22, 16, 22, 18), ColumnCount = 1, RowCount = 5
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var heading = Caption("CLOCK");
        heading.Font = new Font("Segoe UI Semibold", 20);
        heading.Margin = new Padding(0, 0, 0, 14);
        layout.Controls.Add(heading, 0, 0);

        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53));

        var preview = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 20, 8) };
        preview.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        preview.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var frame = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(16, 19, 24), Margin = Padding.Empty };
        frame.Controls.AddRange(new Control[] { picture, noPicture });
        preview.Controls.Add(frame, 0, 0);
        var example = Caption(Loc.L("페이스 예시", "Face sample"), muted: true);
        example.Margin = new Padding(0, 6, 0, 0);
        preview.Controls.Add(example, 0, 1);
        body.Controls.Add(preview, 0, 0);

        var options = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = Padding.Empty };
        options.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        void SetRow(string title, ComboBox field)
        {
            var row = options.RowCount++;
            options.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = Caption(title);
            label.Anchor = AnchorStyles.Left;
            label.Margin = new Padding(0, 0, 14, 0);
            field.AccessibleName = title;
            field.Dock = DockStyle.Fill;
            field.Margin = new Padding(0, 4, 0, 6);
            options.Controls.Add(label, 0, row);
            options.Controls.Add(field, 1, row);
        }
        SetRow(Loc.L("시계 페이스", "Clock face"), face);
        SetRow(Loc.L("시간대", "Time zone"), zone);
        SetRow(Loc.L("시간 형식", "Time format"), timeFormat);
        SetRow(Loc.L("언어", "Language"), language);
        SetRow(Loc.L("모니터", "Displays"), displays);
        body.Controls.Add(options, 1, 0);
        layout.Controls.Add(body, 0, 1);

        var import = ActionButton(Loc.L("공유 링크 가져오기…", "Import share link…"), ImportLink);
        var reset = ActionButton(Loc.L("기본값 복원", "Restore defaults"), () =>
        {
            LoadLink(new Uri(Settings.DefaultLink));
            Select(displays, "primary");
        });
        var importActions = ButtonGroup(import, reset);
        importActions.Margin = Padding.Empty;
        var importInfo = Caption(Loc.L("웹에서 고른 페이스와 세부 옵션을 가져옵니다.", "Bring in a face and its custom options from CLOCK."), muted: true);
        importInfo.Anchor = AnchorStyles.Right;
        var share = SplitRow(importActions, importInfo);
        share.Margin = new Padding(0, 7, 0, 12);
        layout.Controls.Add(share, 0, 2);

        var rule = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = SystemColors.ControlDark, Margin = new Padding(0, 0, 0, 10) };
        layout.Controls.Add(rule, 0, 3);

        var fullScreen = ActionButton(Loc.L("전체 화면 미리 보기", "Full-screen preview"), () =>
        {
            PreviewSettings = Collect();
            DialogResult = DialogResult.Retry;
        });
        var browser = ActionButton(Loc.L("브라우저에서 보기", "Open in browser"), OpenBrowser);
        var ok = ActionButton(Loc.L("확인", "OK"), Save);
        var cancel = new Button { Text = Loc.L("취소", "Cancel"), AutoSize = true, DialogResult = DialogResult.Cancel };
        AcceptButton = ok;
        CancelButton = cancel;

        var footer = SplitRow(ButtonGroup(fullScreen, browser), ButtonGroup(ok, cancel));
        footer.Margin = Padding.Empty;
        layout.Controls.Add(footer, 0, 4);
        Controls.Add(layout);

        face.SelectedIndexChanged += (_, _) => UpdatePicture();
        LoadLink(source);
        Select(displays, settings.SpanDisplays ? "span" : "primary");
        ActiveControl = face;
    }

    private void LoadLink(Uri link)
    {
        source = link;
        var query = HttpUtility.ParseQueryString(link.Query);
        Select(face, ReadOption(query, "f", "ciel"));
        Select(zone, ReadOption(query, "tz", "auto"));
        Select(timeFormat, ReadOption(query, "h24", "1") == "0" ? "0" : "1");
        Select(language, link.AbsolutePath.StartsWith("/en", StringComparison.Ordinal) ? "en" : "ko");
    }

    private Settings Collect()
    {
        var query = HttpUtility.ParseQueryString(source.Query);
        if (ReadOption(query, "f", "ciel") != Value(face))
        {
            // A new face starts with its own defaults, not another face's dial or palette.
            query.Clear();
        }
        query.Remove("embed");
        query.Set("f", Value(face));
        if (Value(zone) == "auto") query.Remove("tz");
        else query.Set("tz", Value(zone));
        if (Value(timeFormat) == "1") query.Remove("h24");
        else query.Set("h24", "0");
        var fields = Enumerable.Range(0, query.Count).SelectMany(index =>
        {
            var key = query.GetKey(index);
            var prefix = key == null ? "" : Uri.EscapeDataString(key) + "=";
            return (query.GetValues(index) ?? Array.Empty<string>()).Select(value => prefix + Uri.EscapeDataString(value));
        });
        var url = new UriBuilder(source)
        {
            Path = Value(language) == "en" ? "/en" : "/",
            Query = string.Join("&", fields),
            Fragment = ""
        };
        return new Settings { Link = url.Uri.AbsoluteUri, SpanDisplays = Value(displays) == "span" };
    }

    private static string ReadOption(System.Collections.Specialized.NameValueCollection query, string key, string fallback)
    {
        var value = query.GetValues(key)?.FirstOrDefault();
        return string.IsNullOrEmpty(value) ? fallback : value!;
    }

    private void UpdatePicture()
    {
        var previous = picture.Image;
        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"faces/{Value(face)}.jpg");
        using var sample = resource == null ? null : Image.FromStream(resource);
        picture.Image = sample == null ? null : new Bitmap(sample);
        previous?.Dispose();
        picture.Visible = picture.Image != null;
        noPicture.Visible = picture.Image == null;
    }

    private void ImportLink()
    {
        using var dialog = new Form
        {
            Text = Loc.L("CLOCK 공유 링크 가져오기", "Import CLOCK share link"),
            ClientSize = new Size(554, 120), Padding = new Padding(14),
            ShowInTaskbar = false, FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false, MaximizeBox = false, StartPosition = FormStartPosition.CenterParent,
            AutoScaleDimensions = new SizeF(96, 96), AutoScaleMode = AutoScaleMode.Dpi, Font = Font
        };
        var hint = Loc.L("CLOCK에서 복사한 시계 공유 링크를 붙여넣으세요.", "Paste a clock share link copied from CLOCK.");
        var input = new TextBox { Dock = DockStyle.Fill, AccessibleName = hint };
        var apply = ActionButton(Loc.L("가져오기", "Import"), () =>
        {
            var link = Settings.ParseLink(input.Text);
            if (link == null)
            {
                MessageBox.Show(dialog, Loc.L("clock.euiyun.com의 시계 공유 링크를 입력하세요.",
                    "Enter a clock share link from clock.euiyun.com."), dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                input.Focus();
                return;
            }
            LoadLink(link);
            dialog.DialogResult = DialogResult.OK;
        });
        var cancel = new Button { Text = Loc.L("취소", "Cancel"), AutoSize = true, DialogResult = DialogResult.Cancel };
        var buttons = ButtonGroup(apply, cancel);
        buttons.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(Caption(hint), 0, 0);
        layout.Controls.Add(input, 0, 1);
        layout.Controls.Add(buttons, 0, 2);
        dialog.Controls.Add(layout);
        dialog.AcceptButton = apply;
        dialog.CancelButton = cancel;
        dialog.ActiveControl = input;
        dialog.ShowDialog(this);
    }

    private void OpenBrowser()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo(Collect().ClockUrl().AbsoluteUri) { UseShellExecute = true });
        }
        catch (System.ComponentModel.Win32Exception)
        {
            ShowError(Loc.L("기본 브라우저를 열지 못했습니다.", "The default browser could not be opened."));
        }
    }

    private void Save()
    {
        try
        {
            Collect().Save();
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or System.IO.IOException)
        {
            ShowError(Loc.L("설정을 저장하지 못했습니다.", "The settings could not be saved."));
        }
    }

    private void ShowError(string text) => MessageBox.Show(this, text, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private static Label Caption(string text, bool muted = false) =>
        new() { Text = text, AutoSize = true, ForeColor = muted ? SystemColors.GrayText : SystemColors.ControlText };

    private static Button ActionButton(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += (_, _) => action();
        return button;
    }

    private static FlowLayoutPanel ButtonGroup(params Control[] buttons)
    {
        var group = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
        group.Controls.AddRange(buttons);
        return group;
    }

    private static TableLayoutPanel SplitRow(Control left, Control right)
    {
        var row = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        row.Controls.Add(left, 0, 0);
        row.Controls.Add(right, 1, 0);
        return row;
    }

    private static ComboBox Choice(params Option[] options)
    {
        var box = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        box.Items.AddRange(options);
        return box;
    }

    private static string Value(ComboBox box) => ((Option)box.SelectedItem).Value;

    private static void Select(ComboBox box, string value)
    {
        var option = box.Items.Cast<Option>().FirstOrDefault(item => item.Value == value);
        if (option == null)
        {
            option = new Option(value, value);
            box.Items.Add(option);
        }
        box.SelectedItem = option;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) picture.Image?.Dispose();
        base.Dispose(disposing);
    }
}
