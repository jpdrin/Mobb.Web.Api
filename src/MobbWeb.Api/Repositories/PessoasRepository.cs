using Dapper;
using MobbWeb.Api.Models.Output;
using System.Data;
using MobbWeb.Api.Repositories.Interfaces;
using MobbWeb.Api.Data;
using System;

namespace MobbWeb.Api.Repositories
{
  public class PessoasRepository : IPessoaRepository
  {
    private readonly DbSession _db;

    public PessoasRepository(DbSession dbSession)
    {
      _db = dbSession;
    }

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
            pessoa.email = c.E_mail;
            pessoa.telefoneCelular = c.Telefone_Celular;
            pessoa.dataNascimentoPessoa = c.Data_Nascimento_Pessoa;
            pessoa.dataCadastroPessoa = c.Data_Cadastro_Pessoa;
            pessoa.codigoUsuarioPessoa = c.Codigo_Usuario_Pessoa;

            return pessoa;
          }).FirstOrDefault();

        });

        return pessoa;
      }
    }

    #region Lista de Países
    public async Task<List<OutPais>> listaPaisesAsync()
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

    #region Lista de Estados
    public async Task<List<OutEstado>> ListaEstadosAsync()
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

    #region Lista de Cidades
    public async Task<List<OutCidade>> ListaCidadesAsync(int ID_Estado)
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

    #region Insere Pessoa
    public async Task InserePessoa(string Nome_Pessoa,
                                   string Sexo_Pessoa,
                                   string Inscricao_Nacional_Pessoa,
                                   string E_Mail,
                                   string Telefone_Celular,
                                   DateTime Data_Nascimento_Pessoa,
                                   string Codigo_Usuario_Pessoa,
                                   string Senha_Pessoa
                                   /*int ID_Cidade,
                                   string Logradouro_Endereco,
                                   string Bairro_Endereco,
                                   string Complemento,
                                   string Numero_Logradouro_Endereco*/)
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
        /*parametros.Add("ID_Cidade", ID_Cidade);
        parametros.Add("Logradouro_Endereco", Logradouro_Endereco);
        parametros.Add("Bairro_Endereco", Bairro_Endereco);
        parametros.Add("Complemento", Complemento);
        parametros.Add("Numero_Logradouro_Endereco", Numero_Logradouro_Endereco);*/
        parametros.Add("Return_Code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parametros.Add("ErrMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        var data = await conn.QueryAsync(sql: "sp_PessoaAdd",
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
  }
}
