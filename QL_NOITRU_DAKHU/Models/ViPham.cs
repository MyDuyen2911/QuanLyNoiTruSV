using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("ViPham")]
public partial class ViPham
{
    [Key]
    public int MaViPham { get; set; }

    [StringLength(255)]
    public string? TenViPham { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MucPhat { get; set; }

    [InverseProperty("MaViPhamNavigation")]
    public virtual ICollection<BienBanViPham> BienBanViPhams { get; set; } = new List<BienBanViPham>();
}
