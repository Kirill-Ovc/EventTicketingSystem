using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EventTicketingSystem.API.Exceptions;

namespace EventTicketingSystem.API.Helpers
{
    public class EntityNotFoundExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<EntityNotFoundExceptionFilter> _logger;

        public EntityNotFoundExceptionFilter(ILogger<EntityNotFoundExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException)
            {
                _logger.LogError(context.Exception, context.Exception.Message);
                context.Result = new JsonResult(context.Exception.Message)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
        }
    }
}
