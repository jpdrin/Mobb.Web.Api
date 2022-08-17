using Dapper;
using System.Data;
using MobbWeb.Api.Models.Output;
using MobbWeb.Api.Repositories.Interfaces;
using MobbWeb.Api.Data;

namespace MobbWeb.Api.Repositories
{
  public class AnunciosRepository : IAnuncioRepository
  {
    private DbSession _db;

    public AnunciosRepository(DbSession dbSession)
    {
      _db = dbSession;
    }

    #region Insere Anuncio
    public async Task InsereAnuncio(int ID_Pessoa,
                                    int ID_Categoria_Anuncio,
                                    int ID_Cidade,
                                    string Titulo_Anuncio,
                                    string Descricao_Anuncio,
                                    decimal Valor_Servico_Anuncio,
                                    int Horas_Servicos_Anuncio,
                                    string Url_Imagens_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("ID_Categoria_Anuncio", ID_Categoria_Anuncio);
        parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("Titulo_Anuncio", Titulo_Anuncio);
        parametros.Add("Descricao_Anuncio", Descricao_Anuncio);
        parametros.Add("Valor_Servico_Anuncio", Valor_Servico_Anuncio);
        parametros.Add("Horas_Servicos_Anuncio", Horas_Servicos_Anuncio);
        parametros.Add("Url_Imagens_Anuncio", Url_Imagens_Anuncio);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var mobbData = await conn.QueryAsync(sql: "sp_AnuncioAdd",
                                                param: parametros,
                                                commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao inserir anúncio" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region Lista Anuncios
    public async Task<List<OutAnuncio>> ListaAnuncios(int ID_Estado,
                                                      int ID_Cidade,
                                                      int ID_Categoria_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Estado", ID_Estado);
        parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("ID_Categoria_Anuncio", ID_Categoria_Anuncio);

        return await Task.Run(async () =>
        {
          string sql = @"SELECT ID_Anuncio            
                               ,ID_Pessoa             
                               ,Titulo_Anuncio        
                               ,Descricao_Anuncio     
                               ,Valor_Servico_Anuncio 
                               ,Horas_Servicos_Anuncio
                               ,ID_Categoria_Anuncio  
                               ,Nome_Categoria_Anuncio
                               ,ID_Cidade             
                               ,Nome_Cidade           
                               ,ID_Estado             
                               ,Nome_Estado
                               ,Url_Imagem_Anuncio
                         FROM dbo.fn_RetornaAnuncios(@ID_Estado, @ID_Cidade, @ID_Categoria_Anuncio);";
          
          var data = await conn.QueryAsync<dynamic>(sql: sql,
                                                    param: parametros,
                                                    commandType: CommandType.Text);

          List<OutAnuncio> anuncios = new List<OutAnuncio>();

          anuncios.AddRange(data.Select(a =>
          {
            OutAnuncio anuncio = new OutAnuncio();

            anuncio.idAnuncio = a.ID_Anuncio;
            anuncio.idPessoa = a.ID_Pessoa;
            anuncio.tituloAnuncio = a.Titulo_Anuncio;
            anuncio.descricaoAnuncio = a.Descricao_Anuncio;
            anuncio.valorServicoAnuncio = a.Valor_Servico_Anuncio;
            anuncio.horasServicoAnuncio = a.Horas_Servicos_Anuncio;
            anuncio.idCategoriaAnuncio = a.ID_Categoria_Anuncio;
            anuncio.nomeCategoriaAnuncio = a.Nome_Categoria_Anuncio;
            anuncio.idCidade = a.ID_Cidade;
            anuncio.nomeCidade = a.Nome_Cidade;
            anuncio.idEstado = a.ID_Estado;
            anuncio.nomeEstado = a.Nome_Estado;
            anuncio.UrlImagemAnuncio = a.Url_Imagem_Anuncio;

            return anuncio;
          }));

          return anuncios;
        });
      }
    }

    public async Task<OutAnuncio> listaAnuncio(int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        OutAnuncio? outAnuncio = await Task.Run(async () =>
        {
          DynamicParameters parametros = new DynamicParameters();
          parametros.Add("@ID_Anuncio", ID_Anuncio);

          string sql = @"SELECT Anuncios.ID_Anuncio
                               ,Anuncios.ID_Pessoa
                               ,Anuncios.Titulo_Anuncio
                               ,Anuncios.Descricao_Anuncio
                               ,Anuncios.Valor_Servico_Anuncio
                               ,Anuncios.Horas_Servicos_Anuncio
                               ,Categoria_Anuncio.ID_Categoria_Anuncio
                               ,Categoria_Anuncio.Nome_Categoria_Anuncio
                               ,Cidades.ID_Cidade
                               ,Cidades.Nome_Cidade
                               ,Estados.ID_Estado
                               ,Estados.Nome_Estado
                         FROM dbo.Anuncios WITH (NOLOCK)     
                              INNER JOIN dbo.Categoria_Anuncio WITH (NOLOCK)
                              ON Categoria_Anuncio.ID_Categoria_Anuncio = Anuncios.ID_Categoria_Anuncio
                              INNER JOIN dbo.Cidades WITH (NOLOCK)
                              ON Cidades.ID_Cidade = Anuncios.ID_Cidade
                              INNER JOIN dbo.Estados WITH (NOLOCK)
                              ON Estados.ID_Estado = Cidades.ID_Estado
                         WHERE ID_Anuncio = @ID_Anuncio";

          var data = await conn.QueryAsync<dynamic>(sql: sql,
                                                    param: parametros,
                                                    commandType: CommandType.Text);
          return data.Select(a =>
          {
            OutAnuncio? anuncio = new OutAnuncio();

            anuncio.idAnuncio = a.ID_Anuncio;
            anuncio.idPessoa = a.ID_Pessoa;
            anuncio.tituloAnuncio = a.Titulo_Anuncio;
            anuncio.descricaoAnuncio = a.Descricao_Anuncio;
            anuncio.valorServicoAnuncio = a.Valor_Servico_Anuncio;
            anuncio.horasServicoAnuncio = a.Horas_Servicos_Anuncio;
            anuncio.idCategoriaAnuncio = a.ID_Categoria_Anuncio;
            anuncio.nomeCategoriaAnuncio = a.Nome_Categoria_Anuncio;
            anuncio.idCidade = a.ID_Cidade;
            anuncio.nomeCidade = a.Nome_Cidade;
            anuncio.idEstado = a.ID_Estado;
            anuncio.nomeEstado = a.Nome_Estado;

            return anuncio;
          }).FirstOrDefault();

        });
        return outAnuncio;
      }
    }
    #endregion

