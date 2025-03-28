using CoCoOnline.Repository;
using CoCoOnline.Service;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//��ȡ���񼯺�
IServiceCollection services = builder.Services;

services.AddSession();

services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.MaxDepth = 4;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = null // �����������ԣ�����������ԭ����
        };
    });
// ������
services.Configure<string>(builder.Configuration.GetSection("ConnectionStrings"));

// ע�� HttpClient
services.AddHttpClient();

//��ӿ���������ͼ����
services.AddControllersWithViews();
//�����ݿ�������ע��Ϊ����
services.AddDbContext<AppDbContext>();
//��Ӳִ�����
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
