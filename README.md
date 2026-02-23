# QuanLyNoiTruSV
ĐỀ TÀI: HỆ THỐNG QUẢN LÝ NỘI TRÚ SINH VIÊN
1. TỔNG QUAN HỆ THỐNG
Hệ thống cho phép:
Quản lý thông tin sinh viên nội trú
Quản lý khu nội trú và phòng ở
Quản lý nhân viên quản lý nội trú
Đăng ký nội trú và xét duyệt đơn đăng ký
Quản lý hợp đồng nội trú
Theo dõi danh sách sinh viên đang ở nội trú
Hỗ trợ tìm kiếm, thêm, sửa, xóa dữ liệu nhanh chóng

2. KIẾN TRÚC HỆ THỐNG
Mô hình hệ thống:
Frontend: ASP.NET Core MVC (Razor View)
Backend: ASP.NET Core (C#)
Database: SQL Server
Kiến trúc 3 lớp:
Presentation Layer (Views)
Business Logic Layer (Controllers)
Data Access Layer (Models + Data + Entity Framework)

3. DATABASE 
Hệ thống sử dụng SQL Server với các bảng chính tương ứng các Model sau:

3.1. Bảng SinhVien
Lưu thông tin sinh viên đăng ký nội trú
MaSinhVien (PK)
HoTen
NgaySinh
GioiTinh
SoDienThoai
Email
Lop
TrangThaiNoiTru
Quan hệ:
1 SinhVien có thể có nhiều DonDangKy
1 SinhVien có thể có nhiều HopDong

3.2. Bảng NhanVien
Lưu thông tin nhân viên quản lý nội trú
MaNhanVien (PK)
HoTen
NgaySinh
GioiTinh
SoDienThoai
Email
ChucVu
Quan hệ:
1 NhanVien được phân công quản lý nhiều KhuNoiTru (qua PhanCongNhanVien)

3.3. Bảng ToaNha
Lưu thông tin các tòa nhà nội trú
MaToaNha (PK)
TenToaNha
DiaChi
Quan hệ:
1 ToaNha có nhiều KhuNoiTru

3.4. Bảng KhuNoiTru
Quản lý khu nội trú thuộc tòa nhà
MaKhu (PK)
TenKhu
MaToaNha (FK)
Quan hệ:
Thuộc 1 ToaNha
Có nhiều Phong
Được quản lý bởi nhiều NhanVien (qua PhanCongNhanVien)

3.5. Bảng Phong
Lưu thông tin phòng ở nội trú
MaPhong (PK)
TenPhong
SucChua
SoLuongHienTai
MaKhu (FK)
Quan hệ:
Thuộc 1 KhuNoiTru
Có nhiều HopDong của SinhVien

3.6. Bảng DonDangKy
Lưu đơn đăng ký nội trú của sinh viên
MaDon (PK)
MaSinhVien (FK)
NgayDangKy
TrangThai (Chờ duyệt / Đã duyệt / Từ chối)
Quan hệ:
Thuộc về 1 SinhVien

3.7. Bảng HopDong
Quản lý hợp đồng nội trú
MaHopDong (PK)
MaSinhVien (FK)
MaPhong (FK)
NgayBatDau
NgayKetThuc
TrangThai
Quan hệ:
1 SinhVien có nhiều HopDong
1 Phong có nhiều HopDong

3.8. Bảng HoaDon
Lưu thông tin hóa đơn thu phí nội trú
MaHoaDon (PK)
MaHopDong (FK)
SoTien
NgayLap
TrangThaiThanhToan
Quan hệ:
Mỗi HoaDon thuộc 1 HopDong

3.9. Bảng NguoiDung
Quản lý tài khoản đăng nhập hệ thống
MaNguoiDung (PK)
TenDangNhap
MatKhau
VaiTro (Admin / NhanVien / SinhVien)
TrangThai
Quan hệ:
Liên kết với SinhVien hoặc NhanVien để xác định người dùng hệ thống

3.10. Bảng PhanCongNhanVien
Phân công nhân viên quản lý khu nội trú
MaPhanCong (PK)
MaNhanVien (FK)
MaKhu (FK)
NgayPhanCong
Quan hệ:
NhanVien – KhuNoiTru là quan hệ nhiều – nhiều thông qua bảng này

4. PHÂN QUYỀN NGƯỜI DÙNG
Hệ thống có 3 vai trò chính: Admin – Nhân viên – Sinh viên

4.1. Admin (Quản trị hệ thống)
Quyền hạn cao nhất, quản lý toàn bộ hệ thống:
Quản lý tài khoản người dùng (NguoiDung)
Quản lý ToaNha, KhuNoiTru, Phong
Quản lý danh sách SinhVien nội trú
Duyệt / từ chối DonDangKy
Lập và quản lý HopDong nội trú
Quản lý HoaDon thu phí
Phân công NhanVien quản lý khu nội trú
Xem báo cáo, thống kê tổng hợp

4.2. Nhân viên (Quản lý nội trú)
Thực hiện nghiệp vụ vận hành hệ thống:
Quản lý thông tin SinhVien
Theo dõi tình trạng phòng và khu nội trú
Xử lý DonDangKy của sinh viên
Lập HopDong nội trú
Tạo và cập nhật HoaDon
Tra cứu danh sách sinh viên theo phòng / khu
Không có quyền quản lý tài khoản Admin hoặc cấu hình hệ thống.

4.3. Sinh viên
Sử dụng hệ thống để đăng ký và theo dõi nội trú:
Đăng ký tài khoản và đăng nhập
Nộp DonDangKy nội trú
Xem trạng thái xét duyệt đơn
Xem thông tin phòng được phân
Xem HopDong nội trú của bản thân
Xem HoaDon và tình trạng thanh toán
Sinh viên chỉ được thao tác trên dữ liệu cá nhân, không chỉnh sửa dữ liệu hệ thống.
