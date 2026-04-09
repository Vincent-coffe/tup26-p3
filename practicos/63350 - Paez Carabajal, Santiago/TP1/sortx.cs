//  PUNTO DE ENTRADA 

try
{
    Console.WriteLine("Paso 1: ParseArgs");
    var config = ParseArgs(args);

    Console.WriteLine("Paso 2: ReadInput");
    var text = ReadInput(config.InputFile);

    Console.WriteLine("Paso 3: ParseDelimited");
    var (headers, rows) = ParseDelimited(text, config);

    Console.WriteLine("Paso 4: SortRows");
    var sorted = SortRows(rows, headers, config);

    Console.WriteLine("Paso 5: Serialize");
    var output = Serialize(headers, sorted, config);

    Console.WriteLine("Paso 6: WriteOutput");
    WriteOutput(config.OutputFile, output);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
}

// parseo de argumentos y configuración

AppConfig ParseArgs(string[] args)
{
    string? input = null;
    string? output = null;
    string delimiter = ",";
    bool noHeader = false;
    var fields = new List<SortField>();
    var positional = new List<string>();

    int i = 0;
    while (i < args.Length)
    {
        var arg = args[i];

        switch (arg)
        {
            case "-i":
            case "--input":
                input = args[++i];
                i++;
                break;

            case "-o":
            case "--output":
                output = args[++i];
                i++;
                break;

            case "-d":
            case "--delimiter":
                delimiter = args[++i] == "\\t" ? "\t" : args[i];
                i++;
                break;

            case "-nh":
            case "--no-header":
                noHeader = true;
                i++;
                break;

            case "-b":
            case "--by":
                fields.Add(ParseSortField(args[++i]));
                i++;
                break;

            case "-h":
            case "--help":
                PrintHelp();
                Environment.Exit(0);
                break;

            default:
                if (arg.StartsWith("-"))
                    throw new Exception($"Opción inválida: {arg}");
                positional.Add(arg);
                i++;
                break;
        }
    }

    if (positional.Count > 0 && input == null) input = positional[0];
    if (positional.Count > 1 && output == null) output = positional[1];

    if (fields.Count == 0)
        throw new Exception("Debes usar -b");

    return new AppConfig(input, output, delimiter, noHeader, fields);
}

SortField ParseSortField(string text)
{
    var parts = text.Split(':');

    string name = parts[0];
    bool numeric = parts.Length > 1 && parts[1] == "num";
    bool desc = parts.Length > 2 && parts[2] == "desc";

    return new SortField(name, numeric, desc);
}

 // lectura del archivo de entrada

string ReadInput(string? file)
{
    if (file == null)
        return Console.In.ReadToEnd();

    if (!File.Exists(file))
        throw new Exception("Archivo no encontrado");

    return File.ReadAllText(file);
}

//  PASO 3 

(List<string> headers, List<string[]> rows) ParseDelimited(string text, AppConfig config)
{
    var lines = text.Split('\n')
                    .Where(l => l.Trim() != "")
                    .ToList();

    List<string> headers;
    int start = 0;

    if (!config.NoHeader)
    {
        headers = lines[0].Split(config.Delimiter).Select(x => x.Trim()).ToList();
        start = 1;
    }
    else
    {
        int cols = lines[0].Split(config.Delimiter).Length;
        headers = Enumerable.Range(0, cols).Select(x => x.ToString()).ToList();
    }

    var rows = new List<string[]>();

    for (int i = start; i < lines.Count; i++)
        rows.Add(lines[i].Split(config.Delimiter));

    return (headers, rows);
}

//  PASO 4 ordenamiento de filas

List<string[]> SortRows(List<string[]> rows, List<string> headers, AppConfig config)
{
    int GetIndex(string name)
    {
        int idx = headers.IndexOf(name);
        if (idx == -1)
            throw new Exception($"Columna no encontrada: {name}");
        return idx;
    }

    var result = rows;

    foreach (var field in config.SortFields.AsEnumerable().Reverse())
    {
        int index = GetIndex(field.Name);

        if (field.Numeric)
        {
            result = field.Descending
                ? result.OrderByDescending(r => double.TryParse(r[index], out var n) ? n : 0).ToList()
                : result.OrderBy(r => double.TryParse(r[index], out var n) ? n : 0).ToList();
        }
        else
        {
            result = field.Descending
                ? result.OrderByDescending(r => r[index]).ToList()
                : result.OrderBy(r => r[index]).ToList();
        }
    }

    return result;
}

//  PASO 5 serialización de datos

string Serialize(List<string> headers, List<string[]> rows, AppConfig config)
{
    var result = "";

    if (!config.NoHeader)
        result += string.Join(config.Delimiter, headers) + "\n";

    foreach (var r in rows)
        result += string.Join(config.Delimiter, r) + "\n";

    return result;
}

//  PASO 6 salida a archivo o consola

void WriteOutput(string? file, string content)
{
    if (file == null)
        Console.WriteLine(content);
    else
        File.WriteAllText(file, content);
}

//  HELP 

void PrintHelp()
{
    Console.WriteLine("Uso: sortx [input] -b campo[:tipo[:orden]]");
}

//  MODELOS 

record SortField(string Name, bool Numeric, bool Descending);

record AppConfig(
    string? InputFile,
    string? OutputFile,
    string Delimiter,
    bool NoHeader,
    List<SortField> SortFields
);