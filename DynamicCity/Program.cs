using DynamicCity.Services;
using DynamicCity.Services.BackgroundServices;
using DynamicCity_Models;
using MongoDB.Bson;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DatabaseAccessData>(
    builder.Configuration.GetSection("DatabaseAccessData"));
builder.Services.Configure<CityInitData>(
    builder.Configuration.GetSection("CityInitData"));
    

builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<ICityService, CityService>();

builder.Services.AddHostedService<TimeLoopBGService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
