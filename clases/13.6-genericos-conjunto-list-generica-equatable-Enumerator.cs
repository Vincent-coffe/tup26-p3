using System;
using System.Collections;
using System.Collections.Generic;

static class Program {
    public static void Main() {
        var a1 = new Alumno("Ana", 20);
        var a2 = new Alumno("Anita", 20);
        var b1 = new Alumno("Bob", 22);

        var clase = new Conjunto<Alumno>();
        clase.Agregar(a1);
        clase.Agregar(a2);
        clase.Agregar(b1);

        Console.Clear();
        Console.WriteLine("== Conjunto<Alumno> ==");
        Console.WriteLine($" Hay {clase.Count} elementos.");
        Console.WriteLine($" - Contiene Ana?: {clase.Contiene(a2)}");
        Console.WriteLine($" - Contiene Bob?: {clase.Contiene(b1)}");
        Console.WriteLine($" - Contiene Carlos?: {clase.Contiene(new Alumno("Carlos", 25))}");
        Console.WriteLine($" Conjunto<Alumno>: {clase}");

        foreach (var alumno in clase) {
            Console.WriteLine($" - {alumno}");
        }

        clase.Where(a => a.Legajo > 20).ToList().ForEach(a => Console.WriteLine($" - {a}"));
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

interface IConjunto<T> : IEnumerable<T> where T : IEquatable<T> {
    void Agregar(T valor);
    void Eliminar(T valor);
    bool Contiene(T valor);
    int Count { get; }
}

class Conjunto<T> : IConjunto<T> where T : IEquatable<T> {
    readonly List<T> elementos = new();

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

    // Implementamos el método GetEnumerator para que la clase Conjunto pueda ser recorrida con un foreach.
    public IEnumerator<T> GetEnumerator() {
        foreach (var elemento in elementos) {
            yield return elemento;
        }
    }

    // Por compatibilidad con IEnumerable, también debemos implementar el método GetEnumerator sin tipo genérico.
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => "{" + string.Join(", ", elementos) + " }";
    public int Count => elementos.Count;
}

