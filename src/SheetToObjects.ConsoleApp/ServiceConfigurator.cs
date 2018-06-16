﻿using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SheetToObjects.ConsoleApp.Models;
using SheetToObjects.Lib;

namespace SheetToObjects.ConsoleApp
{
    public static class ServiceConfigurator
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Adapters.Csv.IProvideSheet, Adapters.Csv.SheetProvider>();
            serviceCollection.AddTransient<Adapters.GoogleSheets.IProvideSheet, Adapters.GoogleSheets.SheetProvider>();
            serviceCollection.AddTransient<Adapters.MicrosoftExcel.IProvideSheet, Adapters.MicrosoftExcel.SheetProvider>();

            serviceCollection.AddTransient<IMapSheetToObjects>(ctx =>
            {
                var sheetMapper = new SheetMapper();

                sheetMapper.AddConfigFor<EpicTrackingModel>(cfg => cfg
                    .MapColumn(column => column.WithColumnLetter("A").MapTo(m => m.SprintNumber))
                    .MapColumn(column => column.WithColumnLetter("B").MapTo(m => m.SprintName))
                    .MapColumn(column => column.WithColumnLetter("C").MapTo(m => m.StoryPointsCompleted))
                    .MapColumn(column => column.WithColumnLetter("D").MapTo(m => m.TotalCompleted))
                    .MapColumn(column => column.WithColumnLetter("E").MapTo(m => m.ForecastNormal))
                    .MapColumn(column => column.WithColumnLetter("F").MapTo(m => m.ForecastHigh))
                    .MapColumn(column => column.WithColumnLetter("G").MapTo(m => m.ForecastLow))
                    .MapColumn(column => column.WithColumnLetter("H").MapTo(m => m.Scope))
                );

                return sheetMapper;
            });

            serviceCollection.AddOptions();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app-settings.json", false)
                .Build();

            serviceCollection.Configure<AppSettings>(configuration.GetSection("Configuration"));

            serviceCollection.AddTransient<CsvApp>();
            serviceCollection.AddTransient<ExcelApp>();
            serviceCollection.AddTransient<GoogleSheetsApp>();

            return serviceCollection;
        }
    }
}
