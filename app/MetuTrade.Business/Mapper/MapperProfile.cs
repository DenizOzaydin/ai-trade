using AutoMapper;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoExchange.Net.CommonObjects;
using MetuTrade.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            
        }

        public static KlineInterval MapInterval(string interval)
        {
            switch (interval)
            {
                case "1-m":
                    return KlineInterval.OneMinute;
                case "3-m":
                    return KlineInterval.ThreeMinutes;
                case "5-m":
                    return KlineInterval.FiveMinutes;
                case "15-m":
                    return KlineInterval.FifteenMinutes;
                case "1-h":
                    return KlineInterval.OneHour;
                case "4-h":
                    return KlineInterval.FourHour;
                case "1-d":
                    return KlineInterval.OneDay;
            }
            throw new ArgumentException($"Interval {interval} is not valid.");
        }

        public static string ReverseMapInterval(KlineInterval interval)
        {
            switch (interval)
            {
                case KlineInterval.OneMinute:
                    return "1-m";
                case KlineInterval.ThreeMinutes:
                    return "3-m";
                case KlineInterval.FiveMinutes:
                    return "5-m";
                case KlineInterval.FifteenMinutes:
                    return "15-m";
                case KlineInterval.OneHour:
                    return "1-h";
                case KlineInterval.FourHour:
                    return "4-h";
                case KlineInterval.OneDay:
                    return "1-d";
            }
            throw new ArgumentException($"Interval {interval} is not valid.");
        }

        public static Bar MapBar(string symbol, string interval, IBinanceKline barModel)
        {
            Bar bar = new Bar();
            bar.Symbol = symbol;
            bar.Interval = interval;
            bar.Open = (double)barModel.OpenPrice;
            bar.High = (double)barModel.HighPrice;
            bar.Low = (double)barModel.LowPrice;
            bar.Close = (double)barModel.ClosePrice;
            bar.Volume = (double)barModel.Volume;
            bar.OpenTime = ((DateTimeOffset)barModel.OpenTime).ToUnixTimeMilliseconds();

            return bar;
        } 
    }
}
