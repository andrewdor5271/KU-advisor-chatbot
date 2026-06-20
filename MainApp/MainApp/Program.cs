using MainApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddRazorPages();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// test db
using (var scope = app.Services.CreateScope())
{
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var sp = scope.ServiceProvider;
        var userManager =
            sp.GetRequiredService<UserManager<IdentityUser>>();
        var user = new IdentityUser
        {
            UserName = "test1@example.com",
            Email = "test1@example.com"
        };

        await userManager.CreateAsync(user, "1234567");

        var Conv = new MainApp.Models.Conversation
        {
            UserId = user.Id,
            Title = "Test 1",
            CreationDatetime = DateTime.Now,

        };

        db.Conversations.Add(Conv);
        db.SaveChanges();

        for (int i = 0; i < 48; i++) {
            db.Messages.Add(
                new MainApp.Models.Message
                {
                    Text = $"Test {i + 1}",
                    CreationDatetime = DateTime.Now,
                    SenderType = i % 2 == 0 ? MainApp.Models.SenderType.User: MainApp.Models.SenderType.Bot,
                    ConversationId = Conv.ConversationId
                }
            );
        }
        db.SaveChanges();
    }
}

app.MapGet("/debug/routes", (EndpointDataSource endpointSource) =>
{
    return endpointSource.Endpoints
        .Select(e => e.DisplayName);
});

app.Run();

