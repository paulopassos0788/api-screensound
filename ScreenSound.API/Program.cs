using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
var app = builder.Build();

app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
{
    return Results.Ok(dal.Listar());
});

app.MapGet("/Artistas/{nome}", ([FromServices] DAL < Artista > dal, string nome) =>
{
    var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

    if (artista == null)
    {
        return Results.NotFound($"Artista {nome} não encontrado.");
    }

    return Results.Ok(artista);
});

app.MapPost("/Artistas", ([FromServices] DAL < Artista > dal, [FromBody] Artista artista) =>
{
    dal.Adicionar(artista);
    return Results.Ok(artista);
});

app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
{
    var artista = dal.RecuperarPor(a => a.Id.Equals(id));
    if (artista == null)
    {
        return Results.NotFound($"Artista {artista.Nome} não encontrado.");
    }
    dal.Deletar(artista);
    return Results.NoContent();
});

app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista) => 
{
    var artistaAtualizar = dal.RecuperarPor(a => a.Id == artista.Id);

    if (artistaAtualizar is null)
    {
        return Results.NotFound();
    }

    artistaAtualizar.Nome = artista.Nome;
    artistaAtualizar.Bio = artista.Bio;
    artistaAtualizar.FotoPerfil = artista.FotoPerfil;

    dal.Atualizar(artistaAtualizar);
    return Results.Ok();
});

app.Run();
