using Blazor.IndexedDB;          
using DeltaDaily.Components.Records;
using Microsoft.JSInterop;

namespace DeltaDaily.Components.DataBase;

public sealed class DeltaDailyDB : IndexedDb
{
    public DeltaDailyDB(IJSRuntime js, string name, int version) : base(js, name, version) { }
    public DeltaDailyDB(IJSRuntime js) : base(js, "deltaDaily", 1) { }

    public IndexedSet<DayRecord> Days { get; set; } = default!;
}
