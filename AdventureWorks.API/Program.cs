using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.Mapper;
using AdventureWorks.BAL.Service;
using AdventureWorks.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.ModelBuilder;
using System.Data;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);


var modelBuilder = new ODataConventionModelBuilder();

builder.Services.AddDbContext<dbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Interface Specification
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddTransient<IDbConnection>(provider => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers().AddOData(
options => options.Select().Filter().OrderBy().Expand().SetMaxTop(null)
);

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
