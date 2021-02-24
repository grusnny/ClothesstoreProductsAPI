using ClothesstoreProductsAPI.Models;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothesstoreProductsAPI.Services
{
    public class GetService
    {
        private readonly ConnectionStrings con;
        public GetService(ConnectionStrings c)
        {
            con = c;
        }
        public async Task<IEnumerable<SqlModelProduct>> GetAllProductsAsync()
        {
            return await Task.Run(() =>
            {

                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM product";
                    var query = c.Query<SqlModelProduct>(sql, commandTimeout: 30);
                    return query;
                }

            });
        }

        public async Task<IEnumerable<SqlModelShoppingCart>> GetAllShoppingCartAsync()
        {
            return await Task.Run(() =>
            {

                using (var c = new MySqlConnection(con.MySQL))
                {
                    var sql = @"SELECT * FROM shoppingcart";
                    var query = c.Query<SqlModelShoppingCart>(sql, commandTimeout: 30);
                    return query;
                }

            });
        }

        public async Task<object> GetProductByNameAsync(string name, int page, int amount)
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
                    var totaldata = querryarrayproducts.Count;
                    if (page <= numberpages && page > 0 && amount > 0)
                    {
                        if (min+amount > totaldata)
                        {
                            amount = amount + min - querryarrayproducts.Count;
                        }
                        var arrayitems = querryarrayproducts.GetRange(min, amount);
                        var result = new
                        {
                            result = arrayitems,
                            page = page,
                            amount = amount,
                            totalpages = numberpages,
                            totaldata = totaldata
                        };
                        return (object)result;

                    }
                    else
                    {
                        var result = new
                        {
                            message = "Wrong values"
                        };
                        return (object)result;
                    }
                }
            });
        }

        public async Task<IEnumerable<SqlModelProduct>> GetMoreSearchedProductsAsync(int top)
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
                    LIMIT 0, " + top;
                    var query = c.Query<SqlModelProduct>(sql, parameter, commandTimeout: 30);

                    return query;
                }
            });

        }

        public async Task<object> GetProductByIdAsync(string Product_Id)
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
                    return result;
                }
            });

        }

    }
}
