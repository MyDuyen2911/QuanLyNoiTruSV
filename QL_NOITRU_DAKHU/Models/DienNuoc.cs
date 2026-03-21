using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("DienNuoc")]
[Index("MaPhong", Name = "IX_DienNuoc_MaPhong")]
public partial class DienNuoc
{
    [Key]
    public int MaDienNuoc { get; set; }

    public int? MaPhong { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public int? ChiSoDau { get; set; }

    public int? ChiSoCuoi { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DonGia { get; set; }

    [ForeignKey("MaPhong")]
    [InverseProperty("DienNuocs")]
    public virtual Phong? MaPhongNavigation { get; set; }
}
