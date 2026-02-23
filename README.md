# CHƯƠNG 1: GIỚI THIỆU ĐỀ TÀI

## 1.1. Lý do chọn đề tài
Trong bối cảnh chuyển đổi số trong giáo dục, việc quản lý khu nội trú sinh viên bằng phương pháp thủ công gây nhiều khó khăn như thất lạc dữ liệu, khó tra cứu thông tin phòng ở, hợp đồng, hóa đơn và khiếu nại. Điều này ảnh hưởng đến hiệu quả quản lý cũng như trải nghiệm của sinh viên khi sinh hoạt tại khu nội trú.

Do đó, việc xây dựng “Hệ thống quản lý khu nội trú sinh viên” là cần thiết nhằm tin học hóa toàn bộ quy trình quản lý phòng ở, sinh viên, hợp đồng, hóa đơn và khiếu nại. Hệ thống giúp nhà trường quản lý tập trung, giảm sai sót và nâng cao hiệu quả vận hành.

## 1.2. Tính cấp thiết của đề tài
Số lượng sinh viên nội trú ngày càng tăng, kéo theo khối lượng dữ liệu quản lý lớn. Nếu không có hệ thống phần mềm hỗ trợ, việc quản lý sẽ gặp nhiều khó khăn trong việc phân phòng, theo dõi hợp đồng, thu phí và xử lý phản ánh của sinh viên.

Vì vậy, xây dựng hệ thống quản lý nội trú là giải pháp cần thiết giúp:
- Quản lý tập trung thông tin sinh viên và phòng ở
- Tự động hóa quy trình đăng ký, hợp đồng và hóa đơn
- Hỗ trợ xử lý khiếu nại nhanh chóng
- Nâng cao hiệu quả quản lý và tính minh bạch

## 1.3. Mục tiêu nghiên cứu
Mục tiêu chính của đề tài là xây dựng hệ thống quản lý khu nội trú sinh viên với các chức năng:
- Quản lý thông tin sinh viên, phòng và tòa nhà
- Quản lý đơn đăng ký phòng và hợp đồng nội trú
- Quản lý hóa đơn và tình trạng thanh toán
- Tiếp nhận và xử lý khiếu nại nội trú
- Phân quyền người dùng: Admin, Nhân viên, Sinh viên

## 1.4. Phạm vi nghiên cứu
- Phạm vi nghiệp vụ: quản lý khu nội trú sinh viên trong phạm vi nhà trường
- Đối tượng quản lý: sinh viên, phòng, tòa nhà, hợp đồng, hóa đơn, khiếu nại
- Hệ thống triển khai dưới dạng ứng dụng web quản lý nội trú

## 1.5. Đối tượng sử dụng
Hệ thống hướng đến các đối tượng:
- Admin: quản lý toàn bộ hệ thống
- Nhân viên: quản lý phòng, hợp đồng, hóa đơn, khiếu nại
- Sinh viên: đăng ký phòng, xem thông tin nội trú và gửi khiếu nại

## 1.6. Phương pháp nghiên cứu
- Phân tích yêu cầu nghiệp vụ quản lý nội trú
- Thiết kế hệ thống theo mô hình 3 tầng
- Xây dựng cơ sở dữ liệu và các chức năng quản lý
- Kiểm thử và đánh giá hệ thống

---

# CHƯƠNG 2: CƠ SỞ LÝ THUYẾT

## 2.1. Hệ thống thông tin quản lý
Hệ thống thông tin quản lý là tập hợp các thành phần phần mềm, cơ sở dữ liệu và quy trình nhằm hỗ trợ thu thập, lưu trữ, xử lý và cung cấp thông tin phục vụ công tác quản lý.

Trong đề tài này, hệ thống giúp quản lý toàn bộ hoạt động nội trú sinh viên một cách tập trung và chính xác.

## 2.2. Mô hình kiến trúc 3 lớp
Hệ thống được xây dựng theo mô hình 3 lớp:
- Presentation Layer: giao diện người dùng
- Business Logic Layer: xử lý nghiệp vụ
- Data Access Layer: truy xuất cơ sở dữ liệu

Mô hình này giúp hệ thống dễ bảo trì, mở rộng và nâng cấp.

## 2.3. Phân quyền người dùng
Hệ thống áp dụng cơ chế phân quyền:
- Admin: toàn quyền quản lý hệ thống
- Nhân viên: quản lý phòng, hợp đồng, hóa đơn, khiếu nại
- Sinh viên: đăng ký phòng, xem thông tin và gửi phản ánh

---

# CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG

## 3.1. Mô tả tổng quan hệ thống
Hệ thống quản lý khu nội trú sinh viên cho phép quản lý thông tin tòa nhà, phòng, sinh viên, hợp đồng, hóa đơn và khiếu nại. Hệ thống hoạt động theo mô hình client-server, trong đó người dùng thao tác qua giao diện web và dữ liệu được xử lý tại server.

## 3.2. Yêu cầu chức năng
- Quản lý tòa nhà và phòng
- Quản lý sinh viên nội trú
- Đăng ký và duyệt đơn đăng ký phòng
- Lập và quản lý hợp đồng nội trú
- Quản lý hóa đơn và thanh toán
- Tiếp nhận và xử lý khiếu nại

## 3.3. Yêu cầu phi chức năng
- Giao diện thân thiện, dễ sử dụng
- Bảo mật thông tin người dùng
- Dữ liệu lưu trữ tập trung, dễ tra cứu
- Hệ thống hoạt động ổn định và chính xác

## 3.4. Thiết kế dữ liệu (Database)
Các bảng chính trong hệ thống:
- ToaNha
- Phong
- SinhVien
- NhanVien
- NguoiDung
- DonDangKy
- HopDong
- HoaDon
- KhieuNaiTru
- PhanCongNhanVien

---

# CHƯƠNG 4: XÂY DỰNG VÀ CÀI ĐẶT HỆ THỐNG

## 4.1. Công nghệ sử dụng
- Backend: ASP.NET Core MVC
- Database: SQL Server
- Frontend: HTML, CSS, JavaScript
- Mô hình: 3-tier architecture

## 4.2. Chức năng chính đã xây dựng
- Quản lý sinh viên nội trú
- Quản lý phòng và tòa nhà
- Quản lý hợp đồng và hóa đơn
- Quản lý khiếu nại nội trú
- Phân quyền Admin, Nhân viên, Sinh viên

---

# CHƯƠNG 5: THỬ NGHIỆM VÀ ĐÁNH GIÁ

## 5.1. Mục tiêu thử nghiệm
- Kiểm tra tính đúng đắn của các chức năng
- Đánh giá khả năng phân quyền người dùng
- Kiểm tra hiệu năng và tính ổn định hệ thống

## 5.2. Kết quả đánh giá
Hệ thống hoạt động ổn định, các chức năng quản lý nội trú được thực hiện chính xác. Phân quyền giữa Admin, Nhân viên và Sinh viên đảm bảo đúng vai trò, giúp nâng cao hiệu quả quản lý khu nội trú.

## 5.3. Hướng phát triển
- Tích hợp thanh toán online cho hóa đơn
- Thêm chức năng thông báo cho sinh viên
- Triển khai hệ thống trên môi trường cloud
