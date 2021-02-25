using ClothesstoreProductsAPI.Models;
using ClothesstoreProductsAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClothesstoreProductsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private PostService postService;
        private GetService getService;

        public ProductsController(ConnectionStrings c)
        {
            getService = new GetService(c);
            postService = new PostService(c);
        }

        [HttpGet]
        public async Task<IEnumerable<SqlModelProduct>> GetAllProducts()
        {
            return await Task.Run(() =>
            {

                return getService.GetAllProductsAsync();

            });
        }

        [HttpGet("shoppingcart")]
        public async Task<IEnumerable<SqlModelShoppingCart>> GetAllShoppingCart()
        {
            return await Task.Run(() =>
            {

                return getService.GetAllShoppingCartAsync();

            });
        }

        // Get /<ProductsController>?search=name
        [HttpGet("search")]
        public async Task<object> GetProductByName(string name, int page, int amount)
        {
            return await Task.Run(() =>
            {

                return getService.GetProductByNameAsync(name,page,amount);

            });
        }

        // Get /<ProductsController>/moresearched?top=maxnumber
        [HttpGet("moresearched")]
        public async Task<IEnumerable<SqlModelProduct>> GetProductMoreSearched(int top)
        {
            return await Task.Run(() =>
            {

                return getService.GetMoreSearchedProductsAsync(top);

            });
        }


        // POST /<ProductsController>
        [HttpPost]
        public async Task<object> PostProduct([FromBody] Product product)
        {          

            return await Task.Run(() =>
            {

                return  postService.PostProductAsync(product);
                
            });
        }

        // POST /<ProductsController>/shoppingcart
        [HttpPost("shoppingcart")]
        public async Task<object> PostToShoppingCart([FromBody] ShoppingCart Shoppingcart)
        {


            return await Task.Run(() =>
            {
                
                return postService.PostShoppingCartAsync(Shoppingcart);

            });
        }

        // Get <ProductsController>/Product_Id
        [HttpGet("{Product_Id}")]
        public async Task<object> GetProductById(string Product_Id)
        {
            return await Task.Run(() =>
            {

                return getService.GetProductByIdAsync(Product_Id);

            });
        }
    }
}
