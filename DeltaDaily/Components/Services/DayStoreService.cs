using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.IndexedDB;
using DeltaDaily.Components.Data;
using DeltaDaily.Components.DataBase;
using DeltaDaily.Components.Records;
using DeltaDaily.Components.Models;

namespace DeltaDaily.Components.Services
{
    public interface IDayStoreService
    {
        Task<DayModel> LoadAsync(DateTime date);
        Task SaveAsync(DayModel day);
        Task<IReadOnlyList<(DateTime date, bool closed)>> ListByMonthAsync(int year, int month);
    }


    public sealed class DayStoreService : IDayStoreService
    {
        private readonly IIndexedDbFactory _factory;
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public DayStoreService(IIndexedDbFactory factory) => _factory = factory;

        private static string Key(DateTime d) => d.ToString("yyyy-MM-dd");

        public async Task<DayModel> LoadAsync(DateTime date)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var key = Key(date);
            var rec = db.Days.SingleOrDefault(x => x.Id == key);
            if (rec is null)
                return new DayModel { Data = date };

            var model = JsonSerializer.Deserialize<DayModel>(rec.Payload, JsonOpts)
                        ?? new DayModel { Data = date };

            model.Data = date;
            return model;
        }

        public async Task SaveAsync(DayModel day)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 1);

            var k = Key(day.Data);
            var rec = db.Days.SingleOrDefault(x => x.Id == k);

            if (rec is null)
            {
                db.Days.Add(new DayRecord
                {
                    Id = k,
                    Date = k, // opcional – pode remover se não for mais usar
                    Closed = day.DiaFechado,
                    UpdatedAtUtc = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(day, JsonOpts)
                });
            }
            else
            {
                rec.Closed = day.DiaFechado;
                rec.UpdatedAtUtc = DateTime.UtcNow;
                rec.Payload = JsonSerializer.Serialize(day, JsonOpts);
                // rec.Date pode ficar como está
            }

            await db.SaveChanges();
        }

        public async Task<IReadOnlyList<(DateTime date, bool closed)>> ListByMonthAsync(int year, int month)
        {
            using var db = await _factory.Create<DeltaDailyDB>("deltaDaily", 2);

            var prefix = $"{year:D4}-{month:D2}-"; // "2025-09-"
            return db.Days
                .Where(r => r.Id.StartsWith(prefix, StringComparison.Ordinal))
                .Select(r => (DateTime.ParseExact(r.Id, "yyyy-MM-dd", null), r.Closed))
                .ToList();
        }

    }
}
