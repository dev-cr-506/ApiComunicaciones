namespace AutoBarato.Comunicaciones.Application.DTOs.Response.Comunicaciones
{
    public class ChatMediaResponse
    {
        public string MediaUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string MediaType { get; set; } 

        public ChatMediaResponse(string laUrlDelMedio, string laUrlDeMiniatura, string elTipoDeMedio)
        {
            MediaUrl = laUrlDelMedio;
            ThumbnailUrl = laUrlDeMiniatura;
            MediaType = elTipoDeMedio;
        }
    }
}