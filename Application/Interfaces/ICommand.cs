﻿using AppCommon.DTOs;
using Events;
using MediatR;

namespace Application.Interfaces
{
    public interface ICommand<TResult> : IRequest<ApiResponse<TResult>>
    {
        IEvent GetEvent(ApiResponse<TResult> response);
    }

    public abstract class BaseCommand<TResult> : ICommand<TResult>
    {
        public string CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public abstract IEvent GetEvent(ApiResponse<TResult> response);
    }
}
