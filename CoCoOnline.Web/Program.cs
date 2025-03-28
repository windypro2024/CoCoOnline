using CoCoOnline.Repository;
using CoCoOnline.Service;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//获取服务集合
IServiceCollection services = builder.Services;

services.AddSession();

services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.MaxDepth = 4;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = null // 禁用命名策略（保持属性名原样）
        };
    });
// 绑定配置
services.Configure<string>(builder.Configuration.GetSection("ConnectionStrings"));

// 注册 HttpClient
services.AddHttpClient();

//添加控制器和视图服务
services.AddControllersWithViews();
//将数据库上下文注册为服务
services.AddDbContext<AppDbContext>();
//添加仓储服务
services.AddScoped<BooksRepository>();
services.AddScoped<BooksService>();
services.AddScoped<UsersRepository>();
services.AddScoped<UsersService>();
services.AddScoped<CategoriesRepository>();
services.AddScoped<CategoriesService>();
services.AddScoped<OrderbooksRepository>();
services.AddScoped<OrderbooksService>();
services.AddScoped<OrdersRepository>();
services.AddScoped<OrdersService>();
services.AddScoped<PublishersRepository>();
services.AddScoped<PublishersService>();
services.AddScoped<ReadercommentsRepository>();
services.AddScoped<ReadercommentsService>();
services.AddScoped<SearchkeywordsRepository>();
services.AddScoped<SearchkeywordsService>();
services.AddScoped<UserrolesRepository>();
services.AddScoped<UserrolesService>();
services.AddScoped<UserstatesRepository>();
services.AddScoped<UserstatesService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
