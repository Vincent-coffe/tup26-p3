#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.5

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;

var inputArgument = new Argument<string?>("input") {
    Description = "Archivo de entrada. Si no se indica, se lee desde stdin.",
    Arity = ArgumentArity.ZeroOrOne
};

var csvOption = new Option<bool>("--csv") {
    Description = "Usa coma como separador."
};

var tsvOption = new Option<bool>("--tsv") {
    Description = "Usa tabulador como separador."
};

var delimiterOption = new Option<string?>("--delimiter") {
    Description = "Usa un separador explícito. Acepta un solo carácter o \\t."
};
delimiterOption.Aliases.Add("-d");

var headerOption = new Option<bool>("--header") {
    Description = "Interpreta la primera fila como encabezado (por defecto)."
};

var noHeaderOption = new Option<bool>("--no-header") {
    Description = "Trata todas las filas como datos."
};

var ignoreCaseOption = new Option<bool>("--ignore-case") {
    Description = "Ignora mayúsculas y minúsculas al comparar texto."
};

var byOption = new Option<string[]>("--by") {
    Description = "Agrega un criterio de orden: campo[:tipo[:direccion]]. Se puede repetir.",
    Required = true
};

var outputOption = new Option<string?>("--output") {
    Description = "Escribe la salida en un archivo."
};
outputOption.Aliases.Add("-o");

var rootCommand = new RootCommand("Ordena filas de un archivo delimitado por columnas.");
rootCommand.Arguments.Add(inputArgument);
rootCommand.Options.Add(csvOption);
rootCommand.Options.Add(tsvOption);
rootCommand.Options.Add(delimiterOption);
rootCommand.Options.Add(headerOption);
rootCommand.Options.Add(noHeaderOption);
rootCommand.Options.Add(ignoreCaseOption);
rootCommand.Options.Add(byOption);
rootCommand.Options.Add(outputOption);

rootCommand.SetAction(parseResult => MainProgram.Run(
    parseResult,
    inputArgument,
    csvOption,
    tsvOption,
    delimiterOption,
    headerOption,
    noHeaderOption,
    ignoreCaseOption,
    byOption,
    outputOption));

return rootCommand.Parse(args).Invoke();

static class MainProgram {
    public static int Run(
        ParseResult parseResult,
        Argument<string?> inputArgument,
        Option<bool> csvOption,
        Option<bool> tsvOption,
        Option<string?> delimiterOption,
        Option<bool> headerOption,
        Option<bool> noHeaderOption,
        Option<bool> ignoreCaseOption,
        Option<string[]> byOption,
        Option<string?> outputOption) {
        try {
            var options = BuildOptions(
                parseResult,
                inputArgument,
                csvOption,
                tsvOption,
                delimiterOption,
                headerOption,
                noHeaderOption,
                ignoreCaseOption,
                byOption,
                outputOption);

            return Run(options);
        } catch (Exception ex) {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    static int Run(AppOptions options) {
        options = ResolveDelimiter(options);

        var inputText = ReadInput(options);
        var lines = SplitLines(inputText);

        if (lines.Count == 0) {
            WriteOutput(options, "");
            return 0;
        }

        string? header = null;
        int dataStart = 0;

        if (options.HasHeader) {
            header = lines[0];
            dataStart = 1;
        }

        var headerColumns = header is null
            ? null
            : SplitRow(header, options.Delimiter);

        var sortRules = BuildSortRules(options.SortSpecs, options.HasHeader, headerColumns);

        var validRows = new List<DataRow>();
        int skipped = 0;

        for (int i = dataStart; i < lines.Count; i++) {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line)) {
                continue;
            }

            var columns = SplitRow(line, options.Delimiter);

            if (TryCreateRow(line, columns, i, sortRules, options, out var row)) {
                validRows.Add(row!);
            } else {
                skipped++;
            }
        }

        validRows.Sort((a, b) => CompareRows(a, b, sortRules, options));

        var outputLines = new List<string>();

        if (header is not null) {
            outputLines.Add(header);
        }

        foreach (var row in validRows) {
            outputLines.Add(row.OriginalLine);
        }

        var outputText = string.Join(Environment.NewLine, outputLines);

        if (outputLines.Count > 0) {
            outputText += Environment.NewLine;
        }

        WriteOutput(options, outputText);

        if (skipped > 0) {
            Console.Error.WriteLine($"Se descartaron {skipped} fila(s) inválida(s).");
        }

        return 0;
    }

