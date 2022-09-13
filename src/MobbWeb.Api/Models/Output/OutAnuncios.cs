using System.Collections.Generic;

namespace MobbWeb.Api.Models.Output
{
    public class OutAnuncios
    {
      public List<OutAnuncio> listaAnuncios {get; set;}
      public int quantidadeRegistros {get; set;}
    }
}