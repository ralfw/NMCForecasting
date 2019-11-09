using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace nmcforecasting.tests.sampleSimulations.softwareDelivery
{
    class IssueRepository
    {
        public static IEnumerable<Issue> Import(string filename)
        {
            using var csvReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(filename) {Delimiters = new[] {";"}};
            csvReader.ReadFields(); // skip headers
            while (csvReader.EndOfData is false) {
                var row = csvReader.ReadFields();
                var tags = row[2].Split(',');
                yield return new Issue(
                    DateTime.Parse(row[0], new CultureInfo("de-DE")),
                    DateTime.Parse(row[1], new CultureInfo("de-DE")),
                    tags,
                    row[3],
                    tags.Contains("bug")
                );
            }
        }
    }
}