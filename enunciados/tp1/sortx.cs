#!/usr/bin/env dotnet

using System.Globalization;

return MainProgram.Run(args);

static class MainProgram {
    public static int Run(string[] args) {
        try {
            var options = ParseArguments(args);
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
        } catch (Exception ex) {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    static Options ParseArguments(string[] args) {
        string? inputPath = null;
        string? outputPath = null;
        char delimiter = ',';
        InputFormat format = InputFormat.Unspecified;
        bool hasHeader = true;
        bool ignoreCase = false;
        var sortSpecs = new List<string>();

        for (int i = 0; i < args.Length; i++) {
            var arg = args[i];

            switch (arg) {
                case "--help":
                case "-h":
                    ShowHelpAndExit();

                    break;

                case "--csv":
                    EnsureFormatNotSet(format);
                    delimiter = ',';
                    format = InputFormat.Csv;
                    break;

                case "--tsv":
                    EnsureFormatNotSet(format);
                    delimiter = '\t';
                    format = InputFormat.Tsv;
                    break;

                case "--delimiter":
                case "-d":
                    EnsureFormatNotSet(format);
                    i++;
                    if (i >= args.Length) {
                        throw new Exception("Falta el valor de --delimiter.");
                    }
                    delimiter = ParseDelimiter(args[i]);
                    format = InputFormat.Custom;
                    break;

                case "--header":
                    hasHeader = true;
                    break;

                case "--no-header":
                    hasHeader = false;
                    break;

                case "--ignore-case":
                    ignoreCase = true;
                    break;

                case "--by":
                    i++;
                    if (i >= args.Length) {
                        throw new Exception("Falta el valor de --by.");
                    }
                    sortSpecs.Add(args[i]);
                    break;

                case "-o":
                case "--output":
                    i++;
                    if (i >= args.Length) {
                        throw new Exception("Falta el valor de --output.");
                    }
                    outputPath = args[i];
                    break;

                default:
                    if (arg.StartsWith("-")) {
                        throw new Exception($"Opción no reconocida: {arg}");
                    }

                    if (inputPath is not null) {
                        throw new Exception("Solo se admite un archivo de entrada.");
                    }

                    inputPath = arg;
                    break;
            }
        }

        if (sortSpecs.Count == 0) {
            throw new Exception("Debe indicar al menos un criterio de orden con --by.");
        }

        if (format == InputFormat.Unspecified && inputPath is null) {
            throw new Exception("Debe indicar el formato con --csv, --tsv o --delimiter, o usar un archivo con extensión .csv o .tsv.");
        }

        return new Options(inputPath, outputPath, delimiter, format, hasHeader, ignoreCase, sortSpecs);
    }

    static void EnsureFormatNotSet(InputFormat format) {
        if (format != InputFormat.Unspecified) {
            throw new Exception("Solo puede indicar uno de estos: --csv, --tsv o --delimiter.");
        }
    }

    static Options ResolveDelimiter(Options options) {
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

    static string ReadInput(Options options) {
        if (!string.IsNullOrWhiteSpace(options.InputPath)) {
            return File.ReadAllText(options.InputPath);
        }

        if (Console.IsInputRedirected) {
            return Console.In.ReadToEnd();
        }

        throw new Exception("Debe indicar un archivo de entrada o usar stdin.");
    }

    static void WriteOutput(Options options, string text) {
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
        Options options,
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

    static bool TryConvertValue(string raw, SortType type, Options options, out object? value) {
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

    static int CompareRows(DataRow a, DataRow b, List<SortRule> rules, Options options) {
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

    static void ShowHelpAndExit() {
        Console.WriteLine("""
sortx - versión simple de referencia

USO:
  dotnet run sortx.cs -- [input] [options]

DESCRIPCIÓN:
    Ordena filas de un archivo delimitado por columnas.
    El archivo puede leerse desde una ruta o desde stdin.

COMPORTAMIENTO POR DEFECTO:
    - se asume que hay encabezado
    - si no se indica el formato, se intenta inferir por la extensión del archivo
        (.csv -> coma, .tsv -> tabulador)
    - la dirección de orden es ascendente
    - el tipo de orden es text

FORMATO:
    --csv                usa coma como separador
    --tsv                usa tabulador como separador
    --delimiter <char>   usa un separador explícito

ENCABEZADO:
    --header             interpreta la primera fila como encabezado (por defecto)
    --no-header          trata todas las filas como datos

ORDENAMIENTO:
    --by <campo[:tipo[:direccion]]>
    agrega un criterio de orden

        campo              nombre de columna o índice (#1, #2, ...)
        tipo               text | number | date | datetime
        direccion          asc | desc

    Se puede repetir --by para ordenar por más de una columna.

OTRAS OPCIONES:
    --ignore-case        ignora mayúsculas y minúsculas al comparar texto
    -o, --output <file>  escribe la salida en un archivo
    -h, --help           muestra esta ayuda

EJEMPLOS:
    dotnet run sortx.cs -- personas.csv --by apellido
    dotnet run sortx.cs -- ventas.tsv --by monto:number:desc
    dotnet run sortx.cs -- datos.txt --delimiter ";" --no-header --by #2:text
    cat personas.csv | dotnet run sortx.cs -- --csv --by apellido

SALIDA:
    Escribe el resultado ordenado en stdout, salvo que se indique --output.

ERRORES COMUNES:
    - falta al menos un criterio --by
    - se indica más de un formato (--csv, --tsv, --delimiter)
    - no se puede inferir el formato desde la extensión del archivo
""");
        Environment.Exit(0);
    }
}

record Options(
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