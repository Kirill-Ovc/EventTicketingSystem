using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EventTicketingSystem.API.Exceptions;

namespace EventTicketingSystem.API.Helpers
{
    public class BusinessExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException)
            {
                context.Result = new JsonResult(context.Exception.Message)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }
    }
}
