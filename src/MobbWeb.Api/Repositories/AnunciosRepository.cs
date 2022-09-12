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
                                    string Telefone_Contato_Anuncio,
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
        parametros.Add("Telefone_Contato_Anuncio", Telefone_Contato_Anuncio);
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

    #region Verifica se existem anúncios
    public async Task<bool> VerificaAnuncios(int ID_Estado,
                                             int ID_Cidade,
                                             int ID_Categoria_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Estado", ID_Estado);
        parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("ID_Categoria_Anuncio", ID_Categoria_Anuncio);

        var sql = @"SELECT TOP 1 1
                    FROM dbo.Anuncios WITH (NOLOCK)
                        INNER JOIN dbo.Cidades WITH (NOLOCK)
                        ON Cidades.ID_Cidade = Anuncios.ID_Cidade
                        INNER JOIN dbo.Estados WITH (NOLOCK)
                        ON Estados.ID_Estado = Cidades.ID_Estado
                    WHERE Estados.ID_Estado = @ID_Estado
                        --//Caso não informado a cidade, listara os anúncios de todo o Estado
                      AND EXISTS (SELECT TOP 1 1
                                  WHERE Anuncios.ID_Cidade = @ID_Cidade                
                                  UNION ALL                
                                  SELECT TOP 1 1
                                  WHERE @ID_Cidade = 0)
                      AND Anuncios.ID_Categoria_Anuncio = @ID_Categoria_Anuncio;";

        var data = await conn.QueryAsync(sql: sql,
                                         param: parametros,
                                         commandType: CommandType.Text);


        return data.Count() > 0;
      }
    }
    #endregion

    #region Lista Anuncios
    public async Task<OutAnuncios> ListaAnuncios(int ID_Estado,
                                                      int ID_Cidade,
                                                      int ID_Categoria_Anuncio,
                                                      int offSet,
                                                      int limite,
                                                      string ordenacao,
                                                      string titulo)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Estado", ID_Estado);
        parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("ID_Categoria_Anuncio", ID_Categoria_Anuncio);
        parametros.Add("offSet", offSet);
        parametros.Add("limite", limite);
        parametros.Add("ordenacao", ordenacao);
        parametros.Add("titulo", titulo);

        return await Task.Run(async () =>
        {

          var data = await conn.QueryMultipleAsync(sql: "sp_DadosAnuncios",
                                                   param: parametros,
                                                   commandType: CommandType.StoredProcedure);

          OutAnuncios outAnuncios = new OutAnuncios();
          List<dynamic> lista = new List<dynamic>();
          List<OutAnuncio> listaAnuncios = new List<OutAnuncio>();

          lista = (await data.ReadAsync<dynamic>()).ToList();

          listaAnuncios.AddRange(lista.Select(a =>
          {
            OutAnuncio anuncio = new OutAnuncio();

            anuncio.idAnuncio = a.ID_Anuncio;
            anuncio.idPessoa = a.ID_Pessoa;
            anuncio.tituloAnuncio = a.Titulo_Anuncio;
            anuncio.descricaoAnuncio = a.Descricao_Anuncio;
            anuncio.valorServicoAnuncio = a.Valor_Servico_Anuncio;
            anuncio.horasServicoAnuncio = a.Horas_Servicos_Anuncio;
            anuncio.telefoneContatoAnuncio = a.Telefone_Contato_Anuncio;
            anuncio.idCategoriaAnuncio = a.ID_Categoria_Anuncio;
            anuncio.nomeCategoriaAnuncio = a.Nome_Categoria_Anuncio;
            anuncio.idCidade = a.ID_Cidade;
            anuncio.nomeCidade = a.Nome_Cidade;
            anuncio.idEstado = a.ID_Estado;
            anuncio.nomeEstado = a.Nome_Estado;
            anuncio.UrlImagemAnuncio = a.Url_Imagem_Anuncio;

            return anuncio;
          }));


          outAnuncios.listaAnuncios = listaAnuncios;
          outAnuncios.quantidadeRegistros = (await data.ReadAsync<int>()).FirstOrDefault();

          return outAnuncios;
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
                               ,Anuncios.Telefone_Contato_Anuncio
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
            anuncio.telefoneContatoAnuncio = a.Telefone_Contato_Anuncio;
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
                               ,Telefone_Contato_Anuncio
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
            anuncio.telefoneContatoAnuncio = c.Telefone_Contato_Anuncio;
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
                                      string Telefone_Contato_Anuncio,
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
        parametros.Add("Telefone_Contato_Anuncio", Telefone_Contato_Anuncio);
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

    #region Lista Comentários do Anúncio
    public async Task<List<OutComentario>> ListaComentariosAnuncio(int ID_Anuncio)
    {
      DynamicParameters parametros = new DynamicParameters();
      parametros.Add("ID_Anuncio", ID_Anuncio);

      using (var conn = _db.Connection)
      {
        string sql = @"SELECT Comentarios_Anuncio.ID_Comentario_Anuncio,
                              Comentarios_Anuncio.ID_Anuncio,
                              Pessoas.ID_Pessoa,
                              Pessoas.Nome_Pessoa,
                              Comentarios_Anuncio.Comentario,
                              Comentarios_Anuncio.ID_Comentario_Anuncio_Pai
                       FROM dbo.Comentarios_Anuncio WITH (NOLOCK)
                            INNER JOIN dbo.Pessoas WITH (NOLOCK)
                            ON Pessoas.ID_Pessoa = Comentarios_Anuncio.ID_Pessoa
                       WHERE ID_Anuncio = @ID_Anuncio";

        var data = await conn.QueryAsync<dynamic>(sql: sql,
                                                  param: parametros,
                                                  commandType: CommandType.Text);

        List<OutComentario> comentarios = new List<OutComentario>();

        comentarios.AddRange(data.Select(c =>
        {
          OutComentario comentario = new OutComentario();

          comentario.idComentarioAnuncio = c.ID_Comentario_Anuncio;
          comentario.idAnuncio = c.ID_Anuncio;
          comentario.idPessoa = c.ID_Pessoa;
          comentario.nomePessoa = c.Nome_Pessoa;
          comentario.comentario = c.Comentario;
          comentario.idComentarioAnuncioPai = c.ID_Comentario_Anuncio_Pai;

          return comentario;
        }));

        return comentarios;
      }
    }
    #endregion

    #region Insere Comentário no Anúncio
    public async Task InsereComentario(int ID_Anuncio,
                                       int ID_Pessoa,
                                       string Comentario,
                                       int ID_Comentario_Anuncio_Pai)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Anuncio", ID_Anuncio);
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("Comentario", Comentario);
        parametros.Add("ID_Comentario_Anuncio_Pai", ID_Comentario_Anuncio_Pai);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_ComentarioAdd",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao Inserir Comentário" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region 
    public async Task<OutComentariosLista> ListaComentariosAnuncioResposta(int ID_Anuncio)
    {
      DynamicParameters parametros = new DynamicParameters();
      parametros.Add("ID_Anuncio", ID_Anuncio);

      using (var conn = _db.Connection)
      {
        string sql = @"SELECT Comentarios_Anuncio.ID_Comentario_Anuncio,
                              Comentarios_Anuncio.ID_Anuncio,
                              Pessoas.ID_Pessoa,
                              Pessoas.Nome_Pessoa,
                              Comentarios_Anuncio.Comentario,
                              Comentarios_Anuncio.ID_Comentario_Anuncio_Pai
                       FROM dbo.Comentarios_Anuncio WITH (NOLOCK)
                            INNER JOIN dbo.Pessoas WITH (NOLOCK)
                            ON Pessoas.ID_Pessoa = Comentarios_Anuncio.ID_Pessoa
                       WHERE ID_Anuncio = @ID_Anuncio";

        var data = await conn.QueryAsync<dynamic>(sql: sql,
                                                  param: parametros,
                                                  commandType: CommandType.Text);

        List<OutComentariosLista> comentarios = new List<OutComentariosLista>();

        comentarios.AddRange(data.Select(c =>
        {
          OutComentariosLista comentario = new OutComentariosLista();

          comentario.idComentarioAnuncio = c.ID_Comentario_Anuncio;
          comentario.idAnuncio = c.ID_Anuncio;
          comentario.idPessoa = c.ID_Pessoa;
          comentario.nomePessoa = c.Nome_Pessoa;
          comentario.comentario = c.Comentario;
          comentario.idComentarioAnuncioPai = c.ID_Comentario_Anuncio_Pai;

          return comentario;
        }));

        OutComentariosLista comentario = new OutComentariosLista();
        comentario.Children = comentario.ObeterListaAninhada(comentarios);

        return comentario;
      }
    }
    #endregion

    #region Lista os anúncios favoritos da pessoa
    public async Task<List<OutAnuncio>> ListaAnunciosFavoritos(int ID_Pessoa)
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
                               ,Telefone_Contato_Anuncio
                               ,Categoria_Anuncio.Nome_Categoria_Anuncio
                         FROM dbo.Anuncios WITH (NOLOCK)     
                             INNER JOIN dbo.Categoria_Anuncio WITH (NOLOCK)
                             ON Categoria_Anuncio.ID_Categoria_Anuncio = Anuncios.ID_Categoria_Anuncio
                             INNER JOIN dbo.Anuncios_Favoritos WITH (NOLOCK)
                             ON Anuncios_Favoritos.ID_Pessoa   = Anuncios.ID_Pessoa AND
                                 Anuncios_Favoritos.ID_Anuncio = Anuncios.ID_Anuncio
                         WHERE Anuncios.ID_Pessoa = @ID_Pessoa;";

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
            anuncio.telefoneContatoAnuncio = c.Telefone_Contato_Anuncio;
            anuncio.nomeCategoriaAnuncio = c.Nome_Categoria_Anuncio;

            return anuncio;
          }));

          return anuncios;
        });
      }
    }
    #endregion

    #region Insere anúncio na listagem de favoritos da Pessoa
    public async Task InsereAnuncioFavorito(int ID_Pessoa, 
                                            int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("ID_Anuncio", ID_Anuncio);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_AnuncioFavoritoAdd",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao inserir anúncio em favoritos" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region Verifica se o anúncio está favoritado
    public async Task<bool> VerificaAnuncioFavorito(int ID_Pessoa,
                                                    int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("ID_Anuncio", ID_Anuncio);

        string sql = @"SELECT TOP 1 1
                       FROM dbo.Anuncios_Favoritos WITH (NOLOCK)
                       WHERE ID_Pessoa  = @ID_Pessoa
                         AND ID_Anuncio = @ID_Anuncio;";

        var data = await conn.QueryAsync(sql: sql,
                                         param: parametros,
                                         commandType: CommandType.Text);
        return data.Count() > 0;
      }
    }
    #endregion

    #region Remove o anúncios dos favoritos
    public async Task RemoveAnuncioFavorito(int ID_Pessoa, 
                                            int ID_Anuncio)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("ID_Anuncio", ID_Anuncio);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_AnuncioFavoritoDel",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao remover o anúncio dos favoritos" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }                                            
    #endregion
  }
}
