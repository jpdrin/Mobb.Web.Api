using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobbWeb.Api.Models.Input;
using MobbWeb.Api.Models.Output;
using MobbWeb.Services;
using MobbWeb.Api.Repositories.Interfaces;
using Aspose.Email.Clients.Smtp;
using Aspose.Email;

namespace MobbWeb.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]

public class PessoasController : ControllerBase
{

  private readonly IPessoaRepository _pessoasRepository;

  public PessoasController(IPessoaRepository pessoaRepository)
  {
    _pessoasRepository = pessoaRepository;
  }

  [HttpGet]
  [Route("authenticated")]
  [Authorize]
  public bool Authenticated() => User.Identity.IsAuthenticated;

  #region Login
  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<dynamic>> Autenticacao([FromBody] PessoaUsuario pessoaInput)
  {
    try
    {
      //Recupera o Usuário do Banco
      OutPessoaUsuario usuario = new OutPessoaUsuario();
      usuario = await _pessoasRepository.Login(pessoaInput.codigoUsuarioPessoa,
                                               pessoaInput.senhaPessoa);

      //Verifica se o usuário existe
      if (usuario == null)
        throw new Exception("Usuário ou senha inválidos");
      // return NotFound(new { message = "Usuário ou senha inválidos" });

      //Gera o Token
      var token = TokenService.GenerateToken(usuario);

      //Oculta a senha
      usuario.senhaUsuarioPessoa = "";

      //Retorna os dados
      return new
      {
        usuario = usuario,
        token = token
      };
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Lista todos os países
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-paises")]
  public async Task<IActionResult> ListaPaises()
  {
    try
    {
      return Ok(await _pessoasRepository.ListaPaises());
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
  #endregion

  #region Lista todos os estados 
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-estados")]
  public async Task<IActionResult> ListaEstados()
  {
    try
    {
      return Ok(await _pessoasRepository.ListaEstados());
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
  #endregion

  #region Lista todas as cidades do estado
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-cidades/{idEstado}")]
  public async Task<IActionResult> ListaCidades(int idEstado)
  {
    try
    {
      return Ok(await _pessoasRepository.ListaCidades(idEstado));
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
  #endregion

  #region Insere os dados da pessoa
  [AllowAnonymous]
  [HttpPost]
  [Route("insere-pessoa")]
  public async Task<ActionResult<dynamic>> InserePessoa([FromBody] Pessoa? pessoa)
  {
    try
    {
      await _pessoasRepository.InserePessoa(pessoa.nomePessoa,
                                            pessoa.sexoPessoa,
                                            pessoa.inscricaoNacionalPessoa,
                                            pessoa.emailPessoa,
                                            pessoa.telefoneCelularPessoa,
                                            pessoa.dataNascimentoPessoa,
                                            pessoa.codigoUsuarioPessoa,
                                            pessoa.senhaUsuarioPessoa);
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Altera os dados da pessoa
  [Authorize]
  [HttpPost]
  [Route("altera-pessoa")]
  public async Task<ActionResult<dynamic>> AlteraPessoa([FromBody] Pessoa? pessoa)
  {
    try
    {
      OutPessoaUsuario usuario = new OutPessoaUsuario();
      usuario = await _pessoasRepository.AlteraPessoa(pessoa.idPessoa.GetValueOrDefault(),
                                            pessoa.nomePessoa,
                                            pessoa.sexoPessoa,
                                            pessoa.inscricaoNacionalPessoa,
                                            pessoa.emailPessoa,
                                            pessoa.telefoneCelularPessoa,
                                            pessoa.dataNascimentoPessoa,
                                            pessoa.codigoUsuarioPessoa,
                                            pessoa.senhaUsuarioPessoa,
                                            pessoa.urlImagemPerfilPessoa);
      return new {usuario};
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Altera a imagem do perfil da pessoa
  [Authorize]
  [HttpPost]
  [Route("altera-imagem-perfil-pessoa")]
  public async Task<ActionResult<dynamic>> AlteraImagemPerfilPessoa([FromBody] ImagemPerfil? imagemPerfil)
  {
    try
    {
      OutImagemPerfil imgPerfil = new OutImagemPerfil();
      imgPerfil = await _pessoasRepository.AlteraImagemPerfilPessoa(imagemPerfil.idPessoa, 
                                                                    imagemPerfil.urlImagemPerfilPessoa, 
                                                                    imagemPerfil.publicIdCloudinaryImagemPerfil);
      return new {imgPerfil};
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Altera a senha da pessoa
  [Authorize]
  [HttpPost]
  [Route("altera-senha-pessoa/{idPessoa}/{senhaAtual}/{novaSenha}/{confirmacaoSenha}")]
  public async Task<ActionResult> AlteraSenhaPessoa(int idPessoa,
                                                    string senhaAtual, 
                                                    string novaSenha, 
                                                    string confirmacaoSenha)
  {
    try
    {
      await _pessoasRepository.AlteraSenhaPessoa(idPessoa, 
                                                 senhaAtual,
                                                 novaSenha,
                                                 confirmacaoSenha);
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Solicita a recuperação de senha por e-mail
  [AllowAnonymous]
  [HttpPost]
  [Route("recupera-senha-email/{inscricaoNacionalPessoa}")]
  public async Task<ActionResult<dynamic>> RecuperaSenhaEmail(string inscricaoNacionalPessoa)
  {
    try
    {
      var recuperacao = await _pessoasRepository.RecuperaSenhaEmail(inscricaoNacionalPessoa);
      // Crie SmtpClient e especifique servidor, porta, nome de usuário e senha
      SmtpClient client = new SmtpClient("SMTP.office365.com", 587, "naoresponda-mobb@hotmail.com", "noReplyMobb123!");

      // Crie instâncias da classe MailMessage e especifique To, From, Subject e Message
      string corpoEmail = string.Concat(@"<body>
                                            <h1>MOBB - Busca de Serviços Online</h1>
                                            <span>Olá ", recuperacao.Nome_Pessoa, @".</span> <br />
                                            <span> Você solicitou a recuperação de senha. Segue sua nova senha para acesso: <h3>", recuperacao.Nova_Senha, @"</h3> </span> <br />
                                            <span>Você pode altera-la posteriormente no menu >Meu Perfil< em nosso site.</span> <br />
                                            <span>Caso não tenha solicitado a recuperação da mesma, entre em contato com nosso suporte: suporte-mobb@hotmail.com </span>
                                          </body>");

      MailMessage mensagem = new MailMessage();
      mensagem.From = "naoresponda-mobb@hotmail.com";
      mensagem.To = recuperacao.E_mail;
      mensagem.Subject = "Recuperação de Senha - MOBB";
      mensagem.HtmlBody = corpoEmail;

      // Enviar mensagens
      client.Send(mensagem);
      return Ok(recuperacao.E_mail);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
  #endregion
}