    static AppOptions BuildOptions(
        ParseResult parseResult,
        Argument<string?> inputArgument,
        Option<bool> csvOption,
        Option<bool> tsvOption,
        Option<string?> delimiterOption,
        Option<bool> headerOption,
        Option<bool> noHeaderOption,
        Option<bool> ignoreCaseOption,
        Option<string[]> byOption,
        Option<string?> outputOption) {
        var inputPath = parseResult.GetValue(inputArgument);
        var outputPath = parseResult.GetValue(outputOption);
        var useCsv = parseResult.GetValue(csvOption);
        var useTsv = parseResult.GetValue(tsvOption);
        var delimiterText = parseResult.GetValue(delimiterOption);
        var headerRequested = parseResult.GetValue(headerOption);
        var noHeaderRequested = parseResult.GetValue(noHeaderOption);
        var ignoreCase = parseResult.GetValue(ignoreCaseOption);
        var sortSpecs = parseResult.GetValue(byOption) ?? Array.Empty<string>();

        if (headerRequested && noHeaderRequested) {
            throw new Exception("No se pueden indicar --header y --no-header al mismo tiempo.");
        }

        int formatFlags = (useCsv ? 1 : 0) + (useTsv ? 1 : 0) + (delimiterText is null ? 0 : 1);
        if (formatFlags > 1) {
            throw new Exception("Solo puede indicar uno de estos: --csv, --tsv o --delimiter.");
        }

        char delimiter = ',';
        InputFormat format = InputFormat.Unspecified;

        if (useCsv) {
            delimiter = ',';
            format = InputFormat.Csv;
        } else if (useTsv) {
            delimiter = '\t';
            format = InputFormat.Tsv;
        } else if (delimiterText is not null) {
            delimiter = ParseDelimiter(delimiterText);
            format = InputFormat.Custom;
        }

        if (format == InputFormat.Unspecified && inputPath is null) {
            throw new Exception("Debe indicar el formato con --csv, --tsv o --delimiter, o usar un archivo con extensión .csv o .tsv.");
        }

        return new AppOptions(
            inputPath,
            outputPath,
            delimiter,
            format,
            !noHeaderRequested,
            ignoreCase,
            sortSpecs.ToList());
    }

    static AppOptions ResolveDelimiter(AppOptions options) {
        if (options.Format != InputFormat.Unspecified || string.IsNullOrWhiteSpace(options.InputPath)) {
            return options;
        }

        return InferFormatFromExtension(options.InputPath) switch {
            InputFormat.Csv => options with { Delimiter = ',', Format = InputFormat.Csv },
            InputFormat.Tsv => options with { Delimiter = '\t', Format = InputFormat.Tsv },
            _ => throw new Exception($"No se pudo inferir el formato desde la extensión '{Path.GetExtension(options.InputPath)}'. Use --csv, --tsv o --delimiter.")
        };
    }

    static InputFormat InferFormatFromExtension(string inputPath) {
        return Path.GetExtension(inputPath).ToLowerInvariant() switch {
            ".csv" => InputFormat.Csv,
            ".tsv" => InputFormat.Tsv,
            _ => InputFormat.Unspecified
        };
    }

    static char ParseDelimiter(string value) {
        if (value == @"\t") {
            return '\t';
        }

        if (value.Length != 1) {
            throw new Exception("El delimitador debe ser un solo carácter o \\t.");
        }

        return value[0];
    }

    static string ReadInput(AppOptions options) {
        if (!string.IsNullOrWhiteSpace(options.InputPath)) {
            return File.ReadAllText(options.InputPath);
        }

        if (Console.IsInputRedirected) {
            return Console.In.ReadToEnd();
        }

        throw new Exception("Debe indicar un archivo de entrada o usar stdin.");
    }

    static void WriteOutput(AppOptions options, string text) {
        if (!string.IsNullOrWhiteSpace(options.OutputPath)) {
            File.WriteAllText(options.OutputPath, text);
        } else {
            Console.Write(text);
        }
    }

    static List<string> SplitLines(string text) {
        return text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n')
            .Where(line => line is not null)
            .ToList();
    }

    static string[] SplitRow(string line, char delimiter) {
        return line.Split(delimiter);
    }

    static List<SortRule> BuildSortRules(List<string> specs, bool hasHeader, string[]? headerColumns) {
        var result = new List<SortRule>();

        foreach (var spec in specs) {
            result.Add(ParseSortRule(spec, hasHeader, headerColumns));
        }

        return result;
    }

