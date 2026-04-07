namespace Tup26.AlumnosApp;

class GitHub {
    readonly string owner;
    readonly string repo;

    public GitHub(string owner = "AlejandroDiBattista", string repo = "tup26-p3") {
        this.owner = owner;
        this.repo = repo;
    }

    public bool AgregarColaborador(string usuario) {
        (string salida, string error, int codigoSalida) = EjecutarGh(new[]
        {
            "api", "--method", "PUT", $"repos/{owner}/{repo}/collaborators/{usuario}", "-f", "permission=push"
        });

        if (codigoSalida != 0) {
            string detalle = string.IsNullOrWhiteSpace(error) ? salida : error;
            Console.WriteLine($"Error al agregar colaborador '{usuario}': {detalle}");
            return false;
        }

        return true;
    }

    public List<string> ListarColaboradores() {
        (string salida, string error, int codigoSalida) = EjecutarGh(new[]
        {
            "api", $"repos/{owner}/{repo}/collaborators", "--jq", ".[] | select(.permissions.push == true) | .login"
        });

        if (codigoSalida != 0) {
            string detalle = string.IsNullOrWhiteSpace(error) ? salida : error;
            Console.WriteLine($"Error al listar colaboradores: {detalle}");
            return new();
        }

        return LeerLineas(salida);
    }

    public List<string> ListarInvitacionesPendientes() {
        (string salida, string error, int codigoSalida) = EjecutarGh(new[]
        {
            "api", $"repos/{owner}/{repo}/invitations", "--paginate", "--jq", ".[].invitee.login"
        });

        if (codigoSalida != 0) {
            string detalle = string.IsNullOrWhiteSpace(error) ? salida : error;
            Console.WriteLine($"Error al listar invitaciones pendientes: {detalle}");
            return new();
        }

        return LeerLineas(salida);
    }

    (string Salida, string Error, int CodigoSalida) EjecutarGh(IEnumerable<string> argumentos) {
        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = "gh",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string argumento in argumentos) {
            startInfo.ArgumentList.Add(argumento);
        }

        Console.WriteLine($"Ejecutando gh {string.Join(' ', argumentos)}...");

        using Process proceso = Process.Start(startInfo)
            ?? throw new InvalidOperationException("No se pudo iniciar gh.");

        string salida = proceso.StandardOutput.ReadToEnd().Trim();
        string error = proceso.StandardError.ReadToEnd().Trim();

        proceso.WaitForExit();

        return (salida, error, proceso.ExitCode);
    }

    static List<string> LeerLineas(string texto) {
        return texto.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(linea => linea.Trim().ToLower())
                    .Where(linea => !string.IsNullOrWhiteSpace(linea))
                    .ToList();
    }
}
