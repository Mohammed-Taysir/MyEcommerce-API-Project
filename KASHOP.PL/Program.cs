using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.Repositry;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;


namespace KASHOP.PL
{
    public class Program
    {

       
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddLocalization(options => options.ResourcesPath = "");

            const string defaultCulture = "en";
            var supportedCultures = new[]
            {
            new CultureInfo(defaultCulture),
            new CultureInfo("ar")
};
            builder.Services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Clear();

                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());


            });

            builder.Services.AddScoped<ICategoryRepositry, CategoryRepositry>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();

            var app = builder.Build();

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