    static SortRule ParseSortRule(string spec, bool hasHeader, string[]? headerColumns) {
        var parts = spec.Split(':');

        var field = parts[0].Trim();
        var type = parts.Length >= 2 ? ParseSortType(parts[1]) : SortType.Text;
        var direction = parts.Length >= 3 ? ParseDirection(parts[2]) : SortDirection.Asc;

        int columnIndex;

        if (field.StartsWith("#")) {
            if (!int.TryParse(field[1..], out int oneBased) || oneBased <= 0) {
                throw new Exception($"Índice inválido: {field}");
            }

            columnIndex = oneBased - 1;
        } else {
            if (!hasHeader) {
                throw new Exception($"No se puede usar nombre de columna sin encabezado: {field}");
            }

            if (headerColumns is null) {
                throw new Exception("No se pudo leer el encabezado.");
            }

            columnIndex = FindColumnIndex(headerColumns, field);

            if (columnIndex < 0) {
                throw new Exception($"No existe la columna: {field}");
            }
        }

        return new SortRule(columnIndex, type, direction);
    }

    static int FindColumnIndex(string[] headerColumns, string field) {
        for (int i = 0; i < headerColumns.Length; i++) {
            if (string.Equals(headerColumns[i].Trim(), field, StringComparison.OrdinalIgnoreCase)) {
                return i;
            }
        }

        return -1;
    }

    static SortType ParseSortType(string text) {
        return text.Trim().ToLowerInvariant() switch {
            "text" => SortType.Text,
            "number" => SortType.Number,
            "date" => SortType.Date,
            "datetime" => SortType.DateTime,
            _ => throw new Exception($"Tipo inválido: {text}")
        };
    }

    static SortDirection ParseDirection(string text) {
        return text.Trim().ToLowerInvariant() switch {
            "asc" => SortDirection.Asc,
            "desc" => SortDirection.Desc,
            _ => throw new Exception($"Dirección inválida: {text}")
        };
    }

    static bool TryCreateRow(
        string originalLine,
        string[] columns,
        int originalIndex,
        List<SortRule> rules,
        AppOptions options,
        out DataRow? row) {
        var keyValues = new object[rules.Count];

        for (int i = 0; i < rules.Count; i++) {
            var rule = rules[i];

            if (rule.ColumnIndex < 0 || rule.ColumnIndex >= columns.Length) {
                row = null;
                return false;
            }

            var raw = columns[rule.ColumnIndex];

            if (!TryConvertValue(raw, rule.Type, options, out var value)) {
                row = null;
                return false;
            }

            keyValues[i] = value!;
        }

        row = new DataRow(originalLine, columns, keyValues, originalIndex);
        return true;
    }

    static bool TryConvertValue(string raw, SortType type, AppOptions options, out object? value) {
        if (options.IgnoreCase) {
            raw = raw.ToLowerInvariant();
        }

        switch (type) {
            case SortType.Text:
                value = raw;
                return true;

            case SortType.Number:
                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var number)) {
                    value = number;
                    return true;
                }

                if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out number)) {
                    value = number;
                    return true;
                }

                value = null;
                return false;

            case SortType.Date:
                if (DateTime.TryParse(raw, out var date)) {
                    value = date.Date;
                    return true;
                }

                value = null;
                return false;

            case SortType.DateTime:
                if (DateTime.TryParse(raw, out var dateTime)) {
                    value = dateTime;
                    return true;
                }

                value = null;
                return false;

            default:
                value = null;
                return false;
        }
    }

    static int CompareRows(DataRow a, DataRow b, List<SortRule> rules, AppOptions options) {
        for (int i = 0; i < rules.Count; i++) {
            var rule = rules[i];
            int result = CompareValues(a.KeyValues[i], b.KeyValues[i], rule.Type);

            if (result != 0) {
                return rule.Direction == SortDirection.Asc ? result : -result;
            }
        }

        return a.OriginalIndex.CompareTo(b.OriginalIndex);
    }

    static int CompareValues(object left, object right, SortType type) {
        return type switch {
            SortType.Text => string.Compare((string)left, (string)right, StringComparison.CurrentCulture),
            SortType.Number => ((decimal)left).CompareTo((decimal)right),
            SortType.Date => ((DateTime)left).CompareTo((DateTime)right),
            SortType.DateTime => ((DateTime)left).CompareTo((DateTime)right),
            _ => 0
        };
    }
}

record AppOptions(
    string? InputPath,
    string? OutputPath,
    char Delimiter,
    InputFormat Format,
    bool HasHeader,
    bool IgnoreCase,
    List<string> SortSpecs
);

record SortRule(
    int ColumnIndex,
    SortType Type,
    SortDirection Direction
);

record DataRow(
    string OriginalLine,
    string[] Columns,
    object[] KeyValues,
    int OriginalIndex
);

enum SortType {
    Text,
    Number,
    Date,
    DateTime
}

enum SortDirection {
    Asc,
    Desc
}

enum InputFormat {
    Unspecified,
    Csv,
    Tsv,
    Custom
}
