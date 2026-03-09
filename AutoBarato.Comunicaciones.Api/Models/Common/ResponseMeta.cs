namespace AutoBarato.Comunicaciones.Api.Models.Common
{
    public class ResponseMeta
    {
        public int Seed { get; set; }
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
        public int? TotalItems { get; set; }
        public int? TotalPages { get; set; }
        public int? PageSize { get; set; }
    }
}
