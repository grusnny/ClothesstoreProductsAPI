using ClothesstoreProductsAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Services
{
    public class PostService
    {
        private readonly ConnectionStrings con;
        public PostService(ConnectionStrings c)
        {
            con = c;
        }

        public async Task<object> PostProductAsync(Product product)
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
            var message = "";

            await Task.Run(() =>
            {
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
                }
            });
            var result = new
            {
                message = message,
                result = product
            };
            return result;
        }
        public async Task<object> PostShoppingCartAsync(ShoppingCart Shoppingcart)
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
                            Products = "[" + Shoppingcart.Products + "]"
                        };

                        try
                        {
                            var userquery = @"INSERT INTO user 
                                (user_id, name, email) 
                                VALUES (@UserId, @Name, @Email)";
                            c.Execute(userquery, user, commandTimeout: 30);
                        }
                        catch (Exception e)
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

                    return result;
                }
            });
        }
    }
}
