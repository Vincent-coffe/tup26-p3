// Conjunto de enteros
// Implementamos la clase Conjunto usando una lista interna de enteros.

var a = new Conjunto();
a.Agregar(5);
a.Agregar(10);
a.Agregar(5); // No se agrega porque ya existe

a[7] = true; // Agrega 7
a[5] = false; // Elimina 5

Console.Clear();
Console.WriteLine("== Conjunto ==");
Console.WriteLine($" Hay {a.Count} elementos."); // 2
Console.WriteLine($" - Contiene  5?: {a.Contiene(5)}");  // True
Console.WriteLine($" - Contiene 10?: {a.Contiene(10)}"); // True
Console.WriteLine($" - Contiene  3?: {a.Contiene(3)}");  // False
Console.WriteLine($" Conjunto: {a}"); // { 5, 10 }
Console.WriteLine($" - Contiene 5?: {a[5]}"); // True
Console.WriteLine($" - Contiene 3?: {a[3]}"); // False

class Conjunto {
    List<int> elementos;

    public Conjunto() {
        elementos = new List<int>();
    }

    public void Agregar(int valor) {
        if (!elementos.Contains(valor)) {
            elementos.Add(valor);
        }
    }

    public void Eliminar(int valor) {
        elementos.Remove(valor);
    }

    public bool Contiene(int valor) {
        return elementos.Contains(valor);
    }

    public bool this[int key] {
        get => Contiene(key);
        set {
            if (value) {
                Agregar(key);
            } else {
                Eliminar(key);
            }
        }
    }

    public int[] Elementos => elementos.ToArray();
    
    public override string ToString() => "{" + string.Join(", ", elementos) + " }";
    public int Count => elementos.Count;
}