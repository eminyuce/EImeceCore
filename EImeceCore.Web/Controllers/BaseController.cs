using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbInfrastructure.Services.IServices;
using EImeceCore.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EImeceCore.Web.Controllers
{

    public abstract class BaseController : Controller
    {
        protected ILoggerFactory LoggerFactory { get; set; }
        protected MyAppSetttings MyAppSetttings { get; set; }
        public BaseController(ILoggerFactory loggerFactory, MyAppSetttings myAppSetttings)
        {
            LoggerFactory = loggerFactory;
            MyAppSetttings = myAppSetttings;
        }
    }

     
}