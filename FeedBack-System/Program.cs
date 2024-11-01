using FeedBack_System.Models;
using FeedBack_System.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
    clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
    return new MongoClient(clientSettings);
});

builder.Services.AddSingleton(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var database = mongoClient.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Feedback>(settings.FeedbackCollection);
});

builder.Services.AddSingleton(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var database = mongoClient.GetDatabase(settings.DatabaseName);
    return database.GetCollection<User>(settings.UsersCollection);
});

builder.Services.AddSingleton<FeedbackService>();
builder.Services.AddScoped<AuthService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowLocalhost3000");

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
