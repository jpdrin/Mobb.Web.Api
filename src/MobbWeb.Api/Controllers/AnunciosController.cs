using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobbWeb.Api.Models;
using MobbWeb.Api.Models.Input;
using MobbWeb.Api.Repositories;
using MobbWeb.Services;
using MobbWeb.Api.Repositories.Interfaces;

namespace MobbWeb.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AnunciosController : ControllerBase
{

  private readonly IAnuncioRepository _anuncioRepository;

  public AnunciosController(IAnuncioRepository anuncioRepository)
  {
    _anuncioRepository = anuncioRepository;
  }

  #region Insere Anuncio
  [Authorize]
  [HttpPost]
  [Route("insere-anuncio")]
  public async Task<ActionResult<dynamic>> InsereAnuncio([FromBody] Anuncio anuncio)
  {
    try
    {
      await _anuncioRepository.InsereAnuncio(anuncio.idPessoa,
                                             anuncio.idCategoriaAnuncio,
                                             anuncio.idCidade,
                                             anuncio.tituloAnuncio,
                                             anuncio.descricaoAnuncio,
                                             anuncio.valorServicoAnuncio,
                                             anuncio.horasServicoAnuncio,
                                             anuncio.telefoneContatoAnuncio,
                                             anuncio.urlImagensAnuncio);
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Verifica se existem anúncios  
  [AllowAnonymous]
  [HttpGet]
  [Route("verifica-anuncios/{idEstado}/{idCidade}/{idCategoriaAnuncio}")]
  public async Task<bool> VerificaAnuncios(int idEstado,
                                           int idCidade,
                                           int idCategoriaAnuncio)
  {
    try
    {
      return await _anuncioRepository.VerificaAnuncios(idEstado, idCidade, idCategoriaAnuncio);
    }
    catch
    {
      throw new Exception("Ocorreu um erro na requisição");
    }
  }
  #endregion

  #region lista-anuncios
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-anuncios/{idEstado}/{idCidade}/{idCategoriaAnuncio}/{offSet}/{limite}")]
  public async Task<IActionResult> ListaAnuncios(int idEstado,
                                                 int idCidade,
                                                 int idCategoriaAnuncio,
                                                 int offSet,
                                                 int limite,
                                                 string ordenacao = "",
                                                 string titulo = "")
  {
    try
    {
      return Ok(await _anuncioRepository.ListaAnuncios(idEstado,
                                                       idCidade,
                                                       idCategoriaAnuncio,
                                                       offSet,
                                                       limite,
                                                       ordenacao,
                                                       titulo));
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("lista-anuncios/{idAnuncio}")]
  public async Task<IActionResult> ListaAnuncios(int idAnuncio)
  {
    try
    {
      return Ok(await _anuncioRepository.listaAnuncio(idAnuncio));
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [Authorize]
  [HttpGet]
  [Route("lista-anuncios-pessoa/{idPessoa}")]
  public async Task<IActionResult> ListaAnunciosPessoa(int idPessoa)
  {
    try
    {
      return Ok(await _anuncioRepository.listaAnunciosPessoa(idPessoa));
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion  

  #region Lista categorias do anúncio
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-categorias-anuncio")]
  public async Task<IActionResult> ListaCategoriasAnuncio()
  {
    try
    {
      return Ok(await _anuncioRepository.ListaCategoriasAnuncio());
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
  #endregion

  #region  Lista imagens do anúncio
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-imagens-anuncio/{idAnuncio}")]
  public async Task<IActionResult> ListaImagensAnuncio(int idAnuncio)
  {
    try
    {
      return Ok(await _anuncioRepository.ListaImagensAnuncio(idAnuncio));
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }

  #endregion

  #region Atualiza Anúncio
  [Authorize]
  [HttpPost]
  [Route("atualiza-anuncio")]
  public async Task<dynamic> AtualizaAnuncio([FromBody] Anuncio anuncio)
  {
    try
    {
      await _anuncioRepository.AtualizaAnuncio(anuncio.idAnuncio,
                                               anuncio.tituloAnuncio,
                                               anuncio.descricaoAnuncio,
                                               anuncio.valorServicoAnuncio,
                                               anuncio.horasServicoAnuncio,
                                               anuncio.telefoneContatoAnuncio,
                                               anuncio.idCategoriaAnuncio,
                                               anuncio.idCidade,
                                               anuncio.urlImagensAnuncio,
                                               anuncio.urlImagensAnuncioDel);

      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region Deleta Anúncio
  [Authorize]
  [HttpDelete]
  [Route("deleta-anuncio/{idAnuncio}")]
  public async Task<IActionResult> DeletaAnuncio(int idAnuncio)
  {
    try
    {
      await _anuncioRepository.DeletaAnuncio(idAnuncio);

      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
  #endregion

  #region Lista Comentários do Anúncio
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-comentarios-anuncio/{idAnuncio}")]
  public async Task<IActionResult> ListaComentariosAnuncio(int idAnuncio)
  {
    try
    {
      return Ok(await _anuncioRepository.ListaComentariosAnuncio(idAnuncio));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
  #endregion

  [Authorize]
  [HttpPost]
  [Route("insere-comentario-anuncio")]
  public async Task<IActionResult> InsereComentario([FromBody] Comentario comentario)
  {
    try
    {
      await _anuncioRepository.InsereComentario(comentario.idAnuncio,
                                                comentario.idPessoa,
                                                comentario.comentario,
                                                comentario.idComentarioAnuncioPai);

      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("retorna-lista-comentarios")]
  public async Task<IActionResult> ListaComentariosAnuncioResposta(int idAnuncio)
  {
    try
    {
      return Ok(await _anuncioRepository.ListaComentariosAnuncioResposta(idAnuncio));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("anuncios-favoritos/{idPessoa}")]
  public async Task<IActionResult> ListaAnunciosFavoritos(int idPessoa)
  {
    try
    {
      return Ok(await _anuncioRepository.ListaAnunciosFavoritos(idPessoa));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}