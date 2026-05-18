# DANH SÁCH TEST CASE (KIỂM THỬ TÍCH HỢP)

Dưới đây là danh sách các kịch bản kiểm thử tích hợp (từ DB -> Backend -> Frontend) cho chức năng **Hồ sơ 360 độ khách hàng (Timeline) & Ghi chú (Notes)**.

| Mã Test Case | Tên Chức Năng | Luồng Kiểm Thử (Steps) | Kết Quả Mong Đợi (Expected Result) | Trạng Thái |
|---|---|---|---|---|
| **TC_CRM_TL_01** | Thêm Ghi Chú | 1. Chọn khách hàng<br>2. Nhập nội dung ghi chú "Khách thích gọi sáng"<br>3. Bấm "Lưu" | - Thông báo lưu thành công.<br>- Database crm.CustomerNotes có record mới.<br>- Dữ liệu cập nhật lên giao diện Timeline. | [ ] Chưa test |
| **TC_CRM_TL_02** | Thêm Ghi Chú (Lỗi) | 1. Bỏ trống ô ghi chú<br>2. Bấm "Lưu" | - Hệ thống chặn và báo "Vui lòng nhập nội dung".<br>- Không có lệnh gọi API về DB. | [ ] Chưa test |
| **TC_CRM_TL_03** | Timeline - [Hóa đơn] | Cấp dữ liệu 1 hóa đơn vào bảng SalesInvoices cho khách hàng A, sau đó xem Timeline. | Timeline hiển thị badge `[Hóa đơn] Khách hàng mua hàng` với nội dung và thời gian. | [ ] Chưa test |
| **TC_CRM_TL_04** | Timeline - [Phản hồi] | Cấp dữ liệu 1 khiếu nại vào bảng CustomerFeedbacks cho khách hàng A, sau đó xem Timeline. | Timeline hiển thị badge `[Phản hồi] Khách khiếu nại sản phẩm lỗi` với nội dung. | [ ] Chưa test |
| **TC_CRM_TL_05** | Timeline - [Lịch hẹn] | Cấp dữ liệu 1 lịch hẹn vào bảng CustomerSchedules, sau đó xem Timeline. | Timeline hiển thị badge `[Lịch hẹn] Nhân viên gọi chăm sóc` kèm giờ. | [ ] Chưa test |
| **TC_CRM_TL_06** | Timeline - Sắp Xếp | Cấp cả 4 loại dữ liệu cho 1 khách hàng vào DB, load lại trang. | Dữ liệu được gộp chung 1 danh sách, sắp xếp **mới nhất nằm trên cùng**. | [ ] Chưa test |


---

# BÁO CÁO LỖI (BUG REPORT FORMAT)

*Điền vào form này nếu trong quá trình thực hiện TC gặp kết quả không như mong đợi.*

**Bug ID:** [Mã sinh tự động hoặc điền tay, ví dụ: BUG_TL_001]
**Người báo cáo:** [Tên người phụ trách test]
**Ngày báo cáo:** [Ngày]

**1. Màn hình phát sinh lỗi:**
- Customer Timeline / Ghi chú khách hàng

**2. Mô tả lỗi:**
- Ghi ngắn gọn lỗi là gì (VD: "Timeline bị ngược thời gian, ngày cũ hiện lên trước")

**3. Các bước tái hiện (Steps to reproduce):**
- Bước 1: Vào màn hình /CRM/CustomerTimeline/Index/1
- Bước 2: Tạo ghi chú mới.
- Bước 3: Quan sát vị trí xuất hiện của ghi chú.

**4. Kết quả thực tế (Actual Result):**
- Ghi chú mới bị đẩy xuống tận cùng dưới đáy màn hình.

**5. Kết quả mong đợi (Expected Result):**
- Ghi chú mới phải hiện trên cùng (do là sự kiện mới nhất).

**6. Mức độ nghiêm trọng (Severity):**
- [ ] Critical / Nghiêm trọng
- [x] Major / Lớn (Ảnh hưởng nghiệp vụ chính)
- [ ] Minor / Nhỏ (UI/UX)
- [ ] Trivial / Linh tinh

**7. Đính kèm (Attachments):**
- [Dán ảnh screenshot vào đây]
- [Dán URL/Log lỗi backend (nếu có) vào đây]
