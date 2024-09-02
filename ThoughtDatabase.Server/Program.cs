using ThoughtDatabase;
using ThoughtDatabase.Server;
using ThoughtDatabase.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Setup ThoughtDatabase
ConfigOptions.Initialize(builder.Configuration);
ServiceUserManager.Load(ConfigOptions.Config.UserDataLocation);

// Add services to the container.
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}); // Allow CORS
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
