using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zootopia.Biz;
using Zootopia.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // ✅ camelCase / PascalCase đều bind được
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .ConfigureApiBehaviorOptions(opt =>
    {
        // ✅ trả rõ lỗi validation như bạn đang thấy
        opt.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(context.ModelState);
    });

// Swagger (nếu swagger vẫn gây lỗi vì upload IFormFile thì ẩn endpoint upload khỏi swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ICitizenRepository, CitizenRepository>();
builder.Services.AddScoped<ICitizenService, CitizenService>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddHttpClient("AiService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiService:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseCors("dev");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('dbo.Citizens', 'PasswordHash') IS NULL
        BEGIN
            ALTER TABLE [dbo].[Citizens]
            ADD [PasswordHash] NVARCHAR(200) NULL;
        END
        """);
    db.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('dbo.Citizens', 'IsActive') IS NULL
        BEGIN
            ALTER TABLE [dbo].[Citizens]
            ADD [IsActive] BIT NOT NULL CONSTRAINT DF_Citizens_IsActive DEFAULT(1);
        END
        """);
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
