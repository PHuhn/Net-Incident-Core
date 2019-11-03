//
// ---------------------------------------------------------------------------
//
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
//
using MediatR;
//
namespace NSG.WebSrv.UI.Api
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
        //
        /// <summary>
        /// Base constructors, so initialize Alerts list of alert-messages.
        /// </summary>
        public BaseApiController()
        {
            //
        }
    }
}