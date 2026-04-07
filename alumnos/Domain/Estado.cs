namespace Tup26.AlumnosApp;

[Flags]
enum Estado : int {
    Vacio = 0,
    Aprobado = 1,
    Pendiente = 2,
    Revision = 4,
    Desaprobado = 8,
}
