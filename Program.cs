using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Repositories.Implementations;
using ShiftRotationAPI.Services.Interfaces;
using ShiftRotationAPI.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Shift Rotation API",
        Version = "v1",
        Description = "REST API for automated employee shift scheduling"
    });
});

// EF Core / MySQL (local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositories
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IShiftPreferenceRepository, ShiftPreferenceRepository>();
builder.Services.AddScoped<IShiftAssignmentRepository, ShiftAssignmentRepository>();
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<ISchedulingPolicyRepository, SchedulingPolicyRepository>();

// Services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IShiftPreferenceService, ShiftPreferenceService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ISchedulingPolicyService, SchedulingPolicyService>();
builder.Services.AddScoped<IManualOverrideService, ManualOverrideService>();
builder.Services.AddScoped<IRotationService, RotationService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Auto-apply migrations + seed data on startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}

// Enable Swagger in ALL environments
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shift Rotation API v1");
    c.RoutePrefix = "swagger";
});

// Disable HTTPS Redirection for local testing
// app.UseHttpsRedirection();

// Serve the HTML/CSS/JS ops console from wwwroot/
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
