using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("HocLuc")]
public partial class HocLuc
{
    [Key]
    public int MaHocLuc { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    [StringLength(20)]
    public string? HocKy { get; set; }

    [StringLength(20)]
    public string? XepLoai { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TyLeGiam { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("HocLucs")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
