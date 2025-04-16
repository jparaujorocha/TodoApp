using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleException(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => NotFound(new { error = ex.Message }),
                ArgumentException => BadRequest(new { error = ex.Message }),
                InvalidOperationException => BadRequest(new { error = ex.Message }),
                _ => StatusCode(500, new { error = "An unexpected error occurred." })
            };
        }

        protected IActionResult Success(object data = null)
        {
            if (data == null)
            {
                return NoContent();
            }

            return Ok(data);
        }

        protected IActionResult Created(object data, string actionName, object routeValues)
        {
            return CreatedAtAction(actionName, routeValues, data);
        }
    }
}