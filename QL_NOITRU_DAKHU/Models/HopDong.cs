using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("HopDong")]
[Index("MaPhong", Name = "IX_HopDong_MaPhong")]
public partial class HopDong
{
    [Key]
    public int MaHopDong { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    public int? MaPhong { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaPhong")]
    [InverseProperty("HopDongs")]
    public virtual Phong? MaPhongNavigation { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("HopDongs")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
