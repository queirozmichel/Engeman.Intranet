using Engeman.Intranet.Library;
using Engeman.Intranet.Repositories;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetConnectionString("EngemanDb") != null)
{
    DatabaseInfo.ConnectionString = builder.Configuration.GetConnectionString("EngemanDb");
}
else
{
    throw new Exception("Unknown Connection String.");
}

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("CookieAuthentication").AddCookie("CookieAuthentication", options =>
      {
          options.Cookie.Name = "UserLoginCookie";
          options.LoginPath = "/Login/Index";
          options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
      });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
      {
          options.Cookie.Name = "UserSession";
          options.IdleTimeout = TimeSpan.FromMinutes(20);
          options.Cookie.HttpOnly = true;
          options.Cookie.IsEssential = true;
      });

builder.Services.AddTransient<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IPostFileRepository, PostFileRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<ICommentFileRepository, CommentFileRepository>();
builder.Services.AddTransient<IPostRestrictionRepository, PostRestrictionRepository>();
builder.Services.AddTransient<ILogRepository, LogRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Index}/{id?}");
app.Run();