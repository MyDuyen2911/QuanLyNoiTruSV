CREATE DATABASE QL_NOITRU_DAKHU;
GO

USE QL_NOITRU_DAKHU;
GO

-- ================= TAIKHOAN =================
CREATE TABLE TaiKhoan
(
    MaTaiKhoan INT IDENTITY PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL
        CHECK (VaiTro IN (N'Admin',N'NhanVien',N'SinhVien')),
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động'
);

-- ================= SINHVIEN =================
CREATE TABLE SinhVien
(
    MaSV INT IDENTITY PRIMARY KEY,
    MSSV VARCHAR(20) UNIQUE,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    CCCD VARCHAR(20) UNIQUE,
    DienThoai VARCHAR(15),
    DiaChi NVARCHAR(255),
    MaTaiKhoan INT UNIQUE,

    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- ================= KHU =================
CREATE TABLE Khu
(
    MaKhu INT IDENTITY PRIMARY KEY,
    TenKhu NVARCHAR(100) NOT NULL,
    LoaiKhu NVARCHAR(50),
    MoTa NVARCHAR(255),
    TrangThai NVARCHAR(20)
);

-- ================= TOANHA =================
CREATE TABLE ToaNha
(
    MaToa INT IDENTITY PRIMARY KEY,
    TenToa NVARCHAR(50),
    MaKhu INT,
    TrangThai NVARCHAR(20),

    FOREIGN KEY (MaKhu) REFERENCES Khu(MaKhu)
);

-- ================= LOAIPHONG =================
CREATE TABLE LoaiPhong
(
    MaLoaiPhong INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(100),
    SoNguoiToiDa INT CHECK (SoNguoiToiDa > 0),
    GiaPhong DECIMAL(12,2) CHECK (GiaPhong >= 0)
);

-- ================= PHONG =================
CREATE TABLE Phong
(
    MaPhong INT IDENTITY PRIMARY KEY,
    TenPhong NVARCHAR(50),
    Tang INT,
    MaToa INT,
    MaLoaiPhong INT,
    TrangThai NVARCHAR(20)
        CHECK (TrangThai IN (N'Trống', N'Đầy', N'Bảo trì')),
    GhiChu NVARCHAR(255),

    FOREIGN KEY (MaToa) REFERENCES ToaNha(MaToa),
    FOREIGN KEY (MaLoaiPhong) REFERENCES LoaiPhong(MaLoaiPhong)
);

-- ================= DONDANGKY =================
CREATE TABLE DonDangKy
(
    MaDon INT IDENTITY PRIMARY KEY,
    MaSV INT,
    MaPhongDangKy INT,
    NgayDangKy DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaPhongDangKy) REFERENCES Phong(MaPhong)
);

-- ================= HOPDONG =================
CREATE TABLE HopDong
(
    MaHopDong INT IDENTITY PRIMARY KEY,
    MaSV INT,
    MaPhong INT,
    NgayBatDau DATE,
    NgayKetThuc DATE,
    TrangThai NVARCHAR(50)
        CHECK (TrangThai IN (N'Đang ở', N'Đã rời')),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong)
);

-- ================= HOADON =================
CREATE TABLE HoaDon
(
    MaHoaDon INT IDENTITY PRIMARY KEY,

    LoaiHoaDon NVARCHAR(50) NOT NULL,

    MaSV INT,
    MaPhong INT,

    ThangNam DATE,

    TongTien DECIMAL(12,2) NOT NULL,
    TyLeGiam DECIMAL(5,2) CHECK (TyLeGiam BETWEEN 0 AND 1),

    TongTienSauGiam AS (TongTien * (1 - ISNULL(TyLeGiam,0))) PERSISTED,

    GhiChu NVARCHAR(255),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong)
);

-- ================= DIENNUOC =================
CREATE TABLE DienNuoc
(
    MaDienNuoc INT IDENTITY PRIMARY KEY,
    MaPhong INT,
    Thang INT,
    Nam INT,
    ChiSoDau INT,
    ChiSoCuoi INT,
    DonGia DECIMAL(10,2),

    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong)
);
GO

