using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EventTicketingSystem.API.Exceptions;

namespace EventTicketingSystem.API.Helpers
{
    public class BusinessExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<BusinessExceptionFilter> _logger;

        public BusinessExceptionFilter(ILogger<BusinessExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException)
            {
                _logger.LogError(context.Exception, context.Exception.Message);
                context.Result = new JsonResult(context.Exception.Message)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }
    }
}
