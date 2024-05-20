using CurrencyApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ssz ";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<CurrencyService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", builder =>
    {
        builder.SetIsOriginAllowed(origin => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();

// docker run --name currency_db -e POSTGRES_PASSWORD=admin -e POSTGRES_USER=admin -e POSTGRES_DB=currency_db -p 5432:5432 -d postgres
// postgresql://admin:admin@localhost:5432/currency_db
// docker run -d -p 8080:8080 \
//   -e "DATABASE_URL=postgresql://admin:admin@localhost:5432/currency_db" \
//   --name frankfurter hakanensari/frankfurter