using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;
using CarsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CarWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalCarController : ControllerBase
    {
        private readonly string connectionString = "server=cis-mssql1.temple.edu;Database=fa25_3342_tul38243;User id=tul38243;Password=ohCh4ivaiN;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        [Route("FindCars")]
        public List<Car> FindCars(string pickup, string dropoff, string carType, decimal minPrice, decimal maxPrice)
        {
            List<Car> cars = new List<Car>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APIFindCars", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PickupLocationCode", pickup);
                    cmd.Parameters.AddWithValue("@DropoffLocationCode", dropoff);
                    cmd.Parameters.AddWithValue("@CarType", carType);
                    cmd.Parameters.AddWithValue("@MinPrice", minPrice);
                    cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);
                    cmd.Parameters.AddWithValue("@AvailableOnly", true);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Car car = new Car();
                            car.CarID = Convert.ToInt32(reader["CarID"]);
                            car.AgencyID = Convert.ToInt32(reader["AgencyID"]);
                            car.CarModel = reader["CarModel"].ToString();
                            car.CarType = reader["CarType"].ToString();
                            car.DailyRate = Convert.ToDecimal(reader["DailyRate"]);
                            car.Available = Convert.ToBoolean(reader["Available"]);
                            car.PickupLocationCode = reader["PickupLocationCode"].ToString();
                            car.DropoffLocationCode = reader["DropoffLocationCode"].ToString();
                            car.ImagePath = reader["ImagePath"].ToString();

                            cars.Add(car);
                        }
                    }
                }
            }

            return cars;
        }

        [HttpGet]
        [Route("FindCarsByAgency")]
        public List<Car> FindCarsByAgency(int agencyId, string pickup, string dropoff, string carType, decimal minPrice, decimal maxPrice)
        {
            List<Car> cars = new List<Car>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APIFindCarsByAgency", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AgencyID", agencyId);
                    cmd.Parameters.AddWithValue("@PickupLocationCode", pickup);
                    cmd.Parameters.AddWithValue("@DropoffLocationCode", dropoff);
                    cmd.Parameters.AddWithValue("@CarType", carType);
                    cmd.Parameters.AddWithValue("@MinPrice", minPrice);
                    cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);
                    cmd.Parameters.AddWithValue("@AvailableOnly", true);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Car car = new Car();
                            car.CarID = Convert.ToInt32(reader["CarID"]);
                            car.AgencyID = Convert.ToInt32(reader["AgencyID"]);
                            car.CarModel = reader["CarModel"].ToString();
                            car.CarType = reader["CarType"].ToString();
                            car.DailyRate = Convert.ToDecimal(reader["DailyRate"]);
                            car.Available = Convert.ToBoolean(reader["Available"]);
                            car.PickupLocationCode = reader["PickupLocationCode"].ToString();
                            car.DropoffLocationCode = reader["DropoffLocationCode"].ToString();
                            car.ImagePath = reader["ImagePath"].ToString();

                            cars.Add(car);
                        }
                    }
                }
            }

            return cars;
        }
    }
}
