---
marp: true
theme: default
paginate: true
style: |
  :root {
        --primary:   #5B43D6;
        --accent:    #8A6CF3;
        --bg:        #FAFAFC;
        --bg-card:   #F3F4FA;
        --code-bg:   #F5F6FB;
        --text:      #1F2430;
        --muted:     #5F677A;
        --good:      #2E8B57;
        --bad:       #C94C5A;
    font-family: 'Segoe UI', system-ui, sans-serif;
  }

  section {
        background: var(--bg);
    color: var(--text);
    padding: 48px 64px;
  }

    h1 { color: var(--primary); font-size: 2.2em; margin-bottom: 0.2em; }
    h2 { color: var(--primary); font-size: 1.5em; border-bottom: 2px solid color-mix(in srgb, var(--primary) 55%, white); padding-bottom: 8px; }
    h3 { color: color-mix(in srgb, var(--primary) 75%, black); font-size: 1.1em; margin-top: 0.8em; }

  code {
        background: var(--code-bg);
        color: #0F4C81;
    padding: 2px 6px;
        border-radius: 6px;
        font-family: 'Cascadia Code', 'Fira Code', Consolas, monospace;
    font-size: 0.88em;
  }

  pre {
        background: white !important;
        border: 1px solid color-mix(in srgb, var(--primary) 20%, white);
        border-left: 5px solid var(--primary);
        border-radius: 10px;
    padding: 16px 20px !important;
    font-size: 0.78em;
    line-height: 1.55;
  }

  pre code {
    background: transparent;
        color: #24324A;
    padding: 0;
  }

    /* Syntax highlighting */
    .hljs-comment,
    .hljs-quote,
    .hljs-meta {
        color: var(--good);
        font-style: italic;
    }

  table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.85em;
    margin-top: 0.6em;
  }
  th {
        background: color-mix(in srgb, var(--primary) 92%, white);
    color: white;
    padding: 8px 12px;
    text-align: left;
  }
  td {
        background: white;
    padding: 7px 12px;
        border-bottom: 1px solid #DCE0EE;
        color: var(--text);
  }

  blockquote {
        background: #F6F4FF;
    border-left: 4px solid var(--accent);
    padding: 10px 18px;
    margin: 12px 0;
        border-radius: 0 10px 10px 0;
    font-style: normal;
        color: #3D465A;
  }

  ul, ol { margin-left: 1.2em; line-height: 1.8; }
  li { margin: 0.2em 0; }
    strong { color: var(--primary); }

  .columns { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
  .bad  { color: var(--bad);  font-weight: bold; }
  .good { color: var(--good); font-weight: bold; }

  /* Slide de portada */
  section.portada {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: flex-start;
        background:
            radial-gradient(circle at top right, rgba(139, 108, 243, 0.16), transparent 34%),
            linear-gradient(135deg, #FFFFFF 0%, #F4F1FF 58%, #EFEFFF 100%);
  }
  section.portada h1 {
    font-size: 3em;
        color: var(--primary);
    margin-bottom: 0.1em;
  }
  section.portada .subtitle {
    font-size: 1.3em;
    color: var(--accent);
    margin-bottom: 2em;
  }
  section.portada .meta {
    color: var(--muted);
    font-size: 0.9em;
  }

  /* Slide de sección */
  section.seccion {
    display: flex;
    flex-direction: column;
    justify-content: center;
        background:
            linear-gradient(135deg, rgba(91, 67, 214, 0.08), rgba(138, 108, 243, 0.04)),
            #FAFAFC;
  }
  section.seccion h2 {
    font-size: 2em;
    border: none;
        color: var(--primary);
  }
    section.seccion p { color: var(--muted); font-size: 1.1em; }

  /* Numeración */
  section::after {
    content: attr(data-marpit-pagination) ' / ' attr(data-marpit-pagination-total);
    color: var(--muted);
    font-size: 0.75em;
  }
---

<!-- _class: portada -->

# LINQ
## Language INtegrated Query

<div class="subtitle">Consultas integradas al lenguaje</div>

<div class="meta">
Programación III · UTN Tucumán · C# 14 / .NET 10
</div>

---

<!-- _class: seccion -->

## El problema
### Antes de LINQ — código imperativo

---

## Sin LINQ: 20 líneas para una consulta simple

```csharp
// Pedidos aprobados del último mes, sin repetir cliente, por monto desc.
List<decimal> resultado = new List<decimal>();
HashSet<int>  clientesVistos = new HashSet<int>();
DateTime      hace30Dias     = DateTime.Today.AddDays(-30);

for (int i = 0; i < pedidos.Count; i++) {
    Pedido p = pedidos[i];
    if (p.Estado != "aprobado") continue;
    if (p.Fecha  <  hace30Dias) continue;
    if (clientesVistos.Contains(p.ClienteId)) continue;

    clientesVistos.Add(p.ClienteId);
    resultado.Add(p.Monto);
}
resultado.Sort((a, b) => b.CompareTo(a));
```

> El **qué** está enterrado en el **cómo**: bucles, índices, colecciones auxiliares.

---

## Con LINQ: 4 líneas, misma consulta

```csharp
var resultado = pedidos
    .Where(p => p.Estado == "aprobado"
             && p.Fecha >= DateTime.Today.AddDays(-30))
    .DistinctBy(p => p.ClienteId)
    .OrderByDescending(p => p.Monto)
    .Select(p => p.Monto)
    .ToList();
```

<br>

Cada línea declara una **intención**.
La lógica se lee como una descripción del resultado — no como un procedimiento.

> LINQ **eleva el nivel de abstracción**: elimina el ruido del código de control y deja visible solo la lógica del dominio.

---

<!-- _class: seccion -->

## Filosofía de diseño
### Declarativo · Funcional · Composición

---

## Imperativo vs Declarativo

| Imperativo                              | Declarativo (LINQ)                       |
|-----------------------------------------|------------------------------------------|
| Describe el **cómo** (pasos)            | Describe el **qué** (resultado)          |
| Muta estado en cada paso                | Transforma datos sin mutar la fuente     |
| Bucles y variables auxiliares           | Composición de operaciones               |
| Orden del código = orden de ejecución   | La ejecución puede optimizarse           |

<br>

---

### Raíces en programación funcional

| Concepto funcional | LINQ            | Qué hace                            |
|--------------------|-----------------|-------------------------------------|
| **map**            | `Select()`      | Transforma cada elemento            |
| **filter**         | `Where()`       | Selecciona según condición          |
| **reduce / fold**  | `Aggregate()`   | Combina todos en un único valor     |

---

## Composición de funciones

Los métodos LINQ se **encadenan**: la salida de uno es la entrada del siguiente.

```
pedidos → Where() → DistinctBy() → OrderBy() → Select() → ToList()
              ↓           ↓             ↓           ↓
           filtrar     deduplicar    ordenar    transformar
```

```csharp
// Cada método hace una sola cosa.
// El encadenamiento construye pipelines legibles y reutilizables.
productos
    .Where(p => p.Activo)           // filtrar
    .OrderBy(p => p.Nombre)         // ordenar
    .Select(p => p.Nombre.ToUpper()) // transformar
    .ToList();                       // materializar
```

---

## Lambdas — funciones como valores

```csharp
// Una lambda es una función sin nombre pasada como argumento
productos.Where(p => p.Precio > 1000)
//               ↑──────────────────────
//         Func<Producto, bool>: recibe Producto, devuelve bool
```

```csharp
Func<Producto, bool>    predicado  = p => p.Precio > 1000;
Func<Producto, string>  selector   = p => p.Nombre;
Func<Producto, decimal> clave      = p => p.Precio;
Func<int, int, int>     acumulador = (acc, x) => acc + x;
```

Esto es **programación de orden superior**: funciones que reciben otras funciones como parámetros.

---

## Uniformidad — la misma sintaxis para todo

```csharp
// Colección en memoria — LINQ to Objects
productos.Where(p => p.Precio > 1000).ToList();

// Base de datos SQL — LINQ to EF Core
db.Productos.Where(p => p.Precio > 1000).ToListAsync();

// Archivos XML — LINQ to XML
documento.Descendants("producto")
         .Where(e => (int)e.Element("precio") > 1000);
```

El código de consulta es **idéntico**.
Solo cambia el tipo del objeto inicial.

---

<!-- _class: seccion -->

## Evaluación diferida
### El concepto más importante de LINQ

---

## Los métodos LINQ no ejecutan nada al llamarse

```csharp
var query = productos
    .Where(p => p.Precio > 1000)   // ← construye un plan, no ejecuta
    .OrderBy(p => p.Nombre)        // ← agrega al plan
    .Select(p => p.Nombre);        // ← agrega al plan

// query es IEnumerable<string> — ningún elemento fue filtrado todavía

// La ejecución ocurre al iterar:
foreach (var nombre in query) {    // ← AQUÍ se ejecuta
    Console.WriteLine(nombre);
}
```

> `query` es una **descripción** de lo que se quiere calcular, no el resultado.

---

## El modelo pull — cadena de iteradores

```
foreach pide el siguiente elemento
    ↓
Select pide el siguiente al que tiene debajo
    ↓
OrderBy necesita TODOS los elementos antes de producir alguno
    ↓
Where pide elementos a la lista, filtrando uno a uno
    ↓
La lista entrega sus elementos
```

**Consecuencia clave:** si solo necesitás el primer elemento, `Where` solo evalúa hasta encontrar el primero que cumple.

```csharp
var primero = productos.Where(p => p.Precio > 1000).First();
// Puede haber evaluado solo 1 o 2 elementos
```

---

## Materialización — ejecutar y guardar

<div class="columns">

<div>

### Métodos que ejecutan

| Método | Retorna |
|--------|---------|
| `ToList()` | `List<T>` |
| `ToArray()` | `T[]` |
| `ToDictionary()` | `Dictionary<K,V>` |
| `First()` | `T` |
| `Count()` | `int` |
| `Any()` | `bool` |
| `Sum()` / `Max()` | `T` |
| `foreach` | — |

</div>

<div>

```csharp
// ✗ Re-ejecuta la query dos veces
var q = productos.Where(p => p.Activo);
int n = q.Count();    // ejecuta
var f = q.First();    // vuelve a ejecutar

// ✓ Materializar una vez
var lista = productos
    .Where(p => p.Activo)
    .ToList();        // ejecuta una sola vez

int n = lista.Count; // O(1)
var f = lista.First(); // no re-ejecuta
```

</div>
</div>

---

<!-- _class: seccion -->

## Los métodos LINQ
### De lo simple a lo complejo

---

## Datos de ejemplo

```csharp
record Producto(int Id, string Nombre, string Categoria,
                decimal Precio, int Stock, bool Activo);

List<Producto> productos = [
    new(1, "Teclado Mecánico",  "Periféricos",     8_500, 50, true),
    new(2, "Monitor 27",        "Monitores",      75_000, 12, true),
    new(3, "Mouse Inalámbrico", "Periféricos",     3_200, 80, true),
    new(4, "Webcam HD",         "Periféricos",    12_000,  0, false),
    new(5, "SSD 1TB",           "Almacenamiento", 18_000, 35, true),
    new(6, "RAM 32GB",          "Componentes",    22_000,  8, true),
    new(7, "Auriculares BT",    "Periféricos",     9_500, 20, true),
    new(8, "Hub USB-C",         "Periféricos",     4_500, 60, true),
];
```

---

## `Where` — filtrar

```csharp
// Filtro simple
var activos    = productos.Where(p => p.Activo);

// Filtro compuesto
var disponibles = productos.Where(p => p.Activo && p.Stock > 0);

// Filtro con rango
var medianos   = productos.Where(p => p.Precio >= 5_000 && p.Precio <= 20_000);

// Filtro por texto
var perifericos = productos.Where(p => p.Categoria == "Periféricos");

// Con índice de posición
var enPosicionImpar = productos.Where((p, i) => i % 2 != 0);
```

---

## `Select` — transformar (map)

```csharp
// Proyectar a string
IEnumerable<string> nombres = productos.Select(p => p.Nombre);

// Proyectar a tipo anónimo
var resumen = productos.Select(p => new {
    p.Nombre,
    p.Precio,
    Disponible = p.Stock > 0
});

// Proyectar con cálculo
var conIva = productos.Select(p => new {
    p.Nombre,
    PrecioConIva = Math.Round(p.Precio * 1.21m, 2),
});

// Con índice de posición
var enumerados = productos.Select((p, i) => $"{i + 1}. {p.Nombre}");
```

---

## `OrderBy` · `Take` · `Skip`

```csharp
// Ordenar
var porPrecio  = productos.OrderBy(p => p.Precio);
var masCaro    = productos.OrderByDescending(p => p.Precio);

// Ordenamiento múltiple
var ordenados = productos
    .OrderBy(p => p.Categoria)
    .ThenByDescending(p => p.Precio);   // dentro de cada categoría: más caros primero
```

```csharp
// Paginación
var pagina2 = productos
    .OrderBy(p => p.Id)
    .Skip((2 - 1) * 10)   // saltar página 1
    .Take(10)             // tomar página 2
    .ToList();

// Los 3 más caros
var top3 = productos.OrderByDescending(p => p.Precio).Take(3);
```

---

## `Any` · `All` · Agregaciones

```csharp
// Verificación — se detienen al encontrar el primero
bool hayBaratos     = productos.Any(p => p.Precio < 5_000);
bool todosActivos   = productos.All(p => p.Activo);
bool tieneElementos = productos.Any();   // ✓ más eficiente que Count() > 0
```

```csharp
// Agregaciones — calculan sobre toda la colección
int     total    = productos.Count();
int     activos  = productos.Count(p => p.Activo);
decimal suma     = productos.Sum(p => p.Precio);
decimal minimo   = productos.Min(p => p.Precio);
double  promedio = productos.Average(p => (double)p.Precio);
```
```csharp
// Obtener el objeto, no solo el valor
Producto masBarato = productos.MinBy(p => p.Precio)!;
Producto masCaro_  = productos.MaxBy(p => p.Precio)!;
```

---

## `GroupBy` — agrupar

```csharp
// Agrupar y proyectar estadísticas por grupo
var estadisticas = productos
    .GroupBy(p => p.Categoria)
    .Select(g => new {
        Categoria = g.Key,
        Cantidad  = g.Count(),
        Promedio  = g.Average(p => p.Precio),
        Total     = g.Sum(p => p.Precio),
    })
    .OrderByDescending(x => x.Total).ToList();
```

```csharp
g.Key        // la clave del grupo ("Periféricos", "Monitores", ...)
g.Count()    // cuántos elementos tiene el grupo
g.Where(...) // filtrar dentro del grupo
g.OrderBy()  // ordenar dentro del grupo
```

---

## `SelectMany` — aplanar (flatMap)

```csharp
// Sin SelectMany: colección de colecciones
var anidado = pedidos.Select(p => p.Items);
// IEnumerable<IEnumerable<Item>> — difícil de usar
```
```csharp
// Con SelectMany: todos los items en una sola secuencia
var items = pedidos.SelectMany(p => p.Items);
// IEnumerable<Item> — plano y usable
```
```csharp
// Caso real: todos los tags únicos de una lista de artículos
var todosLosTags = articulos
    .SelectMany(a => a.Tags)   // "aplana" todas las listas de tags
    .Distinct()
    .OrderBy(t => t)
    .ToList();
```

> Es el **flatMap** de la programación funcional.

---

## `Aggregate` — el reduce general

```csharp
// Mecanismo: aplica una función acumuladora elemento a elemento
int suma = new[] { 1, 2, 3, 4, 5 }
    .Aggregate((acum, x) => acum + x);
// paso 1: acum=1, x=2 → 3
// paso 2: acum=3, x=3 → 6  ...  resultado: 15
```

```csharp
// Caso real: estadísticas en una sola pasada
var stats = productos.Aggregate(
    new { Min = decimal.MaxValue, Max = decimal.MinValue, Suma = 0m, N = 0 },
    (acum, p) => new {
        Min  = Math.Min(acum.Min, p.Precio),
        Max  = Math.Max(acum.Max, p.Precio),
        Suma = acum.Suma + p.Precio,
        N    = acum.N + 1,
    }
);
Console.WriteLine($"Promedio: {stats.Suma / stats.N:N0}");
```

---

## `Join` — combinar dos fuentes

```csharp
// Inner join: solo elementos con coincidencia en ambas colecciones
var pedidosConCliente = pedidos.Join(
    clientes,
    ped  => ped.ClienteId,   // clave en pedidos
    cli  => cli.Id,           // clave en clientes
    (ped, cli) => new {       // resultado de la unión
        cli.Nombre, 
        ped.Fecha, 
        ped.Estado,
    }
);
```

---
## `GroupJoin` — left join
```csharp
// Left join: todos los clientes, tengan pedidos o no
var clientesConPedidos = clientes.GroupJoin(
    pedidos,
    cli => cli.Id,
    ped => ped.ClienteId,
    (cli, pedsCli) => new {
        cli.Nombre, 
        TotalPedidos = pedsCli.Count(),
    }
);
```

---

<!-- _class: seccion -->

## Contraste completo
### El mismo problema: imperativo vs LINQ

---

## Problema: reporte de ventas por categoría

**Dado:** lista de pedidos y productos.
**Obtener:** por categoría — cantidad de pedidos, unidades vendidas y monto total.
**Solo:** pedidos aprobados del último mes. **Orden:** monto descendente.

---

## Solución imperativa — 50+ líneas

```csharp
// Paso 1: indexar productos
var indice = new Dictionary<int, Producto>();
foreach (var p in productos) indice[p.Id] = p;

// Paso 2: filtrar
var filtrados = new List<Pedido>();
foreach (var ped in pedidos)
    if (ped.Estado == "aprobado" && ped.Fecha >= desde) filtrados.Add(ped);
```
---

## Paso 3: agrupar manualmente
```csharp
// Paso 3: agrupar por categoría a mano
var porCategoria = new Dictionary<string, List<Pedido>>();
foreach (var ped in filtrados) {
    if (!indice.TryGetValue(ped.ProductoId, out var prod)) continue;
    if (!porCategoria.ContainsKey(prod.Categoria))
        porCategoria[prod.Categoria] = new List<Pedido>();
    porCategoria[prod.Categoria].Add(ped);
}

```

---

## Paso 4: Calcular métricas y ordenar el resultado

```csharp
var reporte = new List<ReporteCategoria>();

foreach (var kvp in porCategoria) {
    string categoria = kvp.Key;
    List<Pedido> pedidosCategoria = kvp.Value;

    int cantPedidos = 0, unidades = 0;
    decimal montoTotal = 0m;

    foreach (var ped in pedidosCategoria) {
        cantPedidos++;
        unidades += ped.Cantidad;
        montoTotal += ped.Cantidad * indice[ped.ProductoId].Precio;
    }

    reporte.Add(new ReporteCategoria(
        Categoria:   categoria, 
        CantPedidos: cantPedidos, 
        Unidades:    unidades, 
        MontoTotal:  montoTotal ));
}
```
---

## Paso 5: Ordenar por monto
```csharp
reporte.Sort((a, b) 
    => b.MontoTotal.CompareTo(a.MontoTotal));
```

---

## Solución LINQ — 10 líneas

```csharp
var reporte = pedidos
    .Where(ped => ped.Estado == "aprobado"
               && ped.Fecha >= DateTime.Today.AddMonths(-1))
    .Join(productos,
          ped  => ped.ProductoId,
          prod => prod.Id,
          (ped, prod) => new { ped, prod })
    .GroupBy(x => x.prod.Categoria)
    .Select(g => new ReporteCategoria(
        Categoria:   g.Key,
        CantPedidos: g.Count(),
        Unidades:    g.Sum(x => x.ped.Cantidad),
        MontoTotal:  g.Sum(x => x.ped.Cantidad * x.prod.Precio)
    ))
    .OrderByDescending(r => r.MontoTotal)
    .ToList();
```

---

<!-- _class: seccion -->

## LINQ y programación funcional
### La conexión profunda

---

## Inmutabilidad

LINQ **nunca modifica** la colección original.
Cada método devuelve una nueva secuencia.

```csharp
var original = new List<int> { 3, 1, 4, 1, 5, 9 };

var ordenado = original.OrderBy(x => x);    // nueva secuencia
var filtrado = original.Where(x => x > 3);  // nueva secuencia

// original no cambió
Console.WriteLine(string.Join(", ", original));
// 3, 1, 4, 1, 5, 9
```

Esto se llama **pureza funcional**: las funciones no tienen efectos secundarios sobre sus entradas.

---

## Streams potencialmente infinitos

La evaluación diferida permite trabajar con secuencias que **nunca terminan**:

```csharp
// Generador infinito — yield return produce elementos bajo demanda
static IEnumerable<int> NumerosPrimos() {
    yield return 2;
    int n = 3;
    while (true) {
        bool esPrimo = Enumerable.Range(2, (int)Math.Sqrt(n))
        .All(i => n % i != 0);
        if (esPrimo) yield return n;
        n += 2;
    }
}
// Take() materializa solo los primeros 10 — el generador se detiene ahí
var primeros10 = NumerosPrimos().Take(10).ToList();
// [2, 3, 5, 7, 11, 13, 17, 19, 23, 29]
```

---

<!-- _class: seccion -->

## Sintaxis de consulta
### La forma similar a SQL

---

## Dos formas, mismo resultado

El compilador convierte la **sintaxis de consulta** a **sintaxis de métodos** automáticamente.
No hay diferencia de rendimiento — son dos formas de escribir lo mismo.

<div class="columns">

<div>

```csharp
// Sintaxis de consulta
var nombres =
    from p in productos
    where p.Categoria == "Periféricos"
       && p.Stock > 0
    orderby p.Precio descending
    select p.Nombre;
```

</div>

<div>

```csharp
// Sintaxis de métodos
var nombres = productos
    .Where(p =>
        p.Categoria == "Periféricos"
        && p.Stock > 0)
    .OrderByDescending(p => p.Precio)
    .Select(p => p.Nombre);
```

</div>
</div>

---

## Palabras clave disponibles

| Consulta SQL-like | Método equivalente |
|---|---|
| `from x in fuente` | — (punto de entrada) |
| `where condición` | `.Where(x => condición)` |
| `select expresión` | `.Select(x => expresión)` |
| `orderby clave` | `.OrderBy(x => clave)` |
| `orderby clave descending` | `.OrderByDescending(x => clave)` |
| `group x by clave into g` | `.GroupBy(x => clave)` |
| `join ... on ... equals ...` | `.Join(...)` |
| `let variable = expresión` | `.Select(x => new { x, variable })` |

---

## `let` — variable intermedia en la consulta

```csharp
// Sintaxis de consulta — let calcula una vez y reutiliza
var filtrados =
    from p in productos
    let precioConIva = p.Precio * 1.21m
    where precioConIva < 10_000
    select new { p.Nombre, PrecioConIva = precioConIva };

// Sintaxis de métodos — necesita proyección intermedia
var filtrados = productos
    .Select(p => new { p.Nombre, PrecioConIva = p.Precio * 1.21m })
    .Where(x => x.PrecioConIva < 10_000);
```

---

## Lo que la sintaxis de consulta NO puede expresar

```csharp
// Estos métodos solo existen en sintaxis de métodos:
productos.Take(5)
productos.Skip(10)
productos.FirstOrDefault(p => p.Precio < 5_000)
productos.Any(p => p.Activo)
productos.Distinct()
productos.DistinctBy(p => p.Categoria)
productos.SelectMany(p => p.Tags)

// Solución: mezclar ambas sintaxis
var resultado = (
    from p in productos
    where p.Activo
    orderby p.Nombre
    select p
).Take(10).ToList();  // ← método encadenado al final
```

---

## ¿Cuándo usar cada sintaxis?

| Situación | Recomendación |
|---|---|
| Consultas con `join` y `group by` | **Consulta** — más legible |
| Filtrado y proyección simples | **Métodos** — más concisa |
| `Take`, `Any`, `Distinct`, etc. | **Métodos** — obligatorio |
| Encadenamiento largo | **Métodos** — más natural |
| Proyectos C# modernos | **Métodos** — convención predominante |

<br>

> En la práctica, los proyectos C# usan casi exclusivamente la sintaxis de métodos.
> La sintaxis de consulta existe y es válida, pero es infrecuente en código moderno.

---

<!-- _class: seccion -->

## Mejores prácticas

---

## Reglas fundamentales

**`Any()` en lugar de `Count() > 0`**
```csharp
if (pedidos.Any(p => p.Estado == "pendiente")) { ... }   // ✓ para al encontrar el 1°
if (pedidos.Count(p => p.Estado == "pendiente") > 0) { }  // ✗ cuenta todos
```

**Filtrar antes de transformar**
```csharp
productos.Where(p => p.Activo).Select(p => p.Nombre.ToUpper())  // ✓
productos.Select(p => p.Nombre.ToUpper()).Where(n => n.Length > 5) // ✗
```

**Extraer lambdas complejas a métodos**
```csharp
bool EsPedidoValido(Pedido p) =>
    p.Estado == "aprobado" && p.Fecha >= hace30Dias && p.Monto > 5_000;

var resultado = pedidos.Where(EsPedidoValido);   // ✓ legible y testeable
```

---

## Reglas de materialización y efectos

**Materializar una vez, reutilizar**
```csharp
var activos = productos.Where(p => p.Activo).ToList();  // ✓ ejecuta una vez
```

**Sin efectos secundarios en lambdas**
```csharp
// ✗ — efecto secundario: modifica 'log' en una lambda
var precios = productos.Select(p => { log.Add(p.Nombre); return p.Precio; });
```

--- 

**Separar transformación de efectos secundarios**
```csharp
// ✓ — separar transformación de efectos secundarios
var precios = productos.Select(p => p.Precio).ToList();
var log     = productos.Select(p => p.Nombre).ToList();
```

**Usar el método más específico**
```csharp
var masBarato = productos.MinBy(p => p.Precio);        // ✓
var masBarato = productos.OrderBy(p => p.Precio).First(); // ✗ ordena todo para elegir uno
```

---

## Resumen — el mapa completo

| Operación | Métodos |
|---|---|
| **Filtrar** | `Where`, `Distinct`, `DistinctBy`, `Take`, `Skip` |
| **Transformar** | `Select`, `SelectMany` |
| **Ordenar** | `OrderBy`, `ThenBy` (+ `Descending`) |
| **Agregar** | `Sum`, `Count`, `Min`, `Max`, `Average`, `Aggregate` |
| **Buscar** | `First`, `Last`, `Single` (+ `OrDefault`) |
| **Verificar** | `Any`, `All` |
| **Agrupar** | `GroupBy` |
| **Unir** | `Join`, `GroupJoin`, `Zip`, `Concat`, `Union` |
| **Materializar** | `ToList`, `ToArray`, `ToDictionary`, `ToHashSet` |

---

<!-- _class: portada -->

# LINQ

<div class="subtitle">
Select · Where · Aggregate
</div>

<div class="meta">
La fuente nunca se modifica · Los métodos construyen planes · ToList() ejecuta<br><br>
<code>map = Select &nbsp;·&nbsp; filter = Where &nbsp;·&nbsp; reduce = Aggregate</code>
</div>
