using System.Diagnostics;
using System.Security.Claims;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Notifications;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub")
                   ?? throw new UnauthorizedAccessException("User ID not found in token.");
        }

        private string GetTraceId()
        {
            return Activity.Current?.TraceId.ToString() ?? HttpContext.TraceIdentifier;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] bool? isRead = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetMyNotifications. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var result = await _notificationService.GetMyNotificationsAsync(userId, isRead, pageNumber, pageSize, traceId);

            _logger.LogInformation("Completed GetMyNotifications successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<PagedResult<NotificationResponse>>.SuccessResponse(result, "Notifications retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting GetUnreadCount. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            var count = await _notificationService.GetUnreadCountAsync(userId, traceId);

            _logger.LogInformation("Completed GetUnreadCount successfully. Count: {Count}, TraceId: {TraceId}", count, traceId);
            return Ok(ApiResponse<int>.SuccessResponse(count, "Unread count retrieved successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("{notificationId:int}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting MarkAsRead. UserId: {UserId}, NotificationId: {NotificationId}, TraceId: {TraceId}", userId, notificationId, traceId);

            await _notificationService.MarkAsReadAsync(userId, notificationId, traceId);

            _logger.LogInformation("Completed MarkAsRead successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Notification marked as read successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting MarkAllAsRead. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

            await _notificationService.MarkAllAsReadAsync(userId, traceId);

            _logger.LogInformation("Completed MarkAllAsRead successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "All notifications marked as read successfully.", StatusCodes.Status200OK));
        }

        [HttpDelete("{notificationId:int}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            var traceId = GetTraceId();
            var userId = GetUserId();

            _logger.LogInformation("Starting Delete Notification. UserId: {UserId}, NotificationId: {NotificationId}, TraceId: {TraceId}", userId, notificationId, traceId);

            await _notificationService.DeleteAsync(userId, notificationId, traceId);

            _logger.LogInformation("Completed Delete Notification successfully. TraceId: {TraceId}", traceId);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Notification deleted successfully.", StatusCodes.Status200OK));
        }
    }
}