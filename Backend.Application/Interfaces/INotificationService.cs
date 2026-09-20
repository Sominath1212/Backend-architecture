using Backend.Application.Common.Models;
using Backend.Application.DTOs.Notifications;

namespace Backend.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationResponse>> GetMyNotificationsAsync(string userId, bool? isRead, int pageNumber, int pageSize, string traceId);
        Task<int> GetUnreadCountAsync(string userId, string traceId);
        Task MarkAsReadAsync(string userId, int notificationId, string traceId);
        Task MarkAllAsReadAsync(string userId, string traceId);
        Task DeleteAsync(string userId, int notificationId, string traceId);
        Task CreateNotificationAsync(CreateNotificationRequest request, string traceId);
    }
}