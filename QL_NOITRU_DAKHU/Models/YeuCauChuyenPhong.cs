using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("YeuCauChuyenPhong")]
public partial class YeuCauChuyenPhong
{
    [Key]
    public int MaYeuCau { get; set; }

    [Column("MaSV")]
    public int? MaSv { get; set; }

    public int? MaPhongMuonChuyen { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaPhongMuonChuyen")]
    [InverseProperty("YeuCauChuyenPhongs")]
    public virtual Phong? MaPhongMuonChuyenNavigation { get; set; }

    [ForeignKey("MaSv")]
    [InverseProperty("YeuCauChuyenPhongs")]
    public virtual SinhVien? MaSvNavigation { get; set; }
}
