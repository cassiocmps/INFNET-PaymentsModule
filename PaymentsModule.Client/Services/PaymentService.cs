using PaymentsModule.Client.Models;
using System.Text;
using System.Text.Json;

namespace PaymentsModule.Client.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request);
        Task<ApiResponse<PaymentResponse>> ReissuePaymentAsync(Guid paymentId);
        Task<ApiResponse<RefundResponse>> RefundPaymentAsync(RefundRequest request);
    }

    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public PaymentService(IHttpClientFactory httpClientFactory, ILogger<PaymentService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PaymentsAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ApiResponse<PaymentResponse>> CreatePaymentAsync(CreatePaymentRequest request)
        {
            try
            {
                if (!request.PaymentType.HasValue)
                {
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = "Payment type is required"
                    };
                }

                string endpoint = request.PaymentType.Value switch
                {
                    PaymentType.CreditCard => $"api/payments/card?orderId={request.OrderId}&cardId={request.CardId}&amount={request.Amount}",
                    PaymentType.Pix => $"api/payments/pix?orderId={request.OrderId}&amount={request.Amount}",
                    PaymentType.Boleto => $"api/payments/boleto?orderId={request.OrderId}&amount={request.Amount}",
                    _ => throw new ArgumentException("Invalid payment type")
                };

                var response = await _httpClient.PostAsync(endpoint, null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var payment = JsonSerializer.Deserialize<PaymentResponse>(content, _jsonOptions);
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = true,
                        Data = payment,
                        Message = "Payment created successfully"
                    };
                }
                else
                {
                    var errorMessage = ExtractErrorMessage(content);
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return new ApiResponse<PaymentResponse>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<PaymentResponse>> ReissuePaymentAsync(Guid paymentId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/payments/{paymentId}/reissue", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var payment = JsonSerializer.Deserialize<PaymentResponse>(content, _jsonOptions);
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = true,
                        Data = payment,
                        Message = "Payment reissued successfully"
                    };
                }
                else
                {
                    var errorMessage = ExtractErrorMessage(content);
                    return new ApiResponse<PaymentResponse>
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reissuing payment");
                return new ApiResponse<PaymentResponse>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<RefundResponse>> RefundPaymentAsync(RefundRequest request)
        {
            try
            {
                var endpoint = $"api/payments/{request.PaymentId}/refund?reason={Uri.EscapeDataString(request.Reason)}";
                
                HttpContent? content = null;
                if (request.BankAccount != null)
                {
                    var json = JsonSerializer.Serialize(request.BankAccount, _jsonOptions);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refund = JsonSerializer.Deserialize<RefundResponse>(responseContent, _jsonOptions);
                    return new ApiResponse<RefundResponse>
                    {
                        Success = true,
                        Data = refund,
                        Message = "Refund processed successfully"
                    };
                }
                else
                {
                    var errorMessage = ExtractErrorMessage(responseContent);
                    return new ApiResponse<RefundResponse>
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                return new ApiResponse<RefundResponse>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        private string ExtractErrorMessage(string responseContent)
        {
            try
            {
                // Try to parse the error response from the API
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, _jsonOptions);
                
                // Check if it has a "detail" property (from exception middleware)
                if (errorResponse.TryGetProperty("detail", out var detailElement))
                {
                    return detailElement.GetString() ?? "An error occurred";
                }
                
                // Check if it has a "message" property (from other error responses)
                if (errorResponse.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString() ?? "An error occurred";
                }
                
                // If neither property exists, return the raw content
                return responseContent;
            }
            catch
            {
                // If JSON parsing fails, return the raw content
                return responseContent;
            }
        }
    }
}