using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QL_NOITRU_DAKHU.Models;

[Table("Phong")]
public partial class Phong
{
    [Key]
    public int MaPhong { get; set; }

    [StringLength(50)]
    public string? TenPhong { get; set; }

    public int? Tang { get; set; }

    public int? MaToa { get; set; }

    public int? MaLoaiPhong { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<DienNuoc> DienNuocs { get; set; } = new List<DienNuoc>();

    [InverseProperty("MaPhongDangKyNavigation")]
    public virtual ICollection<DonDangKy> DonDangKies { get; set; } = new List<DonDangKy>();

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

    [ForeignKey("MaLoaiPhong")]
    [InverseProperty("Phongs")]
    public virtual LoaiPhong? MaLoaiPhongNavigation { get; set; }

    [ForeignKey("MaToa")]
    [InverseProperty("Phongs")]
    public virtual ToaNha? MaToaNavigation { get; set; }

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<PhongTaiSan> PhongTaiSans { get; set; } = new List<PhongTaiSan>();

    [InverseProperty("MaPhongNavigation")]
    public virtual ICollection<SuaChua> SuaChuas { get; set; } = new List<SuaChua>();

    [InverseProperty("MaPhongMuonChuyenNavigation")]
    public virtual ICollection<YeuCauChuyenPhong> YeuCauChuyenPhongs { get; set; } = new List<YeuCauChuyenPhong>();
}
