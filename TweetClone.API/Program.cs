using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TweetClone.API.Exceptions;
using TweetClone.API.Extensions;
using TweetClone.API.GlobalException;
using TweetClone.API.Services.Implementation;
using TweetClone.API.Services.Interface;
using TweetClone.DATA.Data;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("secrets.json", optional: true);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddExceptionHandler <ExceptionLoggingHandler>();
builder.Services.AddExceptionHandler<InvalidRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<ITweetService, TweetService>();
builder.Services.AddCloudinary(ClodinaryServiceExtension.GetAccount(config));

builder.Services.AddAuthorization();
builder.Services.AddSwaggerOpenAPI();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
