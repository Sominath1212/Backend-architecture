using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Models;
using Backend.Application.DTOs.Notifications;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<NotificationResponse>> GetMyNotificationsAsync(string userId, bool? isRead, int pageNumber, int pageSize, string traceId)
    {
        _logger.LogInformation(
            "Starting GetMyNotificationsAsync. UserId: {UserId}, IsRead: {IsRead}, Page: {Page}, TraceId: {TraceId}",
            userId, isRead, pageNumber, traceId);

        var query = await _unitOfWork.Repository<Notification>()
            .FindAsync(n => n.UserId == userId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value).ToList();
        }

        var totalCount = query.Count;

        var pagedNotifications = query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = pagedNotifications.Select(MapToResponse).ToList();

        _logger.LogInformation(
            "GetMyNotificationsAsync completed successfully. TotalCount: {TotalCount}, Returned: {Returned}, TraceId: {TraceId}",
            totalCount, items.Count, traceId);

        return new PagedResult<NotificationResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<int> GetUnreadCountAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting GetUnreadCountAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var count = await _unitOfWork.Repository<Notification>()
            .CountAsync(n => n.UserId == userId && !n.IsRead);

        _logger.LogInformation("GetUnreadCountAsync completed successfully. Count: {Count}, TraceId: {TraceId}", count, traceId);
        return count;
    }

    public async Task MarkAsReadAsync(string userId, int notificationId, string traceId)
    {
        _logger.LogInformation("Starting MarkAsReadAsync. UserId: {UserId}, NotificationId: {NotificationId}, TraceId: {TraceId}", userId, notificationId, traceId);

        var notification = await _unitOfWork.Repository<Notification>()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification is null)
        {
            _logger.LogWarning("Notification not found or access denied. NotificationId: {NotificationId}, TraceId: {TraceId}", notificationId, traceId);
            throw new NotFoundException("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("MarkAsReadAsync completed successfully. NotificationId: {NotificationId}, TraceId: {TraceId}", notificationId, traceId);
        }
        else
        {
            _logger.LogInformation("Notification already marked as read. NotificationId: {NotificationId}, TraceId: {TraceId}", notificationId, traceId);
        }
    }

    public async Task MarkAllAsReadAsync(string userId, string traceId)
    {
        _logger.LogInformation("Starting MarkAllAsReadAsync. UserId: {UserId}, TraceId: {TraceId}", userId, traceId);

        var unreadNotifications = await _unitOfWork.Repository<Notification>()
            .FindAsync(n => n.UserId == userId && !n.IsRead);

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Repository<Notification>().Update(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("MarkAllAsReadAsync completed successfully. Count: {Count}, TraceId: {TraceId}", unreadNotifications.Count, traceId);
    }

    public async Task DeleteAsync(string userId, int notificationId, string traceId)
    {
        _logger.LogInformation("Starting DeleteAsync. UserId: {UserId}, NotificationId: {NotificationId}, TraceId: {TraceId}", userId, notificationId, traceId);

        var notification = await _unitOfWork.Repository<Notification>()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification is null)
        {
            _logger.LogWarning("Notification not found for deletion. NotificationId: {NotificationId}, TraceId: {TraceId}", notificationId, traceId);
            throw new NotFoundException("Notification not found.");
        }

        _unitOfWork.Repository<Notification>().Delete(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("DeleteAsync completed successfully. NotificationId: {NotificationId}, TraceId: {TraceId}", notificationId, traceId);
    }

    public async Task CreateNotificationAsync(CreateNotificationRequest request, string traceId)
    {
        _logger.LogInformation(
            "Starting CreateNotificationAsync. UserId: {UserId}, Type: {Type}, Title: {Title}, TraceId: {TraceId}",
            request.UserId, request.Type, request.Title, traceId);

        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            ActionUrl = request.ActionUrl,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            IsRead = false
        };

        await _unitOfWork.Repository<Notification>().AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("CreateNotificationAsync completed successfully. NotificationId: {NotificationId}, TraceId: {TraceId}", notification.Id, traceId);
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = notification.ActionUrl,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            CreatedAt = notification.CreatedAt
        };
    }
}