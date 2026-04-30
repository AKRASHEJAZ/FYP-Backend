using API_Layer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ======================
// Services Registration
// ======================

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

Services.RegisterServices(builder);

var app = builder.Build();

// ======================
// Middleware Pipeline
// ======================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();