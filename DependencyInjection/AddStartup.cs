using System.IO.Compression;
using Infra.Data.Repository;
using Infra.Data.RepositoryInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoCoreDbRepository.Interfaces;
using MongoCoreDbRepository.UnitOfWork;
using ProductApp.Service;
using ProductApp.UseCases.AddProduct;
using ProductApp.UseCases.DeleteProduct;
using ProductApp.UseCases.GetProducts;
using ProductApp.UseCases.UpdateProduct;
using ProductApp.UseCasesInterfaces.AddProduct;
using ProductApp.UseCasesInterfaces.DeleteProduct;
using ProductApp.UseCasesInterfaces.GetProducts;
using ProductApp.UseCasesInterfaces.UpdateProduct;
using ProductDbRepository;

namespace DependencyInjection
{
    public static class AddStartup
    {

        public static void AddProductDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IAddProduct, AddProduct>();
            services.AddTransient<IGetProducts, GetProducts>();
            services.AddTransient<IUpdateProduct, UpdateProduct>();
            services.AddTransient<IDeleteProduct, DeleteProduct>();
            services.AddTransient<IProductService, ProductApp.Service.ProductService>();
            services.AddScoped<IMongoContext, ProductMongoContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
        
        public static void AddSwaggerAndSecurityToApp(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice Products V1");
            });
            
            app.UseAuthentication();
            app.UseAuthorization();
        }
        
        public static void AddSwaggerAndDependencies(IServiceCollection services, string title, string version, string description, IConfiguration configuration)
        {
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = title,
                        Version = version,
                        Description = description
                    });
            });
            
            services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                })
                .AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    options.EnableForHttps = true;
                });
            
            services.AddDistributedRedisCache(options =>
            {
                //options.Configuration = configuration.GetConnectionString("ConexaoRedis");
                options.Configuration = "127.0.0.1";
                options.InstanceName = "ProductAPI";
            });

        }
    }
}