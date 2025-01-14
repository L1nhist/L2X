using L2X.Core;
using L2X.Core.Utilities;
using L2X.Exchange.Data;
using L2X.Exchange.Data.Services;
using L2X.Exchange.Site.Servicves;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace L2X.Exchange.Site;

/// <summary>
/// 
/// </summary>
/// <param name="configuration"></param>
public class Startup(IConfiguration configuration)
{
    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Add services to the container
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        //L2X.Exchange.Data.Startup.ConfigureServices(Configuration, services);
        //services.AddScoped<KafkaPublisherService>();
        //services.AddScoped(typeof(KafkaPublisherService<>));
        //services.AddScoped<KafkaConsumerService>();
        //services.AddScoped(typeof(KafkaConsumerService<>));
        //services.AddScoped<IOrderService, OrderService>();
        //services.AddScoped<OrderPlacingService>();

        services.AddPostgreExchangeContext(Configuration);

        L2X.Services.Startup.ConfigureServices(Configuration, services);

		services.AddScoped<IOrderService, OrderService>()
				.AddScoped<DataGenerationService>();
		        //.AddScoped<PreOrderCronJobService>();

		//services.AddLogging(builder => builder.ClearProviders().AddConsole());
		//services.AddHostedService<CronService<OrderPlacingService>>();

		var origins = Configuration["AllowedHosts"];
        if (!Util.IsEmpty(origins))
        {
            var allows = origins.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (allows.Length > 0)
            {
                services.AddCors(opts => opts.AddPolicy("L2XCORS", builder =>
                {
                    builder.WithOrigins(allows).AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }));
            }
            else
            {
                services.AddCors(opts => opts.AddPolicy("L2XCORS", builder =>
                {
                    builder.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }));
            }
        }

        services.AddControllersWithViews()
            .AddJsonOptions(options =>
		        {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
		        });
        services.AddEndpointsApiExplorer();

        //Authen config
        services.AddHttpContextAccessor();

		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        services.AddAuthentication(opts =>
        {
            opts.DefaultScheme = "JWT0CKE";
            opts.DefaultChallengeScheme = "JWT0CKE";
        })
        .AddPolicyScheme("JWT0CKE", "Bearer_Or_Cookie", opts =>
        {
            opts.ForwardDefaultSelector = context =>
            {
                var auth = context.Request.Headers["Authorization"].ToString();
                if (auth.StartsWith("Bearer ") == true)
                    return JwtBearerDefaults.AuthenticationScheme;

                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        })
        .AddCookie(opts =>
        {
            opts.LoginPath = "/login";
            opts.AccessDeniedPath = "/forbidden";
            opts.ExpireTimeSpan = TimeSpan.FromDays(1);
            opts.SlidingExpiration = true;
            opts.Cookie = new CookieBuilder
            {
                Domain = "*",
                HttpOnly = false,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                SecurePolicy = CookieSecurePolicy.Always
            };
            //opts.Events = new CookieAuthenticationEvents
            //{
            //    OnSignedIn = context =>
            //    {
            //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnSignedIn", context.Principal.Identity.Name);
            //        return Task.CompletedTask;
            //    },
            //    OnSigningOut = context =>
            //    {
            //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnSigningOut", context.HttpContext.User.Identity.Name);
            //        return Task.CompletedTask;
            //    },
            //    OnValidatePrincipal = context =>
            //    {
            //        Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnValidatePrincipal", context.Principal.Identity.Name);
            //        return Task.CompletedTask;
            //    }
            //};
        })
        //.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts =>
        //{
        //    opts.Authority = "https://localhost:7245"; // This is the OAuth20.Server URI
        //    opts.ClientId = "CQfCdxqsHECyetQxi5SyCQ";
        //    opts.ClientSecret = "4EOc7xdLEEvOQwCbN2sF6vOGgTTCOUyYnErNx6CU";
        //    opts.ResponseType = "code";
        //    opts.CallbackPath = "/signin-oidc";
        //    opts.SaveTokens = true;
        //    opts.Scope.Add("jwtapitestapp.read");
        //})
        .AddJwtBearer(opts =>
        {
            opts.RequireHttpsMetadata = false;
            opts.SaveToken = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration["JwtToken:Issuer"] ?? "L2X",
                ValidAudience = Configuration["JwtToken:Audience"] ?? "L2X",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtToken:SigningKey"] ?? "")),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
            };
        });
        services.AddAuthorization();

        //Config Swagger UI
        services.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "L2X Echange",
                Version = "v1",
                Description = "An API to perform Exchange services for L2 System",
                //TermsOfService = new Uri("https://localhost:7099/terms"),
                //Contact = new OpenApiContact
                //{
                //    Name = "Ryan Doré",
                //    Email = "linhist@gmail.com",
                //    Url = new Uri("https://facebook.com/linhist"),
                //},
                //License = new OpenApiLicense
                //{
                //    Name = "L2 Exchange API LICX",
                //    Url = new Uri("https://localhost:7099/license"),
                //}
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opts.IncludeXmlComments(xmlPath);

            opts.AddSecurityDefinition("Bearer", new()
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });
            opts.AddSecurityRequirement(new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    /// <summary>
    /// Configure the HTTP request pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        else
        {
            //Config Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "L2 Exchange API V1");
            });
        }

        app.UseCors("L2XCORS");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
    }
}