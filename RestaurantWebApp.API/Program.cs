using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Contracts;
using Contracts.Extensions;
using Contracts.Logger;
using Domain.DbContext;
using Domain.Entities;
using Domain.Repository;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Presentation;
using RestaurantWebApp.API.CustomTokenProvider;
using Services.Implementation;
using Services.JwtFeatures;
using Services.ServiceInterfaces;
using System.Reflection;
using System.Text;



namespace RestaurantWebApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            //builder.Services.ConfigureLoggerService();
            //builder.Services.ConfigureCors();

            //builder.Services.AddControllers().AddNewtonsoftJson();

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(Assembly.Load("Contracts"));
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });


            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = false;

                opt.User.RequireUniqueEmail = true;

                opt.SignIn.RequireConfirmedEmail = true;

                opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
             .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");
             //.AddPasswordValidator<CustomPasswordValidator<User>>();


            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
               opt.TokenLifespan = TimeSpan.FromHours(2));

            builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromDays(3));

            //services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsFactory>();

            //services.AddAutoMapper(typeof(Startup));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
            builder.Services.Configure<SNSConfiguration>(builder.Configuration.GetSection("SNSConfiguration"));
            builder.Services.Configure<NexmoConfiguration>(builder.Configuration.GetSection("NexmoConfiguration"));

            //builder.Services..AddAWSService<IAmazonSimpleNotificationService>();
            //builder.Services.Configure<SNSOptions>(_configuration.GetSection("AWS:SNS"));

            builder.Services.AddScoped<JwtHandler>();

            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            builder.Services.AddTransient<IMailKitEmailService, MailKitEmailService>();

            builder.Services.AddScoped<IAwsStorageService, AwsStorageService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAwsNotificationService, AwsNotificationService>();
            builder.Services.AddScoped<IAwsSubscriptionService, AwsSubscriptionService>();
            builder.Services.AddScoped<INexmoSmsService, NexmoSmsService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IContactService, ContactService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.AddSingleton<IAmazonS3>(s => new AmazonS3Client(builder.Configuration["AwsConfiguration:AwsAccessKeyId"],
                                    builder.Configuration["AwsConfiguration:AwsSecretAccessKey"],
                                    Amazon.RegionEndpoint.GetBySystemName(builder.Configuration["AwsConfiguration:AwsRegion"])));

            // When not running the app locally
            /*builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonSimpleNotificationService>();*/


            //When running the app locally
            var awsOptions = builder.Configuration.GetAWSOptions();
            awsOptions.Credentials = new BasicAWSCredentials(builder.Configuration["AWS:AccessKey"], builder.Configuration["AWS:SecretKey"]);
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonSimpleNotificationService>();


           /* builder.Services.AddSingleton<TwoFactorAuthenticator>(s =>
            {
                var issuer = builder.Configuration["TwoFactorAuth:Issuer"];
                var appName = builder.Configuration["TwoFactorAuth:AppName"];

                return new TwoFactorAuthenticator(issuer, appName);
            });*/


            //builder.Services.Configure<PayStackApiSettings>(builder.Configuration.GetSection("PayStackApiSettings"));
            //builder.Services.AddScoped<PayStackApi>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "Authorization Header with Bearer Scheme (\"bearer {token}\")",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.OperationFilter<SwaggerFileOperationFilter>();
                //options.EnableAnnotations();
                //options.UseInlineDefinitionsForEnums();
                //options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("super-admin", policy =>
                policy.RequireClaim("Role", "super-admin"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}