namespace Tup26.AlumnosApp;

class Program {
    static void Main(string[] args) {
        Alumnos alumnos = AlumnosManager.CargarAlumnos(AppPaths.ArchivoAlumnos);

        // foreach (var a in alumnos.ConGithub())
        // {
        //     a.Practico(1, Estado.Revision);
        // }
        // foreach (var a in alumnos.ConGithub(false))
        // {
        //     a.Practico(1, Estado.Desaprobado);
        // }

        // alumnos[10].Practico(5, Estado.EnProgreso);
        // AlumnosManager.Listar(alumnos);
        // AlumnosManager.Guardar(alumnos, AppPaths.ArchivoAlumnos);
        // AlumnosManager.Listar(alumnos.ConFotos(false), "Alumnos sin foto");
        // AlumnosManager.ActualizarDesdePerfiles(alumnos, "../material-docente/perfil/perfiles");
        // AlumnosManager.Guardar(alumnos, AppPaths.ArchivoAlumnos);
        // AlumnosManager.GuardarJSON(alumnos, "alumnos.json");
        // AlumnosManager.GuardarVCard(alumnos, AppPaths.ArchivoVcf);
        // AlumnosManager.CrearCarpetas(alumnos);
        // AlumnosManager.CopiarFotoPerfil(alumnos, rutaPerfiles);

        // var sinFoto = alumnos.SinFotos();
        // AlumnosManager.Guardar(sinFoto, "alumnos-sin-foto.md");

        // var sinTelefono = alumnos.SinTelefono();
        // AlumnosManager.Guardar(sinTelefono, "alumnos-sin-telefono.md");

        // var sinGitHub = alumnos.FiltrarSinGithub();
        // AlumnosManager.Guardar(sinGitHub, "alumnos-sin-github.md");

        // Console.WriteLine("Enviando mensaje al grupo de WhatsApp...");

        // Alumnos alumnosConTelefonoComision7 = alumnos.EnComision("C7").SinTelefono();
        // AlumnosManager.Listar(alumnosConTelefonoComision7);

        // Alumnos alumnosConTelefonoComision9 = alumnos.EnComision("C9").SinTelefono();

        // AlumnosManager.Listar(alumnos.ParaAgregar());

        // AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C7"), "alumnos-agregar-c7.vcf");
        // AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C9"), "alumnos-agregar-c9.vcf");
        // AlumnosManager.CopiarEnunciadoPracticos(alumnos, "tp1");

        GitHub gh = new GitHub();

        // var C7 = [63415, 63456, 63268, 63402, 63419, 63776, 63399, 63211, 63350, 61581, 63647, 63420, 63354, 63393, 63208, 63387, 63547, 63447, 61490, 63397, 63696];
        // var C9 = [63385, 63217, 63313, 63222, 61801, 63150, 63461, 64016, 61641, 63737, 61057, 63717, 61161, 62844, 63231, 63425, 61907, 63219, 63297, 63388, 63494, 63418, 63412, 63205, 63220, 63232, 63216, 61026];
        // var pedidos = C7 + C9;

        List<string> colaboradores = gh.ListarColaboradores();
        List<string> invitaciones = gh.ListarInvitacionesPendientes();

        Console.WriteLine($"Colaboradores: {string.Join(" ", colaboradores)}");
        Console.WriteLine($"Pendientes: {string.Join(" ", invitaciones)}");
        Console.WriteLine();

        foreach (Alumno a in alumnos) {
            if (a.ConGithub) {
                string usuario = a.GitHub.Trim().ToLower();

                if (colaboradores.Contains(usuario)) {
                    a.Practico(1, Estado.Aprobado);
                }
                else if (invitaciones.Contains(usuario)) {
                    a.Practico(1, Estado.Pendiente);
                }
                else if (gh.AgregarColaborador(a.GitHub)) {
                    a.Practico(1, Estado.Pendiente);
                }
                else {
                    a.Practico(1, Estado.Revision);
                }
            }
            else {
                a.Practico(1, Estado.Desaprobado);
            }
        }

        // AlumnosManager.Guardar(alumnos, AppPaths.ArchivoAlumnos);

        MensajesService.MensajeSinGithub();
        // MensajesService.MensajeGithubErroneo();

        // WAppService wapp = new WAppService();
        // foreach (var p in wapp.Participantes("TUP26-P3-C7"))
        // {
        //     Console.WriteLine($"Participante del grupo C7: {p.Name} | {p.PhoneNumber} | {p.Jid}");
        // }
    }
}
