using Microsoft.EntityFrameworkCore;
using SmartRoom.API.Data;

var builder = WebApplication.CreateBuilder(args);

// connect AppDbContext with PostgreSQL using connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// register controllers to make folder "Controllers" is readable
builder.Services.AddControllers(); 

// add OpneAPI/Swagger 
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// to make routing for controllers work
app.MapControllers();
app.Run();