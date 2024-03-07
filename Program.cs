using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;
using AuthTokenServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using dotenv.net;
using System.Text;
using BLBServices;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<AuthTokenService>();


var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
builder.WebHost.UseUrls($"http://*:{port}");

// add _userManager service
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();



builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    DotEnv.Load();
    var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowedOrigins",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});




builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
{
    DotEnv.Load();
    var Password = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var username = Environment.GetEnvironmentVariable("DB_USERNAME");
    var host = Environment.GetEnvironmentVariable("DB_HOST");
    var database = Environment.GetEnvironmentVariable("DB_DATABASE");
    var connectionString = $"Host={host};Username={username};Password={Password};Database={database};";
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<BLPService>();




var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.UseCors("AllowedOrigins");

app.Run();

