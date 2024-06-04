using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using EventTicketingSystem.API.Exceptions;

namespace EventTicketingSystem.API.Helpers
{
    public class EntityNotFoundExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException)
            {
                context.Result = new JsonResult(context.Exception.Message)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
        }
    }
}
