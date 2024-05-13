using ApiWithJWT.Services;
using ApiWithJWT.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using ApiWithJWT.Wrappers;
using Microsoft.AspNetCore.Identity;
using ApiWithJWT.Models;
using System.Security.Claims;
using ApiWithJWT.Data;
using Microsoft.EntityFrameworkCore;


namespace ApiWithJWT;

public static class RegisterServices
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
	{
		// register dbcontext
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
		});


		// register services
		services.AddTransient<IAccountService, AccountService>();
		

		services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));


		// register identity
		var identitySettings = configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();
		services.AddIdentity<User, Role>(options =>
		{

			options.SignIn.RequireConfirmedAccount = false;
			options.SignIn.RequireConfirmedEmail = false;
			options.User.RequireUniqueEmail = false;

			options.Password.RequireDigit = identitySettings.PasswordRequireDigit;
			options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
			options.Password.RequireNonAlphanumeric = identitySettings.PasswordRequireNonAlphanumic;
			options.Password.RequireUppercase = identitySettings.PasswordRequireUppercase;
			options.Password.RequireLowercase = identitySettings.PasswordRequireLowercase;
		})
			   .AddEntityFrameworkStores<ApplicationDbContext>()
			   .AddDefaultTokenProviders();


		// register jwt Bearer
		var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(options =>
			{
				options.IncludeErrorDetails = true;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ClockSkew = TimeSpan.Zero,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(jwtSettings.Key)
					),
				};

				options.Events = new JwtBearerEvents()
				{
					OnChallenge = context =>
					{
						context.HandleResponse();
						context.Response.StatusCode = 401;
						context.Response.ContentType = "application/json";
						var result = JsonSerializer.Serialize(new BaseResult(new Error(ErrorCode.AccessDenied, "You are not Authorized")));
						return context.Response.WriteAsync(result);
					},
					OnForbidden = context =>
					{
						context.Response.StatusCode = 403;
						context.Response.ContentType = "application/json";
						var result = JsonSerializer.Serialize(new BaseResult(new Error(ErrorCode.AccessDenied, "You are not authorized to access this resource")));
						return context.Response.WriteAsync(result);
					},
					OnTokenValidated = async context =>
					{
						var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
						var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
						if (claimsIdentity.Claims?.Any() != true)
							context.Fail("This token has no claims.");

						var securityStamp = claimsIdentity.FindFirst("AspNet.Identity.SecurityStamp");
						if (securityStamp is null)
							context.Fail("This token has no secuirty stamp");

						var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
						if (validatedUser == null)
							context.Fail("Token secuirty stamp is not valid.");
					},

				};
			});




		services.AddControllers().AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		});


		// add public policy
		services.AddCors(x =>
		{
			x.AddPolicy("Any", b =>
			{
				b.AllowAnyOrigin();
				b.AllowAnyHeader();
				b.AllowAnyMethod();

			});
		});



		//Swagger
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(setup =>
		{
			setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
			});
			setup.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer",
							},
							Scheme = "Bearer",
							Name = "Bearer",
							In = ParameterLocation.Header,
						}, new List<string>()
					},
				});
		});

		return services;
	}
}
