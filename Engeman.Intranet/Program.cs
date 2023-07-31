using Engeman.Intranet.Library;
using Engeman.Intranet.Helpers;
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
        options.Cookie.Name = "UserCookie";
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

builder.Services.AddSingleton<ServiceConfiguration>();
builder.Services.AddTransient<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IPostFileRepository, PostFileRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<ICommentFileRepository, CommentFileRepository>();
builder.Services.AddTransient<IPostRestrictionRepository, PostRestrictionRepository>();
builder.Services.AddTransient<ILogRepository, LogRepository>();
builder.Services.AddTransient<IBlacklistTermRepository, BlacklistTermRepository>();
builder.Services.AddTransient<IKeywordRepository, KeywordRepository>();
builder.Services.AddTransient<IPostKeywordRepository, PostKeywordRepository>();

//Determina qual será a condição de pesquisa a ser utilizada
if (bool.Parse(builder.Configuration.GetSection("SEARCH_CONDITION:FREETEXTTABLE").Value) == true && bool.Parse(builder.Configuration.GetSection("SEARCH_CONDITION:CONTAINSTABLE").Value) == false)
{
  Constants.SearchConditionPOST = Constants.FreeTextTablePOST;
  Constants.SearchConditionCOMMENT = Constants.FreeTextTableCOMMENT;
  Constants.SearchConditionPOSTKEYWORD = Constants.FreeTextTablePOSTKEYWORD;
}
else if (bool.Parse(builder.Configuration.GetSection("SEARCH_CONDITION:FREETEXTTABLE").Value) == false && bool.Parse(builder.Configuration.GetSection("SEARCH_CONDITION:CONTAINSTABLE").Value) == true)
{
  Constants.SearchConditionPOST = Constants.ContainsTablePOST;
  Constants.SearchConditionCOMMENT = Constants.ContainsTableCOMMENT;
  Constants.SearchConditionPOSTKEYWORD = Constants.ContainsTablePOSTKEYWORD;
}
else
{
  throw new Exception("FREETEXTTABLE e CONTAINSTABLE não podem estar ambos TRUE ou FALSE");
}
Constants.Rank = builder.Configuration.GetValue<string>("SEARCH_CONDITION:RANK");

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