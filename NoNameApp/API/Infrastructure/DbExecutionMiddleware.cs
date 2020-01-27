using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace API.Infrastructure {
    public class DbExecutionMiddleware {
        private readonly RequestDelegate _next;
        private readonly UnitOfWork _unitOfWork;

        public DbExecutionMiddleware(RequestDelegate next, UnitOfWork unitOfWork) {
            _next = next;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task InvokeAsync(HttpContext context) {
            if (!_unitOfWork.HasChanges()) {
                await _next.Invoke(context);
            }

            _unitOfWork.CommitChanges();
            await _next.Invoke(context);
        }
    }
}
