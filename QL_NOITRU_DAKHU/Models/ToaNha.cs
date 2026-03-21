using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("ToaNha")]
public partial class ToaNha
{
    [Key]
    public int MaToa { get; set; }

    [StringLength(50)]
    public string? TenToa { get; set; }

    public int? MaKhu { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaKhu")]
    [InverseProperty("ToaNhas")]
    public virtual Khu? MaKhuNavigation { get; set; }

    [InverseProperty("MaToaNavigation")]
    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