CREATE TRIGGER TRG_CheckChiSoDienNuoc
ON DienNuoc
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted
        WHERE ChiSoCuoi < ChiSoDau
    )
    BEGIN
        RAISERROR(N'Chỉ số cuối phải lớn hơn hoặc bằng chỉ số đầu', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ================= VIPHAM =================
CREATE TABLE ViPham
(
    MaViPham INT IDENTITY PRIMARY KEY,
    TenViPham NVARCHAR(255),
    MucPhat DECIMAL(10,2)
);

CREATE TABLE BienBanViPham
(
    MaBienBan INT IDENTITY PRIMARY KEY,
    MaSV INT,
    MaViPham INT,
    NgayLap DATE,
    GhiChu NVARCHAR(255),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaViPham) REFERENCES ViPham(MaViPham)
);

-- ================= HOCLUC =================
CREATE TABLE HocLuc
(
    MaHocLuc INT IDENTITY PRIMARY KEY,
    MaSV INT,
    HocKy NVARCHAR(20),
    XepLoai NVARCHAR(20),
    TyLeGiam DECIMAL(5,2),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV)
);

-- ================= TAISAN =================
CREATE TABLE TaiSan
(
    MaTaiSan INT IDENTITY PRIMARY KEY,
    TenTaiSan NVARCHAR(100),
    LoaiTaiSan NVARCHAR(100)
);

CREATE TABLE PhongTaiSan
(
    MaPhong INT,
    MaTaiSan INT,
    SoLuong INT,
    TinhTrang NVARCHAR(100),

    PRIMARY KEY (MaPhong, MaTaiSan),

    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    FOREIGN KEY (MaTaiSan) REFERENCES TaiSan(MaTaiSan)
);

-- ================= SUACHUA =================
CREATE TABLE SuaChua
(
    MaSuaChua INT IDENTITY PRIMARY KEY,
    MaPhong INT,
    MaTaiSan INT,
    NgayBaoHong DATE,
    TrangThai NVARCHAR(50),
    NgayHoanThanh DATE,

    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    FOREIGN KEY (MaTaiSan) REFERENCES TaiSan(MaTaiSan)
);

-- ================= YEUCAU =================
CREATE TABLE YeuCauChinhSua
(
    MaYeuCau INT IDENTITY PRIMARY KEY,
    MaSV INT,
    NoiDung NVARCHAR(255),
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV)
);

CREATE TABLE YeuCauChuyenPhong
(
    MaYeuCau INT IDENTITY PRIMARY KEY,
    MaSV INT,
    MaPhongMuonChuyen INT,
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaPhongMuonChuyen) REFERENCES Phong(MaPhong)
);

