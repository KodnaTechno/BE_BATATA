using Application.Common.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Interfaces
{

    public interface IQuery<TResult> : IRequest<ApiResponse<TResult>>
    {
    }

    public abstract class BaseQuery<TResult> : IQuery<TResult>
    {
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
    }

    public abstract class BasePagingQuery<TResult> : BaseQuery<TResult>
    {
        public int? PageNumber {  get; set; }
        public int? PageSize { get; set; }

        public int Offset { get; set; }
        public int Limit { get; set; } 

        public bool IsPaging => !IsScrolling
            && PageNumber.HasValue &&  PageNumber.Value > 0
            && PageSize.HasValue && PageSize.Value > 0;

        public bool IsScrolling => Offset >= 0 && Limit > 0;
    }
}
