namespace MobbWeb.Api.Models.Output
{
  public class OutPessoaUsuario
  {
    public int idPessoa { get; set; }
    public string nomePessoa { get; set; } = string.Empty;
    public string sexoPessoa { get; set; } = string.Empty;
    public string inscricaoNacionalPessoa { get; set; } = string.Empty;
    public string emailPessoa { get; set; } = string.Empty;
    public string telefoneCelularPessoa { get; set; } = string.Empty;
    public DateTime dataNascimentoPessoa { get; set; }
    public DateTime dataCadastroPessoa { get; set; }
    public DateTime? dataAlteracaoPessoa {get; set;}    
    public DateTime? dataAlteracaoSenhaPessoa {get; set;}
    public string? urlImagemPerfilPessoa {get; set;}
    public string? publicIDCloudinaryImagemPerfil {get; set;}
    public string codigoUsuarioPessoa { get; set; } = string.Empty;
    public string senhaUsuarioPessoa { get; set; } = string.Empty;
  }
}