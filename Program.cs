using MinimalRideHailingAPI.Endpoints;
using MinimalRideHailingAPI.Endpoints;
using MinimalRideHailingAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

AuthEndpoints.MapEndpoints(app);
PassengerEndpoints.MapEndpoints(app);
DriverEndpoints.MapEndpoints(app);
RideEndpoints.MapEndpoints(app);
AdminEndpoints.MapEndpoints(app);

app.Run();