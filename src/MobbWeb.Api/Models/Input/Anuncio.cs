namespace MobbWeb.Api.Models.Input
{
  public class Anuncio
  {
    public int idPessoa { get; set; }
    public int idAnuncio {get; set;}
    public int idCidade {get; set;}
    public int idCategoriaAnuncio { get; set; }
    public string? tituloAnuncio { get; set; }
    public string? descricaoAnuncio { get; set; }
    public decimal valorServicoAnuncio { get; set; }
    public int horasServicoAnuncio { get; set; }
    public string telefoneContatoAnuncio {get; set;}
    public string? urlImagensAnuncio {get; set;}
    
    public string? urlImagensAnuncioDel {get; set;}
  }
}