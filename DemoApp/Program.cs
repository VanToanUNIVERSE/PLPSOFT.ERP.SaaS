using DemoApp.Data;
using DemoApp.Models;
using DemoApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Sử dụng InMemory Database để demo (không cần SQL Server)
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseInMemoryDatabase("CrmDemoDB"));

builder.Services.AddScoped<ICustomerTimelineService, CustomerTimelineService>();

var app = builder.Build();

// ======== Seed dữ liệu mẫu ========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

    // --- Khách hàng ---
    if (!db.Customers.Any())
    {
        db.Customers.AddRange(
            new Customer { CustomerID = 1, CustomerName = "Công ty TNHH ABC Tech", Phone = "028-3820-1234", Email = "contact@abctech.vn", Address = "123 Nguyễn Huệ, Q.1, TP.HCM", CreatedAt = DateTime.Now.AddMonths(-6) },
            new Customer { CustomerID = 2, CustomerName = "Nguyễn Văn Minh", Phone = "0901-234-567", Email = "minh.nguyen@gmail.com", Address = "456 Lê Lợi, Q.3, TP.HCM", CreatedAt = DateTime.Now.AddMonths(-3) },
            new Customer { CustomerID = 3, CustomerName = "Công ty CP XYZ Solutions", Phone = "024-3755-8888", Email = "info@xyz-solutions.vn", Address = "789 Trần Hưng Đạo, Hoàn Kiếm, Hà Nội", CreatedAt = DateTime.Now.AddMonths(-1) }
        );
    }

    // --- Hóa đơn ---
    if (!db.SalesInvoices.Any())
    {
        db.SalesInvoices.AddRange(
            new SalesInvoice { InvoiceID = 1, CustomerID = 1, InvoiceCode = "INV-2026-001", TotalAmount = 15_500_000m, InvoiceDate = DateTime.Now.AddDays(-20) },
            new SalesInvoice { InvoiceID = 2, CustomerID = 1, InvoiceCode = "INV-2026-002", TotalAmount = 8_200_000m, InvoiceDate = DateTime.Now.AddDays(-10) },
            new SalesInvoice { InvoiceID = 3, CustomerID = 2, InvoiceCode = "INV-2026-003", TotalAmount = 22_000_000m, InvoiceDate = DateTime.Now.AddDays(-5) },
            new SalesInvoice { InvoiceID = 4, CustomerID = 1, InvoiceCode = "INV-2026-004", TotalAmount = 35_800_000m, InvoiceDate = DateTime.Now.AddDays(-2) }
        );
    }

    // --- Phản hồi / Khiếu nại ---
    if (!db.CustomerFeedbacks.Any())
    {
        db.CustomerFeedbacks.AddRange(
            new CustomerFeedback { FeedbackID = 1, CustomerID = 1, Title = "Phản hồi chất lượng sản phẩm", Content = "Sản phẩm rất tốt, đóng gói cẩn thận. Sẽ tiếp tục hợp tác.", Status = "Đã xử lý", CreatedAt = DateTime.Now.AddDays(-15) },
            new CustomerFeedback { FeedbackID = 2, CustomerID = 1, Title = "Yêu cầu hỗ trợ kỹ thuật", Content = "Cần hướng dẫn cài đặt phần mềm phiên bản mới v3.2.", Status = "Đang xử lý", CreatedAt = DateTime.Now.AddDays(-7) },
            new CustomerFeedback { FeedbackID = 3, CustomerID = 2, Title = "Khiếu nại giao hàng trễ", Content = "Đơn hàng #INV-2026-003 giao chậm hơn cam kết 2 ngày.", Status = "Mới", CreatedAt = DateTime.Now.AddDays(-3) },
            new CustomerFeedback { FeedbackID = 4, CustomerID = 3, Title = "Góp ý cải thiện UI", Content = "Giao diện quản lý kho hàng nên thêm chức năng tìm kiếm nâng cao.", Status = "Mới", CreatedAt = DateTime.Now.AddDays(-1) }
        );
    }

    // --- Lịch hẹn / Lịch chăm sóc ---
    if (!db.CustomerSchedules.Any())
    {
        db.CustomerSchedules.AddRange(
            new CustomerSchedule { ScheduleID = 1, CustomerID = 1, Title = "Họp demo sản phẩm mới v4.0", StartTime = DateTime.Now.AddDays(-3), EndTime = DateTime.Now.AddDays(-3).AddHours(2), Status = "Hoàn thành", CreatedAt = DateTime.Now.AddDays(-10), AssignedToUserID = 1 },
            new CustomerSchedule { ScheduleID = 2, CustomerID = 1, Title = "Gọi điện chăm sóc định kỳ", StartTime = DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddMinutes(30), Status = "Chờ", CreatedAt = DateTime.Now.AddDays(-1), AssignedToUserID = 2 },
            new CustomerSchedule { ScheduleID = 3, CustomerID = 2, Title = "Ký hợp đồng gia hạn dịch vụ", StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(5).AddHours(1), Status = "Chờ", CreatedAt = DateTime.Now, AssignedToUserID = 1 },
            new CustomerSchedule { ScheduleID = 4, CustomerID = 3, Title = "Khảo sát nhu cầu khách hàng", StartTime = DateTime.Now.AddDays(7), EndTime = DateTime.Now.AddDays(7).AddHours(1), Status = "Chờ", CreatedAt = DateTime.Now, AssignedToUserID = 1 }
        );
    }

    // --- Ghi chú ---
    if (!db.CustomerNotes.Any())
    {
        db.CustomerNotes.AddRange(
            new CustomerNote { NoteID = 1, CustomerID = 1, Content = "Khách hàng VIP - ưu tiên chăm sóc đặc biệt. Liên hệ qua email trước khi gọi điện.", CreatedByUserID = 1, CreatedAt = DateTime.Now.AddDays(-25) },
            new CustomerNote { NoteID = 2, CustomerID = 1, Content = "Đã gửi báo giá gói Enterprise qua email ngày 10/05/2026. Chờ phản hồi.", CreatedByUserID = 1, CreatedAt = DateTime.Now.AddDays(-12) },
            new CustomerNote { NoteID = 3, CustomerID = 1, Content = "Khách hàng quan tâm đến module HR và Payroll. Cần chuẩn bị slide demo.", CreatedByUserID = 2, CreatedAt = DateTime.Now.AddDays(-5) },
            new CustomerNote { NoteID = 4, CustomerID = 2, Content = "Khách hàng cá nhân, tiềm năng nâng lên gói doanh nghiệp. Theo dõi sát.", CreatedByUserID = 1, CreatedAt = DateTime.Now.AddDays(-8) },
            new CustomerNote { NoteID = 5, CustomerID = 2, Content = "Đã giải quyết khiếu nại giao hàng. Tặng voucher 500K cho đơn tiếp theo.", CreatedByUserID = 2, CreatedAt = DateTime.Now.AddDays(-1) },
            new CustomerNote { NoteID = 6, CustomerID = 3, Content = "Khách hàng mới từ kênh giới thiệu. Cần follow up sau buổi khảo sát.", CreatedByUserID = 1, CreatedAt = DateTime.Now.AddDays(-1) }
        );
    }

    db.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CustomerTimeline}/{action=Index}/{id?}");

app.Run();
