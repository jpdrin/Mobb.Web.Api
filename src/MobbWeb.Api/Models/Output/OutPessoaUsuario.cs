namespace MobbWeb.Api.Models.Output
{
  public class OutPessoaUsuario
  {
    public int idPessoa { get; set; }
    public string nomePessoa { get; set; } = string.Empty;
    public string sexoPessoa { get; set; } = string.Empty;
    public string inscricaoNacionalPessoa { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string telefoneCelular { get; set; } = string.Empty;
    public DateTime dataNascimentoPessoa { get; set; }
    public DateTime dataCadastroPessoa { get; set; }
    public string codigoUsuarioPessoa { get; set; } = string.Empty;
    public string senhaPessoa { get; set; } = string.Empty;
  }
}