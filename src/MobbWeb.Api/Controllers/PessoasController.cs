using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobbWeb.Api.Models.Input;
using MobbWeb.Api.Models.Output;
using MobbWeb.Services;
using MobbWeb.Api.Repositories.Interfaces;

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

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<dynamic>> AutenticacaoAsync([FromBody] PessoaUsuario pessoaInput)
  {

    //Recupera o Usuário do Banco
    OutPessoaUsuario usuario = new OutPessoaUsuario();
    usuario = await _pessoasRepository.Login(pessoaInput.codigoUsuarioPessoa,
                                             pessoaInput.senhaPessoa);

    //Verifica se o usuário existe
    if (usuario == null)
      return NotFound(new { message = "Usuário ou senha inválidos" });

    //Gera o Token
    var token = TokenService.GenerateToken(usuario);

    //Oculta a senha
    usuario.senhaPessoa = "";

    //Cliente cli = new Cliente();

    //Retorna os dados
    return new
    {
      usuario = usuario,
      token = token
    };
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("lista-paises")]
  public async Task<IActionResult> ListaPaisesAsync()
  {
    try
    {
      return Ok(await _pessoasRepository.listaPaisesAsync());
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("lista-estados")]
  public async Task<IActionResult> ListaEstadosAsync()
  {
    try
    {
      return Ok(await _pessoasRepository.ListaEstadosAsync());
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("lista-cidades/{idEstado}")]
  public async Task<IActionResult> ListaCidadesAsync(int idEstado)
  {
    try
    {
      return Ok(await _pessoasRepository.ListaCidadesAsync(idEstado));
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

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
                                            pessoa.senhaUsuarioPessoa
                                            /*pessoa.idCidade,
                                            pessoa.logradouroEndereco,
                                            pessoa.bairroEndereco,
                                            pessoa.complementoEndereco,
                                            pessoa.numeroLogradouroEndereco*/);
      return Ok();
    }
    catch (Exception ex)
    {    
      return StatusCode(400, ex.Message);       
    }
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("teste")]
  public ActionResult teste()
  {
    return Ok("teste");
  }
}