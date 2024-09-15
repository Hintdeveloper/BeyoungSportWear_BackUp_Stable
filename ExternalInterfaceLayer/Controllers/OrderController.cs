using BusinessLogicLayer.Services.Implements;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Order;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using static DataAccessLayer.Entity.Base.EnumBase;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Font = iTextSharp.text.Font;
using DataAccessLayer.Entity;

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVoucherService _IVoucherService;

        public OrderController(IOrderService IOrderService, IVoucherService IVoucherService)
        {
            _orderService = IOrderService;
            _IVoucherService = IVoucherService;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> AddAsync([FromBody] OrderCreateVM request, bool printInvoice = false)
        {
            if (request == null)
            {
                return BadRequest(new { status = "Error", message = "Request cannot be null." });
            }

            var result = await _orderService.CreateAsync(request);

            if (result.Success)
            {
                if (printInvoice)
                {
                    try
                    {
                        var pdfBytes = await CreatePdfBytes(request.HexCode);
                        var filePath = SavePdfToServer(pdfBytes, request.HexCode);

                        return Ok(new { status = "Success", message = "Successfully saved order and printed invoice.", pdfUrl = filePath });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { status = "Error", message = $"Failed to generate or save PDF: {ex.Message}" });
                    }
                }
                else
                {
                    return Ok(new { status = "Success", message = "Successfully saved order." });
                }
            }
            else
            {
                return BadRequest(new { status = "Error", message = result.ErrorMessage });
            }
        }
        private string SavePdfToServer(byte[] pdfBytes, string hexCode)
        {
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var fileName = $"{hexCode}_Invoice.pdf";
            var filePath = Path.Combine(downloadsPath, fileName);

            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            return fileName;
        }
        private async Task<byte[]> CreatePdfBytes(string hexcode)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var normalFont = new Font(bf, 12, Font.NORMAL, BaseColor.BLACK);
                    var boldFont = new Font(bf, 12, Font.BOLD, BaseColor.BLACK);
                    var titleFont = new Font(bf, 18, Font.BOLD, BaseColor.BLACK);

                    var smallerFont = new Font(bf, 10, normalFont.Style, normalFont.Color); 

                    Document doc = new Document(new iTextSharp.text.Rectangle(PageSize.A4.Width / 2, PageSize.A4.Height / 2), 20, 20, 20, 20);
                    PdfWriter.GetInstance(doc, memoryStream);
                    doc.Open();

                    var icon = GetIcon("logo.jpg");

                    doc.Add(new iTextSharp.text.Paragraph("HÓA ĐƠN", titleFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Ngày lập: {DateTime.Now.ToString("dd/MM/yyyy")}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph("\n"));

                    var orderData = await _orderService.GetByHexCodeAsync(hexcode);
                    if (orderData == null)
                    {
                        throw new Exception("Order data is null.");
                    }

                    // Tạo bảng thông tin khách hàng và thanh toán
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.HorizontalAlignment = Element.ALIGN_LEFT;
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new float[] { 1, 1 });

                    infoTable.AddCell(new PdfPCell(new Phrase($"Hóa đơn cho:\n{orderData.CustomerName}\n{orderData.ShippingAddress}\n{orderData.ShippingAddressLine2}", smallerFont))
                    {
                        Border = iTextSharp.text.Rectangle.NO_BORDER
                    });

                    infoTable.AddCell(new PdfPCell(new Phrase($"Thanh toán cho:\nBeyoung Sport Wear", smallerFont))
                    {
                        Border = iTextSharp.text.Rectangle.NO_BORDER
                    });

                    doc.Add(infoTable);
                    doc.Add(new iTextSharp.text.Paragraph("\n"));

                    PdfPTable productTable = new PdfPTable(5) { WidthPercentage = 100 };
                    productTable.SetWidths(new float[] { 1, 4, 1, 2, 2 }); 
                    productTable.AddCell(CreateCell("STT", Element.ALIGN_CENTER, true, boldFont));
                    productTable.AddCell(CreateCell("Tên sản phẩm", Element.ALIGN_CENTER, true, boldFont));
                    productTable.AddCell(CreateCell("SL", Element.ALIGN_CENTER, true, boldFont));
                    productTable.AddCell(CreateCell("Đơn giá", Element.ALIGN_CENTER, true, boldFont));
                    productTable.AddCell(CreateCell("Thành tiền", Element.ALIGN_CENTER, true, boldFont));

                    int index = 1;
                    decimal totalProductAmount = 0;
                    foreach (var detail in orderData.OrderDetailsVM)
                    {
                        decimal productTotal = detail.Quantity * detail.UnitPrice;
                        totalProductAmount += productTotal;

                        productTable.AddCell(CreateCell(index.ToString(), Element.ALIGN_CENTER, false, normalFont));
                        productTable.AddCell(CreateCell(detail.ProductName, Element.ALIGN_LEFT, false, normalFont));
                        productTable.AddCell(CreateCell(detail.Quantity.ToString(), Element.ALIGN_CENTER, false, normalFont));
                        productTable.AddCell(CreateCell(Currency.FormatCurrency(detail.UnitPrice.ToString()), Element.ALIGN_RIGHT, false, normalFont));
                        productTable.AddCell(CreateCell(Currency.FormatCurrency(detail.TotalAmount.ToString()), Element.ALIGN_RIGHT, false, normalFont));

                        index++;
                    }

                    doc.Add(productTable);

                    decimal shippingCost = orderData.Cotsts ?? 0;
                    decimal totalAmountBeforeDiscount = totalProductAmount;

                    decimal discount = 0;
                    if (!string.IsNullOrEmpty(orderData.VoucherCode))
                    {
                        var voucher = await _IVoucherService.GetByCodeAsync(orderData.VoucherCode);
                        if (voucher != null)
                        {
                            if (voucher.Type == Voucher.Types.Percent)
                            {
                                discount = totalAmountBeforeDiscount * (voucher.ReducedValue / 100);
                                if(discount > voucher.MaximumAmount)
                                {
                                    discount = voucher.MaximumAmount;   
                                }

                            }
                            else if (voucher.Type == Voucher.Types.Cash)
                            {
                                if(voucher.ReducedValue > voucher.MaximumAmount)
                                {
                                    discount = voucher.MaximumAmount;
                                }
                                else
                                {
                                    discount = voucher.ReducedValue;
                                }
                            }
                        }
                    }

                    decimal totalPrice = totalAmountBeforeDiscount - discount;

                    decimal finalPrice = totalPrice + shippingCost;

                    PdfPTable totalTable = new PdfPTable(1) { WidthPercentage = 100 };
                    totalTable.SetWidths(new float[] { 1 });

                    totalTable.AddCell(CreateCell($"Phí vận chuyển: {Currency.FormatCurrency(shippingCost.ToString())}", Element.ALIGN_RIGHT, false, boldFont));
                    totalTable.AddCell(CreateCell($"Giảm giá: {Currency.FormatCurrency(discount.ToString())}", Element.ALIGN_RIGHT, false, boldFont));
                    totalTable.AddCell(CreateCell($"Tổng cộng: {Currency.FormatCurrency(finalPrice.ToString())}", Element.ALIGN_RIGHT, true, boldFont));
                    doc.Add(totalTable);

                    doc.Add(new iTextSharp.text.Paragraph("\n"));

                    AddFooterWithLogo(doc, icon);

                    doc.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating PDF: {ex.Message}");
                throw;
            }
        }
        private iTextSharp.text.Image GetIcon(string name)
        {
            var iconUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", name);
            var image = iTextSharp.text.Image.GetInstance(new Uri(iconUrl));
            image.ScaleToFit(60f, 60f);
            image.Alignment = Element.ALIGN_CENTER;

            return image;
        }
        private void AddFooterWithLogo(Document doc, iTextSharp.text.Image logo)
        {
            // Tải font hỗ trợ tiếng Việt
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var footerFont = new Font(bf, 10, Font.ITALIC, BaseColor.GRAY);

            // Tạo bảng footer với 2 cột
            PdfPTable footerTable = new PdfPTable(2)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_CENTER // Căn giữa toàn bộ bảng footer
            };
            footerTable.SetWidths(new float[] { 1, 3 }); // Điều chỉnh tỷ lệ cột

            // Cấu hình ô chứa logo (bên trái)
            logo.ScaleToFit(50f, 50f); // Điều chỉnh kích thước logo nếu cần
            PdfPCell logoCell = new PdfPCell(logo)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 10f // Thêm khoảng cách bên trái nếu cần
            };

            // Cấu hình ô chứa thông tin cửa hàng (bên phải)
            string storeInfo = "Cửa hàng Beyoung Sport Wear\nĐịa chỉ: Nam Từ Liêm - Hà Nội - Việt Nam\nĐiện thoại: 0.334.539.098\nEmail: contact@beyoungsportwear.com";
            PdfPCell footerTextCell = new PdfPCell(new Phrase(storeInfo, footerFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,  // Đảm bảo căn lề trái cho văn bản
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 10f // Thêm khoảng cách bên phải nếu cần
            };

            // Thêm các ô vào bảng theo thứ tự: logo bên trái, thông tin bên phải
            footerTable.AddCell(logoCell);
            footerTable.AddCell(footerTextCell);

            // Thêm một khoảng trống trước khi thêm footer (nếu cần thiết)
            doc.Add(new iTextSharp.text.Paragraph("\n"));

            // Thêm bảng footer vào tài liệu
            doc.Add(footerTable);
        }
        private PdfPCell CreateCell(string text, int alignment, bool isHeader, iTextSharp.text.Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;

            if (isHeader)
            {
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            }

            return cell;
        }
        [HttpGet("printf_order_pdf/{hexCode}")]
        public async Task<IActionResult> GeneratePdf(string hexCode)
        {
            try
            {
                var pdfBytes = await CreatePdfBytes(hexCode);
                if (pdfBytes == null)
                {
                    return NotFound("Order not found or PDF creation failed.");
                }

                var fileName = SavePdfToServer(pdfBytes, hexCode);

                return Ok(new { FileName = fileName, FileBytes = pdfBytes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetByHexCode/{HexCode}")]
        public async Task<IActionResult> GetByHexCode(string HexCode)
        {
            var order = await _orderService.GetByHexCodeAsync(HexCode);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet("GetByStatus/{OrderStatus}")]
        public async Task<IActionResult> GetByStatus(OrderStatus OrderStatus)
        {
            var orders = await _orderService.GetByStatusAsync(OrderStatus);
            if (orders == null || orders.Count == 0)
            {
                return BadRequest("Không có đơn hàng nào");
            }
            return Ok(orders);
        }
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var objCollection = await _orderService.GetAllAsync();

            return Ok(objCollection);
        }
        [HttpGet]
        [Route("allactive")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var objCollection = await _orderService.GetAllActiveAsync();

            return Ok(objCollection);
        }
        [HttpGet("customer/{ID_User}")]
        public async Task<IActionResult> GetByCustomerID(string ID_User)
        {
            var orders = await _orderService.GetByCustomerIDAsync(ID_User);
            if (orders == null) return NotFound();
            return Ok(orders);
        }
        [HttpGet("GetByIDAsync/{ID}")]
        public async Task<IActionResult> GetByIDAsync(Guid ID)
        {
            var order = await _orderService.GetByIDAsync(ID);
            return Ok(order);
        }
        [HttpPut("UpdateOrder/{ID}/{IDUserUpdate}")]
        public async Task<IActionResult> UpdateOrder(Guid ID, [FromBody] OrderUpdateVM request, string IDUserUpdate)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu yêu cầu không hợp lệ.");
            }

            try
            {
                var updateResult = await _orderService.UpdateAsync(ID, request, IDUserUpdate);

                if (!updateResult)
                {
                    return NotFound("Không thể tìm thấy đơn hàng hoặc cập nhật không thành công.");
                }

                return Ok("Cập nhật đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đơn hàng: {ex.Message}");
                return StatusCode(500, "Lỗi server xảy ra.");
            }
        }
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.IDUser) || !Enum.IsDefined(typeof(OrderStatus), request.Status))
            {
                return BadRequest("Dữ liệu yêu cầu không hợp lệ. Vui lòng kiểm tra lại.");
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, request.Status, request.IDUser, request.BillOfLadingCode);

            if (result.Success)
            {
                return Ok("Trạng thái đơn hàng đã được cập nhật thành công.");
            }
            else
            {
                if (result.ErrorMessage == "Không tìm thấy đơn hàng")
                {
                    return NotFound(result.ErrorMessage);
                }
                else
                {
                    return BadRequest(result.ErrorMessage);
                }
            }
        }
        [HttpGet("GetByOrderType/{orderType}")]
        public async Task<IActionResult> GetByOrderType(OrderType orderType)
        {
            try
            {
                var orders = await _orderService.GetByOrderTypeAsync(orderType);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}