-- ================= AUDIT =================
CREATE TABLE AuditLog
(
    MaLog INT IDENTITY PRIMARY KEY,
    MaTaiKhoan INT,
    HanhDong NVARCHAR(255),
    ThoiGian DATETIME DEFAULT GETDATE(),
    MoTa NVARCHAR(255),

    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- ================= TRIGGER =================
CREATE TRIGGER TRG_GiamTienHocLuc
ON HocLuc
AFTER INSERT
AS
BEGIN
    UPDATE hd
    SET TyLeGiam =
        CASE 
            WHEN i.XepLoai = N'Xuất sắc' THEN 0.3
            WHEN i.XepLoai = N'Giỏi' THEN 0.2
            ELSE 0
        END
    FROM HoaDon hd
    JOIN inserted i ON hd.MaSV = i.MaSV
END;

-- ================= PROCEDURE =================
CREATE PROCEDURE sp_PhanPhong
(
    @MaSV VARCHAR(20),
    @MaPhong INT
)
AS
BEGIN
    INSERT INTO HopDong
    (
        MaSV, MaPhong, NgayBatDau, TrangThai
    )
    VALUES
    (
        @MaSV, @MaPhong, GETDATE(), N'Đang ở'
    );
END;

GO

CREATE PROCEDURE sp_TaoHoaDonDienNuoc
(
    @MaPhong INT,
    @Thang INT,
    @Nam INT
)
AS
BEGIN
    DECLARE @TongTien DECIMAL(12,2);
    DECLARE @SoNguoi INT;

    SELECT @TongTien = (ChiSoCuoi - ChiSoDau) * DonGia
    FROM DienNuoc
    WHERE MaPhong = @MaPhong AND Thang = @Thang AND Nam = @Nam;

    SELECT @SoNguoi = COUNT(*)
    FROM HopDong
    WHERE MaPhong = @MaPhong AND TrangThai = N'Đang ở';

    IF @TongTien IS NULL OR @SoNguoi = 0 RETURN;

    INSERT INTO HoaDon (LoaiHoaDon, MaSV, MaPhong, ThangNam, TongTien)
    SELECT
        N'DienNuoc',
        MaSV,
        @MaPhong,
        DATEFROMPARTS(@Nam, @Thang, 1),
        @TongTien / @SoNguoi
    FROM HopDong
    WHERE MaPhong = @MaPhong AND TrangThai = N'Đang ở';
END;
GO

CREATE TABLE NhanVien
(
    MaNV INT IDENTITY PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DienThoai VARCHAR(15),
    Email VARCHAR(100),
    DiaChi NVARCHAR(255),

    ChucVu NVARCHAR(50)
        CHECK (ChucVu IN (N'Admin', N'Nhân viên', N'Quản lý')),

    TrangThai NVARCHAR(20)
        DEFAULT N'Đang làm'
        CHECK (TrangThai IN (N'Đang làm', N'Nghỉ việc')),

    MaTaiKhoan INT UNIQUE,

    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- ================= INDEX =================
CREATE INDEX IX_HopDong_MaPhong ON HopDong(MaPhong);
CREATE INDEX IX_HoaDon_MaSV ON HoaDon(MaSV);
CREATE INDEX IX_DienNuoc_MaPhong ON DienNuoc(MaPhong);

-- Tài khoản
INSERT INTO TaiKhoan(Username, PasswordHash, VaiTro)
VALUES
('admin', '123456', N'Admin'),
('nv01', '123456', N'NhanVien'),
('nv02', '123456', N'NhanVien');

-- Nhân viên
INSERT INTO NhanVien
(
    HoTen, NgaySinh, GioiTinh, DienThoai,
    Email, DiaChi, ChucVu, MaTaiKhoan
)
VALUES
(
    N'Nguyễn Văn Admin',
    '1985-01-01',
    N'Nam',
    '0909000001',
    'admin@ktx.edu.vn',
    N'Cần Thơ',
    N'Admin',
    1
),
(
    N'Trần Thị Nhân Viên',
    '1995-06-10',
    N'Nữ',
    '0909000002',
    'nv01@ktx.edu.vn',
    N'Cần Thơ',
    N'Nhân viên',
    2
),
(
    N'Lê Văn Quản Lý',
    '1992-03-20',
    N'Nam',
    '0909000003',
    'nv02@ktx.edu.vn',
    N'Cần Thơ',
    N'Quản lý',
    3
);
INSERT INTO TaiKhoan(Username, PasswordHash, VaiTro)
VALUES
('sv01', '123', N'SinhVien'),
('sv02', '234', N'SinhVien'),
('sv03', '345', N'SinhVien');

INSERT INTO SinhVien
(MSSV, HoTen, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, MaTaiKhoan)
VALUES
('SV001', N'Nguyễn Văn A', '2003-05-10', N'Nam', '111111111', '0901111111', N'Cần Thơ', 4),
('SV002', N'Trần Thị B', '2003-08-15', N'Nữ', '222222222', '0902222222', N'An Giang', 5),
('SV003', N'Lê Văn C', '2002-12-20', N'Nam', '333333333', '0903333333', N'Vĩnh Long', 6);

INSERT INTO Khu(TenKhu,LoaiKhu,TrangThai)
VALUES
(N'KTX A',N'Thường',N'Hoạt động'),
(N'KTX B',N'Quốc tế',N'Hoạt động');

INSERT INTO ToaNha(TenToa,MaKhu,TrangThai)
VALUES
(N'A1',1,N'Hoạt động'),
(N'A2',1,N'Hoạt động'),
(N'B1',2,N'Hoạt động');

INSERT INTO LoaiPhong(TenLoai,SoNguoiToiDa,GiaPhong)
VALUES
(N'4 người',4,1200000),
(N'6 người',6,900000);

INSERT INTO Phong(TenPhong,Tang,MaToa,MaLoaiPhong,TrangThai)
VALUES
('101',1,1,1,N'Trống'),
('102',1,1,1,N'Trống'),
('201',2,2,2,N'Trống'),
('301',3,3,2,N'Trống');

INSERT INTO HopDong(MaSV, MaPhong, NgayBatDau, NgayKetThuc, TrangThai)
VALUES
('1', 1, '2025-01-01', '2025-12-31', N'Đang ở'),
('2', 1, '2025-01-01', '2025-12-31', N'Đang ở'),
('3', 3, '2025-01-01', '2025-12-31', N'Đang ở');
INSERT INTO DienNuoc(MaPhong, Thang, Nam, ChiSoDau, ChiSoCuoi, DonGia)
VALUES
(1, 3, 2025, 1000, 1200, 3500),
(3, 3, 2025, 500, 650, 3500);

INSERT INTO ViPham(TenViPham, MucPhat)
VALUES
(N'Về trễ', 50000),
(N'Gây ồn ào', 100000);

INSERT INTO BienBanViPham(MaSV, MaViPham, NgayLap, GhiChu)
VALUES
('1', 1, GETDATE(), N'Về trễ sau 23h'),
('2', 2, GETDATE(), N'Mở nhạc lớn');

ALTER TABLE HoaDon
ADD HocKy NVARCHAR(20);

CREATE PROCEDURE sp_ApDungGiamHocLuc
(
    @HocKy NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE hd
    SET TyLeGiam =
        CASE 
            WHEN hl.XepLoai = N'Xuất sắc' THEN 0.3
            WHEN hl.XepLoai = N'Giỏi' THEN 0.2
            ELSE 0
        END
    FROM HoaDon hd
    JOIN HocLuc hl 
        ON hd.MaSV = hl.MaSV
    JOIN HopDong h
        ON h.MaSV = hd.MaSV
    WHERE 
        hl.HocKy = @HocKy
        AND hd.HocKy = @HocKy
        AND h.TrangThai = N'Đang ở';
END

INSERT INTO HocLuc(MaSV, HocKy, XepLoai)
VALUES
('SV001', N'HK1 2025', N'Xuất sắc'),
('SV002', N'HK1 2025', N'Giỏi');

EXEC sp_ApDungGiamHocLuc N'HK1 2025';


ALTER TABLE HoaDon ADD TrangThai NVARCHAR(50) DEFAULT N'Chưa thanh toán';

-- 1. Tự động tìm và tiêu diệt cái luật cũ đang cản đường của cột TyLeGiam
DECLARE @sql NVARCHAR(MAX);
SELECT @sql = 'ALTER TABLE HoaDon DROP CONSTRAINT [' + name + '];'
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('HoaDon') AND name LIKE '%TyLeGiam%';

EXEC sp_executesql @sql;

-- 2. Thêm luật mới chuẩn hơn: Cho phép % Giảm giá từ 0 đến 100
ALTER TABLE HoaDon ADD CONSTRAINT CK_HoaDon_TyLeGiam CHECK (TyLeGiam >= 0 AND TyLeGiam <= 100);