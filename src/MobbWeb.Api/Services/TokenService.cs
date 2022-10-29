using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MobbWeb.Api.Models.Output;
using System.Security.Claims;

namespace MobbWeb.Services
{
  public static class TokenService
  {
    public static string GenerateToken(OutPessoaUsuario pessoa)
    {

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(Settings.Secret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {

        Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.Name, pessoa.codigoUsuarioPessoa),
                    new Claim(ClaimTypes.Role, pessoa.emailPessoa),
                }),
        Expires = DateTime.UtcNow.AddMinutes(90),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}