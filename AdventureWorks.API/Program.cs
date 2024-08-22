using AdventureWorks.API;
using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.Mapper;
using AdventureWorks.BAL.Service;
using AdventureWorks.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<dbContext>((serviceProvider, options) =>
  {
      var interceptor = serviceProvider.GetRequiredService<QueryCountInterceptor>();
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
      options.AddInterceptors(interceptor);
  });

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Use camelCase
        });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Interface Specification
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ISalesOrderHeaderService, SalesOrderHeaderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<QueryTrackerService>();
builder.Services.AddScoped<QueryCountInterceptor>();
builder.Services.AddTransient<IDbConnection>(provider => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<QueryTrackerMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
