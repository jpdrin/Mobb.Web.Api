using MobbWeb.Api.Models.Output;

namespace MobbWeb.Api.Repositories.Interfaces
{
  public interface IPessoaRepository
  {
    Task<dynamic> Login(string NM_Usuario, string Senha);
    Task<List<OutPais>> ListaPaises();
    Task<List<OutEstado>> ListaEstados();
    Task<List<OutCidade>> ListaCidades(int idEstado);
    Task InserePessoa(string Nome_Pessoa,
                      string Sexo_Pessoa,
                      string Inscricao_Nacional_Pessoa,
                      string E_Mail,
                      string Telefone_Celular,
                      DateTime Data_Nascimento_Pessoa,
                      string Codigo_Usuario_Pessoa,
                      string Senha_Pessoa);   

    Task<dynamic> AlteraPessoa(int ID_Pessoa,
                                   string Nome_Pessoa,
                                   string Sexo_Pessoa,
                                   string Inscricao_Nacional_Pessoa,
                                   string E_Mail,
                                   string Telefone_Celular,
                                   DateTime Data_Nascimento_Pessoa,
                                   string Codigo_Usuario_Pessoa,
                                   string Senha_Pessoa,
                                   string URL_Imagem_Perfil_Pessoa);

    Task<dynamic> AlteraImagemPerfilPessoa(int ID_Pessoa,
                                   string URL_Imagem_Perfil_Pessoa,
                                   string Public_ID_Cloudinary_Imagem_Perfil);

    Task AlteraSenhaPessoa(int ID_Pessoa,
                           string Senha_Atual,
                           string Nova_Senha,
                           string Confirmacao_Senha);
    Task<dynamic> RecuperaSenhaEmail(string Inscricao_Nacional_Pessoa);
  }
}