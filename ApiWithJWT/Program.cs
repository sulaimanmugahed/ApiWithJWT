
using ApiWithJWT;
using ApiWithJWT.Data.Seeds;
using Microsoft.AspNetCore.Identity;
using ApiWithJWT.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("Any");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seeding data
using var scope = app.Services.CreateScope();
var scopedService = scope.ServiceProvider;
await DefaultRoles.SeedAsync(scopedService.GetRequiredService<RoleManager<Role>>());
await DefaultAdminUserData.SeedAsync(scopedService.GetRequiredService<UserManager<User>>());

app.Run();
