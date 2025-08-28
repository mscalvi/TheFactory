using Microsoft.JSInterop;
using CommunityToolkit.Maui.Storage;
using System.Text;

namespace ContaJunstaApp.Services;

public class ExportService
{
    private readonly IJSRuntime _js;
    private readonly IFileSaver? _fileSaver;

    public ExportService(IJSRuntime js, IFileSaver? fileSaver = null)
    {
        _js = js;
        _fileSaver = fileSaver;
    }

    public async Task SaveTextAsync(string filename, string content)
    {
        try
        {
            await _js.InvokeVoidAsync("ContaJunstaFiles.saveText", filename, content); // web/js
        }
        catch when (_fileSaver != null)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await _fileSaver!.SaveAsync(filename, ms, CancellationToken.None);        // nativo
        }
    }

    public static string CentsToPtbr(int cents) => (cents / 100.0).ToString("N2");
    public static string CsvEscape(string s) =>
        string.IsNullOrEmpty(s) ? "" :
        (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
          ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
}
