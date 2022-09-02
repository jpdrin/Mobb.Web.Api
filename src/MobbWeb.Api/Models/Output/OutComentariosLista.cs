using System.Collections.Generic;

namespace MobbWeb.Api.Models.Output
{
  public class OutComentariosLista
  {
    public OutComentariosLista()
    {
      Children = new List<OutComentariosLista>();
    }

    public int? idComentarioAnuncio { get; set; }
    public int? idAnuncio { get; set; }
    public int? idPessoa { get; set; }
    public string? nomePessoa { get; set; }
    public string? comentario { get; set; }
    public int? idComentarioAnuncioPai { get; set; }
    public List<OutComentariosLista> Children { get; set; }

    public List<OutComentariosLista> ObeterListaAninhada(List<OutComentariosLista> lstFuncoes)
    {
      var lstFuncoesAninhadas = lstFuncoes.Where(x => x.idComentarioAnuncioPai == 0)
                                           .Select(x => new OutComentariosLista
                                           {
                                             idComentarioAnuncio = x.idComentarioAnuncio,
                                             idComentarioAnuncioPai = x.idComentarioAnuncioPai,
                                             idAnuncio = x.idAnuncio,
                                             idPessoa = x.idPessoa,
                                             nomePessoa = x.nomePessoa,
                                             comentario = x.comentario,
                                             Children = ObterFuncoesFilhas(lstFuncoes, x.idComentarioAnuncio)
                                           }).ToList();
      return lstFuncoesAninhadas;
    }

    private List<OutComentariosLista> ObterFuncoesFilhas(List<OutComentariosLista> lstFunction, int? idComentarioAnuncio)
    {
      return lstFunction.Where(x => x.idComentarioAnuncioPai == idComentarioAnuncio)
                        .Select(x => new OutComentariosLista
                        {
                          idComentarioAnuncio = x.idComentarioAnuncio,
                          idComentarioAnuncioPai = x.idComentarioAnuncioPai,
                          idAnuncio = x.idAnuncio,
                          idPessoa = x.idPessoa,
                          nomePessoa = x.nomePessoa,
                          comentario = x.comentario,
                          Children = ObterFuncoesFilhas(lstFunction, x.idComentarioAnuncio)
                        }).ToList();
    }
  }
}