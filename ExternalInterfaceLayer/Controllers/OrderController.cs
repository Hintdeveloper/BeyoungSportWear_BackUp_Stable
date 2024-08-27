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
            if (request == null) return BadRequest();

            var result = await _orderService.CreateAsync(request);

            if (result.Success)
            {
                if (printInvoice)
                {
                    var pdfBytes = CreatePdfBytes(request);
                    var filePath = SavePdfToServer(pdfBytes, request.HexCode);

                    return Ok(new { status = "Success", message = "Successfully saved order and printed invoice.", pdfUrl = filePath });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Successfully saved order." });
                }
            }
            else
            {
                return BadRequest();
            }
        }
        private string SavePdfToServer(byte[] pdfBytes, string hexCode)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Invoices");
            var fileName = $"{hexCode}_Invoice.pdf";
            var filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            return $"/Invoices/{fileName}";
        }
        private byte[] CreatePdfBytes(OrderCreateVM orderData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("Hóa đơn bán hàng", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph($"Mã đơn hàng: {orderData.HexCode}"));
                doc.Add(new iTextSharp.text.Paragraph($"Khách hàng: {orderData.CustomerName}"));
                doc.Add(new iTextSharp.text.Paragraph($"Số điện thoại: {orderData.CustomerPhone}"));
                doc.Add(new iTextSharp.text.Paragraph($"Email: {orderData.CustomerEmail}"));
                doc.Add(new iTextSharp.text.Paragraph($"Địa chỉ nhận hàng: {orderData.ShippingAddress}"));
                doc.Add(new iTextSharp.text.Paragraph($"Ngày giao hàng: {orderData.ShipDate.ToString("dd/MM/yyyy")}"));
                doc.Add(new iTextSharp.text.Paragraph($"Tổng chi phí: {orderData.TotalAmount}"));
                doc.Add(new iTextSharp.text.Paragraph($"Ghi chú: {orderData.Notes}"));
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph("Chi tiết đơn hàng", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD)));
                PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1, 3, 1, 1 });
                table.AddCell("Tên sản phẩm");
                table.AddCell("Số lượng");
                table.AddCell("Giá bán");
                table.AddCell("Tổng giá");
                foreach (var detail in orderData.OrderDetailsCreateVM)
                {
                    table.AddCell(detail.IDOptions.ToString());
                    table.AddCell(detail.Quantity.ToString());
                    table.AddCell(detail.UnitPrice.ToString());
                    table.AddCell(detail.TotalAmount.ToString());
                }
                doc.Add(table);
                doc.Close();

                return memoryStream.ToArray();
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

        [HttpGet("GetByStatus")]
        public async Task<IActionResult> GetByStatus(OrderStatus OrderStatus)
        {
            var orders = await _orderService.GetByStatusAsync(OrderStatus);
            if (orders == null || orders.Count == 0)
            {
                return NotFound();
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
            if (request == null || request.Status==null || string.IsNullOrEmpty(request.IDUser))
            {
                return BadRequest("Invalid request data");
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, request.Status, request.IDUser);

            if (result)
            {
                return Ok("Order status updated successfully");
            }
            else
            {
                return NotFound("Order not found");
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
