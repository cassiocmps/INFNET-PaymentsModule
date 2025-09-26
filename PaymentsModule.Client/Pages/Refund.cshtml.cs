using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentsModule.Client.Models;
using PaymentsModule.Client.Services;
using System.ComponentModel.DataAnnotations;

namespace PaymentsModule.Client.Pages
{
    public class RefundModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public RefundModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        public RefundRequest Request { get; set; } = new() { BankAccount = new BankAccountInfo() };

        public RefundResponse? RefundResponse { get; set; }
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

            // If bank account fields are empty, don't send bank account info
            if (string.IsNullOrWhiteSpace(Request.BankAccount?.Bank) &&
                string.IsNullOrWhiteSpace(Request.BankAccount?.Agency) &&
                string.IsNullOrWhiteSpace(Request.BankAccount?.AccountNumber))
            {
                Request.BankAccount = null;
            }

            var result = await _paymentService.RefundPaymentAsync(Request);

            if (result.Success)
            {
                RefundResponse = result.Data;
                Message = result.Message;
                IsSuccess = true;
                
                // Clear the form for next refund
                Request = new RefundRequest { BankAccount = new BankAccountInfo() };
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