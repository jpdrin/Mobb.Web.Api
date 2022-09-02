using MobbWeb.Api.Models.Output;

namespace MobbWeb.Api.Repositories.Interfaces
{
  public interface IAnuncioRepository
  {
    Task InsereAnuncio(int ID_Pessoa,
                       int ID_Categoria_Anuncio,
                       int ID_Cidade,
                       string Titulo_Anuncio,
                       string Descricao_Anuncio,
                       decimal Valor_Servico_Anuncio,
                       int Horas_Servicos_Anuncio,
                       string Telefone_Contato_Anuncio,
                       string Url_Imagens_Anuncio);

    Task<List<OutAnuncio>> ListaAnuncios(int ID_Estado,
                                         int ID_Cidade,
                                         int ID_Categoria_Anuncio);
    Task<OutAnuncio> listaAnuncio(int ID_Anuncio);
    Task<List<OutAnuncio>> listaAnunciosPessoa(int ID_Pessoa);
    Task<List<OutCategoriaAnuncio>> ListaCategoriasAnuncio();
    Task<List<OutImagensAnuncio>> ListaImagensAnuncio(int ID_Anuncio);

    Task AtualizaAnuncio(int ID_Anuncio,
                         string Titulo_Anuncio,
                         string Descricao_Anuncio,
                         decimal Valor_Servico_Anuncio,
                         int Horas_Servicos_Anuncio,
                         string Telefone_Contato_Anuncio,
                         int ID_Categoria_Anuncio,
                         int ID_Cidade,
                         string Url_Imagens_Anuncio,
                         string Url_Imagens_Anuncio_Del);

    Task DeletaAnuncio(int ID_Anuncio);

    Task<List<OutComentario>> ListaComentariosAnuncio(int ID_Anuncio);

    Task InsereComentario(int ID_Anuncio,
                          int ID_Pessoa,
                          string Comentario,
                          int ID_Comentario_Anuncio_Pai);

    Task<OutComentariosLista> ListaComentariosAnuncioResposta(int ID_Anuncio);
    // Task<List<OutComentariosLista>> ListaComentariosAnuncioResposta(int ID_Anuncio);
  }
}