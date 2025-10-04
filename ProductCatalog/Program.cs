using Microsoft.EntityFrameworkCore;
using ProductCatalog.API.Middleware;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Caching;
using ProductCatalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<ICacheService, CacheService>();


builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductSearchEngine, ProductSearchEngine>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

SeedData(app.Services);

app.Run();

static void SeedData(IServiceProvider services)
{
    var categoryRepo = services.GetRequiredService<ICategoryRepository>();
    var productRepo = services.GetRequiredService<IProductRepository>();

    // Seed categories
    var electronics = categoryRepo.AddAsync(new Category
    {
        Name = "Electronics",
        Description = "Electronic devices and accessories"
    }).Result;

    var computers = categoryRepo.AddAsync(new Category
    {
        Name = "Computers",
        Description = "Desktop and laptop computers",
        ParentCategoryId = electronics.Id
    }).Result;

    var phones = categoryRepo.AddAsync(new Category
    {
        Name = "Phones",
        Description = "Mobile phones and smartphones",
        ParentCategoryId = electronics.Id
    }).Result;

    var accessories = categoryRepo.AddAsync(new Category
    {
        Name = "Accessories",
        Description = "Computer and phone accessories",
        ParentCategoryId = electronics.Id
    }).Result;

    // Seed products
    productRepo.AddAsync(new Product
    {
        Name = "Gaming Laptop",
        Description = "High-performance gaming laptop with RTX 4070 graphics card",
        SKU = "LAP-001",
        Price = 1499.99m,
        Quantity = 15,
        CategoryId = computers.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Business Laptop",
        Description = "Professional laptop for business use with long battery life",
        SKU = "LAP-002",
        Price = 899.99m,
        Quantity = 25,
        CategoryId = computers.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Ultrabook",
        Description = "Thin and light ultrabook perfect for travel",
        SKU = "LAP-003",
        Price = 1299.99m,
        Quantity = 10,
        CategoryId = computers.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Smartphone Pro",
        Description = "Latest flagship smartphone with 5G connectivity",
        SKU = "PHN-001",
        Price = 999.99m,
        Quantity = 50,
        CategoryId = phones.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Budget Phone",
        Description = "Affordable smartphone with great features",
        SKU = "PHN-002",
        Price = 299.99m,
        Quantity = 100,
        CategoryId = phones.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Wireless Mouse",
        Description = "Ergonomic wireless mouse with precision tracking",
        SKU = "ACC-001",
        Price = 29.99m,
        Quantity = 200,
        CategoryId = accessories.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "Mechanical Keyboard",
        Description = "RGB mechanical keyboard with customizable keys",
        SKU = "ACC-002",
        Price = 149.99m,
        Quantity = 75,
        CategoryId = accessories.Id
    }).Wait();

    productRepo.AddAsync(new Product
    {
        Name = "USB-C Hub",
        Description = "Multi-port USB-C hub with HDMI and card reader",
        SKU = "ACC-003",
        Price = 49.99m,
        Quantity = 150,
        CategoryId = accessories.Id
    }).Wait();

    Console.WriteLine("✅ Database seeded with initial data!");
}