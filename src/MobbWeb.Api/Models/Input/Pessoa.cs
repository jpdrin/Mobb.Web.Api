namespace MobbWeb.Api.Models.Input
{
    public class Pessoa
    {
        public string? nomePessoa {get; set;}
        public string? sexoPessoa {get; set;}
        public string? inscricaoNacionalPessoa {get; set;}
        public string? emailPessoa {get; set;}
        public string? telefoneCelularPessoa {get; set;}
        public DateTime dataNascimentoPessoa {get; set;}
        public string? codigoUsuarioPessoa {get; set;}
        public string? senhaUsuarioPessoa {get; set;}
        public int idCidade {get; set;}
        public string? logradouroEndereco {get; set;}        
        public string? bairroEndereco {get; set;}
        public string? complementoEndereco {get; set;}
        public string? numeroLogradouroEndereco {get; set;}
    }
}