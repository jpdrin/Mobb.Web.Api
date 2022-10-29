namespace MobbWeb.Api.Models.Input
{
    public class Pessoa
    {
        public int? idPessoa {get; set;}
        public string? nomePessoa {get; set;}
        public string? sexoPessoa {get; set;}
        public string? inscricaoNacionalPessoa {get; set;}
        public string? emailPessoa {get; set;}
        public string? telefoneCelularPessoa {get; set;}
        public DateTime dataNascimentoPessoa {get; set;}
        public string? urlImagemPerfilPessoa {get; set;}
        public string? publicIDCloudinaryImagemPerfil {get; set;}
        public string? snAtualizaApenasImagem {get; set;}
        public string? codigoUsuarioPessoa {get; set;}
        public string? senhaUsuarioPessoa {get; set;}        
    }
}