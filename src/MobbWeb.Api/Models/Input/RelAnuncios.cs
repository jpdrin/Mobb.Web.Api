namespace MobbWeb.Api.Models.Input
{
    public class RelAnuncios
    {
        public int idPessoa { get; set;}
        public int idCategoriaAnuncio {get; set;}
        public DateTime dataCadastroInicial {get; set;}
        public DateTime dataCadastroFinal {get; set;}
        public int avaliacaoInicial {get; set;}
        public int avaliacaoFinal {get; set;}
    }
}