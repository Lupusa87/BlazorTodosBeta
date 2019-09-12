using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodosShared;
using TodosWebApi.DataLayer;
using TodosWebApi.DataLayer.TSEntities;
using TodosWebApi.GlobalDataLayer;
using TodosWebApi.JwtSecurity;
using TodosWebApi.Models;

namespace TodosWebApi
{
    public class Startup
    {
        // private readonly ILogger _logger;
        private Timer _timerUserUpdater;

        private Timer _timerReminder;

        public IConfiguration Configuration { get; }

        private readonly TableStorage TS;

        private string DemoUserID;

        //public Startup(IConfiguration configuration, ILogger<Startup> logger)
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //_logger = logger;

            //_logger.LogInformation("Added TodoRepository to services");
            GlobalFunctions.CmdReadEncryptedSettings();

            TS = new TableStorage();
            bool b = TS.EnuserTables().Result;

            TSUserEntity demoUser =  TS.FindUser("DemoUser", false, string.Empty).Result;

            if (demoUser is null)
            {
                TSUser newUser = new TSUser()
                {
                    Email = "DemoUser@ggg.ge",
                    Password = "123456789",
                    FullName = "Demo User",
                    UserName = "DemoUser",
                    UserID = TS.GetNewID("AllUsers", "LastUserID", true).Result,
                    CreateDate = DateTime.Now,
                };

                DemoUserID = newUser.UserID;
                b = TS.AddUser(newUser).Result;
            }
            else
            {
                DemoUserID = demoUser.PartitionKey;
            }

            b = TS.AddActivityLog("AllUser", "Server started", MethodBase.GetCurrentMethod()).Result;


            _timerUserUpdater = new Timer(DoWorkUserUpdater, null, TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(10));
            _timerReminder = new Timer(DoWorkReminder, null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
        }

        private async void DoWorkUserUpdater(object state)
        {
            bool b = await TS.UpdateOfflineUsers();
            b = await TS.UpdateOnlineUsersCount();
        }

        private async void DoWorkReminder(object state)
        {
            bool b;
            string a = TS.GetSetting("AllUsers", "DoActivityLog").Result.Value;
            if (string.IsNullOrEmpty(a))
            {
                if (GlobalData.DoActivityLog)
                {
                    GlobalData.DoActivityLog = false;
                }
            }
            else
            {
                b = a.ToLower().Equals("true");
                if (GlobalData.DoActivityLog!=b)
                {
                    GlobalData.DoActivityLog = b;
                }
            }

            b = await TS.SendTodoReminders(DemoUserID); 
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           
            services.AddMvc()
           .AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret)),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                //ClockSkew = TimeSpan.Zero,


            };


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   //options.RequireHttpsMetadata = false;
                   //options.SaveToken = true;
                   options.TokenValidationParameters = tokenValidationParameters;
               });


            //ეს თიშავს ავტომატურ ვალიდაციას კონტროლერში,
            //რადგან დაშიფრული მონაცემები მოდის ვალიდაციას ვერ გაივლის,
            //მაგ. სიგრძის გამო ან ემაილის ფორმატის დარღვევის გამო.
            //lupusa 4/4/2019
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();


            




            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalData.JWTSecret)), SecurityAlgorithms.HmacSha256),

            };

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));


            app.UseHttpsRedirection();

            app.UseRouting(routes =>
            {
                routes.MapControllers();
            });

           
            //GlobalSqlFunctions.Cmd_Refresh_Settings_In_Memory_From_DB(context);

           
           
          



        }
    }
}
