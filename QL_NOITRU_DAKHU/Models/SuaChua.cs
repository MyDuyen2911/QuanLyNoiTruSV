using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("SuaChua")]
public partial class SuaChua
{
    [Key]
    public int MaSuaChua { get; set; }

    public int? MaPhong { get; set; }

    public int? MaTaiSan { get; set; }

    public DateOnly? NgayBaoHong { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    public DateOnly? NgayHoanThanh { get; set; }

    [ForeignKey("MaPhong")]
    [InverseProperty("SuaChuas")]
    public virtual Phong? MaPhongNavigation { get; set; }

    [ForeignKey("MaTaiSan")]
    [InverseProperty("SuaChuas")]
    public virtual TaiSan? MaTaiSanNavigation { get; set; }
}
