using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HzBISCommands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewBISReports.Controllers.Config;
using NewBISReports.Models;
using Newtonsoft.Json;

namespace NewBISReports.Controllers.Data
{

    [Authorize("AcessoUsuario")]
    public class ImportVisitorController : Controller
    {
        private BSConfig Config { get; set; }

        //[HttpGet("ImportVisitor/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost("ImportVisitor/UploadFiles")]
        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            try
            {
                long size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = "c:\\Horizon\\";

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(filePath = (formFile.FileName), FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                // process uploaded files
                // Don't rely on or trust the FileName property without validation.
                List<BSVisitorsInfo> visitors = GlobalFunctions.ReadExcelVisitor(filePath);
                string cnt = JsonConvert.SerializeObject(visitors);
                string response = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://" + this.Config.RestServer + ":" + this.Config.RestPort);
                    HttpResponseMessage responsePost = await client.PostAsync("/api/BSVisitors/ImportVisitors/", new StringContent(cnt, Encoding.UTF8, "application/json"));
                    response = await responsePost.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(response))
                        throw new Exception(response);
                }
                //return Ok(new { count = files.Count, size, filePath });
                return View();
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Home Controller");
                w.Close();
                w = null;

                return View();
            }
        }

        public ImportVisitorController(IConfiguration configuration)
        {
            try
            {
                string defaultsettings = configuration.GetSection("Default")["Name"];
                this.Config = new BSConfig(defaultsettings, configuration.GetSection(defaultsettings)["BackColor"], configuration.GetSection(defaultsettings)["ForeColor"],
                    configuration.GetSection(defaultsettings)["FontWeight"], configuration.GetSection(defaultsettings)["ImagePath"],
                    configuration.GetSection(defaultsettings)["Meal"], configuration.GetSection(defaultsettings)["BisPath"], configuration.GetSection(defaultsettings)["SystemType"],
                    configuration.GetSection(defaultsettings)["AddressTagPrefix"], configuration.GetSection(defaultsettings)["AddressTagSufix"], configuration.GetSection(defaultsettings)["TagBISServer"],
                    configuration.GetSection(defaultsettings)["RestServer"], configuration.GetSection(defaultsettings)["RestPort"]);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Home Controller");
                w.Close();
                w = null;
            }
        }
    }
}