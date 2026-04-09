using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

try
{
    var config = ParseArgs(args);
    var input = ReadInput(config);
    var parsed = ParseDelimited(input, config);
    var sorted = SortRows(parsed.Rows, config);
    var output = Serialize(parsed.Header, sorted, config);
    WriteOutput(output, config);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

AppConfig ParseArgs(string[] arguments)
{
    string? inputFile = null;
    string? outputFile = null;
    string delimiter = ",";
    bool noHeader = false;
    var sortFields = new List<SortField>();
    var positionalArgs = new List<string>();

    for (int i = 0; i < arguments.Length; i++)
    {
        string arg = arguments[i];
        if (arg == "-h" || arg == "--help")
        {
            Console.WriteLine("Uso: sortx [input [output]] [-b|--by campo[:tipo[:orden]]]...");
            Environment.Exit(0);
        }
        else if (arg == "-i" || arg == "--input") inputFile = arguments[++i];
        else if (arg == "-o" || arg == "--output") outputFile = arguments[++i];
        else if (arg == "-d" || arg == "--delimiter") delimiter = arguments[++i] == "\\t" ? "\t" : arguments[i];
        else if (arg == "-nh" || arg == "--no-header") noHeader = true;
        else if (arg == "-b" || arg == "--by")
        {
            var parts = arguments[++i].Split(':');
            sortFields.Add(new SortField(parts[0], parts.Length > 1 && parts[1] == "num", parts.Length > 2 && parts[2] == "desc"));
        }
        else if (arg.StartsWith("-")) throw new Exception($"Opción desconocida: {arg}");
        else positionalArgs.Add(arg);
    }

    if (inputFile == null && positionalArgs.Count > 0) inputFile = positionalArgs[0];
    if (outputFile == null && positionalArgs.Count > 1) outputFile = positionalArgs[1];

    return new AppConfig(inputFile, outputFile, delimiter, noHeader, sortFields);
}
string ReadInput(AppConfig config)
{
    if (string.IsNullOrEmpty(config.InputFile))
    {
        return Console.In.ReadToEnd();
    }
    return File.ReadAllText(config.InputFile);
}(string[] Header, List<Dictionary<string, string>> Rows) ParseDelimited(string text, AppConfig config)
{
    var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

    if (lines.Length == 0) return (Array.Empty<string>(), new List<Dictionary<string, string>>());

    string[] header;
    int startIndex = 0;

    if (config.NoHeader)
    {
        header = Enumerable.Range(0, lines[0].Split(config.Delimiter).Length).Select(i => i.ToString()).ToArray();
    }
    else
    {
        header = lines[0].Split(config.Delimiter);
        startIndex = 1;
    }

    var rows = new List<Dictionary<string, string>>();
    for (int i = startIndex; i < lines.Length; i++)
    {
        var values = lines[i].Split(config.Delimiter);
        var row = new Dictionary<string, string>();
        for (int j = 0; j < header.Length; j++) row[header[j]] = j < values.Length ? values[j] : "";
        rows.Add(row);
    }
    return (header, rows);
}
List<Dictionary<string, string>> SortRows(List<Dictionary<string, string>> rows, AppConfig config)
{
    if (config.SortFields.Count == 0) return rows;

    Comparison<Dictionary<string, string>> comparison = (a, b) =>
    {
        foreach (var field in config.SortFields)
        {
            if (!a.TryGetValue(field.Name, out var valA) || !b.TryGetValue(field.Name, out var valB))
                throw new Exception($"Columna '{field.Name}' no encontrada para ordenar.");

            int cmp;
            if (field.Numeric)
            {
                double numA = double.TryParse(valA, NumberStyles.Any, CultureInfo.InvariantCulture, out var na) ? na : double.MinValue;
                double numB = double.TryParse(valB, NumberStyles.Any, CultureInfo.InvariantCulture, out var nb) ? nb : double.MinValue;
                cmp = numA.CompareTo(numB);
            }
            else
            {
                cmp = string.Compare(valA, valB, StringComparison.OrdinalIgnoreCase);
            }

            if (cmp != 0) return field.Descending ? -cmp : cmp;
        }
        return 0;
    };

    var rowsArray = rows.ToArray();
    Array.Sort(rowsArray, comparison);
    return rowsArray.ToList();
}string Serialize(string[] header, List<Dictionary<string, string>> rows, AppConfig config)
{
    var lines = new List<string>();
    if (!config.NoHeader && header.Length > 0) lines.Add(string.Join(config.Delimiter, header));

    foreach (var row in rows)
    {
        var values = header.Select(h => row.ContainsKey(h) ? row[h] : "").ToArray();
        lines.Add(string.Join(config.Delimiter, values));
    }
    return string.Join(Environment.NewLine, lines);
}

void WriteOutput(string text, AppConfig config)
{
    if (string.IsNullOrEmpty(config.OutputFile)) Console.Write(text + Environment.NewLine);
    else File.WriteAllText(config.OutputFile, text + Environment.NewLine);
}

record SortField(string Name, bool Numeric, bool Descending);
record AppConfig(string? InputFile, string? OutputFile, string Delimiter, bool NoHeader, List<SortField> SortFields);

