USE [PLPSOFT_ERP_SAAS_V2026];
GO

/* =========================================================
   MODULE 11 - CRM & CUSTOMER CARE
   SEED DATA SCRIPT
   Mục đích:
   - Bổ sung dữ liệu danh mục dùng chung
   - Bổ sung dữ liệu mẫu để demo Feedback, Schedule, Notes, Timeline
   - Có thể chạy nhiều lần, hạn chế trùng dữ liệu bằng IF NOT EXISTS

   Lưu ý:
   - Chạy file này SAU KHI đã chạy script tạo database/bảng.
   - Không tự đổi tên bảng/cột nếu chưa thống nhất với nhóm trưởng.
========================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* =========================================================
   0. KIỂM TRA BẢNG CẦN THIẾT
========================================================= */

IF OBJECT_ID(N'dbo.Companies', N'U') IS NULL
    THROW 50001, N'Thiếu bảng dbo.Companies. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.Branches', N'U') IS NULL
    THROW 50002, N'Thiếu bảng dbo.Branches. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
    THROW 50003, N'Thiếu bảng dbo.Users. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
    THROW 50004, N'Thiếu bảng dbo.Customers. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.SalesInvoices', N'U') IS NULL
    THROW 50005, N'Thiếu bảng dbo.SalesInvoices. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.SystemTypes', N'U') IS NULL
    THROW 50006, N'Thiếu bảng dbo.SystemTypes. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'dbo.SystemTypeValues', N'U') IS NULL
    THROW 50007, N'Thiếu bảng dbo.SystemTypeValues. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'crm.CustomerFeedbacks', N'U') IS NULL
    THROW 50008, N'Thiếu bảng crm.CustomerFeedbacks. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'crm.CustomerSchedules', N'U') IS NULL
    THROW 50009, N'Thiếu bảng crm.CustomerSchedules. Hãy chạy script tạo database trước.', 1;

IF OBJECT_ID(N'crm.CustomerNotes', N'U') IS NULL
    THROW 50010, N'Thiếu bảng crm.CustomerNotes. Hãy chạy script tạo database trước.', 1;
GO

/* =========================================================
   1. SEED SYSTEM TYPES
========================================================= */

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypes WHERE TypeCode = 'FEEDBACK_TYPE')
    INSERT INTO dbo.SystemTypes(TypeCode, TypeName)
    VALUES ('FEEDBACK_TYPE', N'Loại phản hồi');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypes WHERE TypeCode = 'PRIORITY')
    INSERT INTO dbo.SystemTypes(TypeCode, TypeName)
    VALUES ('PRIORITY', N'Mức độ ưu tiên');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypes WHERE TypeCode = 'CRM_STATUS')
    INSERT INTO dbo.SystemTypes(TypeCode, TypeName)
    VALUES ('CRM_STATUS', N'Trạng thái phản hồi CRM');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypes WHERE TypeCode = 'SCHEDULE_TYPE')
    INSERT INTO dbo.SystemTypes(TypeCode, TypeName)
    VALUES ('SCHEDULE_TYPE', N'Loại lịch chăm sóc');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypes WHERE TypeCode = 'SCHEDULE_STATUS')
    INSERT INTO dbo.SystemTypes(TypeCode, TypeName)
    VALUES ('SCHEDULE_STATUS', N'Trạng thái lịch chăm sóc');
GO

/* =========================================================
   2. SEED SYSTEM TYPE VALUES
========================================================= */

DECLARE @FeedbackTypeID BIGINT = (SELECT TypeID FROM dbo.SystemTypes WHERE TypeCode = 'FEEDBACK_TYPE');
DECLARE @PriorityTypeID BIGINT = (SELECT TypeID FROM dbo.SystemTypes WHERE TypeCode = 'PRIORITY');
DECLARE @CrmStatusTypeID BIGINT = (SELECT TypeID FROM dbo.SystemTypes WHERE TypeCode = 'CRM_STATUS');
DECLARE @ScheduleTypeID BIGINT = (SELECT TypeID FROM dbo.SystemTypes WHERE TypeCode = 'SCHEDULE_TYPE');
DECLARE @ScheduleStatusTypeID BIGINT = (SELECT TypeID FROM dbo.SystemTypes WHERE TypeCode = 'SCHEDULE_STATUS');

-- FEEDBACK_TYPE
IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @FeedbackTypeID AND ValueCode = 'COMPLAINT')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@FeedbackTypeID, 'COMPLAINT', N'Khiếu nại');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @FeedbackTypeID AND ValueCode = 'INQUIRY')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@FeedbackTypeID, 'INQUIRY', N'Thắc mắc/Hỏi đáp');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @FeedbackTypeID AND ValueCode = 'SUGGESTION')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@FeedbackTypeID, 'SUGGESTION', N'Góp ý');

-- PRIORITY
IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @PriorityTypeID AND ValueCode = 'LOW')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@PriorityTypeID, 'LOW', N'Thấp');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @PriorityTypeID AND ValueCode = 'MEDIUM')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@PriorityTypeID, 'MEDIUM', N'Trung bình');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @PriorityTypeID AND ValueCode = 'HIGH')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@PriorityTypeID, 'HIGH', N'Cao');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @PriorityTypeID AND ValueCode = 'URGENT')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@PriorityTypeID, 'URGENT', N'Khẩn cấp');

-- CRM_STATUS
IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @CrmStatusTypeID AND ValueCode = 'NEW')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@CrmStatusTypeID, 'NEW', N'Mới');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @CrmStatusTypeID AND ValueCode = 'PROCESSING')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@CrmStatusTypeID, 'PROCESSING', N'Đang xử lý');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @CrmStatusTypeID AND ValueCode = 'RESOLVED')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@CrmStatusTypeID, 'RESOLVED', N'Đã giải quyết');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @CrmStatusTypeID AND ValueCode = 'CLOSED')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@CrmStatusTypeID, 'CLOSED', N'Đã đóng');

-- SCHEDULE_TYPE
IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleTypeID AND ValueCode = 'CALL')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleTypeID, 'CALL', N'Gọi điện chăm sóc');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleTypeID AND ValueCode = 'MEETING')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleTypeID, 'MEETING', N'Hẹn gặp mặt');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleTypeID AND ValueCode = 'REORDER')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleTypeID, 'REORDER', N'Nhắc mua lại');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleTypeID AND ValueCode = 'SERVICE')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleTypeID, 'SERVICE', N'Chăm sóc/Bảo trì');

-- SCHEDULE_STATUS
IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleStatusTypeID AND ValueCode = 'PLANNED')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleStatusTypeID, 'PLANNED', N'Đã lên lịch');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleStatusTypeID AND ValueCode = 'COMPLETED')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleStatusTypeID, 'COMPLETED', N'Hoàn thành');

IF NOT EXISTS (SELECT 1 FROM dbo.SystemTypeValues WHERE TypeID = @ScheduleStatusTypeID AND ValueCode = 'CANCELED')
    INSERT INTO dbo.SystemTypeValues(TypeID, ValueCode, ValueName) VALUES (@ScheduleStatusTypeID, 'CANCELED', N'Đã hủy');
GO

/* =========================================================
   3. SEED DỮ LIỆU PHỤ THUỘC ĐỂ DEMO
========================================================= */

-- Companies
IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE CompanyCode = 'PLP')
    INSERT INTO dbo.Companies(CompanyCode, CompanyName)
    VALUES ('PLP', N'Công ty TNHH Công nghệ Phần mềm Phúc Lam Phương');

DECLARE @CompanyID BIGINT = (SELECT TOP 1 CompanyID FROM dbo.Companies WHERE CompanyCode = 'PLP');

-- Branches
IF NOT EXISTS (SELECT 1 FROM dbo.Branches WHERE BranchCode = 'CN001')
    INSERT INTO dbo.Branches(CompanyID, BranchCode, BranchName)
    VALUES (@CompanyID, 'CN001', N'Chi nhánh chính');

IF NOT EXISTS (SELECT 1 FROM dbo.Branches WHERE BranchCode = 'CN002')
    INSERT INTO dbo.Branches(CompanyID, BranchCode, BranchName)
    VALUES (@CompanyID, 'CN002', N'Chi nhánh Cần Thơ');

DECLARE @BranchMainID BIGINT = (SELECT TOP 1 BranchID FROM dbo.Branches WHERE BranchCode = 'CN001');
DECLARE @BranchCanThoID BIGINT = (SELECT TOP 1 BranchID FROM dbo.Branches WHERE BranchCode = 'CN002');

-- Users
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = 'admin')
    INSERT INTO dbo.Users(CompanyID, UserName, FullName)
    VALUES (@CompanyID, 'admin', N'Quản trị viên');

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = 'crm01')
    INSERT INTO dbo.Users(CompanyID, UserName, FullName)
    VALUES (@CompanyID, 'crm01', N'Nhân viên chăm sóc khách hàng');

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = 'crm02')
    INSERT INTO dbo.Users(CompanyID, UserName, FullName)
    VALUES (@CompanyID, 'crm02', N'Nhân viên xử lý khiếu nại');

DECLARE @AdminUserID BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'admin');
DECLARE @CrmUser01ID BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm01');
DECLARE @CrmUser02ID BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm02');

-- Customers
IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerCode = 'KH001')
    INSERT INTO dbo.Customers(CompanyID, CustomerCode, CustomerName, Phone, Email)
    VALUES (@CompanyID, 'KH001', N'Nguyễn Văn A', '0900000001', 'kh001@example.com');

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerCode = 'KH002')
    INSERT INTO dbo.Customers(CompanyID, CustomerCode, CustomerName, Phone, Email)
    VALUES (@CompanyID, 'KH002', N'Trần Thị B', '0900000002', 'kh002@example.com');

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerCode = 'KH003')
    INSERT INTO dbo.Customers(CompanyID, CustomerCode, CustomerName, Phone, Email)
    VALUES (@CompanyID, 'KH003', N'Công ty Minh Long', '0900000003', 'contact@minhlong.example.com');

DECLARE @Customer01ID BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH001');
DECLARE @Customer02ID BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH002');
DECLARE @Customer03ID BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH003');

-- SalesInvoices
IF NOT EXISTS (SELECT 1 FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD001')
    INSERT INTO dbo.SalesInvoices(InvoiceCode, CustomerID, TotalAmount, CreatedAt)
    VALUES ('HD001', @Customer01ID, 1500000, DATEADD(DAY, -10, SYSDATETIME()));

IF NOT EXISTS (SELECT 1 FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD002')
    INSERT INTO dbo.SalesInvoices(InvoiceCode, CustomerID, TotalAmount, CreatedAt)
    VALUES ('HD002', @Customer02ID, 2200000, DATEADD(DAY, -5, SYSDATETIME()));

IF NOT EXISTS (SELECT 1 FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD003')
    INSERT INTO dbo.SalesInvoices(InvoiceCode, CustomerID, TotalAmount, CreatedAt)
    VALUES ('HD003', @Customer03ID, 8500000, DATEADD(DAY, -2, SYSDATETIME()));

DECLARE @Invoice01ID BIGINT = (SELECT TOP 1 InvoiceID FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD001');
DECLARE @Invoice02ID BIGINT = (SELECT TOP 1 InvoiceID FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD002');
DECLARE @Invoice03ID BIGINT = (SELECT TOP 1 InvoiceID FROM dbo.SalesInvoices WHERE InvoiceCode = 'HD003');

/* =========================================================
   4. LẤY ID DANH MỤC THEO VALUE CODE
========================================================= */

DECLARE @FeedbackComplaintID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'FEEDBACK_TYPE' AND v.ValueCode = 'COMPLAINT'
);

DECLARE @FeedbackInquiryID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'FEEDBACK_TYPE' AND v.ValueCode = 'INQUIRY'
);

DECLARE @FeedbackSuggestionID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'FEEDBACK_TYPE' AND v.ValueCode = 'SUGGESTION'
);

DECLARE @PriorityLowID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'PRIORITY' AND v.ValueCode = 'LOW'
);

DECLARE @PriorityMediumID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'PRIORITY' AND v.ValueCode = 'MEDIUM'
);

DECLARE @PriorityHighID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'PRIORITY' AND v.ValueCode = 'HIGH'
);

DECLARE @PriorityUrgentID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'PRIORITY' AND v.ValueCode = 'URGENT'
);

DECLARE @StatusNewID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'CRM_STATUS' AND v.ValueCode = 'NEW'
);

DECLARE @StatusProcessingID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'CRM_STATUS' AND v.ValueCode = 'PROCESSING'
);

DECLARE @StatusResolvedID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'CRM_STATUS' AND v.ValueCode = 'RESOLVED'
);

DECLARE @StatusClosedID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'CRM_STATUS' AND v.ValueCode = 'CLOSED'
);

DECLARE @ScheduleCallID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'CALL'
);

DECLARE @ScheduleMeetingID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'MEETING'
);

DECLARE @ScheduleReorderID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'REORDER'
);

DECLARE @ScheduleServiceID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'SERVICE'
);

DECLARE @SchedulePlannedID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_STATUS' AND v.ValueCode = 'PLANNED'
);

DECLARE @ScheduleCompletedID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_STATUS' AND v.ValueCode = 'COMPLETED'
);

DECLARE @ScheduleCanceledID BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_STATUS' AND v.ValueCode = 'CANCELED'
);

/* =========================================================
   5. SEED FEEDBACK DEMO
========================================================= */

IF NOT EXISTS (SELECT 1 FROM crm.CustomerFeedbacks WHERE Title = N'Khách phản hồi sản phẩm lỗi')
BEGIN
    INSERT INTO crm.CustomerFeedbacks (
        CompanyID, BranchID, CustomerID, InvoiceID,
        FeedbackTypeID, PriorityID, StatusID,
        Rating, Title, Content, AssignedToUserID, Resolution,
        CreatedAt, ResolvedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID, @BranchMainID, @Customer01ID, @Invoice01ID,
        @FeedbackComplaintID, @PriorityHighID, @StatusNewID,
        3, N'Khách phản hồi sản phẩm lỗi',
        N'Khách báo sản phẩm nhận được bị lỗi cần kiểm tra và xử lý.',
        @CrmUser01ID, NULL,
        DATEADD(DAY, -3, SYSDATETIME()), NULL, 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerFeedbacks WHERE Title = N'Khách hỏi chính sách bảo hành')
BEGIN
    INSERT INTO crm.CustomerFeedbacks (
        CompanyID, BranchID, CustomerID, InvoiceID,
        FeedbackTypeID, PriorityID, StatusID,
        Rating, Title, Content, AssignedToUserID, Resolution,
        CreatedAt, ResolvedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID, @BranchMainID, @Customer02ID, @Invoice02ID,
        @FeedbackInquiryID, @PriorityMediumID, @StatusProcessingID,
        4, N'Khách hỏi chính sách bảo hành',
        N'Khách cần tư vấn thêm về thời gian bảo hành và điều kiện đổi trả.',
        @CrmUser02ID, NULL,
        DATEADD(DAY, -2, SYSDATETIME()), NULL, 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerFeedbacks WHERE Title = N'Khách góp ý cải thiện dịch vụ giao hàng')
BEGIN
    INSERT INTO crm.CustomerFeedbacks (
        CompanyID, BranchID, CustomerID, InvoiceID,
        FeedbackTypeID, PriorityID, StatusID,
        Rating, Title, Content, AssignedToUserID, Resolution,
        CreatedAt, ResolvedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID, @BranchCanThoID, @Customer03ID, @Invoice03ID,
        @FeedbackSuggestionID, @PriorityLowID, @StatusResolvedID,
        5, N'Khách góp ý cải thiện dịch vụ giao hàng',
        N'Khách góp ý nên cập nhật trạng thái giao hàng rõ hơn cho khách doanh nghiệp.',
        @CrmUser01ID, N'Đã ghi nhận góp ý và chuyển bộ phận liên quan cải thiện quy trình thông báo.',
        DATEADD(DAY, -4, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME()), 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerFeedbacks WHERE Title = N'Khiếu nại giao hàng trễ')
BEGIN
    INSERT INTO crm.CustomerFeedbacks (
        CompanyID, BranchID, CustomerID, InvoiceID,
        FeedbackTypeID, PriorityID, StatusID,
        Rating, Title, Content, AssignedToUserID, Resolution,
        CreatedAt, ResolvedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID, @BranchCanThoID, @Customer02ID, @Invoice02ID,
        @FeedbackComplaintID, @PriorityUrgentID, @StatusProcessingID,
        2, N'Khiếu nại giao hàng trễ',
        N'Khách phản ánh đơn hàng giao trễ so với lịch hẹn ban đầu, cần liên hệ xử lý gấp.',
        @CrmUser02ID, NULL,
        DATEADD(HOUR, -8, SYSDATETIME()), NULL, 0, NULL
    );
END
GO

/* =========================================================
   6. SEED SCHEDULE DEMO
========================================================= */

DECLARE @CompanyID2 BIGINT = (SELECT TOP 1 CompanyID FROM dbo.Companies WHERE CompanyCode = 'PLP');
DECLARE @BranchMainID2 BIGINT = (SELECT TOP 1 BranchID FROM dbo.Branches WHERE BranchCode = 'CN001');
DECLARE @BranchCanThoID2 BIGINT = (SELECT TOP 1 BranchID FROM dbo.Branches WHERE BranchCode = 'CN002');
DECLARE @AdminUserID2 BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'admin');
DECLARE @CrmUser01ID2 BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm01');
DECLARE @CrmUser02ID2 BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm02');
DECLARE @Customer01ID2 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH001');
DECLARE @Customer02ID2 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH002');
DECLARE @Customer03ID2 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH003');

DECLARE @ScheduleCallID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'CALL'
);

DECLARE @ScheduleMeetingID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'MEETING'
);

DECLARE @ScheduleReorderID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'REORDER'
);

DECLARE @ScheduleServiceID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_TYPE' AND v.ValueCode = 'SERVICE'
);

DECLARE @SchedulePlannedID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_STATUS' AND v.ValueCode = 'PLANNED'
);

DECLARE @ScheduleCompletedID2 BIGINT = (
    SELECT TOP 1 v.TypeValueID
    FROM dbo.SystemTypeValues v
    JOIN dbo.SystemTypes t ON v.TypeID = t.TypeID
    WHERE t.TypeCode = 'SCHEDULE_STATUS' AND v.ValueCode = 'COMPLETED'
);

IF NOT EXISTS (SELECT 1 FROM crm.CustomerSchedules WHERE Title = N'Gọi điện chăm sóc khách hàng KH001')
BEGIN
    INSERT INTO crm.CustomerSchedules (
        CompanyID, BranchID, CustomerID,
        ScheduleTypeID, StatusID,
        Title, Description,
        StartTime, EndTime,
        CreatedByUserID, AssignedToUserID,
        CreatedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID2, @BranchMainID2, @Customer01ID2,
        @ScheduleCallID2, @SchedulePlannedID2,
        N'Gọi điện chăm sóc khách hàng KH001',
        N'Gọi điện hỏi thăm sau khi khách mua hàng.',
        DATEADD(DAY, 1, SYSDATETIME()),
        DATEADD(MINUTE, 30, DATEADD(DAY, 1, SYSDATETIME())),
        @AdminUserID2, @CrmUser01ID2,
        SYSDATETIME(), 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerSchedules WHERE Title = N'Lịch hẹn tư vấn khách doanh nghiệp')
BEGIN
    INSERT INTO crm.CustomerSchedules (
        CompanyID, BranchID, CustomerID,
        ScheduleTypeID, StatusID,
        Title, Description,
        StartTime, EndTime,
        CreatedByUserID, AssignedToUserID,
        CreatedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID2, @BranchCanThoID2, @Customer03ID2,
        @ScheduleMeetingID2, @SchedulePlannedID2,
        N'Lịch hẹn tư vấn khách doanh nghiệp',
        N'Hẹn gặp khách doanh nghiệp để tư vấn gói dịch vụ và chính sách chăm sóc.',
        DATEADD(DAY, 2, SYSDATETIME()),
        DATEADD(HOUR, 1, DATEADD(DAY, 2, SYSDATETIME())),
        @AdminUserID2, @CrmUser02ID2,
        SYSDATETIME(), 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerSchedules WHERE Title = N'Lịch quá hạn chưa xử lý')
BEGIN
    INSERT INTO crm.CustomerSchedules (
        CompanyID, BranchID, CustomerID,
        ScheduleTypeID, StatusID,
        Title, Description,
        StartTime, EndTime,
        CreatedByUserID, AssignedToUserID,
        CreatedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID2, @BranchMainID2, @Customer02ID2,
        @ScheduleServiceID2, @SchedulePlannedID2,
        N'Lịch quá hạn chưa xử lý',
        N'Dữ liệu mẫu để test chức năng lọc lịch quá hạn.',
        DATEADD(DAY, -1, SYSDATETIME()),
        DATEADD(MINUTE, 30, DATEADD(DAY, -1, SYSDATETIME())),
        @AdminUserID2, @CrmUser01ID2,
        DATEADD(DAY, -2, SYSDATETIME()), 0, NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM crm.CustomerSchedules WHERE Title = N'Nhắc khách mua lại sản phẩm')
BEGIN
    INSERT INTO crm.CustomerSchedules (
        CompanyID, BranchID, CustomerID,
        ScheduleTypeID, StatusID,
        Title, Description,
        StartTime, EndTime,
        CreatedByUserID, AssignedToUserID,
        CreatedAt, IsDeleted, DeletedAt
    )
    VALUES (
        @CompanyID2, @BranchMainID2, @Customer01ID2,
        @ScheduleReorderID2, @ScheduleCompletedID2,
        N'Nhắc khách mua lại sản phẩm',
        N'Đã gọi khách để nhắc mua lại sản phẩm định kỳ.',
        DATEADD(DAY, -5, SYSDATETIME()),
        DATEADD(MINUTE, 20, DATEADD(DAY, -5, SYSDATETIME())),
        @AdminUserID2, @CrmUser01ID2,
        DATEADD(DAY, -6, SYSDATETIME()), 0, NULL
    );
END
GO

/* =========================================================
   7. SEED CUSTOMER NOTES DEMO
========================================================= */

DECLARE @Customer01ID3 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH001');
DECLARE @Customer02ID3 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH002');
DECLARE @Customer03ID3 BIGINT = (SELECT TOP 1 CustomerID FROM dbo.Customers WHERE CustomerCode = 'KH003');
DECLARE @CrmUser01ID3 BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm01');
DECLARE @CrmUser02ID3 BIGINT = (SELECT TOP 1 UserID FROM dbo.Users WHERE UserName = 'crm02');

IF NOT EXISTS (SELECT 1 FROM crm.CustomerNotes WHERE CustomerID = @Customer01ID3 AND Content = N'Khách thích được liên hệ vào buổi sáng.')
    INSERT INTO crm.CustomerNotes(CustomerID, Content, CreatedByUserID, CreatedAt, IsDeleted, DeletedAt)
    VALUES (@Customer01ID3, N'Khách thích được liên hệ vào buổi sáng.', @CrmUser01ID3, DATEADD(DAY, -7, SYSDATETIME()), 0, NULL);

IF NOT EXISTS (SELECT 1 FROM crm.CustomerNotes WHERE CustomerID = @Customer01ID3 AND Content = N'Khách ưu tiên nhận thông tin qua điện thoại.')
    INSERT INTO crm.CustomerNotes(CustomerID, Content, CreatedByUserID, CreatedAt, IsDeleted, DeletedAt)
    VALUES (@Customer01ID3, N'Khách ưu tiên nhận thông tin qua điện thoại.', @CrmUser01ID3, DATEADD(DAY, -2, SYSDATETIME()), 0, NULL);

IF NOT EXISTS (SELECT 1 FROM crm.CustomerNotes WHERE CustomerID = @Customer02ID3 AND Content = N'Khách yêu cầu phản hồi nhanh khi có vấn đề về giao hàng.')
    INSERT INTO crm.CustomerNotes(CustomerID, Content, CreatedByUserID, CreatedAt, IsDeleted, DeletedAt)
    VALUES (@Customer02ID3, N'Khách yêu cầu phản hồi nhanh khi có vấn đề về giao hàng.', @CrmUser02ID3, DATEADD(DAY, -1, SYSDATETIME()), 0, NULL);

IF NOT EXISTS (SELECT 1 FROM crm.CustomerNotes WHERE CustomerID = @Customer03ID3 AND Content = N'Khách doanh nghiệp cần lịch hẹn trước khi đến gặp.')
    INSERT INTO crm.CustomerNotes(CustomerID, Content, CreatedByUserID, CreatedAt, IsDeleted, DeletedAt)
    VALUES (@Customer03ID3, N'Khách doanh nghiệp cần lịch hẹn trước khi đến gặp.', @CrmUser02ID3, DATEADD(DAY, -3, SYSDATETIME()), 0, NULL);
GO

/* =========================================================
   8. QUERY KIỂM TRA SAU KHI SEED
========================================================= */

PRINT N'Đã chạy seed data Module 11 CRM thành công. Kiểm tra dữ liệu bên dưới:';

SELECT 
    t.TypeCode,
    t.TypeName,
    v.ValueCode,
    v.ValueName
FROM dbo.SystemTypes t
JOIN dbo.SystemTypeValues v ON t.TypeID = v.TypeID
WHERE t.TypeCode IN ('FEEDBACK_TYPE', 'PRIORITY', 'CRM_STATUS', 'SCHEDULE_TYPE', 'SCHEDULE_STATUS')
ORDER BY t.TypeCode, v.ValueCode;

SELECT TOP 50 * FROM crm.CustomerFeedbacks ORDER BY CreatedAt DESC;
SELECT TOP 50 * FROM crm.CustomerSchedules ORDER BY StartTime DESC;
SELECT TOP 50 * FROM crm.CustomerNotes ORDER BY CreatedAt DESC;
GO
