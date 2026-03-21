using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("TaiSan")]
public partial class TaiSan
{
    [Key]
    public int MaTaiSan { get; set; }

    [StringLength(100)]
    public string? TenTaiSan { get; set; }

    [StringLength(100)]
    public string? LoaiTaiSan { get; set; }

    [InverseProperty("MaTaiSanNavigation")]
    public virtual ICollection<PhongTaiSan> PhongTaiSans { get; set; } = new List<PhongTaiSan>();

    [InverseProperty("MaTaiSanNavigation")]
    public virtual ICollection<SuaChua> SuaChuas { get; set; } = new List<SuaChua>();
}
