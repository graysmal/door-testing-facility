using CsvHelper.Configuration.Attributes;

namespace door_testing_facility.Models
{
    public class ReaderEvent
    {
        [Name("EVENT_TIME_UTC")]
        public DateTime? EVENTTIME { get; set; }
        
        [Name("READERDESC")]
        public string? READERDESC { get; set; }

        [Name("IDHASH")]
        public string? IDHASH { get; set; }

        [Name("DEVID")]
        public int? DEVID { get; set; }

        [Name("MACHINE")]
        public int? MACHINE { get; set; }
        
    }
}
