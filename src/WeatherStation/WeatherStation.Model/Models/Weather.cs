﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherStation.Model
{
    public class Weather
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("weather_id")]
        public long WeatherID { get; set; }

        [Column("date_time")]
        public DateTime DateTime { get; set; }

        [Column("weather_name")]
        public string WeatherName { get; set; }

        [Column("temperature")]
        public double Temperature { get; set; }

        [Column("humidity")]
        public double Humidity { get; set; }

        [Column("pressure")]
        public double Pressure { get; set; }

        [Column("image_base64")]
        public string ImageBase64 { get; set; }
    }
}