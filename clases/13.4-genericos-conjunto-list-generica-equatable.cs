using System;
using System.Collections.Generic;

static class Program {
    public static void Main() {
        var a1 = new Alumno("Ana", 20);
        var a2 = new Alumno("Anita", 20);
        var b1 = new Alumno("Bob", 22);

        var clase = new Conjunto<Alumno>();
        clase.Agregar(a1);
        clase.Agregar(a2); // No se agrega porque tiene el mismo legajo que Ana
        clase.Agregar(b1);

        Console.Clear();
        Console.WriteLine("== Conjunto<Alumno> ==");
        Console.WriteLine($" Hay {clase.Count} elementos."); // 2
        Console.WriteLine($" - Contiene  Ana?: {clase.Contiene(a2)}");  // True
        Console.WriteLine($" - Contiene  Bob?: {clase.Contiene(b1)}");  // True
        Console.WriteLine($" - Contiene  Carlos?: {clase.Contiene(new Alumno("Carlos", 25))}");  // False
        Console.WriteLine($" Conjunto<Alumno>: {clase}"); // { Ana (20 años), Bob (22 años) }
    }
}

class Alumno(string nombre, int legajo) : IEquatable<Alumno> {
    public string Nombre => nombre;
    public int Legajo => legajo;
    public override string ToString() => $"{nombre} ({legajo} años)";

    public bool Equals(Alumno? otro) {
        if (otro is null) {
            return false;
        }
        return this.Legajo == otro.Legajo;
    }
}

class Conjunto<T> {
    List<T> elementos;

    public Conjunto() {
        elementos = new List<T>();
    }

    public void Agregar(T valor) {
        if (!Contiene(valor)) {
            elementos.Add(valor);
        }
    }

    public void Eliminar(T valor) {
        elementos.Remove(valor);
    }

    public bool Contiene(T valor) {
        return elementos.Contains(valor);
    }

    public override string ToString() => "{" + string.Join(", ", elementos) + " }";
    public int Count => elementos.Count;
}

