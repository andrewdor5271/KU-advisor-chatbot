using MainApp;
using MainApp.Data;
using MainApp.Infrastructure.Conversations;
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


// custom service for convo management can now be dependency injected everywhere
builder.Services.AddScoped<IConversationsService, ConversationsService>();

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

        var Conv1 = new MainApp.Models.Conversation
        {
            Title = "Test 1",
            CreationDatetime = DateTime.UtcNow,

        };
        Conv1.SetUserId(user.Id);
        var Conv2 = new MainApp.Models.Conversation
        {
            Title = "Test 2",
            CreationDatetime = DateTime.UtcNow,

        };
        Conv2.SetUserId(user.Id);
        db.Conversations.AddRange(Conv1, Conv2);
        db.SaveChanges();

        for (int i = 0; i < 48; i++) {
            db.Messages.Add(
                new MainApp.Models.Message
                {
                    Text = $"Test {i + 1}",
                    CreationDatetime = DateTime.Now,
                    SenderType = i % 2 == 0 ? SenderType.User: SenderType.Bot,
                    ConversationId = Conv1.ConversationId
                }
            );
        }
        db.Messages.AddRange(
                new MainApp.Models.Message
                {
                    Text = "aga",
                    CreationDatetime = DateTime.Now,
                    SenderType = SenderType.User,
                    ConversationId = Conv2.ConversationId
                },
                new MainApp.Models.Message
                {
                    Text = "ogo",
                    CreationDatetime = DateTime.Now,
                    SenderType = SenderType.Bot,
                    ConversationId = Conv2.ConversationId
                }
            );
        db.SaveChanges();
    }
}

app.MapGet("/debug/routes", (EndpointDataSource endpointSource) =>
{
    return endpointSource.Endpoints
        .Select(e => e.DisplayName);
});

app.Run();

