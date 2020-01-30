using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using System;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public class TransactionFilter : IAsyncResultFilter {
        private readonly UnitOfWork _unitOfWork;
        public TransactionFilter(UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            if (_unitOfWork.HasChanges()) {
                await _unitOfWork.CommitChangesAsync();
            }
            await next();
        }
    }
}
