﻿using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}