using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("AuditLog")]
public partial class AuditLog
{
    [Key]
    public int MaLog { get; set; }

    public int? MaTaiKhoan { get; set; }

    [StringLength(255)]
    public string? HanhDong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGian { get; set; }

    [StringLength(255)]
    public string? MoTa { get; set; }

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("AuditLogs")]
    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
}
