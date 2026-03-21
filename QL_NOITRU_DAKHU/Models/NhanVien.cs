using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("NhanVien")]
[Index("MaTaiKhoan", Name = "UQ__NhanVien__AD7C65288EEF3CA8", IsUnique = true)]
public partial class NhanVien
{
    [Key]
    [Column("MaNV")]
    public int MaNv { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    public string? ChucVu { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    public int? MaTaiKhoan { get; set; }

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("NhanVien")]
    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
}
