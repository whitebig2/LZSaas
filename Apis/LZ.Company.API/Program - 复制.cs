using LZ.Common.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("Url") ?? "");
// Add services to the container.
//builder.Services.AddMCodeConsul();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddControllers().AddDapr();

//添加鉴权认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("companyservicekey", options =>
{
    //鉴权中心服务地址
    options.Authority = "http://localhost:8866";
    options.RequireHttpsMetadata = false;
    options.Audience = "CompanyService";
    options.ForwardDefaultSelector = context => "Introspection";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
//app.UseHealthCheckMiddleware();
//app.UseConsulCheckService();
app.MapControllers();

app.Run();
