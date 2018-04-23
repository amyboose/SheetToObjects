using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using SheetToObjects.Infrastructure.GoogleSheets;
using SheetToObjects.Lib;
using SheetToObjects.Specs.Builders;
using Xunit;

namespace SheetToObjects.Specs.Infrastructure
{
    public class GoogleSheetsConverterSpecs
    {
        private readonly Mock<IGenerateColumnLetters> _columnLettersGenerator;

        public GoogleSheetsConverterSpecs()
        {
            _columnLettersGenerator = new ColumnLetterGeneratorBuilder()
                .WithColumnLetters("A", "B", "C")
                .Build();
        }

        [Fact]
        public void GivenNoResponseData_WhenConvertingToSheet_ArgumentExceptionIsThrown()
        {
            var converter = new GoogleSheetsConverter(_columnLettersGenerator.Object);

            Action result = () => converter.Convert(null);

            result.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyResponseData_WhenConvertingToSheet_NoCellsAreCreated()
        {
            var responseData = new GoogleSheetResponse();

            var converter = new GoogleSheetsConverter(_columnLettersGenerator.Object);

            var result = converter.Convert(responseData);

            result.Rows.Should().BeEmpty();
        }

        [Fact]
        public void GivenResponseDataContainsRow_WhenConvertingToSheet_CellsInRowAreCreated()
        {
            var rowZeroColumnAValue = "row";
            var rowZeroColumnBValue = "0";
            var rowZeroColumnCValue = "text";

            var responseData = new SheetDataResponseBuilder()
                .WithRow(new List<string>{ rowZeroColumnAValue, rowZeroColumnBValue, rowZeroColumnCValue })
                .Build();

            var converter = new GoogleSheetsConverter(_columnLettersGenerator.Object);

            var result = converter.Convert(responseData);

            result.Rows.Should().NotBeEmpty();
            result.Rows.Single().Cells.Should().Contain(c => c.Value.ToString() == rowZeroColumnAValue);
            result.Rows.Single().Cells.Should().Contain(c => c.Value.ToString() == rowZeroColumnBValue);
            result.Rows.Single().Cells.Should().Contain(c => c.Value.ToString() == rowZeroColumnCValue);
        }

        [Fact]
        public void GivenResponseDataContainsCell_WhenConvertingToSheet_CellShouldHaveCorrectColumnName()
        {
            var responseData = new SheetDataResponseBuilder()
                .WithRow(new List<string> { "myValue" })
                .Build();

            var converter = new GoogleSheetsConverter(_columnLettersGenerator.Object);

            var result = converter.Convert(responseData);

            result.Rows.Single().Cells.Should().NotBeEmpty();
            result.Rows.Single().Cells.Should().Contain(c => c.ColumnLetter == "A");
        }

        [Fact]
        public void GivenResponseDataContainsCell_WhenConvertingToSheet_CellShouldHaveCorrectRowNumber()
        {
            var responseData = new SheetDataResponseBuilder()
                .WithRow(new List<string> { "myValue" })
                .Build();

            var converter = new GoogleSheetsConverter(_columnLettersGenerator.Object);

            var result = converter.Convert(responseData);

            result.Rows.Single().Cells.Should().NotBeEmpty();
            result.Rows.Single().Cells.Should().Contain(c => c.RowNumber == 1);
        }
    }
}
