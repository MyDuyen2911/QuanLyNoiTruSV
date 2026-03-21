using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("Khu")]
public partial class Khu
{
    [Key]
    public int MaKhu { get; set; }

    [StringLength(100)]
    public string TenKhu { get; set; } = null!;

    [StringLength(50)]
    public string? LoaiKhu { get; set; }

    [StringLength(255)]
    public string? MoTa { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaKhuNavigation")]
    public virtual ICollection<ToaNha> ToaNhas { get; set; } = new List<ToaNha>();
}
