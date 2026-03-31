using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


class Alumno {
    public int legajo;
    public string comision = string.Empty;
    public string nombre = string.Empty;
    public string apellido = string.Empty;
    public string telefono = string.Empty;
    public bool tieneFoto;
    public string gitHub = string.Empty;

    public string CarpetaNombre => $"{legajo} - {apellido}, {nombre}";
    public string TelefonoId => FormatearTelefonoId(telefono);

    static string FormatearTelefonoId(string telefono) {
        string digitos = Regex.Replace(telefono.Trim(), @"\D", string.Empty);

        if (string.IsNullOrWhiteSpace(digitos)) {
            return string.Empty;
        }

        if (digitos.StartsWith("549")) {
            return digitos;
        }

        if (digitos.StartsWith("54")) {
            return $"549{digitos.Substring(2)}";
        }

        if (digitos.StartsWith("0")) {
            digitos = digitos.Substring(1);
        }

        return $"549{digitos}";
    }
}

class Alumnos: IEnumerable<Alumno> {
    public List<Alumno> Lista { get; set; } = new List<Alumno>();

    public int Count => Lista.Count;

    public Alumno this[int index] {
        get { return Lista[index]; }
    }

    public Alumnos(List<Alumno> alumnos) {
        Lista = alumnos ?? new List<Alumno>();
    }   

    public void Agregar(Alumno alumno) {
        Lista.Add(alumno);
    }

    public Alumnos FiltrarSinGithub() =>
        new Alumnos( Lista.Where(alumno => string.IsNullOrWhiteSpace(alumno.gitHub)).ToList());

    public Alumnos SinTelefono() =>
        new Alumnos( Lista.Where(alumno => string.IsNullOrWhiteSpace(alumno.TelefonoId)).ToList());

    public Alumnos SinFotos() =>
        new Alumnos( Lista.Where(alumno => !alumno.tieneFoto).ToList() );

    public Alumnos EnComision(string comision) =>
        new Alumnos( Lista.Where(alumno => string.Equals(alumno.comision, comision, StringComparison.OrdinalIgnoreCase)).ToList() );

    public Alumnos ParaAgregar() =>
        new Alumnos( Lista.Where(alumno => alumno.gitHub == "(agregar)").ToList() );

    public IEnumerator<Alumno> GetEnumerator() {
        return Lista.GetEnumerator();
    }  

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

class AlumnosManager {
    const int LongitudMaximaLineaVcard = 75;

    public static Alumnos CargarAlumnos(string rutaArchivo) {
        Alumnos alumnos = new Alumnos(new List<Alumno>());
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

                Alumno? alumno = null;

                if (linea.Contains("|")) {
                    alumno = ParsearAlumnoFormatoLegado(linea);
                } else {
                    alumno = ParsearAlumnoFormatoMarkdown(linea, comisionActual);
                }

                if (alumno != null) {
                    alumnos.Agregar(alumno);
                }
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }
        
        return alumnos;
    }

    static Alumno? ParsearAlumnoFormatoLegado(string linea) {
        string[] datos = linea.Split('|');
        if (datos.Length < 7) {
            return null;
        }

        if (!int.TryParse(datos[0].Trim(), out int legajo)) {
            return null;
        }

        return new Alumno {
            legajo    = legajo,
            comision  = LimpiarCampo(datos[1]),
            nombre    = LimpiarCampo(datos[2]),
            apellido  = LimpiarCampo(datos[3]),
            telefono  = ParsearTelefono(datos[4]),
            tieneFoto = ParsearFoto(datos[5]),
            gitHub    = ParsearGitHub(datos[6])
        };
    }

    static Alumno? ParsearAlumnoFormatoMarkdown(string linea, string comisionActual) {
        string[] columnas = Regex.Split(linea.TrimEnd(), @"\s{2,}");
        if (columnas.Length < 5) {
            return null;
        }

        if (!int.TryParse(columnas[0].Trim(), out int legajo)) {
            return null;
        }

        string apellido = string.Empty;
        string nombre = string.Empty;
        string nombreCompleto = LimpiarCampo(columnas[1]);
        int indiceComa = nombreCompleto.IndexOf(',');

        if (indiceComa >= 0) {
            apellido = LimpiarCampo(nombreCompleto.Substring(0, indiceComa));
            nombre = LimpiarCampo(nombreCompleto.Substring(indiceComa + 1));
        } else {
            apellido = LimpiarCampo(nombreCompleto);
        }

        return new Alumno {
            legajo    = legajo,
            comision  = LimpiarCampo(comisionActual),
            nombre    = nombre,
            apellido  = apellido,
            telefono  = ParsearTelefono(columnas[2]),
            tieneFoto = ParsearFoto(columnas[3]),
            gitHub    = ParsearGitHub(columnas[4])
        };
    }

