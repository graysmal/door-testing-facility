using CsvHelper;
using Microsoft.EntityFrameworkCore;
using MudBlazor;


namespace door_testing_facility.Models
{
    public class ReaderEventService
    {
        public List<ReaderEvent> Events = new List<ReaderEvent>();
        public DateRange? SelectedRangeFilter { get; set; }
        public string SelectedOpener { get; set; } = default!;
        public string? SelectedDoor { get; set; } = default!;
        public DayOfWeek? SelectedDayOfWeek { get; set; } = default!;

        public ReaderEventService(String csvPath) {
            _ = SetEvents(File.Open(csvPath, FileMode.Open));
        }
        /*public ReaderEventService(IDbContextFactory<PhysicalDeviceTestingContext> contextFactory)
        {
            Events = contextFactory.CreateDbContext().BadgeReaders.Select(x => (ReaderEvent)x).ToList();
            try
            {
                SelectedRangeFilter = new DateRange(GetEarliestEventDate()!.Value.Date, GetLatestEventDate()!.Value.Date);
            }
            catch (Exception e) {
                Console.WriteLine("Something went wrong getting earliest and latest events for dataset, dataset empty? | " + e.Message);
            }
            
            SelectedOpener = GetTopXOpeners(1).FirstOrDefault().Key!; // Set default selected opener for opener insights panel
        }*/

        public async Task SetEvents(Stream stream) {
            using (var reader = new StreamReader(stream)) // Set max bytes because file surpasses default max
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                var records = new List<ReaderEvent>();
                await foreach (var record in csv.GetRecordsAsync<ReaderEvent>()) // Steps through as GetRecordsAsync is an enumerable
                {
                    records.Add(record);
                }
                Events = records;
                SelectedRangeFilter = new DateRange(GetEarliestEventDate()!.Value.Date, GetLatestEventDate()!.Value.Date);
                SelectedOpener = GetTopXOpeners(1).FirstOrDefault().Key!; // Set default selected opener for opener insights panel
            }
        }
        public DateTime? GetEarliestEventDate()
        {
            return Events.Min(x => x.EVENTTIME)!.Value;
        }

        public DateTime? GetLatestEventDate()
        {
            return Events.Max(x => x.EVENTTIME)!.Value;
        }

        public bool IsRangeOneDay() 
        {
            if (SelectedRangeFilter == null) return false;
            return SelectedRangeFilter!.Start == SelectedRangeFilter!.End;
        }

        public List<ReaderEvent> GetFilteredEvents()
        {
            return Events.Where(e => (e.EVENTTIME!.Value.Date >= SelectedRangeFilter!.Start!.Value.Date && e.EVENTTIME!.Value.Date <= SelectedRangeFilter!.End!.Value.Date)).ToList();
        }

        // Returns top X openers within selected timeframe
        public Dictionary<String, int> GetTopXOpeners(int x)
        {
            List<ReaderEvent> e = GetFilteredEvents().GroupBy(x => x.IDHASH)
                .OrderByDescending(x => x.Count())
                .Select(x => x.First())
                .ToList();
            return e.GroupBy(x => x.IDHASH)
                .OrderByDescending(x => x.Count())
                .Take(x)
                .ToDictionary(x => x.Key!, x => x.Count());
        }

        // Returns top X doors within selected timeframe
        public Dictionary<String, int> GetTopXDoors(int x)
        {
            List<ReaderEvent> e = GetFilteredEvents().GroupBy(x => x.READERDESC)
                .OrderByDescending(x => x.Count())
                .Select(x => x.First())
                .ToList();
            return e.GroupBy(x => x.READERDESC)
                .OrderByDescending(x => x.Count())
                .Take(x)
                .ToDictionary(x => x.Key!, x => x.Count());
        }

        public int RangeDaysCount()
        {
            return (SelectedRangeFilter!.End!.Value - SelectedRangeFilter.Start!.Value).Days + 1;
        }

        public List<DateTime>? GetAllActiveDays()
        {
            return Enumerable.Range(0, (GetLatestEventDate() - GetEarliestEventDate()).GetValueOrDefault().Days + 1)
                .Select(i => GetEarliestEventDate()!.Value.AddDays(i))
                .ToList();
        }
        public List<DateTime>? GetAllSelectedDays()
        {
            return Enumerable.Range(0, (SelectedRangeFilter!.End!.Value - SelectedRangeFilter.Start!.Value).Days + 1)
                .Select(i => SelectedRangeFilter!.Start!.Value.AddDays(i))
                .ToList();
        }

        public List<DateTime>? GetAllActiveHours()
        {
            return Enumerable.Range(0, 24)
                .Select(i => new DateTime().AddHours(i))
                .ToList();
        }

    }
}
