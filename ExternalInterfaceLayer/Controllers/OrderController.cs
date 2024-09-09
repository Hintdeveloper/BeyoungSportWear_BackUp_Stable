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

namespace ExternalInterfaceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService IOrderService)
        {
            _orderService = IOrderService;
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
                    Document doc = new Document(PageSize.A4.Rotate(), 50, 50, 25, 25);
                    PdfWriter.GetInstance(doc, memoryStream);
                    doc.Open();
                    var orderData = await _orderService.GetByHexCodeAsync(hexcode);

                    if (orderData == null)
                    {
                        throw new Exception("Order data is null.");
                    }

                    var titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.BLACK);
                    var subTitleFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.GRAY);
                    var normalFont = new Font(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 12, Font.NORMAL, BaseColor.BLACK);

                    var icon = GetIcon("logo.jpg");

                    doc.Add(icon);
                    doc.Add(new iTextSharp.text.Paragraph("Sales Invoice", titleFont));
                    doc.Add(new iTextSharp.text.Paragraph("\n"));

                    doc.Add(new iTextSharp.text.Paragraph($"Order Code: {ViString.RemoveSign4VietnameseString(orderData.HexCode)}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Customer: {ViString.RemoveSign4VietnameseString(orderData.CustomerName)}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Phone Number: {ViString.RemoveSign4VietnameseString(orderData.CustomerPhone)}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Email: {ViString.RemoveSign4VietnameseString(orderData.CustomerEmail)}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Payment Status: {ViString.RemoveSign4VietnameseString(orderData.PaymentStatus.ToString())}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Shipping Address: {ViString.RemoveSign4VietnameseString(orderData.ShippingAddress)}", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"Delivery Date: {orderData.ShipDate.ToString("dd/MM/yyyy")}", normalFont));

                    var totalAmountText = Currency.FormatCurrency(orderData.TotalAmount.ToString());
                    var totalAmountInWords = ViString.RemoveSign4VietnameseString(Currency.NumberToText((double)orderData.TotalAmount, true));
                    doc.Add(new iTextSharp.text.Paragraph($"Total Cost: {totalAmountText} ({totalAmountInWords})", normalFont));
                    if (!string.IsNullOrEmpty(orderData.Notes))
                    {
                        doc.Add(new iTextSharp.text.Paragraph($"Notes: {ViString.RemoveSign4VietnameseString(orderData.Notes)}", normalFont));
                    }
                    doc.Add(new iTextSharp.text.Paragraph("\n"));

                    doc.Add(new iTextSharp.text.Paragraph("Order Details", subTitleFont));

                    PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 1, 3, 1, 2, 2 });

                    table.AddCell(CreateCell("Image", Element.ALIGN_CENTER, true));
                    table.AddCell(CreateCell("Product Name", Element.ALIGN_CENTER, true));
                    table.AddCell(CreateCell("Quantity", Element.ALIGN_CENTER, true));
                    table.AddCell(CreateCell("Unit Price", Element.ALIGN_CENTER, true));
                    table.AddCell(CreateCell("Total Price", Element.ALIGN_CENTER, true));

                    foreach (var detail in orderData.OrderDetailsVM)
                    {
                        var image = iTextSharp.text.Image.GetInstance(new Uri(detail.ImageURL));
                        image.ScaleToFit(40f, 40f);

                        PdfPCell imageCell = new PdfPCell(image)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = Rectangle.BOX,
                            BorderWidth = 1f, // Độ rộng viền
                            BorderColor = BaseColor.BLACK, // Màu sắc viền
                            Padding = 5f // Khoảng cách từ viền đến nội dung
                        };


                        PdfPCell productCell = new PdfPCell(new iTextSharp.text.Paragraph(
                        $"{ViString.RemoveSign4VietnameseString(detail.ProductName)}\nSize: {detail.SizeName}\nColor: {detail.ColorName}",
                        new Font(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10, Font.NORMAL, BaseColor.BLACK)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = Rectangle.BOX,
                            BorderWidth = 1f,
                            BorderColor = BaseColor.BLACK
                        };

                        PdfPCell unitpriceCell = new PdfPCell(new iTextSharp.text.Paragraph(
                        $"{Currency.FormatCurrency(detail.UnitPrice.ToString())}\n {ViString.RemoveSign4VietnameseString(Currency.NumberToText((double)detail.UnitPrice, true))}",
                        new Font(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10, Font.NORMAL, BaseColor.BLACK)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = Rectangle.BOX,
                            BorderWidth = 1f,
                            BorderColor = BaseColor.BLACK
                        };

                        PdfPCell totalCell = new PdfPCell(new iTextSharp.text.Paragraph(
                        $"{Currency.FormatCurrency(detail.TotalAmount.ToString())}\n {ViString.RemoveSign4VietnameseString(Currency.NumberToText((double)detail.TotalAmount, true))}",
                        new Font(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10, Font.NORMAL, BaseColor.BLACK)))
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_TOP,
                            Border = Rectangle.BOX,
                            BorderWidth = 1f,
                            BorderColor = BaseColor.BLACK
                        };

                        table.AddCell(imageCell);
                        table.AddCell(productCell);
                        table.AddCell(detail.Quantity.ToString());
                        table.AddCell(unitpriceCell);
                        table.AddCell(totalCell);
                    }

                    doc.Add(table);

                    AddFooter(doc);

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
            // Tạo đường dẫn đầy đủ đến hình ảnh trong thư mục wwwroot
            var iconUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", name);

            // Lấy hình ảnh từ đường dẫn tuyệt đối
            var image = iTextSharp.text.Image.GetInstance(new Uri(iconUrl));

            // Điều chỉnh kích thước hình ảnh và căn giữa
            image.ScaleToFit(60f, 60f);
            image.Alignment = Element.ALIGN_CENTER;

            return image;
        }
        private void AddFooter(Document doc)
        {
            PdfPTable footerTable = new PdfPTable(1) { WidthPercentage = 100 };
            footerTable.SetWidths(new float[] { 1 });

            PdfPCell footerCell = new PdfPCell(new iTextSharp.text.Phrase("Thank you for your business!", new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC, BaseColor.GRAY)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = Rectangle.NO_BORDER
            };
            footerTable.AddCell(footerCell);

            doc.Add(footerTable);
        }

        private PdfPCell CreateCell(string text, int alignment, bool isHeader = false)
        {
            var font = isHeader ? new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE) : new Font(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 10, Font.NORMAL, BaseColor.BLACK);
            var cell = new PdfPCell(new iTextSharp.text.Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                Padding = 5,
                BackgroundColor = isHeader ? BaseColor.GRAY : BaseColor.WHITE
            };
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
        [HttpGet]
        [Route("GetOrderDetailsByID/{ID_Order}")]
        public async Task<IActionResult> GetOrderDetailsByID(Guid ID_Order)
        {
            var order = await _orderService.GetOrderDetailsVMByIDAsync(ID_Order);
            if (order == null) return NotFound();
            return Ok(order);
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

        [HttpPut("MarkAsCancelled/{ID_Order}/{IDUserUpdate}")]
        public async Task<IActionResult> MarkAsCancelled(Guid ID_Order, string IDUserUpdate)
        {
            var result = await _orderService.MarkAsCancelledAsync(ID_Order, IDUserUpdate);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPut("MarkAsTrackingCheckAsync/{ID_Order}/{IDUserUpdate}")]
        public async Task<IActionResult> MarkAsTrackingCheckAsync(Guid ID_Order, string IDUserUpdate)
        {
            var result = await _orderService.MarkAsTrackingCheckAsync(ID_Order, IDUserUpdate);
            if (!result) return BadRequest();
            return Ok();
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