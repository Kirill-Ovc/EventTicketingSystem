using EventTicketingSystem.API.Extensions;
using EventTicketingSystem.API.Helpers;
using EventTicketingSystem.API.Swagger;
using EventTicketingSystem.DataAccess.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<EntityNotFoundExceptionFilter>();
        options.Filters.Add<BusinessExceptionFilter>();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddBusinessServices();
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddMemoryCache();

var app = builder.Build();

// Initialize Database with initial data.
app.Services.InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
