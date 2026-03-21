using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("HoaDon")]
[Index("MaSv", Name = "IX_HoaDon_MaSV")]
public partial class HoaDon
{
    [Key]
    public int MaHoaDon { get; set; }

    [StringLength(50)]
    public string LoaiHoaDon { get; set; } = null!;

    [Column("MaSV")]
    public int? MaSv { get; set; }

    public int? MaPhong { get; set; }

    public DateOnly? ThangNam { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal TongTien { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TyLeGiam { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? TongTienSauGiam { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [StringLength(20)]
    public string? HocKy { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaPhong")]
    [InverseProperty("HoaDons")]
    public virtual Phong? MaPhongNavigation { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("HoaDons")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
