namespace QuanLyNoiTruSV.Models
{
    public class PhanCongNhanVien
    {
        public int Id { get; set; }

        public int NhanVienId { get; set; }
        public NhanVien NhanVien { get; set; }

        public int KhuNoiTruId { get; set; }
        public KhuNoiTru KhuNoiTru { get; set; }
    }
}