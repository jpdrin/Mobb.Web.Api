using MobbWeb.Api.Models.Output;

namespace MobbWeb.Api.Repositories.Interfaces
{
  public interface IPessoaRepository
  {
    Task<dynamic> Login(string NM_Usuario, string Senha);
    Task<List<OutPais>> listaPaisesAsync();
    Task<List<OutEstado>> ListaEstadosAsync();
    Task<List<OutCidade>> ListaCidadesAsync(int idEstado);
    Task InserePessoa(string Nome_Pessoa,
                      string Sexo_Pessoa,
                      string Inscricao_Nacional_Pessoa,
                      string E_Mail,
                      string Telefone_Celular,
                      DateTime Data_Nascimento_Pessoa,
                      string Codigo_Usuario_Pessoa,
                      string Senha_Pessoa,
                      int ID_Cidade,
                      string Logradouro_Endereco,
                      string Bairro_Endereco,
                      string Complemento,
                      string Numero_Logradouro_Endereco);
  }
}