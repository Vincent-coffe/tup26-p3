namespace Tup26.AlumnosApp;

class Alumnos : IEnumerable<Alumno> {
    public List<Alumno> Lista { get; set; } = new();

    public int Count => Lista.Count;

    public Alumno this[int index] => Lista[index];

    public Alumnos(IEnumerable<Alumno> alumnos) {
        Lista = alumnos?.ToList() ?? new();
    }

    public void Agregar(Alumno alumno) {
        Lista.Add(alumno);
    }

    public Alumnos ConGithub(bool tiene = true) =>
        new(Lista.Where(alumno => tiene == alumno.ConGithub));

    public Alumnos ConPractico(int numero, Estado estado) =>
        new(Lista.Where(alumno =>
            alumno.practicos != null &&
            alumno.practicos.Count >= numero &&
            alumno.practicos[numero - 1].HasFlag(estado)));

    public Alumnos ConTelefono(bool tiene = true) =>
        new(Lista.Where(alumno => tiene == alumno.ConTelefono));

    public Alumnos ConFotos(bool tiene = true) =>
        new(Lista.Where(alumno => tiene == alumno.ConFoto));

    public Alumnos EnComision(string comision) =>
        new(Lista.Where(alumno =>
            string.Equals(alumno.Comision, comision, StringComparison.OrdinalIgnoreCase)));

    public Alumnos ParaAgregar() =>
        new(Lista.Where(alumno => alumno.GitHub == "(agregar)"));

    public IEnumerator<Alumno> GetEnumerator() {
        return Lista.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
