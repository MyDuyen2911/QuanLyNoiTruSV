using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNoiTruSV.Models
{
    public class KhuNoiTru
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khu không được để trống")]
        [StringLength(100)]
        public string TenKhu { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(500)]
        public string? MoTa { get; set; }

        // Quan hệ 1 Khu - nhiều Tòa nhà
        public ICollection<ToaNha> ToaNhas { get; set; } = new List<ToaNha>();
    }
}