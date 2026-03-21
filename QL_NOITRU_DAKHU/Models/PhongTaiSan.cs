using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[PrimaryKey("MaPhong", "MaTaiSan")]
[Table("PhongTaiSan")]
public partial class PhongTaiSan
{
    [Key]
    public int MaPhong { get; set; }

    [Key]
    public int MaTaiSan { get; set; }

    public int? SoLuong { get; set; }

    [StringLength(100)]
    public string? TinhTrang { get; set; }

    [ForeignKey("MaPhong")]
    [InverseProperty("PhongTaiSans")]
    public virtual Phong MaPhongNavigation { get; set; } = null!;

    [ForeignKey("MaTaiSan")]
    [InverseProperty("PhongTaiSans")]
    public virtual TaiSan MaTaiSanNavigation { get; set; } = null!;
}
