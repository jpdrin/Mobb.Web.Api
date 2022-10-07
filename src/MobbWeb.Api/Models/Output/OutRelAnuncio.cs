namespace MobbWeb.Api.Models.Output
{
    public class OutRelAnuncio
    {
      public int idAnuncio { get; set; }
      public int idPessoa { get; set; }    
      public string? tituloAnuncio { get; set; }
      public decimal valorServicoAnuncio { get; set; }
      public int horasServicoAnuncio { get; set; }
      public string? telefoneContatoAnuncio {get; set;}
      public string? nomeCategoriaAnuncio {get; set;}
      public string? nomeCidade {get; set;}
      public string? ufEstado {get; set;}
      public string? avaliacaoAnuncio {get; set;}
      public string? avaliacaoAnuncioPessoa {get; set;}
      public DateTime dataCadastroAnuncio {get; set;}
      public int? qtdAvaliacoesAnuncio {get; set;}
      public int? qtdMensagensRecebidas {get; set;}
      public int? qtdComentariosAnuncio {get; set;}
      public int? qtdComentariosRealizados {get; set;}
      public string? interacaoMensagem {get; set;}

      
    }
}