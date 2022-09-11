using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MobbWeb;
using MobbWeb.Api.Data;
using MobbWeb.Api.Repositories;
using MobbWeb.Api.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<DbSession>(); //Adicionei para funcionar a conexão da injeção de dependência
builder.Services.AddTransient<IAnuncioRepository, AnunciosRepository>();
builder.Services.AddTransient<IPessoaRepository, PessoasRepository>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => //ADICIONEI
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "MOBB", Version = "v1" });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
  {
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});


#region ADicionei
var key = Encoding.ASCII.GetBytes(Settings.Secret); //ADICIONEI

builder.Services.AddAuthentication(x =>
{ //ADICIONEI
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
  x.RequireHttpsMetadata = false;
  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero //Faz com que o tempo de expiração do token seja exato
  };
});

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true); //Faz com que aceite campos sem valores para requisição (Retira o Is Required)
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication(); //Adicionei;
app.UseAuthorization();
app.UseCors(option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());//Adicionei, para que deixe eu consumir dos CORS diferente

app.MapControllers();

app.Run();