    public static void Guardar(Alumnos alumnos, string rutaArchivo) {
        try {
            List<Alumno> alumnosOrdenados =  new List<Alumno>(alumnos);
            alumnosOrdenados.Sort(CompararAlumnos);

            StringBuilder sb = new StringBuilder();
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
                    sb.AppendLine("Legajo  Nombre y Apellido           Telefono         Foto  Github");
                    sb.AppendLine("------  --------------------------  ---------------  ----  --------------------");
                }

                sb.AppendLine(FormatearFila(alumno));
            }

            if (comisionActual != null) {
                sb.AppendLine("```");
            }
            
            File.WriteAllText(rutaArchivo, sb.ToString().TrimEnd() + Environment.NewLine, Encoding.UTF8);
            Console.WriteLine($"Alumnos guardados en: {rutaArchivo}");
        } catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
        }
    }

    public static void Listar(Alumnos alumnos, string titulo = "Listado de Alumnos") {
        if (alumnos == null || alumnos.Count == 0) {
            Console.WriteLine("No hay alumnos para mostrar.");
            return;
        }

        List<Alumno> alumnosOrdenados = new List<Alumno>(alumnos);
        alumnosOrdenados.Sort(CompararAlumnos);

        string encabezado = FormatearFilaTabla("Legajo", "Nombre y Apellido", "Telefono", "Foto", "GitHub", "Comision");
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
                alumno.legajo.ToString(),
                ObtenerNombreApellido(alumno),
                FormatearTexto(alumno.telefono),
                alumno.tieneFoto ? "Si" : "No",
                FormatearGitHub(alumno.gitHub),
                ObtenerComision(alumno)));
        }

        Console.WriteLine(separador);
        Console.WriteLine($"Total de alumnos: {alumnos.Count}");
        Console.WriteLine();
    }

    static string FormatearFilaTabla(string legajo, string nombreApellido, string telefono, string foto, string gitHub, string comision) {
        string colLegajo         = AjustarColumna(legajo, 6);
        string colNombreApellido = AjustarColumna(nombreApellido, 26);
        string colTelefono       = AjustarColumna(telefono, 15);
        string colFoto           = AjustarColumna(foto, 4);
        string colGitHub         = AjustarColumna(gitHub, 20);
        string colComision       = AjustarColumna(comision, 10);

        return $"{colLegajo}  {colNombreApellido}  {colTelefono}  {colFoto}  {colGitHub}  {colComision}";
    }

    
    static int CompararAlumnos(Alumno alumnoA, Alumno alumnoB) {
        int comparacion = string.Compare(ObtenerComision(alumnoA), ObtenerComision(alumnoB), StringComparison.OrdinalIgnoreCase);
        if (comparacion != 0) {
            return comparacion;
        }

        comparacion = string.Compare(FormatearTexto(alumnoA.apellido), FormatearTexto(alumnoB.apellido), StringComparison.OrdinalIgnoreCase);
        if (comparacion != 0) {
            return comparacion;
        }

        comparacion = string.Compare(FormatearTexto(alumnoA.nombre), FormatearTexto(alumnoB.nombre), StringComparison.OrdinalIgnoreCase);
        if (comparacion != 0) {
            return comparacion;
        }

        return alumnoA.legajo.CompareTo(alumnoB.legajo);
    }

    static string ObtenerComision(Alumno alumno) {
        return FormatearTexto(alumno.comision);
    }

    static string FormatearFila(Alumno alumno) {
        string legajo         = AjustarColumna(alumno.legajo.ToString(), 6);
        string nombreApellido = AjustarColumna(ObtenerNombreApellido(alumno), 26);
        string telefono       = AjustarColumna(FormatearTexto(alumno.telefono), 15);
        string foto           = AjustarColumna(alumno.tieneFoto ? "Si" : "No", 4);
        string gitHub         = AjustarColumna(FormatearGitHub(alumno.gitHub), 20);

        return $"{legajo}  {nombreApellido}  {telefono}  {foto}  {gitHub}";
    }

    static string ObtenerNombreApellido(Alumno alumno) {
        string apellido = FormatearTexto(alumno.apellido);
        string nombre   = FormatearTexto(alumno.nombre);

        if (apellido == "—") {
            return nombre;
        }

        if (nombre == "—") {
            return apellido;
        }

        return $"{apellido}, {nombre}";
    }

    static string AjustarColumna(string texto, int ancho) {
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

        if (valor == "—" || valor == "-") {
            return string.Empty;
        }

        return valor;
    }

    static string ParsearTelefono(string texto) {
        return LimpiarCampo(texto);
    }

    static string ParsearGitHub(string texto) {
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

    static bool ParsearFoto(string texto) {
        return string.Equals(texto.Trim(), "Si", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(texto.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    static bool TieneMismoLegajo(string nombreCarpeta, int legajo) {
        return nombreCarpeta.StartsWith($"{legajo} - ") || nombreCarpeta.StartsWith($"{legajo}_");
    }

    static List<string> BuscarCarpetasMismoLegajo(string rutaBase, int legajo) {
        List<string> carpetasMismoLegajo = new List<string>();

        foreach (string carpetaExistente in Directory.GetDirectories(rutaBase)) {
            string nombreCarpetaExistente = Path.GetFileName(carpetaExistente);

            if (TieneMismoLegajo(nombreCarpetaExistente, legajo)) {
                carpetasMismoLegajo.Add(carpetaExistente);
            }
        }

        return carpetasMismoLegajo;
    }

    public static void CrearCarpetas(Alumnos alumnos) {
        string rutaBase = "TP";
        Directory.CreateDirectory(rutaBase);

        foreach (Alumno alumno in alumnos) {
            string nombreCarpeta = alumno.CarpetaNombre;
            string rutaCarpeta = Path.Combine(rutaBase, nombreCarpeta);

            try {
                List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(rutaBase, alumno.legajo);

                if (carpetasMismoLegajo.Count == 0) {
                    Directory.CreateDirectory(rutaCarpeta);
                    Console.WriteLine($"Carpeta creada: {rutaCarpeta}");
                    continue;
                }

                if (carpetasMismoLegajo.Count > 1) {
                    Console.WriteLine($"Hay múltiples carpetas para el legajo {alumno.legajo}. Revisar manualmente antes de renombrar.");
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
            } catch (Exception ex) {
                Console.WriteLine($"Error al crear la carpeta para {nombreCarpeta}: {ex.Message}");
            }
        }
    }

    public static void CopiarFotoPerfil(Alumnos alumnos, string rutaFotos) {
        string rutaBase = "TP";

        foreach (Alumno alumno in alumnos) {
            if (string.IsNullOrWhiteSpace(alumno.TelefonoId)) {
                continue;
            }

            string rutaFotoOrigen = Path.Combine(rutaFotos, alumno.TelefonoId, "foto.png");
            if (!File.Exists(rutaFotoOrigen)) {
                continue;
            }

            List<string> carpetasMismoLegajo = BuscarCarpetasMismoLegajo(rutaBase, alumno.legajo);
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
            } catch (Exception ex) {
                Console.WriteLine($"Error al copiar la foto para {alumno.CarpetaNombre}: {ex.Message}");
            }
        }
    }

    public static void ActualizarDesdePerfiles(Alumnos alumnos, string rutaPerfiles) {
        Dictionary<int, Alumno> porLegajo = new Dictionary<int, Alumno>();

        foreach (Alumno alumno in alumnos) {
            porLegajo[alumno.legajo] = alumno;
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
                    } else if (l.StartsWith("- Github:")) {
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

                if (!string.IsNullOrWhiteSpace(gitHub) && alumno.gitHub != gitHub) {
                    Console.WriteLine($"  GitHub actualizado {alumno.CarpetaNombre}: '{alumno.gitHub}' -> '{gitHub}'");
                    alumno.gitHub = gitHub;
                    actualizado = true;
                }

                if (!actualizado) {
                    Console.WriteLine($"  Sin cambios: {alumno.CarpetaNombre}");
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error al leer perfil {rutaPerfil}: {ex.Message}");
            }
        }
    }

    public static void GuardarJSON(Alumnos alumnos, string rutaArchivo) {
        try {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");

            for (int i = 0; i < alumnos.Count; i++) {
                Alumno alumno = alumnos[i];
                sb.AppendLine("  {");
                sb.AppendLine($"    \"legajo\": {alumno.legajo},");
                sb.AppendLine($"    \"comision\": \"{EscaparJson(alumno.comision)}\",");
                sb.AppendLine($"    \"nombre\": \"{EscaparJson(alumno.nombre)}\",");
                sb.AppendLine($"    \"apellido\": \"{EscaparJson(alumno.apellido)}\",");
                sb.AppendLine($"    \"telefono\": \"{EscaparJson(alumno.telefono)}\",");
                sb.AppendLine($"    \"tieneFoto\": {alumno.tieneFoto.ToString().ToLowerInvariant()},");
                sb.AppendLine($"    \"gitHub\": \"{EscaparJson(alumno.gitHub)}\"");
                sb.Append("  }");

                if (i < alumnos.Count - 1) {
                    sb.Append(',');
                }

                sb.AppendLine();
            }

            sb.AppendLine("]");
            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"Alumnos guardados en JSON: {rutaArchivo}");
        } catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo JSON: {ex.Message}");
        }
    }

    public static void GuardarVCard(Alumnos alumnos, string rutaArchivo) {
        try {
            List<Alumno> alumnosConTelefono = alumnos
                .Where(alumno => !string.IsNullOrWhiteSpace(alumno.TelefonoId))
                .OrderBy(ObtenerComision)
                .ThenBy(alumno => FormatearTexto(alumno.apellido), StringComparer.OrdinalIgnoreCase)
                .ThenBy(alumno => FormatearTexto(alumno.nombre), StringComparer.OrdinalIgnoreCase)
                .ThenBy(alumno => alumno.legajo)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (Alumno alumno in alumnosConTelefono) {
                AppendVCardContacto(sb, alumno);
            }

            File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine($"Contactos vCard guardados en: {rutaArchivo}");
        } catch (Exception ex) {
            Console.WriteLine($"Error al guardar el archivo vCard: {ex.Message}");
        }
    }

    static void AppendVCardContacto(StringBuilder sb, Alumno alumno) {
        string apellido = FormatearTextoVcard(alumno.apellido);
        string nombre = FormatearTextoVcard(alumno.nombre);
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
        sb.AppendLine($"ORG:TUP 2026 - Programacion III");
        sb.AppendLine($"CATEGORIES:{etiquetaBusqueda}");
        sb.AppendLine($"NOTE:Legajo {alumno.legajo} | Comision {comision} | Etiqueta {etiquetaBusqueda} | GitHub {ObtenerGitHubVisible(alumno)}");
        sb.AppendLine($"X-TUP-LEGAJO:{alumno.legajo}");
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
        string apellido = FormatearTexto(alumno.apellido);
        string nombre = FormatearTexto(alumno.nombre);

        if (apellido == "—") {
            return nombre;
        }

        if (nombre == "—") {
            return apellido;
        }

        return $"{nombre} {apellido}";
    }

    static string ObtenerGitHubVisible(Alumno alumno) {
        string gitHub = FormatearGitHub(alumno.gitHub);
        return gitHub == "-" ? "sin GitHub" : gitHub;
    }

    static string ObtenerEtiquetaBusqueda(Alumno alumno) {
        return $"TUP26-P3-{ObtenerComision(alumno)}";
    }

    static string ObtenerEtiquetaVisible(Alumno alumno) {
        return $"{ObtenerEtiquetaBusqueda(alumno)}-{alumno.legajo}";
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


class WAppService {
    readonly string? store;
    readonly TimeSpan timeout;
    bool gruposHabilitados = true;
    List<GrupoWhatsApp> grupos = new List<GrupoWhatsApp>();

    public WAppService(string? store = null, TimeSpan? timeout = null, bool refrescarGrupos = true) {
        this.store = store;
        this.timeout = timeout ?? TimeSpan.FromMinutes(5);

        try {
            CargarGrupos(refrescarGrupos);
        } catch (InvalidOperationException ex) when (EsErrorAutenticacionWacli(ex)) {
            gruposHabilitados = false;
            grupos = new List<GrupoWhatsApp>();
            Console.WriteLine("Aviso: wacli no está autenticado; se omite la carga de grupos.");
        }
    }

    public void EnviarMensajeAGrupo(string grupo, string mensaje, bool json = false) {
        string grupoJid = ResolverJidGrupo(grupo);
        EnviarTexto(grupoJid, mensaje, json);
    }

    public void EnviarTexto(string destinatario, string mensaje, bool json = false) {
        if (string.IsNullOrWhiteSpace(destinatario)) {
            throw new ArgumentException("El destinatario no puede estar vacío.", nameof(destinatario));
        }

        if (string.IsNullOrWhiteSpace(mensaje)) {
            throw new ArgumentException("El mensaje no puede estar vacío.", nameof(mensaje));
        }

        List<string> argumentos = new List<string> { "send", "text", "--to", destinatario, "--message", mensaje };

        Ejecutar(json, argumentos);
    }

    public void InvitarParticipanteAGrupo(string grupo, params string[] usuarios) {
        InvitarParticipantesAGrupo(grupo, false, usuarios);
    }

    public void InvitarParticipantesAGrupo(string grupo, bool json, params string[] usuarios) {
        if (string.IsNullOrWhiteSpace(grupo)) {
            throw new ArgumentException("El grupo no puede estar vacío.", nameof(grupo));
        }

        if (usuarios is null || usuarios.Length == 0) {
            throw new ArgumentException("Debes indicar al menos un participante.", nameof(usuarios));
        }

        string grupoJid = ResolverJidGrupo(grupo);

        List<string> argumentos = new List<string> { "groups", "participants", "add", "--jid", grupoJid };

        foreach (string usuario in usuarios) {
            if (string.IsNullOrWhiteSpace(usuario)) { continue; }

            argumentos.Add("--user");
            argumentos.Add(usuario);
        }

        if (!argumentos.Any(argumento => argumento == "--user")) {
            throw new ArgumentException("Debes indicar al menos un participante válido.", nameof(usuarios));
        }

        Ejecutar(json, argumentos);
    }

    public void InvitarGrupoComision(IEnumerable<Alumno> alumnos, bool json = false) {
        if (alumnos is null) {
            throw new ArgumentNullException(nameof(alumnos));
        }

        List<Alumno> alumnosValidos = alumnos
            .Where(alumno =>
                !string.IsNullOrWhiteSpace(alumno.comision) &&
                !string.IsNullOrWhiteSpace(alumno.TelefonoId))
            .ToList();

        foreach (IGrouping<string, Alumno> grupoComision in alumnosValidos
            .GroupBy(alumno => ObtenerReferenciaGrupoPorComision(alumno.comision))
            .OrderBy(grupo => grupo.Key, StringComparer.OrdinalIgnoreCase)) {
            string[] usuarios = grupoComision
                .Select(alumno => alumno.TelefonoId)
                .Where(telefono => !string.IsNullOrWhiteSpace(telefono))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (usuarios.Length == 0) {
                continue;
            }

            Console.WriteLine($"Invitando {usuarios.Length} alumno(s) al grupo {grupoComision.Key}...");
            InvitarParticipantesAGrupo(grupoComision.Key, json, usuarios);
        }
    }

    public string? BuscarJidGrupoPorDescripcion(string descripcion, bool refrescar = false) {
        if (string.IsNullOrWhiteSpace(descripcion)) {
            throw new ArgumentException("La descripción no puede estar vacía.", nameof(descripcion));
        }

        GrupoWhatsApp? grupo = BuscarGrupoPorReferencia(descripcion, refrescar);
        return grupo?.Jid;
    }

    public void RecargarGrupos() {
        if (!gruposHabilitados) {
            Console.WriteLine("Aviso: no se pueden recargar grupos porque wacli no está autenticado.");
            return;
        }

        CargarGrupos(true);
    }

    public List<GrupoWhatsApp> ListarGrupos(bool refrescar = false) {
        if (!gruposHabilitados) {
            return new List<GrupoWhatsApp>();
        }

        if (refrescar || grupos.Count == 0) {
            CargarGrupos(refrescar);
        }

        return new List<GrupoWhatsApp>(grupos);
    }

    string ResolverJidGrupo(string grupo) {
        if (string.IsNullOrWhiteSpace(grupo)) {
            throw new ArgumentException("El grupo no puede estar vacío.", nameof(grupo));
        }

        string referencia = grupo.Trim();

        if (EsJidGrupo(referencia)) {
            return referencia;
        }

        GrupoWhatsApp? coincidencia = BuscarGrupoPorReferencia(referencia, false);
        if (coincidencia != null) {
            return coincidencia.Jid;
        }

        coincidencia = BuscarGrupoPorReferencia(referencia, true);
        if (coincidencia != null) {
            return coincidencia.Jid;
        }

        throw new InvalidOperationException($"No se encontró ningún grupo para '{referencia}'.");
    }

    GrupoWhatsApp? BuscarGrupoPorReferencia(string referencia, bool refrescar) {
        string textoBuscado = referencia.Trim();
        List<GrupoWhatsApp> gruposDisponibles = ListarGrupos(refrescar);

        List<GrupoWhatsApp> coincidenciasExactas = gruposDisponibles
            .Where(grupo =>
                string.Equals(grupo.Nombre, textoBuscado, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(grupo.Jid, textoBuscado, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (coincidenciasExactas.Count == 1) {
            return coincidenciasExactas[0];
        }

        if (coincidenciasExactas.Count > 1) {
            string nombres = string.Join(", ", coincidenciasExactas.Select(grupo => $"{grupo.Nombre} ({grupo.Jid})"));
            throw new InvalidOperationException($"La referencia exacta '{textoBuscado}' coincide con varios grupos: {nombres}");
        }

        List<GrupoWhatsApp> coincidenciasParciales = gruposDisponibles
            .Where(grupo =>
                grupo.Nombre.Contains(textoBuscado, StringComparison.OrdinalIgnoreCase) ||
                grupo.Jid.Contains(textoBuscado, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (coincidenciasParciales.Count == 1) {
            return coincidenciasParciales[0];
        }

        if (coincidenciasParciales.Count > 1) {
            string nombres = string.Join(", ", coincidenciasParciales.Select(grupo => $"{grupo.Nombre} ({grupo.Jid})"));
            throw new InvalidOperationException($"La búsqueda '{textoBuscado}' coincide con varios grupos: {nombres}");
        }

        return null;
    }

    void CargarGrupos(bool refrescar) {
        if (!gruposHabilitados) {
            grupos = new List<GrupoWhatsApp>();
            return;
        }

        if (refrescar) {
            Ejecutar(false, new List<string> { "groups", "refresh" });
        }

        string salida = EjecutarYObtenerSalida(false, new List<string> { "groups", "list" });
        grupos = ParsearGrupos(salida);
    }

    static bool EsJidGrupo(string texto) {
        return texto.EndsWith("@g.us", StringComparison.OrdinalIgnoreCase);
    }

    static string ObtenerReferenciaGrupoPorComision(string comision) {
        string valor = comision.Trim();

        if (string.IsNullOrWhiteSpace(valor)) {
            throw new ArgumentException("La comisión no puede estar vacía.", nameof(comision));
        }

        if (EsJidGrupo(valor)) {
            return valor;
        }

        if (valor.StartsWith("TUP26-P3-", StringComparison.OrdinalIgnoreCase)) {
            return valor;
        }

        return $"TUP26-P3-{valor}";
    }

    static List<GrupoWhatsApp> ParsearGrupos(string salida) {
        List<GrupoWhatsApp> grupos = new List<GrupoWhatsApp>();

        foreach (string linea in salida.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)) {
            string actual = linea.Trim();

            if (string.IsNullOrWhiteSpace(actual) || actual.StartsWith("NAME", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            string[] columnas = Regex.Split(actual, @"\s{2,}");
            if (columnas.Length < 3) {
                continue;
            }

            string nombre = columnas[0].Trim();
            string jid = columnas[1].Trim();
            string creado = columnas[2].Trim();

            if (!jid.EndsWith("@g.us", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            DateTime? fechaCreacion = null;
            if (DateTime.TryParse(creado, out DateTime fecha)) {
                fechaCreacion = fecha;
            }

            grupos.Add(new GrupoWhatsApp(nombre, jid, fechaCreacion));
        }

        return grupos;
    }

    void Ejecutar(bool json, List<string> argumentos) {
        string salida = EjecutarYObtenerSalida(json, argumentos);

        if (!string.IsNullOrWhiteSpace(salida)) {
            Console.WriteLine(salida);
        }
    }

    string EjecutarYObtenerSalida(bool json, List<string> argumentos) {
        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = "wacli",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (json) {
            startInfo.ArgumentList.Add("--json");
        }

        if (!string.IsNullOrWhiteSpace(store)) {
            startInfo.ArgumentList.Add("--store");
            startInfo.ArgumentList.Add(store);
        }

        startInfo.ArgumentList.Add("--timeout");
        startInfo.ArgumentList.Add(FormatearDuracionWacli(timeout));

        foreach (string argumento in argumentos) {
            startInfo.ArgumentList.Add(argumento);
        }

        using Process proceso = Process.Start(startInfo) ?? throw new InvalidOperationException("No se pudo iniciar wacli.");
        proceso.StandardInput.Close();

        if (!proceso.WaitForExit((int)timeout.TotalMilliseconds)) {
            try {
                proceso.Kill(entireProcessTree: true);
            } catch {
                // Ignoramos errores al intentar detener el proceso.
            }

            throw new TimeoutException($"wacli no terminó dentro de {timeout}.");
        }

        string salida = proceso.StandardOutput.ReadToEnd().Trim();
        string error = proceso.StandardError.ReadToEnd().Trim();

        if (proceso.ExitCode != 0) {
            string detalle = string.IsNullOrWhiteSpace(error) ? salida : error;
            throw new InvalidOperationException($"wacli falló con código {proceso.ExitCode}: {detalle}");
        }

        return salida;
    }

    static string FormatearDuracionWacli(TimeSpan duracion) {
        if (duracion <= TimeSpan.Zero) {
            return "1s";
        }

        if (duracion.TotalSeconds < 60 && duracion.TotalSeconds == Math.Floor(duracion.TotalSeconds)) {
            return $"{(int)duracion.TotalSeconds}s";
        }

        if (duracion.TotalMinutes < 60 && duracion.TotalSeconds % 60 == 0) {
            return $"{(int)duracion.TotalMinutes}m";
        }

        int horas = (int)duracion.TotalHours;
        int minutos = duracion.Minutes;
        int segundos = duracion.Seconds;

        StringBuilder sb = new StringBuilder();

        if (horas > 0) {
            sb.Append($"{horas}h");
        }

        if (minutos > 0) {
            sb.Append($"{minutos}m");
        }

        if (segundos > 0 || sb.Length == 0) {
            sb.Append($"{segundos}s");
        }

        return sb.ToString();
    }

    static bool EsErrorAutenticacionWacli(Exception ex) {
        return ex.Message.Contains("not authenticated", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("auth", StringComparison.OrdinalIgnoreCase);
    }
}

record GrupoWhatsApp(string Nombre, string Jid, DateTime? Creado);


class Program {

    
    static void Main(string[] args) {
        var rutaPerfiles = "../material-docente/perfil/perfiles";
        var rutaSalida = "alumnos.md";
        Alumnos alumnos = AlumnosManager.CargarAlumnos( "alumnos.md");
        AlumnosManager.ActualizarDesdePerfiles(alumnos, rutaPerfiles);
        // AlumnosManager.Guardar(alumnos, rutaSalida);
        // AlumnosManager.GuardarJSON(alumnos, "alumnos.json");
        // AlumnosManager.GuardarVCard(alumnos, "alumnos.vcf");
        // AlumnosManager.CrearCarpetas(alumnos);
        AlumnosManager.CopiarFotoPerfil(alumnos, rutaPerfiles);

        // var sinFoto = alumnos.SinFotos();
        // AlumnosManager.Guardar(sinFoto, "alumnos-sin-foto.md");

        // var sinTelefono = alumnos.SinTelefono();
        // AlumnosManager.Guardar(sinTelefono, "alumnos-sin-telefono.md");

        // var sinGitHub = alumnos.FiltrarSinGithub();
        // AlumnosManager.Guardar(sinGitHub, "alumnos-sin-github.md");


        WAppService wapp = new WAppService();
        Console.WriteLine("Enviando mensaje al grupo de WhatsApp...");

        Alumnos alumnosConTelefonoComision7 = alumnos.EnComision("C7").SinTelefono();
        AlumnosManager.Listar(alumnosConTelefonoComision7);

        Alumnos alumnosConTelefonoComision9 = alumnos.EnComision("C9").SinTelefono();
        
        AlumnosManager.Listar(alumnosConTelefonoComision9);
        // wapp.InvitarGrupoComision(alumnos);

        AlumnosManager.Listar(alumnos.ParaAgregar());

        AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C7"), "alumnos-agregar-c7.vcf");
        AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C9"), "alumnos-agregar-c9.vcf");
        // wapp.InvitarGrupoComision(alumnos.ParaAgregar());

    }
}

