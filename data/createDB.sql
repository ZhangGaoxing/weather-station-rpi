CREATE DATABASE WeatherStation
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8';

create table weather (
   weather_id           SERIAL not null,
   date_time            TIMESTAMP            null,
   weather_name         VARCHAR(10)          null,
   temperature          NUMERIC(5,2)         null,
   humidity             NUMERIC(5,2)         null,
   pressure             NUMERIC(10,2)        null,
   image_base64         TEXT                 null,
   constraint PK_WEATHER primary key (weather_id)
);