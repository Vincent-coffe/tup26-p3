
// sortx [input [output]] [-b|--by campo[:tipo[:orden]]]...
//       [-i|--input input] [-o|--output output]
//       [-d|--delimiter delimitador]
//       [-nh|--no-header] [-h|--help]

Console.WriteLine($"sortx {string.Join(" ", args)}");

using System;

class SortX
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== PROGRAMA DE ORDENAMIENTO ===");

        int cantidad = PedirCantidad();
        int[] numeros = CargarNumeros(cantidad);

        OrdenarNumeros(numeros);

        MostrarNumeros(numeros);
    }

    static int PedirCantidad()
    {
        Console.Write("Ingrese la cantidad de números a ordenar: ");
        return int.Parse(Console.ReadLine());
    }

    static int[] CargarNumeros(int cantidad)
    {
        int[] arreglo = new int[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Console.Write("Ingrese el número " + (i + 1) + ": ");
            arreglo[i] = int.Parse(Console.ReadLine());
        }

        return arreglo;
    }

    static void OrdenarNumeros(int[] arreglo)
    {
        for (int i = 0; i < arreglo.Length - 1; i++)
        {
            for (int j = 0; j < arreglo.Length - i - 1; j++)
            {
                if (arreglo[j] > arreglo[j + 1])
                {
                    int temp = arreglo[j];
                    arreglo[j] = arreglo[j + 1];
                    arreglo[j + 1] = temp;
                }
            }
        }
    }

    static void MostrarNumeros(int[] arreglo)
    {
        Console.WriteLine("\nNúmeros ordenados:");

        foreach (int numero in arreglo)
        {
            Console.Write(numero + " ");
        }

        Console.WriteLine();
    }
}
