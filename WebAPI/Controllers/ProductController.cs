using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private ApplicationSettings _appSettings;
        private ProductContext _context;

        public ProductController(UserManager<AppUser> userManager, ProductContext context, SignInManager<AppUser> signInManager, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;

        }
        [HttpPost]
        [Route("addProduct")]
        public async Task<Object> AddProduct(ProductModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var product = new Product()
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                UserId = user.Id,
                CategoryId=model.CategoryId==""?null:model.CategoryId
                
                
            };
            try
            {
              await _context.Products.AddAsync(product);
              await _context.SaveChangesAsync();
                return Ok(product);

            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            

            
        }
        [HttpPost]
        [Route("addCategory")]
        public async Task<Object> AddCategory(CategoryModel model)
        {
           
            var category = new Category()
            {
                Name = model.Name,
                
            };
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return Ok(category);

            }
            catch (Exception ex)
            {
                throw ex;
            }




        }
        [HttpGet]
        [Route("getProducts")]
        public async Task<Object> GetProducts()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var products = _context.Products.Select(i=>new ProductModel {
                Id=i.Id,
                Name=i.Name,
                Description=i.Description,
                Price=i.Price,
                UserId=i.UserId,
                CategoryId=i.CategoryId,
                Category=i.Category
            
            }).Where(i => i.UserId == user.Id).ToList();
            return Ok(products);

        }
        [HttpGet]
        [Route("getCategories")]
        public Object GetCategories()
        {
           
            var products = _context.Categories.ToList();
            return Ok(products);

        }
        [HttpPost]
        [Route("updateProduct")]
        public Object UpdateProduct(ProductModel model)
        {
            var product = _context.Products.Where(i => i.Id == model.Id).FirstOrDefault();
            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;
            _context.Products.Update(product);
            _context.SaveChanges();
            return Ok(product);
        }
        [HttpDelete]
        [Route("deleteProduct")]

        public object DeleteProduct(string id)
        {
            var product = _context.Products.Where(i => i.Id == id).FirstOrDefault();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(product);
    }

    }
}