    #region Lista anúncios da pessoa
    public async Task<List<OutAnuncio>> listaAnunciosPessoa(int ID_Pessoa)
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          DynamicParameters parametros = new DynamicParameters();
          parametros.Add("@ID_Pessoa", ID_Pessoa);

          string sql = @"SELECT Anuncios.ID_Anuncio
                               ,Anuncios.ID_Pessoa
                               ,Anuncios.Titulo_Anuncio
                               ,Anuncios.Descricao_Anuncio
                               ,Anuncios.Valor_Servico_Anuncio
                               ,Anuncios.Horas_Servicos_Anuncio
                               ,Categoria_Anuncio.Nome_Categoria_Anuncio
                         FROM dbo.Anuncios WITH (NOLOCK)     
                              INNER JOIN dbo.Categoria_Anuncio WITH (NOLOCK)
                              ON Categoria_Anuncio.ID_Categoria_Anuncio = Anuncios.ID_Categoria_Anuncio
                         WHERE ID_Pessoa = @ID_Pessoa";

          var data = conn.Query<dynamic>(sql: sql,
                                         param: parametros,
                                         commandType: CommandType.Text);

          List<OutAnuncio> anuncios = new List<OutAnuncio>();

          anuncios.AddRange(data.Select(c =>
          {
            OutAnuncio anuncio = new OutAnuncio();

            anuncio.idAnuncio = c.ID_Anuncio;
            anuncio.idPessoa = c.ID_Pessoa;
            anuncio.tituloAnuncio = c.Titulo_Anuncio;
            anuncio.descricaoAnuncio = c.Descricao_Anuncio;
            anuncio.valorServicoAnuncio = c.Valor_Servico_Anuncio;
            anuncio.horasServicoAnuncio = c.Horas_Servicos_Anuncio;
            anuncio.nomeCategoriaAnuncio = c.Nome_Categoria_Anuncio;

            return anuncio;
          }));

