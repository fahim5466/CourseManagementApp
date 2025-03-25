using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.API.Filters
{
    public class PreprocessRequestDtoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (IRequestDto requestDto in context.ActionArguments.Values.Where(v => v is IRequestDto).Cast<IRequestDto>())
            {
                requestDto.Preprocess();
            }
        }
    }
}
