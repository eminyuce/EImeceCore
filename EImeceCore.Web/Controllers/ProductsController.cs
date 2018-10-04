﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbInfrastructure.Entities;
using DbInfrastructure.Services.IServices;
using EImeceCore.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EImeceCore.Web.Controllers
{
    public class ProductsController : BaseController
    {

        // https://blog.todotnet.com/2017/07/publishing-and-running-your-asp-net-core-project-on-linux/
        public IProductService ProductService { get; set; }
        private ILogger<ProductsController> Logger { get; set; }

        public ProductsController(
            IProductService ProductService,
            ILoggerFactory loggerFactory,
            MyAppSetttings myAppSetttings) : base(loggerFactory, myAppSetttings)
        {
            this.ProductService = ProductService;
            this.Logger = loggerFactory.CreateLogger<ProductsController>();

        }
        public async Task<IActionResult> Index()
        {
            var items = await ProductService.GetAllFromCache();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            Product model = new Product();
            if (id > 0)
            {
                model = await ProductService.GetSingleAsync(id);

                if (model == null)
                {
                    return RedirectToAction(nameof(Index));
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var t = await ProductService.SaveOrUpdateAsync(product, product.Id);
            ProductService.RemoveCache();
            return RedirectToAction(nameof(Index));
        }


    }
}