          return anuncios;
        });
      }
    }
    #endregion

    #region Lista categorias do anúncio
    public async Task<List<OutCategoriaAnuncio>> ListaCategoriasAnuncio()
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          var data = conn.Query<dynamic>(@"SELECT ID_Categoria_Anuncio
                                                 ,Nome_Categoria_Anuncio
                                                 ,Url_Imagem_Categoria_Anuncio
                                           FROM dbo.Categoria_Anuncio WITH (NOLOCK)");

          List<OutCategoriaAnuncio> categorias = new List<OutCategoriaAnuncio>();

          categorias.AddRange(data.Select(c =>
          {
            OutCategoriaAnuncio categoria = new OutCategoriaAnuncio();

            categoria.idCategoriaAnuncio = c.ID_Categoria_Anuncio;
            categoria.nomeCategoriaAnuncio = c.Nome_Categoria_Anuncio;
            categoria.urlImagemCategoriaAnuncio = c.Url_Imagem_Categoria_Anuncio;

            return categoria;
          }));

          return categorias;

        });
      }
    }
    #endregion

    #region Lista imagens do anúncio
    public async Task<List<OutImagensAnuncio>> ListaImagensAnuncio(int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          DynamicParameters parametros = new DynamicParameters();
          parametros.Add("@ID_Anuncio", ID_Anuncio);

          string sql = @"SELECT ID_Imagem_Anuncio
                               ,ID_Anuncio
                               ,Url_Imagem_Anuncio
                               ,Public_ID_Cloudinary_Imagem_Anuncio
                        FROM dbo.Imagens_Anuncios WITH (NOLOCK)
                        WHERE ID_Anuncio = @ID_Anuncio;";

          var data = conn.Query<dynamic>(sql: sql,
                                         param: parametros,
                                         commandType: CommandType.Text);

          List<OutImagensAnuncio> ImagensAnuncio = new List<OutImagensAnuncio>();

          ImagensAnuncio.AddRange(data.Select(a =>
          {
            OutImagensAnuncio imagemAnuncio = new OutImagensAnuncio();
            imagemAnuncio.idImagemAnuncio = a.ID_Imagem_Anuncio;
            imagemAnuncio.idAnuncio = a.ID_Anuncio;
            imagemAnuncio.urlImagemAnuncio = a.Url_Imagem_Anuncio;
            imagemAnuncio.publicIdCloudinaryImagemAnuncio = a.Public_ID_Cloudinary_Imagem_Anuncio;

            return imagemAnuncio;
          }));

          return ImagensAnuncio;

        });
      }
    }
    #endregion

    #region Atualiza Anúncio
    public async Task AtualizaAnuncio(int ID_Anuncio,
                                      string Titulo_Anuncio,
                                      string Descricao_Anuncio,
                                      decimal Valor_Servico_Anuncio,
                                      int Horas_Servicos_Anuncio,
                                      int ID_Categoria_Anuncio,
                                      int ID_Cidade,
                                      string Url_Imagens_Anuncio,
                                      string Url_Imagens_Anuncio_Del)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("@ID_Anuncio", ID_Anuncio);
        parametros.Add("Titulo_Anuncio", Titulo_Anuncio);
        parametros.Add("Descricao_Anuncio", Descricao_Anuncio);
        parametros.Add("Valor_Servico_Anuncio", Valor_Servico_Anuncio);
        parametros.Add("Horas_Servicos_Anuncio", Horas_Servicos_Anuncio);
        parametros.Add("ID_Categoria_Anuncio", ID_Categoria_Anuncio);
        parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("Url_Imagens_Anuncio", Url_Imagens_Anuncio);
        parametros.Add("Url_Imagens_Anuncio_Del", Url_Imagens_Anuncio_Del);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_AnuncioUpd",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao atualizar anúncio" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region Deleta Anúncio
    public async Task DeletaAnuncio(int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("@ID_Anuncio", ID_Anuncio);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_AnuncioDel",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao deletar anúncio" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion
  }
}
