// Requires NuGet package Microsoft.Extensions.Configuration.Json

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using log4net.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CoreAPI
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public Startup(IHostingEnvironment env)

        {
            var builder = new ConfigurationBuilder()

                .SetBasePath(env.ContentRootPath)

                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)

                .AddEnvironmentVariables();

            Configuration = builder.Build();
            
        }


        public IConfigurationRoot Configuration
        {
            get;
            set;
        }
        public static string ConnectionString
        {
            get;
            private set;
        }

        public static string SHA256Key
        {
            get;
            private set;
        }

        public static string gateway_ReqQR
        {
            get;
            private set;
        }
        public static string gateway_QueryResult
        {
            get;
            private set;
        }
        public static string gateway_QueryBalance
        {
            get;
            private set;
        }

        public static string gateway_auth
        {
            get;
            private set;
        }

        public static string gateway_eWalletBalance
        {
            get;
            private set;
        }

        public static string gateway_eWalletDedectuion
        {
            get;
            private set;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add the configuration singleton here
            services.AddSingleton<IConfiguration>(Configuration);

            SwaggerConfig(services);

            //token verification service
            VerifyAuthentication(services);

            
            //HttpClientFactory
            //services.AddHttpClient();
        }

        private static void VerifyAuthentication(IServiceCollection services)
        {
            byte[] signingKey = Convert.FromBase64String(Environment.GetEnvironmentVariable("SigningKey"));
            var issuerSigningKey = new X509SecurityKey(new X509Certificate2(new X509Certificate(signingKey)));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                IssuerSigningKey = issuerSigningKey
            };
            services.AddAuthentication()
                .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddLog4Net();
            app.UseHttpsRedirection();
            app.UseMvc();

            ConnectionString = Configuration["ConnectionStrings:DBConn"];
            SHA256Key = Configuration["SHA256Key"];

            gateway_ReqQR = Configuration["gateway:iJoozQR"] + "/api/Req";
            gateway_QueryResult = Configuration["gateway:iJoozQR"] + "/api/QueryResult";
            gateway_QueryBalance = Configuration["gateway:iJoozQR"] + "/api/QueryBalance";

            gateway_auth = Configuration["gateway:auth"]+ "/connect/token";

            gateway_eWalletBalance = Configuration["gateway:eWallet"]+ "/api/EWallets/user/{0}";
            gateway_eWalletDedectuion = Configuration["gateway:eWallet"] + "/api/EWallets/saveDeduct";
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "EWallet API V1"); });

        }

        private static void SwaggerConfig(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "QR Management API",
                    Description = "QR Management",
                    Contact = new OpenApiContact { Name = "Chen Xiaojie", Email = "sujaychan@gmail.com" }
                });
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Bearer", openApiSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new List<string>()
                    }
                });
            });
        }

        //public static string GetConnectionString()
        //{
        //    return Startup.ConnectionString;
        //}
    }
}
