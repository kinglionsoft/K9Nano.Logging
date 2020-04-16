using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using K9Nano.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace K9Nano.Logging.Web.Controllers
{
    public class DownloadController : ControllerBase
    {
        private readonly ILoggingStore _loggingStore;

        public DownloadController(ILoggingStore loggingStore)
        {
            _loggingStore = loggingStore;
        }

        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> Download(string app, DateTimeOffset from, DateTimeOffset to)
        {
            if (from > to)
            {
                return BadRequest("开始时间不能大于结束时间");
            }

            var result = await _loggingStore.QueryAsync(app == AppConsts.GroupAll ? null : app, from, to.AddDays(1), HttpContext.RequestAborted);
            if (result.Count == 0)
            {
                return BadRequest("没有查询到数据");
            }
            var memory = new MemoryStream();

            using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, true))
            {
                var entry = zip.CreateEntry(app + ".log");
                await using var writeStream = entry.Open();
                await using var writer = new StreamWriter(writeStream, Encoding.UTF8);
                foreach (var entity in result)
                {
                    await writer.WriteLineAsync(entity.ToString());
                }
            }

            memory.Seek(0, SeekOrigin.Begin);

            return File(memory, "application/x-zip-compressed", app + "-log.zip");
        }
    }
}