using System;

namespace QuanLyNoiTruSV.Models
{
    public class DonDangKy
    {
        public int Id { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; }

        public int SinhVienId { get; set; }
        public SinhVien SinhVien { get; set; }

        public int PhongId { get; set; }
        public Phong Phong { get; set; }
    }
}