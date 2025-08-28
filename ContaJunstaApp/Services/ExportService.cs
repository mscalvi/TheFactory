using Microsoft.JSInterop;
using System.Text;

namespace ContaJunstaApp.Services;

public class ExportService
{
    private readonly IJSRuntime _js;
    private readonly IFileSaver? _fileSaver;
    public ExportService(IJSRuntime js, IServiceProvider sp)
    {
        _js = js;
        _fileSaver = sp.GetService<IFileSaver>();
    }

    public async Task SaveTextAsync(string filename, string content)
    {
        try
        {
            await _js.InvokeVoidAsync("ContaJunstaAppFiles.saveText", filename, content); // web
        }
        catch
        {
            if (_fileSaver is null) throw;
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await _fileSaver.SaveAsync(filename, ms, CancellationToken.None);          // Android nativo
        }
    }

    // helpers STATIC (para usar como ExportService.CentsToPtbr(...))
    public static string CentsToPtbr(int cents) => (cents / 100.0).ToString("N2");

    public static string CsvEscape(string s)
    {
        if (s is null) return "";
        var needQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
        if (!needQuotes) return s;
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}
