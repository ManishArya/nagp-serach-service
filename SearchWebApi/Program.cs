using SearchWebApi.Elastic;
using SearchWebApi.Models;
using SearchWebApi.Services;
using SearchWebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.AddEnvironmentVariables().Build();

builder.Services.AddCors(c => c.AddPolicy("policy", builder => builder.WithOrigins("https://yellow-grass-02e688510.6.azurestaticapps.net").AllowAnyMethod().AllowAnyHeader()));

builder.Services.Configure<ElasticSettings>(configuration.GetSection(nameof(ElasticSettings)));
builder.Services.Configure<ImageSettings>(configuration.GetSection(nameof(ImageSettings)));
// Add services to the container.
builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddSingleton<ElasticSearchService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("policy");

app.UseAuthorization();

app.MapControllers();

app.Run();
