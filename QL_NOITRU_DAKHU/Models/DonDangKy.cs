using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("DonDangKy")]
public partial class DonDangKy
{
    [Key]
    public int MaDon { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    public int? MaPhongDangKy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDangKy { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaPhongDangKy")]
    [InverseProperty("DonDangKies")]
    public virtual Phong? MaPhongDangKyNavigation { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("DonDangKies")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
