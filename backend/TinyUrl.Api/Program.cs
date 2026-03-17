using Microsoft.EntityFrameworkCore;
using TinyUrl.Api.data;
using TinyUrl.Api.dtos;
using TinyUrl.Api.models;
using TinyUrl.Api.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.MapPost("api/urls", (
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
    dbContext.SaveChangesAsync();

    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    var response = new ShortUrlResponse
    {
        Id = shortUrlEntity.Id,
        OriginalUrl = shortUrlEntity.OriginalUrl,
        ShortCode = shortUrlEntity.ShortCode,
        ShortUrl = $"{baseUrl}/r/{shortUrlEntity.ShortCode}",
        IsPrivate = shortUrlEntity.IsPrivate,
        ClickCount = shortUrlEntity.ClickCount
    };

    return Results.Created($"/api/urls/{shortUrlEntity.Id}", response);
});

app.MapGet("api/urls/public", (AppDbContext dbContext, HttpContext httpContext) => {
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    var items = dbContext.ShortUrls
        .Where(x => !x.IsPrivate)
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new ShortUrlResponse
        {
            Id = x.Id,
            OriginalUrl = x.OriginalUrl,
            ShortCode = x.ShortCode,
            ShortUrl = $"{baseUrl}/r/{x.ShortCode}",
            IsPrivate = x.IsPrivate,
            ClickCount = x.ClickCount
        })
        .ToList();

    return Results.Ok(items);
});

app.MapGet("/r/{shortCode}", (string shortCode, AppDbContext dbContext) =>
{
    var item = dbContext.ShortUrls
        .SingleOrDefault(x => x.ShortCode == shortCode);

    if (item == null)
    {
        return Results.NotFound("Short URL not found.");
    }

    item.ClickCount += 1;
    dbContext.SaveChanges();

    return Results.Redirect(item.OriginalUrl);
});

app.MapGet("api/urls/search", (string? term, AppDbContext dbContext, HttpContext httpContext) =>
{
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

    var query = dbContext.ShortUrls.AsQueryable();

    // When search text exists
    if (!string.IsNullOrWhiteSpace(term))
    {
        query = query.Where(x =>
            x.OriginalUrl.Contains(term) ||
            x.ShortCode.Contains(term));
    }
    else
    {
        // When search box is cleared to show public URLs again
        query = query.Where(x => !x.IsPrivate);
    }

    var items = query
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new ShortUrlResponse
        {
            Id = x.Id,
            OriginalUrl = x.OriginalUrl,
            ShortCode = x.ShortCode,
            ShortUrl = $"{baseUrl}/r/{x.ShortCode}",
            IsPrivate = x.IsPrivate,
            ClickCount = x.ClickCount
        })
        .ToList();

    return Results.Ok(items);
});

app.MapDelete("api/urls/{id}", (Guid id, AppDbContext dbContext) =>
{
    var item = dbContext.ShortUrls.FirstOrDefault(x => x.Id == id);

    if (item == null)
    {
        return Results.NotFound("URL not found.");
    }

    dbContext.ShortUrls.Remove(item);
    dbContext.SaveChanges();

    return Results.NoContent();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
