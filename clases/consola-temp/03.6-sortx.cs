using System.Globalization;

var configuracion = LeerConfiguracion(args);
var texto = LeerEntrada(configuracion.InputPath);
var csv = ParsearCsv(texto);
Ordenar(csv, configuracion.ColumnasOrden);
var salida = GenerarCsv(csv);
EscribirSalida(configuracion.OutputPath, salida);

Configuracion LeerConfiguracion(string[] args) {
    string? inputPath = null;
    string? outputPath = null;
    var columnasOrden = new List<string>();

    for (var i = 0; i < args.Length; i++) {
        var arg = args[i];

        if (arg == "--help" || arg == "-h") {
            MostrarAyuda();
            Environment.Exit(0);
        }

        if (arg.StartsWith("--by=")) {
            AgregarColumnasOrden(columnasOrden, arg[5..]);
            continue;
        }

        if (arg == "--by") {
            if (i + 1 >= args.Length) {
                throw new Exception("Falta el valor de --by.");
            }

            i++;
            AgregarColumnasOrden(columnasOrden, args[i]);
            continue;
        }

        if (arg.StartsWith("-")) {
            throw new Exception($"Opción no reconocida: {arg}");
        }

        if (inputPath is null) {
            inputPath = arg;
            continue;
        }

        if (outputPath is null) {
            outputPath = arg;
            continue;
        }

        throw new Exception("Solo se admite un archivo de entrada y uno de salida.");
    }

    if (columnasOrden.Count == 0) {
        throw new Exception("Debe indicar al menos un criterio con --by=nombre|apellido|edad.");
    }

    return new Configuracion(inputPath, outputPath, columnasOrden);
}

void AgregarColumnasOrden(List<string> columnasOrden, string valor) {
    var partes = valor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    foreach (var parte in partes) {
        var columna = parte.Trim().ToLowerInvariant();

        if (columna is not "nombre" and not "apellido" and not "edad") {
            throw new Exception($"Columna de orden inválida: {parte}");
        }

        columnasOrden.Add(columna);
    }
}

string LeerEntrada(string? inputPath) {
    if (string.IsNullOrWhiteSpace(inputPath)) {
        return Console.In.ReadToEnd();
    }

    return File.ReadAllText(inputPath);
}

Csv ParsearCsv(string texto) {
    var lineas = texto
        .Replace("\r\n", "\n")
        .Replace('\r', '\n')
        .Split('\n', StringSplitOptions.RemoveEmptyEntries);

    if (lineas.Length == 0) {
        return new Csv(new List<string>(), new List<Dictionary<string, string>>());
    }

    var cabeceras = lineas[0]
        .Split(',', StringSplitOptions.None)
        .Select(c => c.Trim())
        .ToList();

    var filas = new List<Dictionary<string, string>>();

    for (var i = 1; i < lineas.Length; i++) {
        var lineasTexto = lineas[i].Trim();
        if (string.IsNullOrWhiteSpace(lineasTexto)) {
            continue;
        }

        var campos = lineasTexto.Split(',', StringSplitOptions.None);
        var fila = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var j = 0; j < cabeceras.Count; j++) {
            fila[cabeceras[j]] = j < campos.Length ? campos[j].Trim() : string.Empty;
        }

        filas.Add(fila);
    }

    return new Csv(cabeceras, filas);
}

void Ordenar(Csv csv, List<string> columnasOrden) {
    csv.Filas.Sort((a, b) => CompararFilas(a, b, columnasOrden));
}

int CompararFilas(Dictionary<string, string> a, Dictionary<string, string> b, List<string> columnasOrden) {
    foreach (var columna in columnasOrden) {
        var comparacion = CompararValor(ObtenerValor(a, columna), ObtenerValor(b, columna), columna);
        if (comparacion != 0) {
            return comparacion;
        }
    }

    return 0;
}

string ObtenerValor(Dictionary<string, string> fila, string columna) {
    return fila.TryGetValue(columna, out var valor) ? valor : string.Empty;
}

int CompararValor(string valorA, string valorB, string columna) {
    if (columna == "edad") {
        var tieneA = int.TryParse(valorA, out var numeroA);
        var tieneB = int.TryParse(valorB, out var numeroB);

        if (tieneA && tieneB) {
            return numeroA.CompareTo(numeroB);
        }
    }

    return string.Compare(valorA, valorB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
}

string GenerarCsv(Csv csv) {
    var lineas = new List<string> {
        string.Join(',', csv.Cabeceras)
    };

    foreach (var fila in csv.Filas) {
        var campos = csv.Cabeceras.Select(c => fila.TryGetValue(c, out var valor) ? valor : string.Empty);
        lineas.Add(string.Join(',', campos));
    }

    return string.Join(Environment.NewLine, lineas) + Environment.NewLine;
}

void EscribirSalida(string? outputPath, string texto) {
    if (string.IsNullOrWhiteSpace(outputPath)) {
        Console.Write(texto);
        return;
    }

    File.WriteAllText(outputPath, texto);
}

void MostrarAyuda() {
    Console.WriteLine("USO: dotnet 03.6-sortx.cs [input] [output] --by=nombre|apellido|edad");
    Console.WriteLine();
    Console.WriteLine("Lee un CSV, lo ordena por una o más columnas y escribe el resultado.");
    Console.WriteLine();
    Console.WriteLine("ARGUMENTOS:");
    Console.WriteLine("  input    archivo de entrada. Si no se indica, lee desde stdin.");
    Console.WriteLine("  output   archivo de salida. Si no se indica, escribe a stdout.");
    Console.WriteLine("  --by     columna de orden: nombre, apellido o edad. Se puede repetir o separar con comas.");
    Console.WriteLine();
    Console.WriteLine("EJEMPLOS:");
    Console.WriteLine("  dotnet 03.6-sortx.cs personas.csv --by=apellido");
    Console.WriteLine("  dotnet 03.6-sortx.cs personas.csv salida.csv --by=apellido --by=nombre");
    Console.WriteLine("  cat personas.csv | dotnet 03.6-sortx.cs --by=edad");
}

record Configuracion(string? InputPath, string? OutputPath, List<string> ColumnasOrden);
record Csv(List<string> Cabeceras, List<Dictionary<string, string>> Filas);
