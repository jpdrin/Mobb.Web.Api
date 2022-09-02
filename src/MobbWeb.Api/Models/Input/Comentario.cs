namespace MobbWeb.Api.Models.Input
{
    public class Comentario
    {
        public int idAnuncio {get; set;}
        public int idPessoa {get; set;}
        public string? comentario {get; set;}
        public int idComentarioAnuncioPai {get; set;}
    }
}