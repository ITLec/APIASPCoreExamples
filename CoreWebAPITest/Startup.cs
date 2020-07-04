using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoreWebAPITest.Middlewares;
using CoreWebAPITest.RouteConstraints;
using CoreWebAPITest.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CoreWebAPITest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string AllowAllOriginsPolicy = "AllowAllOriginsPolicy";
        public static DataProtectionHelper DataProtectionHelper { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();


            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsPolicy, builder => builder
                 .WithOrigins("http://itlec.com")
                 .SetIsOriginAllowed((host) => true)
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 );
            });
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ITLec API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://ITLec.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Rasheed Gomaa",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/spboyer"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://ITLec.com/license"),
                    } 
                });

                //https://stackoverflow.com/questions/57796805/swashbuckle-swagger-asp-net-core-pass-api-key-as-default-header-value-in-request
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Enter your Api Key below:",
                Name = ApiKeyMiddleware.API_KEY,
                In = ParameterLocation.Query,
                Type =  SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                      new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                        },
                        new List<string>()
                    }
                });

            });

            services.ConfigureSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            services.AddSwaggerGenNewtonsoftSupport();
            
            //services.Configure<RouteOptions>(options =>
            //     options.ConstraintMap.Add("validAccount", typeof(CustomValidationConstraint)));
            services.Configure<RouteOptions>(options =>
                 options.ConstraintMap.Add("validIP", typeof(CustomValidationConstraint)));

            services.Configure<RouteOptions>(options =>
                 options.ConstraintMap.Add("enum", typeof(EnumerationConstraint)));

            ////https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders =
            //        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //});

            //Add header: Authorization= bearer JWT which is generated from api/auth/token
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>
              {
                  x.TokenValidationParameters = new TokenValidationParameters
                  {
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authnetication")),
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidIssuer = "Tokens:Issuer",
                      ValidAudience = "Tokens:Audience"
                  };
              });


            var serviceProvider = services.BuildServiceProvider();

            DataProtectionHelper = ActivatorUtilities.CreateInstance<DataProtectionHelper>(serviceProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //HttpConfiguration config = new HttpConfiguration();
            //var constraintResolver = new System.Web.Http.Routing.DefaultInlineConstraintResolver();
            //constraintResolver.ConstraintMap.Add("enum", typeof(EnumerationConstraint));
            //config.MapHttpAttributeRoutes(constraintResolver);


//            app.AddJwtBearerAuthentication("PacktAuthentication", new JwtBearerOptions()
//            {
//                options.ClaimsIssuer = Configuration["Tokens:Issuer"];
//            options.Audience = Configuration["Tokens:Audience"];
//            TokenValidationParameters = new TokenValidationParameters()
//            {
//                ValidIssuer = Configuration["Tokens:Issuer"],
//                ValidAudience = Configuration["Tokens:Audience"],
//                ValidateIssuerSigningKey = true,
//                IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(Configuration["Tok ens:Key"])),
//                ValidateLifetime = true
//            }
//});


           app.RegisterTimerMiddleware();
          //  app.RegisterApiKeyMiddleware();
            app.RegisterForwardProxyMiddleware();
            app.RegisterClientIpAddressMiddleware();


            app.UseAuthentication();

            //enables the Static File Middleware.
            app.UseStaticFiles();

        //    app.UseMvc();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ITLec API V1");
             //   c.RoutePrefix = string.Empty;
            });

            app.UseForwardedHeaders();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(AllowAllOriginsPolicy);

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
