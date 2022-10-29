using Dapper;
using MobbWeb.Api.Models.Output;
using System.Data;
using MobbWeb.Api.Repositories.Interfaces;
using MobbWeb.Api.Data;

namespace MobbWeb.Api.Repositories
{
  public class PessoasRepository : IPessoaRepository
  {
    private readonly DbSession _db;

    public PessoasRepository(DbSession dbSession)
    {
      _db = dbSession;
    }

    #region Login
    public async Task<dynamic> Login(string NM_Usuario, string Senha)
    {

      using (var conn = _db.Connection)
      {
        OutPessoaUsuario? pessoa = await Task.Run(() =>
        {
          DynamicParameters parametros = new DynamicParameters();
          parametros.Add("Codigo_Usuario_Pessoa", NM_Usuario);
          parametros.Add("Senha_Pessoa", Senha);
          parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
          parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

          var mobbData = conn.Query<dynamic>(sql: "sp_LoginUsuario",
                                                          param: parametros,
                                                          commandType: CommandType.StoredProcedure);

          int Return_Code = parametros.Get<Int32>("Return_Code");
          string ErrMsg = parametros.Get<string>("ErrMsg");

          return mobbData.Select(c =>
          {
            OutPessoaUsuario pessoa = new OutPessoaUsuario();

            pessoa.idPessoa = c.ID_Pessoa;
            pessoa.nomePessoa = c.Nome_Pessoa;
            pessoa.sexoPessoa = c.Sexo_Pessoa;
            pessoa.inscricaoNacionalPessoa = c.Inscricao_Nacional_Pessoa;
            pessoa.emailPessoa = c.E_mail;
            pessoa.telefoneCelularPessoa = c.Telefone_Celular;
            pessoa.dataNascimentoPessoa = c.Data_Nascimento_Pessoa;
            pessoa.dataCadastroPessoa = c.Data_Cadastro_Pessoa;
            pessoa.dataAlteracaoPessoa = c.Data_Alteracao_Pessoa;
            pessoa.dataAlteracaoSenhaPessoa = c.Data_Alteracao_Senha_Pessoa;
            pessoa.urlImagemPerfilPessoa = c.URL_Imagem_Perfil_Pessoa;
            pessoa.publicIDCloudinaryImagemPerfil = c.Public_ID_Cloudinary_Imagem_Perfil;
            pessoa.codigoUsuarioPessoa = c.Codigo_Usuario_Pessoa;

            return pessoa;
          }).FirstOrDefault();

        });

        return pessoa;
      }
    }
    #endregion

