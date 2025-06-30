
try
{
    var builder = WebApplication.CreateBuilder(args);

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ConfiguredCorsPolicy", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerExtension();
    builder.Services.AddApplicationLayer();
    builder.Services.AddPersistenceInfrastructure(builder.Configuration);

    var app = builder.Build();

    await app.ApplyMigrationsAsync();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {

    }
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "User Management API v1");
        c.RoutePrefix = string.Empty;
    });

  
    app.UseHttpsRedirection();
    app.UseCors("ConfiguredCorsPolicy");
    app.MapControllers();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapCarter();
    app.UseAzureServiceBusConsumer();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }

