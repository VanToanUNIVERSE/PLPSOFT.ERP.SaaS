-- MySQL Script converted from SQL Server
CREATE DATABASE IF NOT EXISTS `PLPSOFT_ERP_SAAS_V2026` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `PLPSOFT_ERP_SAAS_V2026`;

-- Table: Companies
CREATE TABLE `Companies` (
    `CompanyID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyCode` VARCHAR(50) NOT NULL,
    `CompanyName` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`CompanyID`),
    UNIQUE KEY `UQ_Companies_CompanyCode` (`CompanyCode`)
) ENGINE=InnoDB;

-- Table: Branches
CREATE TABLE `Branches` (
    `BranchID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyID` BIGINT NOT NULL,
    `BranchCode` VARCHAR(50) NOT NULL,
    `BranchName` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`BranchID`),
    CONSTRAINT `FK_Branches_Companies` FOREIGN KEY (`CompanyID`) REFERENCES `Companies` (`CompanyID`)
) ENGINE=InnoDB;

-- Table: Customers
CREATE TABLE `Customers` (
    `CustomerID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyID` BIGINT NOT NULL,
    `CustomerCode` VARCHAR(50) NOT NULL,
    `CustomerName` VARCHAR(255) NOT NULL,
    `Phone` VARCHAR(20) DEFAULT NULL,
    `Email` VARCHAR(100) DEFAULT NULL,
    PRIMARY KEY (`CustomerID`),
    CONSTRAINT `FK_Customers_Companies` FOREIGN KEY (`CompanyID`) REFERENCES `Companies` (`CompanyID`)
) ENGINE=InnoDB;

-- Table: SalesInvoices
CREATE TABLE `SalesInvoices` (
    `InvoiceID` BIGINT NOT NULL AUTO_INCREMENT,
    `InvoiceCode` VARCHAR(50) NOT NULL,
    `CustomerID` BIGINT NOT NULL,
    `TotalAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`InvoiceID`),
    CONSTRAINT `FK_SalesInvoices_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `Customers` (`CustomerID`)
) ENGINE=InnoDB;

-- Table: SystemTypes
CREATE TABLE `SystemTypes` (
    `TypeID` BIGINT NOT NULL AUTO_INCREMENT,
    `TypeCode` VARCHAR(50) NOT NULL,
    `TypeName` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`TypeID`),
    UNIQUE KEY `UQ_SystemTypes_TypeCode` (`TypeCode`)
) ENGINE=InnoDB;

-- Table: SystemTypeValues
CREATE TABLE `SystemTypeValues` (
    `TypeValueID` BIGINT NOT NULL AUTO_INCREMENT,
    `TypeID` BIGINT NOT NULL,
    `ValueCode` VARCHAR(50) NOT NULL,
    `ValueName` VARCHAR(100) NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (`TypeValueID`),
    UNIQUE KEY `UQ_SystemTypeValues_TypeID_ValueCode` (`TypeID`, `ValueCode`),
    CONSTRAINT `FK_SystemTypeValues_SystemTypes` FOREIGN KEY (`TypeID`) REFERENCES `SystemTypes` (`TypeID`)
) ENGINE=InnoDB;

-- Table: Users
CREATE TABLE `Users` (
    `UserID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyID` BIGINT NOT NULL,
    `UserName` VARCHAR(100) NOT NULL,
    `FullName` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`UserID`),
    UNIQUE KEY `UQ_Users_UserName` (`UserName`),
    CONSTRAINT `FK_Users_Companies` FOREIGN KEY (`CompanyID`) REFERENCES `Companies` (`CompanyID`)
) ENGINE=InnoDB;

-- Schema crm tables (using crm_ prefix)

-- Table: crm_CustomerFeedbacks
CREATE TABLE `crm_CustomerFeedbacks` (
    `FeedbackID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyID` BIGINT NOT NULL,
    `BranchID` BIGINT NOT NULL,
    `CustomerID` BIGINT NOT NULL,
    `InvoiceID` BIGINT DEFAULT NULL,
    `FeedbackTypeID` BIGINT NOT NULL,
    `PriorityID` BIGINT NOT NULL,
    `StatusID` BIGINT NOT NULL,
    `Rating` INT NOT NULL DEFAULT 5,
    `Title` VARCHAR(200) NOT NULL,
    `Content` LONGTEXT NOT NULL,
    `AssignedToUserID` BIGINT DEFAULT NULL,
    `Resolution` LONGTEXT DEFAULT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ResolvedAt` DATETIME DEFAULT NULL,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    `DeletedAt` DATETIME DEFAULT NULL,
    PRIMARY KEY (`FeedbackID`),
    CONSTRAINT `FK_CustomerFeedbacks_AssignedUser` FOREIGN KEY (`AssignedToUserID`) REFERENCES `Users` (`UserID`),
    CONSTRAINT `FK_CustomerFeedbacks_Branches` FOREIGN KEY (`BranchID`) REFERENCES `Branches` (`BranchID`),
    CONSTRAINT `FK_CustomerFeedbacks_Companies` FOREIGN KEY (`CompanyID`) REFERENCES `Companies` (`CompanyID`),
    CONSTRAINT `FK_CustomerFeedbacks_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `Customers` (`CustomerID`),
    CONSTRAINT `FK_CustomerFeedbacks_FeedbackType` FOREIGN KEY (`FeedbackTypeID`) REFERENCES `SystemTypeValues` (`TypeValueID`),
    CONSTRAINT `FK_CustomerFeedbacks_Priority` FOREIGN KEY (`PriorityID`) REFERENCES `SystemTypeValues` (`TypeValueID`),
    CONSTRAINT `FK_CustomerFeedbacks_SalesInvoices` FOREIGN KEY (`InvoiceID`) REFERENCES `SalesInvoices` (`InvoiceID`),
    CONSTRAINT `FK_CustomerFeedbacks_Status` FOREIGN KEY (`StatusID`) REFERENCES `SystemTypeValues` (`TypeValueID`),
    CONSTRAINT `CK_CustomerFeedbacks_Rating` CHECK (`Rating` >= 1 AND `Rating` <= 5)
) ENGINE=InnoDB;

-- Table: crm_CustomerNotes
CREATE TABLE `crm_CustomerNotes` (
    `NoteID` BIGINT NOT NULL AUTO_INCREMENT,
    `CustomerID` BIGINT NOT NULL,
    `Content` LONGTEXT NOT NULL,
    `CreatedByUserID` BIGINT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    `DeletedAt` DATETIME DEFAULT NULL,
    PRIMARY KEY (`NoteID`),
    CONSTRAINT `FK_CustomerNotes_CreatedByUser` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`),
    CONSTRAINT `FK_CustomerNotes_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `Customers` (`CustomerID`)
) ENGINE=InnoDB;

-- Table: crm_CustomerSchedules
CREATE TABLE `crm_CustomerSchedules` (
    `ScheduleID` BIGINT NOT NULL AUTO_INCREMENT,
    `CompanyID` BIGINT NOT NULL,
    `BranchID` BIGINT NOT NULL,
    `CustomerID` BIGINT NOT NULL,
    `ScheduleTypeID` BIGINT NOT NULL,
    `StatusID` BIGINT NOT NULL,
    `Title` VARCHAR(200) NOT NULL,
    `Description` VARCHAR(500) DEFAULT NULL,
    `StartTime` DATETIME NOT NULL,
    `EndTime` DATETIME NOT NULL,
    `CreatedByUserID` BIGINT NOT NULL,
    `AssignedToUserID` BIGINT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsDeleted` BOOLEAN NOT NULL DEFAULT FALSE,
    `DeletedAt` DATETIME DEFAULT NULL,
    PRIMARY KEY (`ScheduleID`),
    CONSTRAINT `FK_CustomerSchedules_AssignedUser` FOREIGN KEY (`AssignedToUserID`) REFERENCES `Users` (`UserID`),
    CONSTRAINT `FK_CustomerSchedules_Branches` FOREIGN KEY (`BranchID`) REFERENCES `Branches` (`BranchID`),
    CONSTRAINT `FK_CustomerSchedules_Companies` FOREIGN KEY (`CompanyID`) REFERENCES `Companies` (`CompanyID`),
    CONSTRAINT `FK_CustomerSchedules_CreatedByUser` FOREIGN KEY (`CreatedByUserID`) REFERENCES `Users` (`UserID`),
    CONSTRAINT `FK_CustomerSchedules_Customers` FOREIGN KEY (`CustomerID`) REFERENCES `Customers` (`CustomerID`),
    CONSTRAINT `FK_CustomerSchedules_ScheduleType` FOREIGN KEY (`ScheduleTypeID`) REFERENCES `SystemTypeValues` (`TypeValueID`),
    CONSTRAINT `FK_CustomerSchedules_Status` FOREIGN KEY (`StatusID`) REFERENCES `SystemTypeValues` (`TypeValueID`),
    CONSTRAINT `CK_CustomerSchedules_Time` CHECK (`EndTime` >= `StartTime`)
) ENGINE=InnoDB;

-- Initial Data

INSERT INTO `Companies` (`CompanyID`, `CompanyCode`, `CompanyName`) VALUES (1, 'PLP', 'Công ty TNHH Công nghệ Phần mềm Phúc Lam Phương');
INSERT INTO `Branches` (`BranchID`, `CompanyID`, `BranchCode`, `BranchName`) VALUES (1, 1, 'CN001', 'Chi nhánh chính');
INSERT INTO `Users` (`UserID`, `CompanyID`, `UserName`, `FullName`) VALUES (1, 1, 'admin', 'Quản trị viên');
INSERT INTO `Users` (`UserID`, `CompanyID`, `UserName`, `FullName`) VALUES (2, 1, 'crm01', 'Nhân viên chăm sóc khách hàng');
INSERT INTO `Customers` (`CustomerID`, `CompanyID`, `CustomerCode`, `CustomerName`, `Phone`, `Email`) VALUES (1, 1, 'KH001', 'Nguyễn Văn A', '0900000001', 'kh001@example.com');
INSERT INTO `SalesInvoices` (`InvoiceID`, `InvoiceCode`, `CustomerID`, `TotalAmount`, `CreatedAt`) VALUES (1, 'HD001', 1, 1500000.00, '2026-05-10 18:18:59');

INSERT INTO `SystemTypes` (`TypeID`, `TypeCode`, `TypeName`) VALUES (1, 'FEEDBACK_TYPE', 'Loại phản hồi'), (2, 'PRIORITY', 'Mức độ ưu tiên'), (3, 'CRM_STATUS', 'Trạng thái phản hồi CRM'), (4, 'SCHEDULE_TYPE', 'Loại lịch chăm sóc'), (5, 'SCHEDULE_STATUS', 'Trạng thái lịch chăm sóc');

INSERT INTO `SystemTypeValues` (`TypeValueID`, `TypeID`, `ValueCode`, `ValueName`, `IsActive`) VALUES 
(1, 1, 'COMPLAINT', 'Khiếu nại', 1), (2, 1, 'INQUIRY', 'Thắc mắc/Hỏi đáp', 1), (3, 1, 'SUGGESTION', 'Góp ý', 1),
(4, 2, 'LOW', 'Thấp', 1), (5, 2, 'MEDIUM', 'Trung bình', 1), (6, 2, 'HIGH', 'Cao', 1), (7, 2, 'URGENT', 'Khẩn cấp', 1),
(8, 3, 'NEW', 'Mới', 1), (9, 3, 'PROCESSING', 'Đang xử lý', 1), (10, 3, 'RESOLVED', 'Đã giải quyết', 1), (11, 3, 'CLOSED', 'Đã đóng', 1),
(12, 4, 'CALL', 'Gọi điện chăm sóc', 1), (13, 4, 'MEETING', 'Hẹn gặp mặt', 1), (14, 4, 'REORDER', 'Nhắc mua lại', 1), (15, 4, 'SERVICE', 'Chăm sóc/Bảo trì', 1),
(16, 5, 'PLANNED', 'Đã lên lịch', 1), (17, 5, 'COMPLETED', 'Hoàn thành', 1), (18, 5, 'CANCELED', 'Đã hủy', 1);

INSERT INTO `crm_CustomerFeedbacks` (`FeedbackID`, `CompanyID`, `BranchID`, `CustomerID`, `InvoiceID`, `FeedbackTypeID`, `PriorityID`, `StatusID`, `Rating`, `Title`, `Content`, `AssignedToUserID`, `Resolution`, `CreatedAt`, `ResolvedAt`, `IsDeleted`, `DeletedAt`) VALUES (1, 1, 1, 1, 1, 1, 6, 8, 3, 'Khách phản hồi sản phẩm lỗi', 'Khách báo sản phẩm nhận được bị lỗi cần kiểm tra và xử lý.', 2, NULL, '2026-05-10 18:18:59', NULL, 0, NULL);
INSERT INTO `crm_CustomerNotes` (`NoteID`, `CustomerID`, `Content`, `CreatedByUserID`, `CreatedAt`, `IsDeleted`, `DeletedAt`) VALUES (1, 1, 'Khách thích được liên hệ vào buổi sáng.', 2, '2026-05-10 18:18:59', 0, NULL);
INSERT INTO `crm_CustomerSchedules` (`ScheduleID`, `CompanyID`, `BranchID`, `CustomerID`, `ScheduleTypeID`, `StatusID`, `Title`, `Description`, `StartTime`, `EndTime`, `CreatedByUserID`, `AssignedToUserID`, `CreatedAt`, `IsDeleted`, `DeletedAt`) VALUES (1, 1, 1, 1, 12, 16, 'Gọi điện chăm sóc khách hàng KH001', 'Gọi điện hỏi thăm sau khi khách mua hàng.', '2026-05-11 18:18:59', '2026-05-11 18:48:59', 1, 2, '2026-05-10 18:18:59', 0, NULL);
