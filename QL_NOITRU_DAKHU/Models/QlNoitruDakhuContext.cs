using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

public partial class QlNoitruDakhuContext : DbContext
{
    public QlNoitruDakhuContext()
    {
    }

    public QlNoitruDakhuContext(DbContextOptions<QlNoitruDakhuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BienBanViPham> BienBanViPhams { get; set; }

    public virtual DbSet<DienNuoc> DienNuocs { get; set; }

    public virtual DbSet<DonDangKy> DonDangKies { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<HocLuc> HocLucs { get; set; }

    public virtual DbSet<HopDong> HopDongs { get; set; }

    public virtual DbSet<Khu> Khus { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongTaiSan> PhongTaiSans { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<SuaChua> SuaChuas { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TaiSan> TaiSans { get; set; }

    public virtual DbSet<ToaNha> ToaNhas { get; set; }

    public virtual DbSet<ViPham> ViPhams { get; set; }

    public virtual DbSet<YeuCauChinhSua> YeuCauChinhSuas { get; set; }

    public virtual DbSet<YeuCauChuyenPhong> YeuCauChuyenPhongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-4RM8FNV;Database=QL_NOITRU_DAKHU;User Id=sa;Password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.MaLog).HasName("PK__AuditLog__3B98D24A38FAD224");

            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.AuditLogs).HasConstraintName("FK__AuditLog__MaTaiK__03F0984C");
        });

        modelBuilder.Entity<BienBanViPham>(entity =>
        {
            entity.HasKey(e => e.MaBienBan).HasName("PK__BienBanV__D0123202499B44D1");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.BienBanViPhams).HasConstraintName("FK__BienBanViP__MaSV__6C190EBB");

            entity.HasOne(d => d.MaViPhamNavigation).WithMany(p => p.BienBanViPhams).HasConstraintName("FK__BienBanVi__MaViP__6D0D32F4");
        });

        modelBuilder.Entity<DienNuoc>(entity =>
        {
            entity.HasKey(e => e.MaDienNuoc).HasName("PK__DienNuoc__C9AB716B57965BF0");

            entity.ToTable("DienNuoc", tb => tb.HasTrigger("TRG_CheckChiSoDienNuoc"));

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DienNuocs).HasConstraintName("FK__DienNuoc__MaPhon__66603565");
        });

        modelBuilder.Entity<DonDangKy>(entity =>
        {
            entity.HasKey(e => e.MaDon).HasName("PK__DonDangK__3D89F5681B1C585B");

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaPhongDangKyNavigation).WithMany(p => p.DonDangKies).HasConstraintName("FK__DonDangKy__MaPho__59FA5E80");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.DonDangKies).HasConstraintName("FK__DonDangKy__MaSV__59063A47");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13BA5A4A389");

            entity.Property(e => e.TongTienSauGiam).HasComputedColumnSql("([TongTien]*((1)-isnull([TyLeGiam],(0))))", true);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HoaDons).HasConstraintName("FK__HoaDon__MaPhong__6383C8BA");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HoaDons).HasConstraintName("FK__HoaDon__MaSV__628FA481");
        });

        modelBuilder.Entity<HocLuc>(entity =>
        {
            entity.HasKey(e => e.MaHocLuc).HasName("PK__HocLuc__97954DD50CEE3709");

            entity.ToTable("HocLuc", tb => tb.HasTrigger("TRG_GiamTienHocLuc"));

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HocLucs).HasConstraintName("FK__HocLuc__MaSV__6FE99F9F");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong).HasName("PK__HopDong__36DD434223963B9C");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HopDongs).HasConstraintName("FK__HopDong__MaPhong__5EBF139D");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HopDongs).HasConstraintName("FK__HopDong__MaSV__5DCAEF64");
        });

        modelBuilder.Entity<Khu>(entity =>
        {
            entity.HasKey(e => e.MaKhu).HasName("PK__Khu__3BDA934A7E907936");
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.MaLoaiPhong).HasName("PK__LoaiPhon__230212178378895A");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70ACF6A7A35");

            entity.Property(e => e.TrangThai).HasDefaultValue("Đang làm");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.NhanVien).HasConstraintName("FK__NhanVien__MaTaiK__0D7A0286");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5BED5427D9");

            entity.HasOne(d => d.MaLoaiPhongNavigation).WithMany(p => p.Phongs).HasConstraintName("FK__Phong__MaLoaiPho__4CA06362");

            entity.HasOne(d => d.MaToaNavigation).WithMany(p => p.Phongs).HasConstraintName("FK__Phong__MaToa__4BAC3F29");
        });

        modelBuilder.Entity<PhongTaiSan>(entity =>
        {
            entity.HasKey(e => new { e.MaPhong, e.MaTaiSan }).HasName("PK__PhongTai__D866222036F6B11F");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhongTaiSans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhongTaiS__MaPho__74AE54BC");

            entity.HasOne(d => d.MaTaiSanNavigation).WithMany(p => p.PhongTaiSans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhongTaiS__MaTai__75A278F5");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__2725081A198E3FE8");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.SinhVien).HasConstraintName("FK__SinhVien__MaTaiK__3F466844");
        });

        modelBuilder.Entity<SuaChua>(entity =>
        {
            entity.HasKey(e => e.MaSuaChua).HasName("PK__SuaChua__AEB1B07759F21DCF");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.SuaChuas).HasConstraintName("FK__SuaChua__MaPhong__787EE5A0");

            entity.HasOne(d => d.MaTaiSanNavigation).WithMany(p => p.SuaChuas).HasConstraintName("FK__SuaChua__MaTaiSa__797309D9");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C65290D18E94D");

            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");
        });

        modelBuilder.Entity<TaiSan>(entity =>
        {
            entity.HasKey(e => e.MaTaiSan).HasName("PK__TaiSan__8DB7C7BE3037E641");
        });

        modelBuilder.Entity<ToaNha>(entity =>
        {
            entity.HasKey(e => e.MaToa).HasName("PK__ToaNha__31493444E98B68A2");

            entity.HasOne(d => d.MaKhuNavigation).WithMany(p => p.ToaNhas).HasConstraintName("FK__ToaNha__MaKhu__440B1D61");
        });

        modelBuilder.Entity<ViPham>(entity =>
        {
            entity.HasKey(e => e.MaViPham).HasName("PK__ViPham__F1921D896EADA9A1");
        });

        modelBuilder.Entity<YeuCauChinhSua>(entity =>
        {
            entity.HasKey(e => e.MaYeuCau).HasName("PK__YeuCauCh__CFA5DF4EE301EFA6");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.YeuCauChinhSuas).HasConstraintName("FK__YeuCauChin__MaSV__7C4F7684");
        });

        modelBuilder.Entity<YeuCauChuyenPhong>(entity =>
        {
            entity.HasKey(e => e.MaYeuCau).HasName("PK__YeuCauCh__CFA5DF4E47366E5B");

            entity.HasOne(d => d.MaPhongMuonChuyenNavigation).WithMany(p => p.YeuCauChuyenPhongs).HasConstraintName("FK__YeuCauChu__MaPho__00200768");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.YeuCauChuyenPhongs).HasConstraintName("FK__YeuCauChuy__MaSV__7F2BE32F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
