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

    Task<bool> VerificaAnuncios(int ID_Estado,
                                int ID_Cidade,
                                int ID_Categoria_Anuncio);
    Task<OutAnuncios> ListaAnuncios(int ID_Estado,
                                         int ID_Cidade,
                                         int ID_Categoria_Anuncio,
                                         int offSet,
                                         int limite,
                                         string ordenacao,
                                         string titulo);
    Task<OutAnuncio> ListaAnuncio(int ID_Anuncio);
    Task<OutAnuncios> ListaAnunciosPessoa(int ID_Pessoa,
                                          int offSet,
                                          int limite,
                                          string ordenacao,
                                          string titulo);

    Task<OutAnuncios> ListaAnunciosFavoritos(int ID_Pessoa,
                                             int offSet,
                                             int limite,
                                             string ordenacao,
                                             string titulo);
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

    Task<List<int>> ListaIDAnunciosFavoritos(int ID_Pessoa);

    Task InsereAnuncioFavorito(int ID_Pessoa,
                               int ID_Anuncio);

    Task<bool> VerificaAnuncioFavorito(int ID_Pessoa,
                                       int ID_Anuncio);

    Task RemoveAnuncioFavorito(int ID_Pessoa,
                               int ID_Anuncio);

    Task InsereAvaliacaoAnuncio(int ID_Anuncio,
                                 int ID_Pessoa,
                                 int Avaliacao_Anuncio);

    Task<decimal> AvaliacaoAnuncioPessoa(int ID_Anuncio,
                                          int ID_Pessoa);

    Task DeletaComentarioAnuncio(int ID_Comentario);

    Task<List<OutRelAnuncio>> RelAnunciosCadPessoa(int ID_Pessoa,
                                                   int ID_Categoria_Anuncio,
                                                   DateTime Data_Cadastro_Inicial,
                                                   DateTime Data_Cadastro_Final,
                                                   int Avaliacao_Inicial,
                                                   int Avaliacao_Final);

    Task<List<OutRelAnuncio>> RelInteracoesAnunciosCadPessoa(int ID_Pessoa,
                                                              int ID_Categoria_Anuncio,
                                                              DateTime Data_Cadastro_Inicial,
                                                              DateTime Data_Cadastro_Final,
                                                              int Avaliacao_Inicial,
                                                              int Avaliacao_Final);

    Task<List<OutRelAnuncio>> RelInteracoesAnunciosFavPessoa(int ID_Pessoa,
                                                             int ID_Categoria_Anuncio,
                                                             DateTime Data_Cadastro_Inicial,
                                                             DateTime Data_Cadastro_Final,
                                                             int Avaliacao_Inicial,
                                                             int Avaliacao_Final);

    Task InsereMensagemAnuncio(int ID_Anuncio,
                               int ID_Pessoa);

    Task<bool> VerificaInteracaoAnuncio(int ID_Anuncio, 
                                        int ID_Pessoa);

    Task<List<OutRelAnuncio>> RelEstatisticasAnuncios(int ID_Categoria_Anuncio,
                                                      int ID_Estado,
                                                      int ID_Cidade,
                                                      DateTime Data_Cadastro_Inicial,
                                                      DateTime Data_Cadastro_Final,
                                                      int Avaliacao_Inicial,
                                                      int Avaliacao_Final);
  }
}