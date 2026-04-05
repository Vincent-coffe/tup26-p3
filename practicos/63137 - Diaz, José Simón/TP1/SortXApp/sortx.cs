try
{
    AppConfig appConfig = ParseArgs(args);

    string rawInputText = ReadInput(appConfig.InputFile);

    var parsedData = ParseDelimited(rawInputText, appConfig.Delimiter, appConfig.NoHeader);

    Console.WriteLine("Archivo parseado correctamente");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Error: {exception.Message}");
    Environment.Exit(1);
}

AppConfig ParseArgs(string[] args)
{
    bool helpRequested = args.Any(argument => IsFlag(argument, Flags.Help));

    if (args.Length == 0 || helpRequested)
    {
        PrintHelp();
        Environment.Exit(0);
    }

    string? inputFilePath = null;
    string? outputFilePath = null;
    string delimiter = Defaults.DefaultDelimiter;
    bool noHeader = false;
    List<SortField> sortFields = new();

    List<string> positionalArguments = args
        .TakeWhile(argument => !argument.StartsWith("-"))
        .ToList();

    if (positionalArguments.Count >= 1) inputFilePath = positionalArguments[0];
    if (positionalArguments.Count >= 2) outputFilePath = positionalArguments[1];

    for (int currentIndex = positionalArguments.Count; currentIndex < args.Length; currentIndex++)
    {
        string currentArgument = args[currentIndex];

        switch (currentArgument)
        {
            case var _ when IsFlag(currentArgument, Flags.By):
                sortFields.Add(ParseSortField(RequireNextArg(args, currentIndex, currentArgument)));
                currentIndex++;
                break;

            case var _ when IsFlag(currentArgument, Flags.Input):
                inputFilePath = RequireNextArg(args, currentIndex, currentArgument);
                currentIndex++;
                break;

            case var _ when IsFlag(currentArgument, Flags.Output):
                outputFilePath = RequireNextArg(args, currentIndex, currentArgument);
                currentIndex++;
                break;

            case var _ when IsFlag(currentArgument, Flags.Delimiter):
                string rawDelimiter = RequireNextArg(args, currentIndex, currentArgument);
                delimiter = rawDelimiter == Escapes.Tab ? "\t" : rawDelimiter;
                currentIndex++;
                break;

            case var _ when IsFlag(currentArgument, Flags.NoHeader):
                noHeader = true;
                break;

            default:
                throw new ArgumentException($"Opción desconocida: '{currentArgument}'");
        }
    }

    return new AppConfig(inputFilePath, outputFilePath, delimiter, noHeader, sortFields);
}

string ReadInput(string? inputFilePath)
{
    if (inputFilePath is null)
        return Console.In.ReadToEnd();

    if (!File.Exists(inputFilePath))
        throw new FileNotFoundException($"El archivo de entrada no existe: '{inputFilePath}'");

    return File.ReadAllText(inputFilePath);
}

(List<Dictionary<string, string>> Rows, List<string> Headers) ParseDelimited(
    string rawText,
    string delimiter,
    bool noHeader)
{
    List<string[]> allLines = rawText
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.TrimEnd('\r').Split(delimiter))
        .ToList();

    if (allLines.Count == 0)
        throw new InvalidDataException("El archivo está vacío.");

    List<string> headers = noHeader
        ? Enumerable.Range(0, allLines[0].Length).Select(i => i.ToString()).ToList()
        : allLines[0].ToList();

    var dataLines = noHeader ? allLines : allLines.Skip(1);

    var rows = dataLines
        .Select(fields => MapFieldsToHeaders(fields, headers))
        .ToList();

    return (rows, headers);
}

Dictionary<string, string> MapFieldsToHeaders(string[] fields, List<string> headers)
{
    var row = new Dictionary<string, string>();

    for (int i = 0; i < headers.Count; i++)
        row[headers[i]] = i < fields.Length ? fields[i].Trim() : string.Empty;

    return row;
}