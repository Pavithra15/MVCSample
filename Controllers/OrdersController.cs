using EJ2MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EJ2MVC.Controllers
{
    public class OrdersController : ApiController
    {
        public static List<WeatherForecast> WeatherData = new List<WeatherForecast>();
        public static List<WeatherForecast> GetAllWeatherRecords()
        {
            if (WeatherData.Count() == 0)
            {
				WeatherData.Add(new WeatherForecast("Freezing", -3, new DateTime()));
                WeatherData.Add(new WeatherForecast("Bracing", 6, new DateTime()));
                WeatherData.Add(new WeatherForecast("Scorching", 8, new DateTime()));
                WeatherData.Add(new WeatherForecast("Cool", 2, new DateTime()));
                WeatherData.Add(new WeatherForecast("Warm", 12, new DateTime()));
            }
            return WeatherData;
        }
        // GET: api/Orders
        [HttpGet]
        public object Get()
        {
            //var queryString = HttpContext.Current.Request.QueryString;
            //int skip = Convert.ToInt32(queryString["$skip"]);
            //int take = Convert.ToInt32(queryString["$top"]);
            //var data = OrdersDetails.GetAllRecords().ToList();
            //return take != 0 ? new { Items = data.Skip(skip).Take(take).ToList(), Count = data.Count() } : new { Items = data, Count = data.Count() };
            var items = GetAllWeatherRecords().ToList();
            var queryString = HttpContext.Current.Request.QueryString;


            int skip = Convert.ToInt32(queryString["$skip"]); //get paging queries

            int take = Convert.ToInt32(queryString["$top"]);

            string filter = queryString["$filter"]; // get filter queries

            string sort = queryString["$orderby"]; //get sort queries

            string auto = queryString["$inlineCount"];
            if (filter != null) // handle filter opertaion
            {
                if (filter.Contains("substring")) //searching 
                {

                    var key = filter.Split(new string[] { "'" }, StringSplitOptions.None)[1];
                    items = items.Where(fil => fil.Summary.ToLower().ToString().Contains(key.ToLower())
                                            || fil.Temperature.ToString().Contains(key)
                                            ).ToList();
                }
                else
                {
                    var newfiltersplits = filter;
                    var filtersplits = newfiltersplits.Split('(', ')', ' ');
                    var filterfield = filtersplits[1];
                    var filtervalue = filtersplits[3];

                    if (filtersplits.Length == 5)
                    {
                        if (filtersplits[1] == "tolower")
                        {
                            filterfield = filter.Split('(', ')', '\'')[2];
                            filtervalue = filter.Split('(', ')', '\'')[4];
                        }
                    }
                    if (filtersplits.Length != 5)
                    {
                        filterfield = filter.Split('(', ')', '\'')[3];
                        filtervalue = filter.Split('(', ')', '\'')[5];

                    }

                    switch (filterfield)
                    {


                        case "Temperature":

                            items = (from cust in items
                                     where cust.Temperature.ToString() == filtervalue.ToString()
                                     select cust).ToList();
                            break;
                        case "Summary":

							items = (from item in items
									 where item.Summary.ToLower().StartsWith(filtervalue.ToLower())
                                     select item).ToList();

                            break;
                    }
                }
            }

            var count = items.Count();
            return new { Items = items, Count = count };
    }
        public class WeatherForecast
        {
			public WeatherForecast(string Summary, int Temperature, DateTime Date)
			{
				this.Summary = Summary;
				this.Temperature = Temperature;
				this.Date = Date;
			}

			public string Summary { get; set; }

            public int Temperature { get; set; }

            public DateTime Date { get; set; }
        }
        // GET: api/Orders/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Orders
        [HttpPost]
        public object Post(OrdersDetails value)
        {
            OrdersDetails.GetAllRecords().Insert(0, value);
            return value;
        }

        // PUT: api/Orders/5
        [HttpPut]
        public object Put(OrdersDetails value)
        {
            var ord = value;
            OrdersDetails val = OrdersDetails.GetAllRecords().Where(or => or.OrderID == ord.OrderID).FirstOrDefault();
            val.OrderID = ord.OrderID;
            val.EmployeeID = ord.EmployeeID;
            val.CustomerID = ord.CustomerID;
            val.Freight = ord.Freight;
            val.OrderDate = ord.OrderDate;
            val.ShipCity = ord.ShipCity;
            return value;
        }

        // DELETE: api/Orders/5
        public object Delete(int id)
        {
            OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().Where(or => or.OrderID == id).FirstOrDefault());
            return Json(id);
        }
    }
}
