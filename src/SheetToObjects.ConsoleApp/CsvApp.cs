﻿using System;
using System.IO;
using Newtonsoft.Json;
using SheetToObjects.Adapters.Csv;
using SheetToObjects.ConsoleApp.Models;
using SheetToObjects.Lib;

namespace SheetToObjects.ConsoleApp
{
    public class CsvApp
    {
        private readonly IProvideCsv _csvProvider;
        private readonly IConvertResponseToSheet<CsvData> _csvDataConverter;
        private readonly IMapSheetToObjects _sheetMapper;

        public CsvApp(
            IProvideCsv csvProvider,
            IConvertResponseToSheet<CsvData> csvDataConverter,
            IMapSheetToObjects sheetMapper)
        {
            _csvProvider = csvProvider;
            _csvDataConverter = csvDataConverter;
            _sheetMapper = sheetMapper;
        }

        public void Run()
        {
            
            var fileStream = File.Open(@"./profiles.csv", FileMode.Open);
            var csvData = _csvProvider.Get(fileStream, ';');
            var sheet = _csvDataConverter.Convert(csvData);

            //do the actual mapping
            var result = _sheetMapper.Map(sheet).To<ProfileModel>();

            //write csv data, sheet and model to console
            WriteToConsole(csvData, sheet, result.ParsedModels, result.ValidationErrors);
        }
        
        private static void WriteToConsole(params object[] objects)
        {
            foreach (var obj in objects)
            {
                Console.WriteLine("===============================================================");
                ConsoleWriteJson(obj);
                Console.WriteLine("===============================================================");
            }

            Console.ReadLine();
        }

        private static void ConsoleWriteJson(object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}