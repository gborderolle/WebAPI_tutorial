using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI_tutorial.Filters
{
    /// <summary>
    /// Los Filtros ejecutan código en determinado momento del sistema
    /// Ej: Filtros de acción: inicio y fin de ejecución de endpoints
    /// Ej: Filtros de autorización
    /// Ej: Filtros de excepción
    /// Ej: Filtros de recursos
    /// 
    /// Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816114#notes
    /// 
    /// Se pueden aplicar globalmente, controlador o acción
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
