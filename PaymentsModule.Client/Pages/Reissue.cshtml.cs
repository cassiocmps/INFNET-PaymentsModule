using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentsModule.Client.Models;
using PaymentsModule.Client.Services;
using System.ComponentModel.DataAnnotations;

namespace PaymentsModule.Client.Pages
{
    public class ReissueModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public ReissueModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Payment ID is required")]
        public Guid PaymentId { get; set; }

        public PaymentResponse? PaymentResponse { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public void OnGet()
        {
            // Initialize with empty form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _paymentService.ReissuePaymentAsync(PaymentId);

            if (result.Success)
            {
                PaymentResponse = result.Data;
                Message = result.Message;
                IsSuccess = true;
                
                // Clear the form for next reissue
                PaymentId = Guid.Empty;
            }
            else
            {
                Message = result.Message;
                IsSuccess = false;
            }

            return Page();
        }
    }
}