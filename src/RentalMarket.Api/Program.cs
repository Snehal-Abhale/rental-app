using Microsoft.EntityFrameworkCore;
using RentalMarket.Application.Listings;
using RentalMarket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5246");

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Add Database Context (Connecting to Docker SQL Container)
var connectionString = "Server=localhost,1433;Database=RentalDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Add Dependency Injection (Wire up Interfaces)
// "When someone asks for IListingRepository, give them ListingRepository"
builder.Services.AddScoped<IListingRepository, ListingRepository>();

// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//// app.UseHttpsRedirection();
app.MapControllers(); // Enable Controllers

// Create the Database automatically (Good for dev, bad for prod)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();