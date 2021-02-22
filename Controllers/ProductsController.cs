using ClothesstoreProductsAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClothesstoreProductsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ConnectionStrings con;
        public ProductsController(ConnectionStrings c)
        {
            con = c;
        }

        /*// GET: api/<ProductsController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Product vm)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM product 
                                WHERE (@product_id = 1 OR product_id= @product_id) 
                                AND (@name IS NULL OR UPPER(name) = UPPER(@name))";
                    var query = c.Query<Product>(sql, vm, commandTimeout: 30);
                    return Ok(query);
                }
            });
        }*/

        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] Product vm)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM product";
                    var query = c.Query<Product>(sql, vm, commandTimeout: 30);
                    return Ok(query);
                }
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetByName(string name)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM product WHERE name LIKE "+ "\'" +"%"+name+"%"+ "\'" ;
                    var query = c.Query<Product>(sql, commandTimeout: 30);
                    return Ok(query);
                }
            });
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product vm)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"INSERT INTO product 
                            (product_id, name, brand, thumbnail, pictures, description, price, discountPrice, discountPercent, city_code, seller_id, currency, rating) 
                            VALUES (@product_id, @name, @brand, @thumbnail, @pictures, @description, @price, @discountPrice, @discountPercent, @city_code, @seller_id, @currency, @rating)";
                    c.Execute(sql, vm, commandTimeout: 30);
                    return Ok(vm);
                }
            });
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
