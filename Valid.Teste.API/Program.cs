using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Valid.Teste.API.AutoMapper;
using Valid.Teste.API.FakerConfig;
using Valid.Teste.API.Handler;
using Valid.Teste.API.Services;
using Valid.Teste.Domain.Interfaces;
using Valid.Teste.Infrastructure;
using Valid.Teste.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>(); 
builder.Services.AddSingleton<IProfileRepository, ProfileRepository>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAuthorizationHandler, ProfilePermissionHandler>();
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEditProfile", policy =>
        policy.Requirements.Add(new ProfileOperationRequirement("CanEdit")))
    .AddPolicy("CanDeleteProfile", policy =>
        policy.Requirements.Add(new ProfileOperationRequirement("CanDelete")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.AccessDeniedPath = "/api/auth/access-denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddSingleton<ProfileParameterGenerator>();
builder.Services.AddHostedService<ProfileBackgroundService>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    dbInitializer.EnsureDatabaseSetup();
}

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

