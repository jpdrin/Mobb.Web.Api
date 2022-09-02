namespace MobbWeb.Api.Models.Output
{
    public class OutComentario
    {
        public int idComentarioAnuncio {get; set;}
        public int idAnuncio {get; set;}
        public int idPessoa {get; set;}
        public string? nomePessoa {get; set;}
        public string? comentario {get; set;}
        public int idComentarioAnuncioPai { get; set; }
    }
}