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

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            return await Task.Run(() =>
            {
               
                using (var c = new MySqlConnection(con.MySQL))
                {
                        var sql = @"SELECT * FROM product";
                        var query = c.Query<SqlModelProduct>(sql, commandTimeout: 30);
                        return Ok(query);
                }

            });
        }

        // Get /<ProductsController>?search=name
        [HttpGet("search")]
        public async Task<IActionResult> GetProductByName(string name, int page, int amount)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {

                    var parameter = new { Name = "%" + name + "%" };
                    var sql = @"SELECT * FROM product WHERE name LIKE @Name;";
                    var query = c.Query<SqlModelProduct>(sql, parameter, commandTimeout: 30);
                    var querryarrayproducts = query.ToList();
                    var numberpages = Math.Ceiling((float)querryarrayproducts.Count / (float)amount);
                    var min = (page - 1) * amount;
                    var max = page * amount;
                    if (max <= querryarrayproducts.Count &&  page <= numberpages && page > 0 && amount > 0)
                    {

                            var arrayitems = querryarrayproducts.GetRange(min, amount);
                            var result = new
                            {
                                result = arrayitems,
                                page = page,
                                amount = amount,
                                totalpages = numberpages
                            };
                            return Ok(result);

                    }
                    else 
                    {
                        var result = new
                        {
                            message = "Wrong values"
                        };
                        return Ok(result);
                    }
                    
                }
            });
        }

        // Get /<ProductsController>/moresearched?top=maxnumber
        [HttpGet("moresearched")]
        public async Task<IActionResult> GetProductMoreSearched(string top)
        {
            return await Task.Run(() =>
            {
                using (var c = new MySqlConnection(con.MySQL))
                {

                    var parameter = new { Top = top };

                    var sql = @"SELECT  * 
                    FROM KazAVbNWEA.search S, KazAVbNWEA.detail D
                    WHERE S.product_id = D.detail_id
                    ORDER BY count DESC 
                    LIMIT 0, "+top;
                    var query = c.Query<SqlModelProduct>(sql, parameter, commandTimeout: 30);

                    return Ok(query);
                }
            });
        }


        // POST /<ProductsController>
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            ProductDetail detail = new ProductDetail();
            detail = (ProductDetail)product.Product_Detail;
            City city = new City();
            city = (City)detail.City;
            Seller seller = new Seller();
            seller = (Seller)detail.Seller;
            product.Detail_Id = product.Product_Id;
            product.Product_Detail.DetailId = product.Product_Id;

            var parameters = new
            {
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

            return await Task.Run(() =>
            {
                var message = "";
                using (var c = new MySqlConnection(con.MySQL))
                {                  

                    try
                    {

                        var ProductQuery = @"INSERT INTO product 
                                   (product_id, detail_id, name, price, discountprice, discountpercent,images) 
                            VALUES (@Product_Id,@Product_Id, @Name, @Price, @DiscountPrice, @DiscountPercent, @Images)";
                        c.Execute(ProductQuery, product, commandTimeout: 30);
                    }
                    catch (Exception e)
                    {

                        message = message + "There is already a product registered with that Product_Id. ";

                    }
                    try
                    {

                        var DetailQuery = @"INSERT INTO detail 
                                   (detail_id, seller_id, city_code, name, description, price, discountprice, images, brand, thumbnail, currency, rating) 
                            VALUES (@DetailId, @SellerId, @Code, @Name, @Description, @Price, @DiscountPrice, @ImagesD, @Brand, @Thumbnail, @Currency, @Rating)";
                        c.Execute(DetailQuery, parameters, commandTimeout: 30);

                    }
                    catch (Exception exc)
                    {

                        message = message + "There is already a product detail registered with that Id. ";

                    }

                    try
                    {
                        var SellerQuery = @"INSERT INTO seller 
                            (seller_id, name, logo) 
                            VALUES (@SellerId, @Name, @Logo)";
                        c.Execute(SellerQuery, seller, commandTimeout: 30);
                    }
                    catch (Exception e)
                    {
                        message = message + "Seller already exists, The current record will be used. ";

                    }
                    try
                    {
                        var CityQuery = @"INSERT INTO city 
                            (name, code) 
                            VALUES (@Name, @Code)";
                        c.Execute(CityQuery, city, commandTimeout: 30);
                    }
                    catch (Exception ex)
                    {
                        message = message + "City already exists, The current record will be used. ";

                    }
                    var result = new
                    {
                        message = message,
                        result = product
                    };
                    return  Ok(result);
                }
            });
        }

        // POST /<ProductsController>/shoppingcart
        [HttpPost("shoppingcart")]
        public async Task<IActionResult> PostToShoppingCart([FromBody] ShoppingCart Shoppingcart)
        {

            var parameter = new 
            {
                ShoppingcartId = Shoppingcart.User.UserId
            
            };

            return await Task.Run(() =>
            {
                var message = "";

                using (var c = new MySqlConnection(con.MySQL))
                {

                    var sql = @"SELECT * FROM shoppingcart Where shoppingcart_id = @ShoppingcartId;";
                    var query = c.Query<SqlModelShoppingCart>(sql, parameter, commandTimeout: 30);

                    SqlModelShoppingCart shoppingcart = new SqlModelShoppingCart();
                    var querryarrayshopping = query.ToArray();

                    if (querryarrayshopping.Length > 0)
                    {
                        shoppingcart = (SqlModelShoppingCart)querryarrayshopping[0];
                        var listOfProducts = shoppingcart.Products;
                        listOfProducts = listOfProducts.TrimEnd(']');

                        var parameters = new
                        {
                            ShoppingcartId = shoppingcart.shoppingcart_id,
                            Products = listOfProducts + "," + Shoppingcart.Products + "]"
                        };
                        try
                        {

                            var searchqueryupdate = @"UPDATE shoppingcart 
                                            SET products = @Products
                                            WHERE shoppingcart_id = @ShoppingcartId";
                            c.Execute(searchqueryupdate, parameters, commandTimeout: 30);

                            message = message + "Shoppingcart updated. ";
                        }

                        catch (Exception e)
                        {

                        }
                    }
                    else
                    {
                        User user = new User();
                        user = (User)Shoppingcart.User;

                        var parameters = new
                        {
                            ShoppingcartId = user.UserId,
                            Products = "["+ Shoppingcart.Products + "]"
                        };

                        try
                        {
                            var userquery = @"INSERT INTO user 
                                (user_id, name, email) 
                                VALUES (@UserId, @Name, @Email)";
                            c.Execute(userquery, user, commandTimeout: 30);
                        }
                        catch(Exception e)
                        {

                            message = message + "User already exists, The current record will be used. ";

                        }
                        try
                        {
                            var searchquery = @"INSERT INTO shoppingcart 
                                (shoppingcart_id, products) 
                                VALUES (@ShoppingcartId, @Products)";
                            c.Execute(searchquery, parameters, commandTimeout: 30);
                        }
                        catch (Exception ex)
                        {

                            message = message + "Error creating  Shoppingcart";

                        }

                        message = message + "ShoppingCart created. ";

                    }
                    var result = new
                    {
                        message = message,
                        result = Shoppingcart
                    };

                    return Ok(result);
                }
            });
        }

        // Get <ProductsController>/Product_Id
        [HttpGet("{Product_Id}")]
        public async Task<IActionResult> GetProductById(string Product_Id)
        {
            return await Task.Run(() =>
            {
                var message = "";

                using (var c = new MySqlConnection(con.MySQL))
                {

                    var parammeter = new { Id = Product_Id };
                    var sql = @"SELECT * FROM detail Where detail_id = @Id;";
                    var query = c.Query<SqlModelProductDetail>(sql, parammeter, commandTimeout: 30);

                    SqlModelProductDetail productdetail = new SqlModelProductDetail();
                    var querryarraydetail = query.ToArray();

                    if (querryarraydetail.Length > 0)
                    {
                        productdetail = (SqlModelProductDetail)querryarraydetail[0];

                        try
                        {

                            var searchquery = @"INSERT INTO search 
                                (product_id, count) 
                                VALUES (@Detail_Id, 1)";
                            c.Execute(searchquery, productdetail, commandTimeout: 30);

                            message = message + "Search added. ";
                        }

                        catch (Exception e)
                        {

                            try
                            {

                                var searchsql = @"SELECT * FROM search Where product_id = @Detail_Id;";
                                var searchquery = c.Query<Search>(searchsql, productdetail, commandTimeout: 30);

                                Search search = new Search();
                                var querryarraysearch = searchquery.ToArray();
                                if (querryarraysearch.Length > 0)
                                {

                                    search = (Search)querryarraysearch[0];
                                    search.Count = search.Count + 1;

                                    try
                                    {

                                        var searchqueryupdate = @"UPDATE search 
                                            SET count = @Count
                                            WHERE product_id = @Product_Id";
                                        c.Execute(searchqueryupdate, search, commandTimeout: 30);

                                        message = message + "Search updated. ";

                                    }
                                    catch (Exception ex)
                                    {

                                        message = message + "Error updating search table. ";

                                    }
                                }
                                else
                                {

                                    message = message + "Could not find record in search table. ";

                                }
                            }
                            catch (Exception exc)
                            {

                                message = message + "Error selecting from search table. ";

                            }
                        }
                    }
                    else
                    {

                        message = message + "Could not find record in detail table. ";

                    }
                    var result = new
                    {
                        message = message,
                        result = productdetail
                    };
                    return Ok(result);
                }
            });
        }
    }
}
