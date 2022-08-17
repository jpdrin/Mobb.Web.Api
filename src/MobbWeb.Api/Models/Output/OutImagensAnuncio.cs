namespace MobbWeb.Api.Models.Output
{
  public class OutImagensAnuncio
  {
    public int idImagemAnuncio { get; set; }
    public int idAnuncio { get; set; }
    public string? urlImagemAnuncio { get; set; }
    public string? publicIdCloudinaryImagemAnuncio {get; set;}
  }
}