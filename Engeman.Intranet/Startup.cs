using Engeman.Intranet.Library;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Engeman.Intranet
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      if (Configuration.GetConnectionString("EngemanDb") != null)
      {
        DatabaseInfo.ConnectionString = Configuration.GetConnectionString("EngemanDb");
      } else
      {
        throw new Exception("Unknown Connection String.");
      }
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();
      services.AddAuthentication("CookieAuthentication").AddCookie("CookieAuthentication", options =>
      {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
      });
      services.AddDistributedMemoryCache();
      services.AddSession(options =>
      {
        options.Cookie.Name = "UserSession";
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
      });
      services.AddTransient<IUserAccountRepository, UserAccountRepository>();
      services.AddTransient<IPostRepository, PostRepository>();
      services.AddTransient<IDepartmentRepository, DepartmentRepository>();
      services.AddTransient<IArchiveRepository, ArchiveRepository>();
      services.AddTransient<IPostCommentRepository, PostCommentRepository>();
      services.AddTransient<IPostCommentFileRepository, PostCommentFileRepository>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      } else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseSession();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");
      });
    }
  }
}
