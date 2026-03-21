using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("BienBanViPham")]
public partial class BienBanViPham
{
    [Key]
    public int MaBienBan { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    public int? MaViPham { get; set; }

    public DateOnly? NgayLap { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("BienBanViPhams")]
    public virtual SinhVien? MaSvNavigation { get; set; }

    [ForeignKey("MaViPham")]
    [InverseProperty("BienBanViPhams")]
    public virtual ViPham? MaViPhamNavigation { get; set; }
}
