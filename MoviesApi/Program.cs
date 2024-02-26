using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MoviesApi.Data;
using MoviesApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(Options=> 
    Options.UseSqlServer(ConnectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=> {


    options.AddSecurityDefinition("Bearer" ,new OpenApiSecurityScheme
    {
        Name="Authorization" ,
        Type =SecuritySchemeType.ApiKey ,
        Scheme= "Bearer" ,
        BearerFormat ="JWT" ,
        In=ParameterLocation.Header ,
        Description="Enter Your JWT Key"
    });

});
builder.Services.AddTransient<IGenresServices, GenresServices>();
builder.Services.AddTransient<IMoviesServices, MoviesServices>();
builder.Services.AddAutoMapper(typeof(Program));
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
