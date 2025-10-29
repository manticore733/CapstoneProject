
using APCapstoneProject.Data;
using APCapstoneProject.Repository;
using APCapstoneProject.Service;
using APCapstoneProject.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APCapstoneProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<BankingAppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("myconn"));
            });

            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });


            builder.Services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();

            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddScoped<IBankRepository, BankRepository>();
            builder.Services.AddScoped<IBankService, BankService>();

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();


            builder.Services.AddScoped<IBankRepository, BankRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();




            // --- ADD THIS LINE TO REGISTER YOUR SETTINGS ---
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            //// Bind Cloudinary settings from appsettings.json
            //builder.Services.Configure<CloudinarySettingsDTO>(
            //    builder.Configuration.GetSection("CloudinarySettings"));






            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
        }
    }
}
