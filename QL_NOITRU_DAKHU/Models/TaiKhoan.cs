using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("TaiKhoan")]
[Index("Username", Name = "UQ__TaiKhoan__536C85E45593DE31", IsUnique = true)]
public partial class TaiKhoan
{
    [Key]
    public int MaTaiKhoan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string VaiTro { get; set; } = null!;

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual NhanVien? NhanVien { get; set; }

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual SinhVien? SinhVien { get; set; }
}
