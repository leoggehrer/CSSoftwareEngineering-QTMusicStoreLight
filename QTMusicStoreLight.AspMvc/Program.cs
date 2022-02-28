var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add logic controllers
builder.Services.AddTransient<QTMusicStoreLight.Logic.Controllers.ArtistsController>();
builder.Services.AddTransient<QTMusicStoreLight.Logic.Controllers.AlbumsController>();
builder.Services.AddTransient<QTMusicStoreLight.Logic.Controllers.GenresController>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
