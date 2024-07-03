using HealthTracking.Authentication.Configration;
using HealthTracking.DataService.Data;
using HealthTracking.DataService.IConfigration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Update the JWT config from the settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var connectionString = builder.Configuration.GetConnectionString("DefalutConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddApiVersioning(options =>
{
    // provide to the client the different api verions that we have
    options.ReportApiVersions = true;

    // this will allow the api to automatically provide a defalut api verion
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.DefaultApiVersion = ApiVersion.Default;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultScheme =
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

    // save the token in the AuthenticationProperties after a successful authorization.
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        // validate the token if it generated with our secret key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false, // TODO:  Update
        ValidateAudience = false, // TODO:  Update
        RequireExpirationTime = false, // TODO:  Update
        ValidateLifetime = true
    };
});

// 
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
