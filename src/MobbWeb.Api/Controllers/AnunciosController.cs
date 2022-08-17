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
                                             anuncio.urlImagensAnuncio);
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(400, ex.Message);
    }
  }
  #endregion

  #region lista-anuncios
  [AllowAnonymous]
  [HttpGet]
  [Route("lista-anuncios/{idEstado}/{idCidade}/{idCategoriaAnuncio}")]
  public async Task<IActionResult> ListaAnuncios(int idEstado,
                                                 int idCidade,
                                                 int idCategoriaAnuncio)
  {
    try
    {
      return Ok(await _anuncioRepository.ListaAnuncios(idEstado, 
                                                       idCidade, 
                                                       idCategoriaAnuncio));
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
}