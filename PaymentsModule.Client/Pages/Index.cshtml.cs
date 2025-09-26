using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentsModule.Client.Models;
using PaymentsModule.Client.Services;

namespace PaymentsModule.Client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public IndexModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        public CreatePaymentRequest Request { get; set; } = new();

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

            // Validate credit card specific fields
            if (Request.PaymentType == PaymentType.CreditCard && !Request.CardId.HasValue)
            {
                ModelState.AddModelError("Request.CardId", "Card ID is required for credit card payments");
                return Page();
            }

            var result = await _paymentService.CreatePaymentAsync(Request);

            if (result.Success)
            {
                PaymentResponse = result.Data;
                Message = result.Message;
                IsSuccess = true;
                
                // Clear the form for next payment
                Request = new CreatePaymentRequest();
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
