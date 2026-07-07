using MainApp;
using MainApp.Data;
using MainApp.Grpc.Protos;
using MainApp.Infrastructure.Conversations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DevPostgresConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddGrpcClient<MessageService.MessageServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Grpc:MainappLlmworker"]!);
});

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
                    CreationDatetime = DateTime.UtcNow,
                    SenderType = i % 2 == 0 ? SenderType.User: SenderType.Bot,
                    ConversationId = Conv1.ConversationId
                }
            );
        }
        db.Messages.AddRange(
                new MainApp.Models.Message
                {
                    Text = "aga",
                    CreationDatetime = DateTime.UtcNow,
                    SenderType = SenderType.User,
                    ConversationId = Conv2.ConversationId
                },
                new MainApp.Models.Message
                {
                    Text = "ogo",
                    CreationDatetime = DateTime.UtcNow,
                    SenderType = SenderType.Bot,
                    ConversationId = Conv2.ConversationId
                }
            );
        db.SaveChanges();

        var FaqConv1 = new MainApp.Models.FAQConversation
        {
            Description = "How to choose courses for the next semester",
            CreationDatetime = DateTime.UtcNow,
            IdentityUser = user,
            IdentityUserId = user.Id
        };
        var FaqConv2 = new MainApp.Models.FAQConversation
        {
            Description = "Where to find advising and registration deadlines",
            CreationDatetime = DateTime.UtcNow,
            IdentityUser = user,
            IdentityUserId = user.Id
        };
        db.FAQConversations.AddRange(FaqConv1, FaqConv2);
        db.SaveChanges();

        db.FAQMessages.AddRange(
            new MainApp.Models.FAQMessage
            {
                Text = "I am not sure which electives fit my study plan.",
                CreationDatetime = DateTime.UtcNow.AddMinutes(-8),
                SenderType = SenderType.User,
                FAQConversationId = FaqConv1.FAQConversationId,
                FAQConversation = FaqConv1
            },
            new MainApp.Models.FAQMessage
            {
                Text = "Start with required courses, then use electives to cover missing credits and prerequisites. If two courses unlock later modules, prioritize those first.",
                CreationDatetime = DateTime.UtcNow.AddMinutes(-7),
                SenderType = SenderType.Bot,
                FAQConversationId = FaqConv1.FAQConversationId,
                FAQConversation = FaqConv1
            },
            new MainApp.Models.FAQMessage
            {
                Text = "Where do I check registration deadlines?",
                CreationDatetime = DateTime.UtcNow.AddMinutes(-6),
                SenderType = SenderType.User,
                FAQConversationId = FaqConv2.FAQConversationId,
                FAQConversation = FaqConv2
            },
            new MainApp.Models.FAQMessage
            {
                Text = "Use the knowledge base and the official academic calendar. For personal exceptions, contact the advising office before the add/drop period closes.",
                CreationDatetime = DateTime.UtcNow.AddMinutes(-5),
                SenderType = SenderType.Bot,
                FAQConversationId = FaqConv2.FAQConversationId,
                FAQConversation = FaqConv2
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
