using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("YeuCauChinhSua")]
public partial class YeuCauChinhSua
{
    [Key]
    public int MaYeuCau { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    [StringLength(255)]
    public string? NoiDung { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("YeuCauChinhSuas")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
