
using API.IRepository.Repository;
using API.IRepository;
using API.Repository;
using Data.IRepository;
using Data.Models;
using Data.Repositories;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<DbContextApp>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });



            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Đăng ký Repository
            builder.Services.AddScoped<IKhachHangVoucherRepo, KhachHangVoucherRepo>();
            builder.Services.AddScoped<IGiayDotGiamGiaRepository, GiayDotGiamGiaRepository>();
            builder.Services.AddScoped<IAnhRepository, AnhRepository>();
            builder.Services.AddScoped<IChatLieuRepository, ChatLieuRepository>();
            builder.Services.AddScoped<IDeGiayRepository, DeGiayRepository>();
            builder.Services.AddScoped<IGiamGiaRepository, GiamGiaRepository>();
            builder.Services.AddScoped<IGiayRepository, GiayRepository>();
            builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
            builder.Services.AddScoped<IDiaChiKhachHangRepository, DiaChiKhachHangRepository>();
            builder.Services.AddScoped<IGiayChiTietRepository, GiayChiTietRepository>();
            builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
            builder.Services.AddScoped<IGioHangChiTietRepository, GioHangChiTietRepository>();
            builder.Services.AddScoped<IHoaDonRepo, HoaDonRepo>();
            builder.Services.AddScoped<IMauSacRepository, MauSacRepository>();
            builder.Services.AddScoped<IKichCoRepository, KichCoRepository>();
            builder.Services.AddScoped<INhanVienRepository, NhanVienRepository>();
            builder.Services.AddScoped<IThuongHieuRepository, ThuongHieuRepository>();
            builder.Services.AddScoped<IVoucherRepo, VoucherRepo>();
            builder.Services.AddScoped<IKieuDangRepository, KieuDangRepository>();
            builder.Services.AddScoped<ITheLoaiGiayRepository, TheLoaiGiayRepository>();
            builder.Services.AddScoped<IChiTietHoaDonRepository, ChiTietHoaDonRepository>();
            builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
            builder.Services.AddScoped<IChucVuRepository, ChucVuRepository>();
            builder.Services.AddScoped<IReturnRepository, ReturnRepository>();
            builder.Services.AddScoped<IGiayYeuThichRepository, GiayYeuThichRepository>();
            builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles(); // cái này phải có

            // Cấu hình để truy cập được /Uploads/images/*
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads")),
                RequestPath = "/Uploads"
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
