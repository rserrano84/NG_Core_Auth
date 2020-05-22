using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularCore1.Data;
using AngularCore1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularCore1.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        //Get:api/values
        [HttpGet("[Action]")]
        [Authorize(Policy ="RequireLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpPost("[Action]")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formData)
        {
            var newProduct = new ProductModel()
            {
                Name = formData.Name,
                ImageUrl = formData.ImageUrl,
                Description = formData.Description,
                OutOfStock = formData.OutOfStock,
                Price = formData.Price
            };

            await _db.Products.AddAsync(newProduct);
            _db.SaveChanges();
            return Ok(new JsonResult("The product was sucesfully added"));
        }

        // api/product/1
        [HttpPut("[Action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel formData)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _db.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            //if product was found
            product.Name = formData.Name;
            product.ImageUrl = formData.ImageUrl;
            product.Description = formData.Description;
            product.OutOfStock = formData.OutOfStock;
            product.Price = formData.Price;
            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(new JsonResult("The product with id"+id+"is updated"));
        }

        // api/product/1
        [HttpDelete("[Action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var findProduct = await _db.Products.FirstAsync(p => p.ProductId == id);

            if(findProduct==null)
            {
                return NotFound();
            }

            _db.Products.Remove(findProduct);
            await _db.SaveChangesAsync();
            return Ok(new JsonResult("The product with id" + id + "is deleted"));
        }

    } 
}
