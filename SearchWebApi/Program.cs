using SearchWebApi.Services;
using SearchWebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(c => c.AddPolicy("policy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
// Add services to the container.
builder.Services.AddTransient<ISearchService, SearchService>();
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

app.UseCors("policy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
