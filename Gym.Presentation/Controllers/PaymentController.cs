using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Payments;
using Gym.Application.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gym.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[SwaggerTag("Payment management — create payments & view member payment history.")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _service;
    public PaymentsController(PaymentService service)
    {
        _service = service;
    }

    // =============================
    // 1) Create Payment (Admin only)
    // =============================
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Create a payment",
        Description = "Admin xác nhận member thanh toán gói tập (plan)."
    )]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), 200)]
    public async Task<IActionResult> Pay([FromBody] CreatePaymentRequest req, CancellationToken ct)
    {
        var dto = await _service.PayAsync(req, ct);
        return Ok(ApiResponse<PaymentDto>.Ok(dto, "Payment created"));
    }

    // ========================================
    // 2) Get payments by member (Any logged-in)
    // ========================================
    [HttpGet("member/{memberId:guid}")]
    [SwaggerOperation(
        Summary = "Get payments by member",
        Description = "Trả về lịch sử thanh toán của 1 member."
    )]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentDto>>), 200)]
    public async Task<IActionResult> GetByMember(Guid memberId, CancellationToken ct)
    {
        var list = await _service.GetByMemberAsync(memberId, ct);
        return Ok(ApiResponse<IEnumerable<PaymentDto>>.Ok(list));
    }
}