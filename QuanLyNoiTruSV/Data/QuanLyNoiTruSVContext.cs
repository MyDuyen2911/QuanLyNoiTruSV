using Microsoft.EntityFrameworkCore;
using QuanLyNoiTruSV.Models;

namespace QuanLyNoiTruSV.Data
{
    public class QuanLyNoiTruSVContext : DbContext
    {
        public QuanLyNoiTruSVContext(DbContextOptions<QuanLyNoiTruSVContext> options)
            : base(options)
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<KhuNoiTru> KhuNoiTrus { get; set; }
        public DbSet<ToaNha> ToaNhas { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DonDangKy> DonDangKys { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhanCongNhanVien> PhanCongNhanViens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình tiền tệ cho Hóa đơn
            modelBuilder.Entity<HoaDon>()
                .Property(h => h.SoTien)
                .HasPrecision(18, 2); // decimal(18,2)
        }
    }
}