    #region Lista todos os países
    public async Task<List<OutPais>> ListaPaises()
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          var data = conn.Query<dynamic>(@"SELECT ID_Pais
                                                 ,Nome_Pais
                                                 ,Nome_PT_Pais
                                                 ,Sigla_Pais
                                          FROM dbo.Paises WITH (NOLOCK);");
          List<OutPais> paises = new List<OutPais>();

          paises.AddRange(data.Select(p =>
          {
            OutPais pais = new OutPais();

            pais.idPais = p.ID_Pais;
            pais.nomePais = p.Nome_Pais;
            pais.nomePtPais = p.Nome_PT_Pais;
            pais.siglaPais = p.Sigla_Pais;

            return pais;
          }));

          return paises;
        });
      }
    }
    #endregion

    #region Lista todos os estados
    public async Task<List<OutEstado>> ListaEstados()
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          var data = conn.Query<dynamic>(@"SELECT ID_Estado
                                                  ,Nome_Estado
                                                  ,UF_Estado
                                                  ,DDD_Estado
                                                  ,ID_Pais
                                            FROM dbo.Estados WITH (NOLOCK)");
          List<OutEstado> estados = new List<OutEstado>();

          estados.AddRange(data.Select(e =>
          {
            OutEstado estado = new OutEstado();

            estado.idEstado = e.ID_Estado;
            estado.nomeEstado = e.Nome_Estado;
            estado.ufEstado = e.UF_Estado;
            estado.dddEstado = e.DDD_Estado;
            estado.idPais = e.ID_Pais;

            return estado;
          }));

          return estados;
        });
      }
    }
    #endregion

    #region Lista todas as cidades do estado
    public async Task<List<OutCidade>> ListaCidades(int ID_Estado)
    {
      using (var conn = _db.Connection)
      {
        return await Task.Run(() =>
        {
          DynamicParameters parametros = new DynamicParameters();
          parametros.Add("@ID_Estado", ID_Estado);

          string sql = @"SELECT ID_Cidade
                              ,Nome_Cidade
                              ,ID_Estado
                        FROM dbo.Cidades WITH (NOLOCK)
                        WHERE ID_Estado = @ID_Estado";

          var data = conn.Query<dynamic>(sql: sql,
                                        param: parametros,
                                        commandType: CommandType.Text);

          List<OutCidade> cidades = new List<OutCidade>();

          cidades.AddRange(data.Select(c =>
          {
            OutCidade cidade = new OutCidade();

            cidade.idCidade = c.ID_Cidade;
            cidade.nomeCidade = c.Nome_Cidade;
            cidade.idEstado = c.ID_Estado;

            return cidade;
          }));

          return cidades;
        });
      }
    }
    #endregion

    #region Insere os dados da pessoa
    public async Task InserePessoa(string Nome_Pessoa,
                                   string Sexo_Pessoa,
                                   string Inscricao_Nacional_Pessoa,
                                   string E_Mail,
                                   string Telefone_Celular,
                                   DateTime Data_Nascimento_Pessoa,
                                   string Codigo_Usuario_Pessoa,
                                   string Senha_Pessoa)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("Nome_Pessoa", Nome_Pessoa);
        parametros.Add("Sexo_Pessoa", Sexo_Pessoa);
        parametros.Add("Inscricao_Nacional_Pessoa", Inscricao_Nacional_Pessoa);
        parametros.Add("E_Mail", E_Mail);
        parametros.Add("Telefone_Celular", Telefone_Celular);
        parametros.Add("Data_Nascimento_Pessoa", Data_Nascimento_Pessoa);
        parametros.Add("Codigo_Usuario_Pessoa", Codigo_Usuario_Pessoa);
        parametros.Add("Senha_Pessoa", Senha_Pessoa);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_PessoaAdd",
                                         param: parametros,
                                         commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao inserir usuário" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region Altera os dados da pessoa
    public async Task<dynamic> AlteraPessoa(int ID_Pessoa,
                                   string Nome_Pessoa,
                                   string Sexo_Pessoa,
                                   string Inscricao_Nacional_Pessoa,
                                   string E_Mail,
                                   string Telefone_Celular,
                                   DateTime Data_Nascimento_Pessoa,
                                   string Codigo_Usuario_Pessoa,
                                   string Senha_Pessoa,
                                   string URL_Imagem_Perfil_Pessoa)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("Nome_Pessoa", Nome_Pessoa);
        parametros.Add("Sexo_Pessoa", Sexo_Pessoa);
        parametros.Add("Inscricao_Nacional_Pessoa", Inscricao_Nacional_Pessoa);
        parametros.Add("E_Mail", E_Mail);
        parametros.Add("Telefone_Celular", Telefone_Celular);
        parametros.Add("Data_Nascimento_Pessoa", Data_Nascimento_Pessoa);
        parametros.Add("Codigo_Usuario_Pessoa", Codigo_Usuario_Pessoa);
        parametros.Add("Senha_Pessoa", Senha_Pessoa);
        parametros.Add("URL_Imagem_Perfil_Pessoa", URL_Imagem_Perfil_Pessoa);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var mobbData = await conn.QueryAsync(sql: "sp_PessoaUpd",
                                             param: parametros,
                                             commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao alterar os dados" : ErrMsg;
          throw new Exception(ErrMsg);
        }

        return mobbData.Select(c =>
        {
          OutPessoaUsuario pessoa = new OutPessoaUsuario();

          pessoa.idPessoa = c.ID_Pessoa;
          pessoa.nomePessoa = c.Nome_Pessoa;
          pessoa.sexoPessoa = c.Sexo_Pessoa;
          pessoa.inscricaoNacionalPessoa = c.Inscricao_Nacional_Pessoa;
          pessoa.emailPessoa = c.E_mail;
          pessoa.telefoneCelularPessoa = c.Telefone_Celular;
          pessoa.dataNascimentoPessoa = c.Data_Nascimento_Pessoa;
          pessoa.dataCadastroPessoa = c.Data_Cadastro_Pessoa;
          pessoa.dataAlteracaoPessoa = c.Data_Alteracao_Pessoa;
          pessoa.dataAlteracaoSenhaPessoa = c.Data_Alteracao_Senha_Pessoa;
          pessoa.urlImagemPerfilPessoa = c.URL_Imagem_Perfil_Pessoa;
          pessoa.publicIDCloudinaryImagemPerfil = c.Public_ID_Cloudinary_Imagem_Perfil;
          pessoa.codigoUsuarioPessoa = c.Codigo_Usuario_Pessoa;

          return pessoa;
        }).FirstOrDefault();
      }
    }
    #endregion

    #region Altera a imagem do perfil da pessoa
    public async Task<dynamic> AlteraImagemPerfilPessoa(int ID_Pessoa,
                                   string URL_Imagem_Perfil_Pessoa,
                                   string Public_ID_Cloudinary_Imagem_Perfil)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("URL_Imagem_Perfil_Pessoa", URL_Imagem_Perfil_Pessoa);
        parametros.Add("Public_ID_Cloudinary_Imagem_Perfil", Public_ID_Cloudinary_Imagem_Perfil);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var mobbData = await conn.QueryAsync(sql: "sp_ImagemPerfilUpd",
                                             param: parametros,
                                             commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao alterar os dados" : ErrMsg;
          throw new Exception(ErrMsg);
        }

        return mobbData.Select(c =>
        {
          OutImagemPerfil imagemPerfil = new OutImagemPerfil();

          imagemPerfil.urlImagemPerfilPessoa = c.URL_Imagem_Perfil_Pessoa;
          imagemPerfil.publicIdCloudinaryImagemPerfil = c.Public_ID_Cloudinary_Imagem_Perfil;

          return imagemPerfil;
        }).FirstOrDefault();
      }
    }
    #endregion

    #region Altera a senha da pessoa
    public async Task AlteraSenhaPessoa(int ID_Pessoa,
                                        string Senha_Atual,
                                        string Nova_Senha,
                                        string Confirmacao_Senha)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("ID_Pessoa", ID_Pessoa);
        parametros.Add("Senha_Atual", Senha_Atual);
        parametros.Add("Nova_Senha", Nova_Senha);
        parametros.Add("Confirmacao_Senha", Confirmacao_Senha);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var mobbData = await conn.QueryAsync(sql: "sp_SenhaUpd",
                                             param: parametros,
                                             commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao alterar a senha" : ErrMsg;
          throw new Exception(ErrMsg);
        }
      }
    }
    #endregion

    #region Solicita a recuperação de senha por e-mail
    public async Task<dynamic> RecuperaSenhaEmail(string Inscricao_Nacional_Pessoa)
    {
      using (var conn = _db.Connection)
      {
        DynamicParameters parametros = new DynamicParameters();
        parametros.Add("Inscricao_Nacional_Pessoa", Inscricao_Nacional_Pessoa);
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryFirstOrDefaultAsync<dynamic>(sql: "sp_RecuperaSenhaPessoa",
                                                                param: parametros,
                                                                commandType: CommandType.StoredProcedure);

        int Return_Code = parametros.Get<Int32>("Return_Code");
        string ErrMsg = parametros.Get<string>("ErrMsg");

        if (Return_Code > 0)
        {
          ErrMsg = ErrMsg.Equals("") ? "Erro ao Recuperar a Senha" : ErrMsg;
          throw new Exception(ErrMsg);
        }

        return data;
      }
    }
    #endregion
  }
}
