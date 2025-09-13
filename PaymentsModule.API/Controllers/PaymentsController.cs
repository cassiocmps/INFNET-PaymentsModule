using Microsoft.AspNetCore.Mvc;
using PaymentsModule.Domain.Interfaces.Services;
using PaymentsModule.Domain.Models;

namespace PaymentsModule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("card")]
        public async Task<IActionResult> CreateCardPayment(Guid orderId, Guid cardId, decimal amount)
        {
            var payment = await _paymentService.CreateCardPaymentChargeAsync(orderId, cardId, amount);
            return Ok(payment);
        }

        [HttpPost("pix")]
        public async Task<IActionResult> CreatePixPayment(Guid orderId, decimal amount)
        {
            var payment = await _paymentService.CreatePixPaymentAsync(orderId, amount);
            return Ok(payment);
        }

        [HttpPost("boleto")]
        public async Task<IActionResult> CreateBoletoPayment(Guid orderId, decimal amount)
        {
            var payment = await _paymentService.CreateBoletoPaymentAsync(orderId, amount);
            return Ok(payment);
        }

        [HttpPost("{id}/reissue")]
        public async Task<IActionResult> ReissuePayment(Guid id)
        {
            var payment = await _paymentService.ReissuePaymentAsync(id);
            return Ok(payment);
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RefundPayment(Guid id, BankAccount bankAccount)
        {
            var refundTransactionId = await _paymentService.RefundPaymentAsync(id, bankAccount);
            return Ok(new { PaymentId = id, RefundTransactionId = refundTransactionId });
        }
    }
}