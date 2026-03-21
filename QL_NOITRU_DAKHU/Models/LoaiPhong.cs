using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("LoaiPhong")]
public partial class LoaiPhong
{
    [Key]
    public int MaLoaiPhong { get; set; }

    [StringLength(100)]
    public string? TenLoai { get; set; }

    public int? SoNguoiToiDa { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? GiaPhong { get; set; }

    [InverseProperty("MaLoaiPhongNavigation")]
    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
