# QuanLyNoiTruSV
ĐỀ TÀI: HỆ THỐNG QUẢN LÝ NỘI TRÚ SINH VIÊN
1. TỔNG QUAN HỆ THỐNG

Hệ thống cho phép:

- Quản lý thông tin sinh viên nội trú

- Quản lý khu nội trú và phòng ở

- Quản lý nhân viên quản lý nội trú

- Đăng ký nội trú và xét duyệt đơn đăng ký

- Quản lý hợp đồng nội trú

- Theo dõi danh sách sinh viên đang ở nội trú

- Hỗ trợ tìm kiếm, thêm, sửa, xóa dữ liệu nhanh chóng


2. KIẾN TRÚC HỆ THỐNG

Mô hình hệ thống:

- Frontend: ASP.NET Core MVC (Razor View)

- Backend: ASP.NET Core (C#)

- Database: SQL Server

Kiến trúc 3 lớp:

- Presentation Layer (Views)

- Business Logic Layer (Controllers)

- Data Access Layer (Models + Data + Entity Framework)


3. DATABASE

Hệ thống sử dụng SQL Server với các bảng chính tương ứng các Model sau:


3.1. Bảng SinhVien:

- MaSinhVien (PK)

- HoTen

- NgaySinh

- GioiTinh

- SoDienThoai

- Email

- Lop

- TrangThaiNoiTru


3.2. Bảng NhanVien

- MaNhanVien (PK)

- HoTen

- NgaySinh

- GioiTinh

- SoDienThoai

- Email

- ChucVu



3.3. Bảng ToaNha

- MaToaNha (PK)

- TenToaNha

- DiaChi



3.4. Bảng KhuNoiTru


- MaKhu (PK)

- TenKhu

- MaToaNha (FK)


3.5. Bảng Phong

- MaPhong (PK)

- TenPhong

- SucChua

- SoLuongHienTai

- MaKhu (FK)


3.6. Bảng DonDangKy


- MaDon (PK)

- MaSinhVien (FK)

- NgayDangKy

- TrangThai (Chờ duyệt / Đã duyệt / Từ chối)


3.7. Bảng HopDong

- MaHopDong (PK)

- MaSinhVien (FK)

- MaPhong (FK)

- NgayBatDau

- NgayKetThuc

- TrangThai


3.8. Bảng HoaDon

- Lưu thông tin hóa đơn thu phí nội trú

- MaHoaDon (PK)

- MaHopDong (FK)

- SoTien

- NgayLap

- TrangThaiThanhToan


3.9. Bảng NguoiDung

- MaNguoiDung (PK)

- TenDangNhap

- MatKhau

- VaiTro (Admin / NhanVien / SinhVien)

- TrangThai


3.10. Bảng PhanCongNhanVien

- MaPhanCong (PK)

- MaNhanVien (FK)

- MaKhu (FK)

- NgayPhanCong


4. PHÂN QUYỀN NGƯỜI DÙNG

Hệ thống có 3 vai trò chính: Admin – Nhân viên – Sinh viên


4.1. Admin (Quản trị hệ thống)

- Quyền hạn cao nhất, quản lý toàn bộ hệ thống:

- Quản lý tài khoản người dùng (NguoiDung)

- Quản lý ToaNha, KhuNoiTru, Phong

- Quản lý danh sách SinhVien nội trú

- Duyệt / từ chối DonDangKy

- Lập và quản lý HopDong nội trú

- Quản lý HoaDon thu phí

- Phân công NhanVien quản lý khu nội trú

- Xem báo cáo, thống kê tổng hợp


4.2. Nhân viên (Quản lý nội trú)

- Thực hiện nghiệp vụ vận hành hệ thống:

- Quản lý thông tin SinhVien

- Theo dõi tình trạng phòng và khu nội trú

- Xử lý DonDangKy của sinh viên

- Lập HopDong nội trú

- Tạo và cập nhật HoaDon

- Tra cứu danh sách sinh viên theo phòng / khu

Không có quyền quản lý tài khoản Admin hoặc cấu hình hệ thống.


4.3. Sinh viên

- Sử dụng hệ thống để đăng ký và theo dõi nội trú:

- Đăng ký tài khoản và đăng nhập

- Nộp DonDangKy nội trú

- Xem trạng thái xét duyệt đơn

- Xem thông tin phòng được phân

- Xem HopDong nội trú của bản thân

- Xem HoaDon và tình trạng thanh toán

Sinh viên chỉ được thao tác trên dữ liệu cá nhân, không chỉnh sửa dữ liệu hệ thống.

