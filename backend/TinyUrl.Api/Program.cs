using Microsoft.EntityFrameworkCore;
using TinyUrl.Api.data;
using TinyUrl.Api.dtos;
using TinyUrl.Api.models;
using TinyUrl.Api.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();
var env = builder.Environment;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (env.IsDevelopment())
    {
        // SQLite
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    }
    else
    {
        // Azure SQL Server
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapPost("/api/add", (
    CreateShortUrlRequest request,
    AppDbContext dbContext,
    HttpContext httpContext) =>
{
    if (string.IsNullOrWhiteSpace(request.OriginalUrl))
    {
        return Results.BadRequest("OriginalUrl is required.");
    }

    if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out var uri) ||
        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
    {
        return Results.BadRequest("Please provide a valid HTTP/HTTPS URL.");
    }

    string shortCode;
    do
    {
        shortCode = ShortCodeService.GenerateCode(6);
    }
    while (dbContext.ShortUrls.Any(x => x.ShortCode == shortCode));

    var shortUrlEntity = new ShortUrl
    {
        Id = Guid.NewGuid(),
        OriginalUrl = request.OriginalUrl.Trim(),
        ShortCode = shortCode,
        IsPrivate = request.IsPrivate,
        ClickCount = 0,
        CreatedAt = DateTime.UtcNow,
        ModifiedAt = DateTime.UtcNow
    };

    dbContext.ShortUrls.Add(shortUrlEntity);
    dbContext.SaveChanges();

    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    var response = new ShortUrlResponse
    {
        Id = shortUrlEntity.Id,
        OriginalUrl = shortUrlEntity.OriginalUrl,
        ShortCode = shortUrlEntity.ShortCode,
        ShortUrl = $"{baseUrl}/{shortUrlEntity.ShortCode}",
        IsPrivate = shortUrlEntity.IsPrivate,
        ClickCount = shortUrlEntity.ClickCount
    };

    return Results.Created($"/api/add/{shortUrlEntity.ShortCode}", response);
});

app.MapGet("/api/public", (AppDbContext dbContext, HttpContext httpContext) => {
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    var items = dbContext.ShortUrls
        .Where(x => !x.IsPrivate)
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new ShortUrlResponse
        {
            Id = x.Id,
            OriginalUrl = x.OriginalUrl,
            ShortCode = x.ShortCode,
            ShortUrl = $"{baseUrl}/{x.ShortCode}",
            IsPrivate = x.IsPrivate,
            ClickCount = x.ClickCount
        })
        .ToList();

    return Results.Ok(items);
});

app.MapGet("/{code}", (string code, AppDbContext dbContext) =>
{
    var item = dbContext.ShortUrls
        .FirstOrDefault(x => x.ShortCode == code);

    if (item == null)
    {
        return Results.NotFound("Short URL not found.");
    }

    item.ClickCount += 1;
    dbContext.SaveChanges();

    return Results.Redirect(item.OriginalUrl);
});

app.MapDelete("/api/delete/{code}", (string code, AppDbContext dbContext) =>
{
    var item = dbContext.ShortUrls
        .FirstOrDefault(x => x.ShortCode == code);

    if (item == null)
        return Results.NotFound("Not found");

    dbContext.ShortUrls.Remove(item);
    dbContext.SaveChanges();

    return Results.NoContent();
});

app.MapDelete("/api/delete-all", (AppDbContext dbContext) =>
{
    var items = dbContext.ShortUrls.ToList();

    dbContext.ShortUrls.RemoveRange(items);
    dbContext.SaveChanges();

    return Results.NoContent();
});

app.MapPut("/api/update/{code}", (
    string code,
    CreateShortUrlRequest request,
    AppDbContext dbContext) =>
{
    var item = dbContext.ShortUrls
        .FirstOrDefault(x => x.ShortCode == code);

    if (item == null)
        return Results.NotFound();

    item.OriginalUrl = request.OriginalUrl;
    item.IsPrivate = request.IsPrivate;
    item.ModifiedAt = DateTime.UtcNow;

    dbContext.SaveChanges();

    return Results.Ok(item);
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
