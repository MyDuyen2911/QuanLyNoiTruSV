using System;
using System.Collections.Generic;

namespace QuanLyNoiTruSV.Models
{
    public class HopDong
    {
        public int Id { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }

        public int SinhVienId { get; set; }
        public SinhVien SinhVien { get; set; }

        public int PhongId { get; set; }
        public Phong Phong { get; set; }

        public ICollection<HoaDon> HoaDons { get; set; }
    }
}