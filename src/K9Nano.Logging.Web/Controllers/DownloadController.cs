using System;
using System.Threading.Tasks;
using K9Nano.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace K9Nano.Logging.Web.Controllers
{
    public class DownloadController: ControllerBase
    {
        private readonly ILoggingStore _loggingStore;

        public DownloadController(ILoggingStore loggingStore)
        {
            _loggingStore = loggingStore;
        }

        public Task<IActionResult> Download(string application, DateTime from, DateTime to)
        {

        }
    }
}