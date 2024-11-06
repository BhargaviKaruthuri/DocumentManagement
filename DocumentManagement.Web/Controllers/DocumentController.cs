using Microsoft.AspNetCore.Mvc;


namespace DocumentManagement.Web.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddDocuments()
        {
            return View();
        }
        public IActionResult ListView()
        {
            return View();
        }
        public IActionResult ViewDocuments()
        {
            return View();
        }
    }
}
