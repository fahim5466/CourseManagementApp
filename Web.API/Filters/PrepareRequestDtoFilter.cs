using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.API.Filters
{
    public class PreprocessRequestDtoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (IPreprocessDto requestDto in context.ActionArguments.Values.Where(v => v is IPreprocessDto).Cast<IPreprocessDto>())
            {
                requestDto.Preprocess();
            }
        }
    }
}
