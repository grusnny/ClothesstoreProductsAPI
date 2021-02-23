using ClothesstoreProductsAPI.Models;
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
        public async Task<IActionResult> GetAll()
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM product";
                    var query = c.Query<SqlModelProduct>(sql,  commandTimeout: 30);
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
                    var query = c.Query<SqlModelProduct>(sql, commandTimeout: 30);
                    return Ok(query);
                }
            });
        }


        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            ProductDetail detail = new ProductDetail();
            detail = (ProductDetail)product.Product_Detail;
            City city = new City();
            city = (City)detail.City;
            Seller seller = new Seller();
            seller = (Seller)detail.Seller;
            product.Detail_Id = product.Product_Id;
            product.Product_Detail.DetailId = product.Product_Id;



            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {
                    try
                    {
                        var SellerQuery = @"INSERT INTO seller 
                            (seller_id, name, logo) 
                            VALUES (@SellerId, @Name, @Logo)";
                        c.Execute(SellerQuery, seller, commandTimeout: 30);
                    }
                    catch (Exception e) {

                        return Ok("Seller ya existe con respuesta: "+ e);

                    }
                    var CityQuery = @"INSERT INTO city 
                            (name, code) 
                            VALUES (@Name, @Code)";
                    c.Execute(CityQuery, city, commandTimeout: 30);

                    var parameters = new {
                        DetailId = product.Detail_Id, 
                        SellerId = seller.SellerId, 
                        Code = city.Code,
                        Name = product.Name,
                        Description = detail.Description,
                        Price = product.Price,
                        DiscountPrice = product.DiscountPrice,
                        ImagesD = detail.ImagesD,
                        Brand = detail.Brand,
                        Thumbnail = detail.Thumbnail,
                        Currency = detail.Currency,
                        Rating = detail.Rating
                    };

                    var DetailQuery = @"INSERT INTO detail 
                                   (detail_id, seller_id, city_code, name, description, price, discountprice, images, brand, thumbnail, currency, rating) 
                            VALUES (@DetailId, @SellerId, @Code, @Name, @Description, @Price, @DiscountPrice, @ImagesD, @Brand, @Thumbnail, @Currency, @Rating)";
                    c.Execute(DetailQuery,parameters, commandTimeout: 30);

                    var ProductQuery = @"INSERT INTO product 
                                   (product_id, detail_id, name, price, discountprice, discountpercent,images) 
                            VALUES (@Product_Id,@Product_Id, @Name, @Price, @DiscountPrice, @DiscountPercent, @Images)";
                    c.Execute(ProductQuery, product, commandTimeout: 30);


                    return Ok(product);
                }
            });
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }
    }
}
