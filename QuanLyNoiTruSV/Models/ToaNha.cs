using System.Collections.Generic;

namespace QuanLyNoiTruSV.Models
{
    public class ToaNha
    {
        public int Id { get; set; }
        public string TenToa { get; set; }

        public int KhuNoiTruId { get; set; }
        public KhuNoiTru KhuNoiTru { get; set; }

        public ICollection<Phong> Phongs { get; set; }
    }
}