using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("SinhVien")]
[Index("Mssv", Name = "UQ__SinhVien__6CB3B7F8DB0A388D", IsUnique = true)]
[Index("Cccd", Name = "UQ__SinhVien__A955A0AA0CB0C4E5", IsUnique = true)]
[Index("MaTaiKhoan", Name = "UQ__SinhVien__AD7C6528421D766B", IsUnique = true)]
public partial class SinhVien
{
    [Key]
    [Column("MaSV")]
    public int MaSv { get; set; }

    [Column("MSSV")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Mssv { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [Column("CCCD")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Cccd { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    public int? MaTaiKhoan { get; set; }

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<BienBanViPham> BienBanViPhams { get; set; } = new List<BienBanViPham>();

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<DonDangKy> DonDangKies { get; set; } = new List<DonDangKy>();

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<HocLuc> HocLucs { get; set; } = new List<HocLuc>();

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("SinhVien")]
    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<YeuCauChinhSua> YeuCauChinhSuas { get; set; } = new List<YeuCauChinhSua>();

    [InverseProperty("MaSvNavigation")]
    public virtual ICollection<YeuCauChuyenPhong> YeuCauChuyenPhongs { get; set; } = new List<YeuCauChuyenPhong>();
}
