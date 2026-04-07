namespace Tup26.AlumnosApp;

static class AlumnosManager {
    public static Alumnos CargarAlumnos(string rutaArchivo) {
        Alumnos alumnos = new(Array.Empty<Alumno>());
        string comisionActual = string.Empty;

        try {
            string[] lineas = File.ReadAllLines(rutaArchivo, Encoding.UTF8);

            foreach (string linea in lineas) {
                if (string.IsNullOrWhiteSpace(linea)) {
                    continue;
                }

                if (linea.StartsWith("## ")) {
                    comisionActual = linea.Substring(3).Trim();
                    continue;
                }

                if (linea.StartsWith("#") || linea.StartsWith("```") || linea.StartsWith("Legajo") || linea.StartsWith("------")) {
                    continue;
                }

                Alumno? alumno = ExtraerAlumnoFormatoMarkdown(linea, comisionActual);

                if (alumno != null) {
                    alumnos.Agregar(alumno);
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }

        return alumnos;
    }

    public static void Guardar(Alumnos alumnos, string rutaArchivo) {
        try {
            List<Alumno> alumnosOrdenados = new(alumnos);
            alumnosOrdenados.Sort(Alumno.Comparar);

            StringBuilder sb = new();
            sb.AppendLine("# TUP 2026 - Programación III");
            sb.AppendLine();

            string? comisionActual = null;

            foreach (Alumno alumno in alumnosOrdenados) {
                string comisionAlumno = ObtenerComision(alumno);

                if (comisionActual != comisionAlumno) {
                    if (comisionActual != null) {
                        sb.AppendLine("```");
                        sb.AppendLine();
                    }

                    comisionActual = comisionAlumno;
                    sb.AppendLine($"## {comisionActual}");
                    sb.AppendLine("```text");
                    sb.AppendLine("LegajX  Nombre y Apellido                Teléfono         Foto  GitHub                   Prácticos          Exámenes        ");
                    sb.AppendLine("------  -------------------------------  ---------------  ----  -----------------------  -----------------  -----------------");
                }

                sb.AppendLine(FormatearFila(alumno));
            }

            if (comisionActual != null) {
                sb.AppendLine("```");
            }

            File.WriteAllText(rutaArchivo, sb.ToString().TrimEnd() + Environment.NewLine, Encoding.UTF8);
            Console.WriteLine($"Alumnos guardados en: {rutaArchivo}");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
        }
    }

    public static void Listar(Alumnos alumnos, string titulo = "Listado de Alumnos") {
        if (alumnos == null || alumnos.Count == 0) {
            Console.WriteLine("No hay alumnos para mostrar.");
            return;
        }

        List<Alumno> alumnosOrdenados = new(alumnos);
        alumnosOrdenados.Sort(Alumno.Comparar);

        string encabezado = FormatearFilaTabla("Legajo", "Nombre y Apellido", "Telefono", "Foto", "GitHub", "Comision", "Practicos", "Examenes");
        string separador = new string('-', encabezado.Length);
        ConsoleColor colorAnterior = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(titulo.ToUpper());
        Console.ForegroundColor = colorAnterior;

        Console.WriteLine(separador);
        Console.WriteLine(encabezado);
        Console.WriteLine(separador);

        foreach (Alumno alumno in alumnosOrdenados) {
            Console.WriteLine(FormatearFilaTabla(
                alumno.Legajo.ToString(),
                ObtenerNombreApellido(alumno),
                FormatearTexto(alumno.Telefono),
                alumno.TieneFoto ? "Si" : "No",
                FormatearGitHub(alumno.GitHub),
                ObtenerComision(alumno),
                FormatearEstados(alumno.practicos),
                FormatearEstados(alumno.examenes)));
        }

        Console.WriteLine(separador);
        Console.WriteLine($"Total de alumnos: {alumnos.Count}");
        Console.WriteLine();
    }

    public static void CrearCarpetas(Alumnos alumnos) {
        string rutaBase = AppPaths.PracticosDirectory;
        Directory.CreateDirectory(rutaBase);

        foreach (Alumno alumno in alumnos) {
            string nombreCarpeta = alumno.CarpetaNombre;
            string rutaCarpeta = Path.Combine(rutaBase, nombreCarpeta);

            try {
                List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(rutaBase, alumno.Legajo);

                if (carpetasMismoLegajo.Count == 0) {
                    Directory.CreateDirectory(rutaCarpeta);
                    Console.WriteLine($"Carpeta creada: {rutaCarpeta}");
                    continue;
                }

                if (carpetasMismoLegajo.Count > 1) {
                    Console.WriteLine($"Hay múltiples carpetas para el legajo {alumno.Legajo}. Revisar manualmente antes de renombrar.");
                    continue;
                }

                string rutaCarpetaExistente = carpetasMismoLegajo[0];
                string nombreCarpetaExistente = Path.GetFileName(rutaCarpetaExistente);

                if (nombreCarpetaExistente == nombreCarpeta) {
                    Console.WriteLine($"Carpeta existente correcta: {rutaCarpetaExistente}");
                    continue;
                }

                Directory.Move(rutaCarpetaExistente, rutaCarpeta);
                Console.WriteLine($"Carpeta renombrada: {rutaCarpetaExistente} -> {rutaCarpeta}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error al crear la carpeta para {nombreCarpeta}: {ex.Message}");
            }
        }
    }

    public static void CopiarFotoPerfil(Alumnos alumnos, string rutaFotos) {
        string rutaBase = AppPaths.PracticosDirectory;

        if (!Directory.Exists(rutaBase)) {
            Console.WriteLine($"No existe la carpeta base de prácticos: {rutaBase}");
            return;
        }

        foreach (Alumno alumno in alumnos) {
            if (string.IsNullOrWhiteSpace(alumno.TelefonoId)) {
                continue;
            }

            string rutaFotoOrigen = Path.Combine(rutaFotos, alumno.TelefonoId, "foto.png");
            if (!File.Exists(rutaFotoOrigen)) {
                continue;
            }

            List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(rutaBase, alumno.Legajo);
            if (carpetasMismoLegajo.Count != 1) {
                continue;
            }

            string rutaCarpetaAlumno = carpetasMismoLegajo[0];
            if (!Directory.Exists(rutaCarpetaAlumno)) {
                continue;
            }

            string rutaFotoDestino = Path.Combine(rutaCarpetaAlumno, "foto.png");
            if (File.Exists(rutaFotoDestino)) {
                continue;
            }

            try {
                File.Copy(rutaFotoOrigen, rutaFotoDestino);
                Console.WriteLine($"Foto copiada: {rutaFotoOrigen} -> {rutaFotoDestino}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error al copiar la foto para {alumno.CarpetaNombre}: {ex.Message}");
            }
        }
    }

    public static void CopiarEnunciadoPracticos(Alumnos alumnos, string practico) {
        string nombrePractico = practico.Trim();
        string rutaOrigen = Path.Combine(AppPaths.EnunciadosDirectory, nombrePractico);
        string rutaBasePracticos = AppPaths.PracticosDirectory;
        string carpetaPractico = nombrePractico.ToLower();

        if (string.IsNullOrWhiteSpace(nombrePractico)) {
            Console.WriteLine("Debe indicar el nombre del práctico a copiar.");
            return;
        }

        if (!Directory.Exists(rutaOrigen)) {
            Console.WriteLine($"No existe la carpeta del enunciado: {rutaOrigen}");
            return;
        }

        if (!Directory.Exists(rutaBasePracticos)) {
            Console.WriteLine($"No existe la carpeta base de prácticos: {rutaBasePracticos}");
            return;
        }

        foreach (Alumno alumno in alumnos) {
            string rutaAlumno = Path.Combine(rutaBasePracticos, alumno.CarpetaNombre);

            if (!Directory.Exists(rutaAlumno)) {
                Console.WriteLine($"No existe la carpeta del alumno: {rutaAlumno}");
                continue;
            }

            string rutaDestino = Path.Combine(rutaAlumno, carpetaPractico);

            try {
                CopiarContenidoDirectorio(rutaOrigen, rutaDestino);
                Console.WriteLine($"Enunciado copiado: {rutaOrigen} -> {rutaDestino}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error al copiar el enunciado para {alumno.CarpetaNombre}: {ex.Message}");
            }
        }
    }

    public static void ActualizarDesdePerfiles(Alumnos alumnos, string rutaPerfiles) {
        Dictionary<int, Alumno> porLegajo = new();

        foreach (Alumno alumno in alumnos) {
            porLegajo[alumno.Legajo] = alumno;
        }

        foreach (string carpetaPerfil in Directory.GetDirectories(rutaPerfiles)) {
            string rutaPerfil = Path.Combine(carpetaPerfil, "perfil.md");
            if (!File.Exists(rutaPerfil)) {
                continue;
            }

            try {
                int legajo = 0;
                string gitHub = string.Empty;

                foreach (string linea in File.ReadAllLines(rutaPerfil, Encoding.UTF8)) {
                    string l = linea.Trim();

                    if (l.StartsWith("- Legajo:")) {
                        string valor = l.Substring("- Legajo:".Length).Trim();
                        int.TryParse(valor, out legajo);
                    }
                    else if (l.StartsWith("- Github:")) {
                        string valor = l.Substring("- Github:".Length).Trim();

                        if (!string.IsNullOrWhiteSpace(valor) &&
                            !valor.StartsWith("No", StringComparison.OrdinalIgnoreCase)) {
                            gitHub = valor;
                        }
                    }
                }

                if (legajo == 0 || !porLegajo.ContainsKey(legajo)) {
                    continue;
                }

                Alumno alumno = porLegajo[legajo];
                bool actualizado = false;

                if (!string.IsNullOrWhiteSpace(gitHub) && alumno.GitHub != gitHub) {
                    Console.WriteLine($"  GitHub actualizado {alumno.CarpetaNombre}: '{alumno.GitHub}' -> '{gitHub}'");
                    alumno.GitHub = gitHub;
                    actualizado = true;
                }

                if (!actualizado) {
                    Console.WriteLine($"  Sin cambios: {alumno.CarpetaNombre}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error al leer perfil {rutaPerfil}: {ex.Message}");
            }
        }
    }

    public static void GuardarJSON(Alumnos alumnos, string rutaArchivo) {
        try {
            StringBuilder sb = new();
            sb.AppendLine("[");

            for (int i = 0; i < alumnos.Count; i++) {
                Alumno alumno = alumnos[i];
                sb.AppendLine("  {");
                sb.AppendLine($"    \"legajo\": {alumno.Legajo},");
                sb.AppendLine($"    \"comision\": \"{EscaparJson(alumno.Comision)}\",");
                sb.AppendLine($"    \"nombre\": \"{EscaparJson(alumno.Nombre)}\",");
                sb.AppendLine($"    \"apellido\": \"{EscaparJson(alumno.Apellido)}\",");
                sb.AppendLine($"    \"telefono\": \"{EscaparJson(alumno.Telefono)}\",");
                sb.AppendLine($"    \"tieneFoto\": {alumno.TieneFoto.ToString().ToLowerInvariant()},");
                sb.AppendLine($"    \"gitHub\": \"{EscaparJson(alumno.GitHub)}\",");
                sb.AppendLine($"    \"practicos\": [ {string.Join(", ", alumno.practicos.Select(p => string.Concat(((char)34).ToString(), p.ToEmoji(), ((char)34).ToString())))} ],");
                sb.AppendLine($"    \"examenes\": [ {string.Join(", ", alumno.examenes.Select(e => string.Concat(((char)34).ToString(), e.ToEmoji(), ((char)34).ToString())))} ]");
                sb.Append("  }");

                if (i < alumnos.Count - 1) {
                    sb.Append(',');
                }

                sb.AppendLine();
            }

            sb.AppendLine("]");
            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"Alumnos guardados en JSON: {rutaArchivo}");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo JSON: {ex.Message}");
        }
    }

    public static void GuardarVCard(Alumnos alumnos, string rutaArchivo) {
        try {
            List<Alumno> alumnosConTelefono = alumnos
                .Where(alumno => !string.IsNullOrWhiteSpace(alumno.TelefonoId))
                .OrderBy(ObtenerComision)
                .ThenBy(alumno => FormatearTexto(alumno.Apellido), StringComparer.OrdinalIgnoreCase)
                .ThenBy(alumno => FormatearTexto(alumno.Nombre), StringComparer.OrdinalIgnoreCase)
                .ThenBy(alumno => alumno.Legajo)
                .ToList();

            StringBuilder sb = new();

            foreach (Alumno alumno in alumnosConTelefono) {
                AppendVCardContacto(sb, alumno);
            }

            File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine($"Contactos vCard guardados en: {rutaArchivo}");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo vCard: {ex.Message}");
        }
    }

    static Alumno? ExtraerAlumnoFormatoMarkdown(string linea, string comisionActual) {
        List<string> columnas = Regex.Split(linea.TrimEnd(), @"\s{2,}").ToList();

        while (columnas.Count < 7) {
            columnas.Add(string.Empty);
        }

        if (!int.TryParse(columnas[0].Trim(), out int legajo)) {
            return null;
        }

        (string apellido, string nombre) = ExtraerApellidoNombre(columnas[1]);

        return new Alumno {
            Legajo = legajo,
            Comision = LimpiarCampo(comisionActual),
            Nombre = nombre,
            Apellido = apellido,
            Telefono = ExtraerTelefono(columnas[2]),
            TieneFoto = ExtraerFoto(columnas[3]),
            GitHub = ExtraerGitHub(columnas[4]),
            practicos = ExtraerPracticos(columnas[5]),
            examenes = ExtraerExamenes(columnas[6])
        };
    }

    static string FormatearFilaTabla(string legajo, string nombreApellido, string telefono, string foto, string gitHub, string comision, string pruebas, string examenes) {
        string colLegajo = AjustarColumna(legajo, 6);
        string colNombreApellido = AjustarColumna(nombreApellido, 26);
        string colTelefono = AjustarColumna(telefono, 15);
        string colFoto = AjustarColumna(foto, 4);
        string colGitHub = AjustarColumna(gitHub, 25);
        string colComision = AjustarColumna(comision, 10);
        string colPruebas = AjustarColumna(pruebas, 20);
        string colExamenes = AjustarColumna(examenes, 20);

        return $"{colLegajo}  {colNombreApellido}  {colTelefono}  {colFoto}  {colGitHub}  {colComision}  {colPruebas}  {colExamenes}";
    }

    static string ObtenerComision(Alumno alumno) {
        return FormatearTexto(alumno.Comision);
    }

    static string FormatearFila(Alumno alumno) {
        string legajo = AjustarColumna(alumno.Legajo.ToString(), 6);
        string nombreApellido = AjustarColumna(ObtenerNombreApellido(alumno), 31);
        string telefono = AjustarColumna(FormatearTexto(alumno.Telefono), 15);
        string foto = AjustarColumna(alumno.TieneFoto ? "Si" : "No", 4);
        string gitHub = AjustarColumna(FormatearGitHub(alumno.GitHub), 23);
        string pruebas = AjustarColumna(FormatearEstados(alumno.practicos, 10));
        string examenes = AjustarColumna(FormatearEstados(alumno.examenes, 10));

        return $"{legajo}  {nombreApellido}  {telefono}  {foto}  {gitHub}  {pruebas}  {examenes}";
    }

    static string ObtenerNombreApellido(Alumno alumno) {
        string apellido = FormatearTexto(alumno.Apellido);
        string nombre = FormatearTexto(alumno.Nombre);

        if (apellido == "—") {
            return nombre;
        }

        if (nombre == "—") {
            return apellido;
        }

        return $"{apellido}, {nombre}";
    }

    static string AjustarColumna(string texto, int ancho = 20) {
        string valor = FormatearTexto(texto);

        if (valor.Length > ancho) {
            return valor.Substring(0, ancho);
        }

        return valor.PadRight(ancho);
    }

    static string FormatearTexto(string texto) {
        if (string.IsNullOrWhiteSpace(texto)) {
            return "—";
        }

        return texto.Trim();
    }

    static string LimpiarCampo(string texto) {
        string valor = texto.Trim();
        if (valor == "—") {
            return string.Empty;
        }

        return valor;
    }

    static string ExtraerTelefono(string texto) {
        return LimpiarCampo(texto);
    }

    static (string, string) ExtraerApellidoNombre(string nombreCompleto) {
        string apellido;
        string nombre;
        string[] partes = nombreCompleto.Split(',', 2);

        if (partes.Length == 2) {
            apellido = LimpiarCampo(partes[0]);
            nombre = LimpiarCampo(partes[1]);
        }
        else {
            apellido = LimpiarCampo(nombreCompleto);
            nombre = string.Empty;
        }

        return (apellido, nombre);
    }

    static string ExtraerGitHub(string texto) {
        string valor = LimpiarCampo(texto);
        if (string.Equals(valor, "No", StringComparison.OrdinalIgnoreCase)) {
            return string.Empty;
        }

        return valor;
    }

    static string FormatearGitHub(string gitHub) {
        if (string.IsNullOrWhiteSpace(gitHub)) {
            return "-";
        }

        return gitHub.Trim();
    }

    static string FormatearEstados(List<Estado> estados, int maxEstados = 20) {
        string valor = string.Empty;

        if (estados != null && estados.Count > 0) {
            valor = string.Join(string.Empty, estados.Select(e => e.ToEmoji()));
        }

        valor = valor.Replace(" ", "⚪️");

        while (StringInfo.ParseCombiningCharacters(valor).Length < maxEstados) {
            valor += "⚪️";
        }

        return valor;
    }

    static List<Estado> ParsearEstados(string texto) {
        string valor = texto.Trim();

        if (string.IsNullOrWhiteSpace(valor) || valor == "-" || valor == "—") {
            return new();
        }

        List<Estado> estados = new();
        TextElementEnumerator enumerador = StringInfo.GetTextElementEnumerator(valor);

        while (enumerador.MoveNext()) {
            string elemento = enumerador.GetTextElement();
            Estado estado = EstadoExtensions.Parse(elemento);

            if (estado != Estado.Vacio) {
                estados.Add(estado);
            }
        }

        return estados;
    }

    static bool ExtraerFoto(string texto) {
        return string.Equals(texto.Trim(), "Si", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(texto.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    static List<Estado> ExtraerPracticos(string pruebas) {
        return ParsearEstados(pruebas);
    }

    static List<Estado> ExtraerExamenes(string examenes) {
        return ParsearEstados(examenes);
    }

    static bool TieneMismoLegajo(string nombreCarpeta, int legajo) {
        return nombreCarpeta.StartsWith($"{legajo} - ") || nombreCarpeta.StartsWith($"{legajo}_");
    }

    static List<string> BuscarCarpetasMismoLegajo(string rutaBase, int legajo) {
        List<string> carpetasMismoLegajo = new();

        if (!Directory.Exists(rutaBase)) {
            return carpetasMismoLegajo;
        }

        foreach (string carpetaExistente in Directory.GetDirectories(rutaBase)) {
            string nombreCarpetaExistente = Path.GetFileName(carpetaExistente);

            if (TieneMismoLegajo(nombreCarpetaExistente, legajo)) {
                carpetasMismoLegajo.Add(carpetaExistente);
            }
        }

        return carpetasMismoLegajo;
    }

    static void CopiarContenidoDirectorio(string rutaOrigen, string rutaDestino) {
        Directory.CreateDirectory(rutaDestino);

        foreach (string archivoOrigen in Directory.GetFiles(rutaOrigen)) {
            string nombreArchivo = Path.GetFileName(archivoOrigen);
            string archivoDestino = Path.Combine(rutaDestino, nombreArchivo);
            File.Copy(archivoOrigen, archivoDestino, overwrite: true);
        }

        foreach (string subdirectorioOrigen in Directory.GetDirectories(rutaOrigen)) {
            string nombreSubdirectorio = Path.GetFileName(subdirectorioOrigen);
            string subdirectorioDestino = Path.Combine(rutaDestino, nombreSubdirectorio);
            CopiarContenidoDirectorio(subdirectorioOrigen, subdirectorioDestino);
        }
    }

    static void AppendVCardContacto(StringBuilder sb, Alumno alumno) {
        string apellido = FormatearTextoVcard(alumno.Apellido);
        string nombre = FormatearTextoVcard(alumno.Nombre);
        string nombreCompleto = FormatearTextoVcard(ObtenerNombreVisible(alumno));
        string comision = FormatearTextoVcard(ObtenerComision(alumno));
        string etiquetaBusqueda = FormatearTextoVcard(ObtenerEtiquetaBusqueda(alumno));
        string etiquetaVisible = FormatearTextoVcard(ObtenerEtiquetaVisible(alumno));
        string telefonoE164 = $"+{alumno.TelefonoId}";

        sb.AppendLine("BEGIN:VCARD");
        sb.AppendLine("VERSION:3.0");
        sb.AppendLine($"N:{apellido};{nombre};;;");
        sb.AppendLine($"FN:{nombreCompleto} | {etiquetaVisible}");
        sb.AppendLine($"NICKNAME:{etiquetaVisible}");
        sb.AppendLine("ORG:TUP 2026 - Programacion III");
        sb.AppendLine($"CATEGORIES:{etiquetaBusqueda}");
        sb.AppendLine($"NOTE:Legajo {alumno.Legajo} | Comision {comision} | Etiqueta {etiquetaBusqueda} | GitHub {ObtenerGitHubVisible(alumno)}");
        sb.AppendLine($"X-TUP-LEGAJO:{alumno.Legajo}");
        sb.AppendLine($"X-TUP-COMISION:{comision}");
        sb.AppendLine($"TEL;TYPE=CELL;TYPE=VOICE:{telefonoE164}");
        sb.AppendLine("END:VCARD");
    }

    static string FormatearTextoVcard(string texto) {
        string valor = FormatearTexto(texto);
        return valor
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");
    }

    static string ObtenerNombreVisible(Alumno alumno) {
        string apellido = FormatearTexto(alumno.Apellido);
        string nombre = FormatearTexto(alumno.Nombre);

        if (apellido == "—") {
            return nombre;
        }

        if (nombre == "—") {
            return apellido;
        }

        return $"{nombre} {apellido}";
    }

    static string ObtenerGitHubVisible(Alumno alumno) {
        string gitHub = FormatearGitHub(alumno.GitHub);
        return gitHub == "-" ? "sin GitHub" : gitHub;
    }

    static string ObtenerEtiquetaBusqueda(Alumno alumno) {
        return $"TUP26-P3-{ObtenerComision(alumno)}";
    }

    static string ObtenerEtiquetaVisible(Alumno alumno) {
        return $"{ObtenerEtiquetaBusqueda(alumno)}-{alumno.Legajo}";
    }

    static string EscaparJson(string texto) {
        return texto
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }
}
