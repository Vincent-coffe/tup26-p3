# Tipos de datos

## Objetivo de la clase

En esta clase la idea es profundizar una pregunta que ya apareció al hablar de variables:

> ¿Qué clase de dato estamos guardando y qué consecuencias tiene eso en el programa?

No alcanza con saber que una variable “tiene un tipo”. También hace falta entender:

- qué tipos existen,
- cómo se comportan,
- qué operaciones permiten,
- cuándo conviene usar uno u otro,
- y cómo impactan en memoria, conversiones y diseño del código.

Esta clase funciona como puente entre la sintaxis básica y una comprensión más sólida del modelo de tipos de C#.

## Idea de apertura

Una buena pregunta para empezar es esta:

> ¿Es lo mismo guardar un `10`, guardar un `"10"` y guardar algo que podría no tener valor?

La respuesta obvia es “no”, pero vale la pena usar esa diferencia para introducir una idea central:

> en programación, el tipo de un dato no solo describe su forma, también condiciona las operaciones que tienen sentido sobre él.

No sumamos igual números, textos, referencias, valores nulos o colecciones. Si el estudiante internaliza eso, después entiende mucho mejor por qué el compilador acepta algunas cosas y rechaza otras.

## Tipos primitivos

Los tipos primitivos son los ladrillos más básicos con los que se construyen muchos programas.

En una primera etapa conviene trabajar especialmente con:

- `int`
- `double`
- `bool`
- `char`

### `int`

Se usa para representar números enteros.

```cs
int edad = 20;
int cantidad = 150;
```

Sirve cuando el dato no necesita parte decimal.

### `double`

Se usa para representar números reales o con decimales.

```cs
double precio = 19.99;
double altura = 1.75;
```

Es importante remarcar que trabajar con decimales en computación no siempre significa precisión matemática perfecta. Más adelante eso será relevante cuando se hablen temas financieros o comparaciones numéricas.

## Literales y cómo reconocer su tipo

En C# muchas veces el tipo numérico puede inferirse a partir de **cómo está escrito el literal**.

Un **literal** es el valor escrito directamente en el código.

Por ejemplo:

```cs
10
3.14
0xFF
0b1010
2.5m
"Hola"
'A'
true
```

No todos representan números: algunos son enteros, otros reales, y otros corresponden a texto o valores lógicos.

### Cómo identificar los tipos más comunes

Una regla práctica muy útil es mirar la forma del literal:

- si está entre comillas dobles, es `string`
- si está entre comillas simples y tiene un solo carácter, es `char`
- si es `true` o `false`, es `bool`
- si tiene solo dígitos enteros, normalmente es `int`
- si tiene punto decimal, normalmente es `double`
- si termina en `m` o `M`, es `decimal`
- si termina en `f` o `F`, es `float`
- si termina en `l` o `L`, es `long`
- si empieza con `0x`, está en hexadecimal
- si empieza con `0b`, está en binario

### Literales enteros decimales

Cuando se escribe un entero sin prefijo ni sufijo, C# intenta interpretarlo como `int`.

```cs
int edad = 20;
var cantidad = 150;
```

Acá `20` y `150` se interpretan como enteros decimales.

Si el número no entra en un `int`, el compilador puede considerar tipos enteros más grandes según el literal y el contexto, pero para una primera aproximación conviene enseñar esta regla práctica:

> un entero “normal”, sin adornos, suele ser un `int`.

### Literales reales

Cuando el literal tiene parte decimal, C# por defecto lo interpreta como `double`.

```cs
double precio = 19.99;
var altura = 1.75;
```

También puede escribirse usando notación científica:

```cs
double numeroGrande = 1.2e6;
double numeroPequeno = 4.5e-3;
```

Eso significa:

- `1.2e6` → $1.2 \times 10^6$
- `4.5e-3` → $4.5 \times 10^{-3}$

### Sufijos para indicar el tipo

Cuando se quiere dejar más explícito el tipo del literal, pueden usarse sufijos.

Algunos de los más comunes son:

- `f` o `F` para `float`
- `d` o `D` para `double`
- `m` o `M` para `decimal`
- `l` o `L` para `long`
- `u` o `U` para `uint`

Ejemplos:

```cs
float temperatura = 25.5f;
double promedio = 8.75d;
decimal saldo = 1500.25m;
long poblacion = 5000000L;
uint codigo = 123U;
```

### Qué conviene remarcar

Este punto es especialmente importante con `decimal`.

```cs
decimal precio = 10.5m;
```

Sin la `m`, ese literal sería interpretado como `double`, y el compilador no lo convierte automáticamente a `decimal` en todos los casos.

### Representación hexadecimal

C# permite escribir enteros en hexadecimal usando el prefijo `0x`.

```cs
int color = 0xFF;
int mascara = 0x0F;
```

En hexadecimal se usan los símbolos:

- `0` a `9`
- `A` a `F`

donde:

- `A` vale `10`
- `B` vale `11`
- `C` vale `12`
- `D` vale `13`
- `E` vale `14`
- `F` vale `15`

Entonces:

- `0xFF` representa `255` en decimal,
- `0x0F` representa `15` en decimal.

### Representación binaria

También se pueden escribir enteros en binario usando el prefijo `0b`.

```cs
int ocho = 0b1000;
int diez = 0b1010;
```

En binario solo se usan `0` y `1`.

Esto resulta útil para:

- entender cómo se representan los datos a bajo nivel,
- trabajar con banderas,
- y explicar operadores bit a bit más adelante.

### Separadores visuales en literales

Para hacer más legibles algunos números, C# permite usar `_` como separador visual.

```cs
int millones = 1_000_000;
int mascara = 0b1111_0000;
int color = 0xFF_EC_DE_5E;
```

Ese guion bajo no cambia el valor. Solo hace que el literal sea más fácil de leer.

### Idea práctica para clase

Una forma útil de resumir esto es:

- si tiene solo dígitos enteros, normalmente se interpreta como `int`,
- si tiene coma flotante, normalmente se interpreta como `double`,
- si tiene sufijo, el sufijo manda,
- si empieza con `0x`, está en hexadecimal,
- si empieza con `0b`, está en binario.

Y si el literal está entre comillas o usa una palabra reservada como `true`/`false`, ya no estamos frente a un número, sino frente a texto o un valor lógico.

Es una simplificación útil para empezar a leer código sin que cada número parezca una especie nueva.

### `bool`

Representa valores lógicos: verdadero o falso.

```cs
bool activo = true;
bool esMayor = false;
```

Se usa mucho en condiciones, validaciones y control de flujo.

### `char`

Representa un único carácter.

```cs
char inicial = 'A';
char separador = '-';
```

Conviene remarcar la diferencia entre:

- `'A'` → un `char`,
- `"A"` → un `string`.

Parece mínimo, pero ahí ya aparecen dos tipos muy distintos.

### `char` y Unicode

En C#, un `char` representa un carácter usando el estándar **Unicode**, que busca dar una codificación común para textos de muchísimos idiomas y símbolos.

La idea importante para una primera clase es esta:

- `char` no está pensado solo para letras inglesas,
- también puede representar muchos otros caracteres del mundo textual,
- como letras acentuadas, signos, símbolos y muchos caracteres de otros alfabetos.

Por ejemplo:

```cs
char letra = 'ñ';
char simbolo = '€';
```

Esto ayuda a entender que las computadoras no “guardan letras” de forma mágica, sino códigos numéricos que después se interpretan como caracteres según un estándar.

No hace falta entrar acá en todos los detalles de codificación, UTF-16 o pares suplentes. Para este nivel alcanza con dejar la idea de que `char` trabaja sobre Unicode y que el texto en C# no se limita al ASCII clásico.

## Operadores y precedencia

Los tipos no viven aislados: se usan junto con operadores para producir resultados.

### Operadores aritméticos

Con números suelen aparecer operadores como:

- `+`
- `-`
- `*`
- `/`
- `%`

```cs
int a = 10;
int b = 3;

int suma = a + b;
int resta = a - b;
int producto = a * b;
int cociente = a / b;
int resto = a % b;
```

Acá conviene mostrar que `10 / 3` con enteros no da `3.333...`, sino `3`, porque el tipo influye en el resultado de la operación.

### Operadores relacionales

Permiten comparar valores:

- `==`
- `!=`
- `>`
- `<`
- `>=`
- `<=`

> Nota: El `==` suele tener un trato especial, las clases implementar Equals y el operador de igualdad para definir qué significa que dos objetos sean iguales. Eso es un tema más avanzado, pero es bueno dejar la idea de que no siempre “igual” es lo mismo en distintos tipos.
> Nota 2: Las comparaciones de relacion `<`, `>`, suelen ser una forma comoda de usar IComparable, pero no todos los tipos las implementan. Por ejemplo, no se pueden comparar dos objetos de una clase sin definir esa lógica.

```cs
bool resultado = 10 > 5;
```

### Operadores lógicos

Se usan con expresiones booleanas:

- `&&` (y)
- `||` (o)
- `!` (no)

```cs
bool puedeEntrar = edad >= 18 && activo;
```

### Precedencia

No todas las operaciones se evalúan en cualquier orden. La **precedencia** indica qué operadores se resuelven primero.

```cs
int resultado = 2 + 3 * 4;
```

No da `20`, sino `14`, porque la multiplicación tiene mayor precedencia que la suma.

Si se quiere dejar explícito el orden, conviene usar paréntesis:

```cs
int resultado = (2 + 3) * 4;
```

## `string`, interpolación y verbatim strings

`string` merece una sección propia porque es uno de los tipos más usados y, al mismo tiempo, uno de los que más engañan al principio.

### Qué es `string`

Representa texto.

```cs
string nombre = "Ada";
string mensaje = "Hola mundo";
```

Aunque a simple vista parece “un conjunto de caracteres”, en C# `string` es un tipo de referencia con comportamiento especial y muy optimizado.

### Concatenación

Una forma simple de construir texto es usando `+`.

```cs
string saludo = "Hola, " + nombre;
```

Funciona, pero no siempre es la forma más clara.

### Interpolación

La interpolación permite insertar expresiones dentro de una cadena usando `$`.

```cs
string nombre = "Ada";
int edad = 20;

string mensaje = $"{nombre} tiene {edad} años";
```

Suele ser más legible que concatenar muchos fragmentos.

### Verbatim strings

Los **verbatim strings** se escriben con `@` y permiten:

- usar saltos de línea literales,
- escribir rutas o textos sin escapar tantas barras invertidas.

```cs
string ruta = @"C:\Users\Ada\Documentos";
string texto = @"Línea 1
Línea 2";
```

### Strings multilínea

En C# un texto multilínea puede escribirse de distintas maneras, pero para una introducción conviene mostrar dos caminos simples.

#### Opción 1: verbatim string con `@`

Si se usa `@`, el texto puede escribirse ocupando varias líneas directamente en el código.

```cs
string mensaje = @"Hola
este texto
ocupa varias líneas";
```

En este caso, los saltos de línea escritos en el código forman parte del contenido del `string`.

#### Opción 2: secuencias de escape

También puede construirse un string multilínea usando `\n` para indicar un salto de línea.

```cs
string mensaje = "Hola\neste texto\nocupa varias líneas";
```

Esto es útil cuando el texto se arma de manera más controlada o cuando no conviene escribirlo visualmente en varias líneas dentro del código.

### Qué conviene remarcar

Ambas opciones sirven, pero no se leen igual:

- con `@`, el texto queda más parecido a cómo se verá realmente,
- con `\n`, el programador controla explícitamente dónde aparecen los saltos.

Para materiales de clase, ejemplos largos o textos visibles, el verbatim string suele ser más claro.

También puede combinarse `@` con `$`.

```cs
string carpeta = "Documentos";
string ruta = $@"C:\Users\Ada\{carpeta}";
```

### Conversion automatica a `string`

En C#, muchos tipos pueden convertirse automáticamente a `string` cuando se usan en un contexto de texto.

```cs
int numero = 42;
string mensaje = "El número es: " + numero;
```

En ese caso, el `int` se convierte a su representación textual sin que el programador tenga que hacer nada explícito.
Esto se debe a que todas las clases del sistema heredan de `object`, que tiene un método `ToString()`. Cuando se necesita un `string`, el sistema llama a ese método para obtener la representación textual del objeto. Por eso, aunque `numero` es un `int`, se puede usar en una concatenación de `string` y el resultado es un texto que representa ese número.  

Esto tambien puede funcionar en las clases que definamos nosotros mismos, siempre que implementemos el método `ToString()` para devolver una representación adecuada del objeto.

### Inmutabilidad

Una idea muy importante es esta:

> los `string` son **inmutables**.

Eso significa que, una vez creado un texto, su contenido no se modifica. Cuando parece que cambiamos un `string`, en realidad se crea uno nuevo.

```cs
string nombre = "Ana";
nombre = nombre + " María";
```

No se “edita” el objeto original carácter por carácter. Se genera un nuevo texto y luego la variable apunta a ese nuevo valor.

**

## Tipos por valor y por referencia

Esta distinción es una de las más importantes de todo C#.

### Tipos por valor

Cuando se copia un tipo por valor, se copia el dato.

```cs
int a = 10;
int b = a;
b = 20;
```

Después de eso:

- `a` sigue valiendo `10`,
- `b` vale `20`.

Cada variable tiene su propia copia.

### Tipos por referencia

Cuando se copia un tipo por referencia, normalmente se copia la referencia al mismo objeto.

```cs
var persona1 = new Persona();
var persona2 = persona1;
```

En ese caso, ambas variables apuntan al mismo objeto.

Si ese objeto cambia, el cambio puede verse desde las dos variables.

### Qué conviene fijar

La diferencia importante no es “uno vive en stack y otro en heap” como fórmula mágica simplificada, sino:

- cómo se copia,
- cómo se comparte,
- y cómo se comporta al pasarlo a métodos o reasignarlo.

### Casos típicos

En general:

- `int`, `double`, `bool`, `char` y `struct` suelen ser tipos por valor,
- `string`, arrays, clases, listas y objetos creados con `new` suelen ser tipos por referencia.

### Ojo con `string`

`string` es tipo por referencia, pero por su inmutabilidad no siempre se percibe como otros objetos mutables. Eso suele confundir bastante al principio.

## Nullable types, coalescencia y null-forgiving operator

El valor `null` representa la ausencia de un objeto o de un valor válido en ciertos contextos.

Trabajar con `null` es importante porque muchos errores clásicos aparecen justamente cuando se intenta usar algo que en realidad no existe.

### Nullable reference types

En C# moderno se puede expresar si una referencia puede ser nula o no.

```cs
string nombre = "Ada";
string? apodo = null;
```

Acá:

- `string` indica que la referencia no debería ser nula,
- `string?` indica que puede ser nula.

Esto permite que el compilador ayude a detectar posibles problemas antes de ejecutar.

### Nullable value types

Algunos tipos por valor también pueden volverse anulables usando `?`.

```cs
int? edad = null;
double? precio = 10.5;
```

Esto es útil cuando hace falta representar “todavía no hay dato”.

### Operador de coalescencia nula `??`

Permite usar un valor alternativo cuando la expresión de la izquierda es `null`.

```cs
string? nombre = null;
string texto = nombre ?? "Sin nombre";
```

Si `nombre` es `null`, el resultado será `"Sin nombre"`.

### Operador de asignación por coalescencia `??=`

Permite asignar un valor solo si la variable todavía es `null`.

```cs
string? titulo = null;
titulo ??= "Título por defecto";
```

### Null-forgiving operator `!`

El operador `!` le dice al compilador: “confiá en mí, acá no va a ser `null`”.

```cs
string? nombre = ObtenerNombre();
Console.WriteLine(nombre!.Length);
```

### Precaución importante

Ese operador **no vuelve mágico al dato** ni evita errores reales. Solo silencia la advertencia del compilador.

Si el valor era realmente `null`, el problema sigue existiendo y puede fallar en ejecución.

Por eso conviene usarlo solo cuando haya una razón bien justificada.

## Conversión de tipos, casting y `as`

No siempre un dato viene en el tipo que necesitamos. Por eso existe la conversión de tipos.

### Conversión implícita

Ocurre cuando la conversión es segura y el compilador la permite automáticamente.

```cs
int numero = 10;
double resultado = numero;
```

Pasar de `int` a `double` suele ser seguro.

### Conversión explícita o casting

Cuando la conversión puede implicar pérdida de información o no es obviamente segura, hay que indicarlo explícitamente.

```cs
double precio = 19.99;
int entero = (int)precio;
```

Acá se pierde la parte decimal.

### Conversión con métodos auxiliares

También existen métodos como:

- `int.Parse`
- `double.Parse`
- `Convert.ToInt32`
- `TryParse`

Por ejemplo:

```cs
string texto = "42";
int numero = int.Parse(texto);
```

O mejor aún, si se quiere evitar errores por entrada inválida:

```cs
string texto = "42";
bool ok = int.TryParse(texto, out int numero);
```

### `as`

El operador `as` se usa con referencias o tipos anulables para intentar una conversión sin lanzar excepción si falla.

```cs
object dato = "hola";
string? texto = dato as string;
```

Si la conversión no funciona, el resultado será `null`.

### Diferencia importante

- el casting tradicional puede fallar con una excepción si no es válido,
- `as` devuelve `null` cuando no puede convertir.

No compiten entre sí: sirven para escenarios distintos.

## Arrays, tuplas y tipos anónimos

Hasta ahora vimos tipos simples. Ahora conviene introducir algunas estructuras livianas para agrupar o almacenar datos.

### Arrays

Un array permite guardar varios elementos del mismo tipo.

```cs
int[] numeros = new int[] { 1, 2, 3, 4 };
string[] nombres = new string[] { "Ana", "Luis", "Ada" };
```

También puede usarse una sintaxis más compacta:

```cs
int[] numeros = { 1, 2, 3, 4 };
```

### Qué conviene remarcar

- todos los elementos tienen el mismo tipo,
- el tamaño queda definido al crearlo,
- los índices empiezan en `0`.

```cs
int primerNumero = numeros[0];
```

Los arrays son fundamentales porque introducen la idea de colección ordenada y acceso por posición.

### Cómo acceder a los elementos

El acceso a un elemento se hace usando corchetes e indicando su índice.

```cs
int[] numeros = { 10, 20, 30, 40 };

int primero = numeros[0];
int segundo = numeros[1];
int ultimo = numeros[3];
```

Acá conviene remarcar dos cosas muy importantes:

- el primer elemento está en la posición `0`,
- intentar acceder a una posición inexistente produce un error en tiempo de ejecución.

Por ejemplo, si un array tiene 4 elementos, sus índices válidos son:

- `0`
- `1`
- `2`
- `3`

Intentar hacer esto sería incorrecto:

```cs
// numeros[4]
```

porque esa posición no existe.

### Cómo modificar elementos

También se puede escribir sobre una posición existente.

```cs
int[] numeros = { 10, 20, 30 };
numeros[1] = 99;
```

Después de eso, el array queda conceptualmente así:

- `10`
- `99`
- `30`

Es importante distinguir esto de los `string`: los arrays sí permiten modificar sus elementos.

### Recorrer un array

Una de las operaciones más comunes es recorrer todos sus elementos.

#### Con `for`

```cs
int[] numeros = { 10, 20, 30, 40 };

for (int i = 0; i < numeros.Length; i++)
{
    Console.WriteLine(numeros[i]);
}
```

Esto es útil cuando hace falta trabajar con la posición de cada elemento.

#### Con `foreach`

```cs
int[] numeros = { 10, 20, 30, 40 };

foreach (int numero in numeros)
{
    Console.WriteLine(numero);
}
```

Esto es más cómodo cuando solo interesa leer cada valor, sin preocuparse por el índice.

### Propiedades y funciones básicas

Aunque un array no es una colección dinámica como `List<T>`, tiene operaciones y utilidades básicas muy importantes.

#### `Length`

Indica cuántos elementos tiene el array.

```cs
int[] numeros = { 10, 20, 30, 40 };
int cantidad = numeros.Length;
```

Esto devuelve `4`.

#### `Array.Sort`

Permite ordenar los elementos.

```cs
int[] numeros = { 4, 1, 3, 2 };
Array.Sort(numeros);
```

Después de eso, el array queda ordenado.

#### `Array.Reverse`

Permite invertir el orden de los elementos.

```cs
int[] numeros = { 1, 2, 3, 4 };
Array.Reverse(numeros);
```

#### `Array.IndexOf`

Permite buscar en qué posición está un elemento.

```cs
int[] numeros = { 10, 20, 30 };
int posicion = Array.IndexOf(numeros, 20);
```

En este caso, `posicion` vale `1`.

Si el elemento no existe, devuelve `-1`.

#### `Array.Exists`

Permite verificar si existe algún elemento que cumpla cierta condición.

```cs
int[] numeros = { 10, 20, 30 };
bool hayMayores = Array.Exists(numeros, n => n > 25);
```

#### `Array.Copy`

Permite copiar elementos de un array a otro.

```cs
int[] origen = { 1, 2, 3 };
int[] destino = new int[3];

Array.Copy(origen, destino, 3);
```

### Qué idea debería quedar

Un array sirve para:

- guardar varios datos del mismo tipo,
- acceder por posición,
- recorrer secuencias de valores,
- y aplicar operaciones básicas de orden, búsqueda o copia.

No es la colección más flexible del ecosistema, pero sí una de las más importantes para entender cómo se organizan datos en memoria y cómo se trabaja con secuencias.

### Por qué muchas funciones de arrays son estáticas

Un detalle que suele llamar la atención es que varias operaciones sobre arrays aparecen como funciones estáticas de `Array`:

```cs
Array.Sort(numeros);
Array.Reverse(numeros);
Array.IndexOf(numeros, 20);
```

y no como métodos de instancia del estilo:

```cs
// numeros.Sort()
```

La intuición principal es esta:

- un array es un tipo especial del runtime,
- tiene algunas propiedades propias, como `Length`,
- pero muchas operaciones útiles son algoritmos generales que pueden aplicarse a cualquier array.

Por eso `.NET` concentra varias de esas herramientas en la clase `Array`, que funciona como una utilidad común.

Una forma simple de explicarlo es:

> `Length` describe al array que tengo delante; ordenar, copiar o buscar son operaciones generales que pueden aplicarse sobre cualquier array.

Esto también ayuda a diferenciar arrays de otras colecciones como `List<T>`, que sí tienen una API más rica de métodos de instancia porque fueron diseñadas como estructuras dinámicas y más flexibles.

### Tuplas

Una tupla permite agrupar varios valores, posiblemente de distintos tipos, sin necesidad de definir una clase completa.

```cs
var persona = (Nombre: "Ada", Edad: 20, Activa: true);
```

Después se puede acceder así:

```cs
Console.WriteLine(persona.Nombre);
Console.WriteLine(persona.Edad);
```

### Cuándo sirven

Son útiles para:

- devolver varios valores desde un método,
- agrupar datos simples de forma rápida,
- hacer pruebas o prototipos.

Si la estructura empieza a tener demasiada lógica o demasiado significado de dominio, probablemente convenga crear una clase o un `record`.

### Tipos anónimos

Un tipo anónimo permite crear un objeto con propiedades sin declarar previamente una clase con nombre.

```cs
var alumno = new { Nombre = "Ana", Edad = 21 };
```

El compilador genera internamente un tipo apropiado para esa forma.

### Cuándo aparecen mucho

Son muy comunes en:

- consultas LINQ,
- proyecciones temporales,
- transformaciones de datos internas.

### Limitación conceptual

Son cómodos para uso local, pero no para diseñar contratos públicos del sistema. Si algo necesita identidad clara y reutilización, mejor darle un tipo con nombre.

## Demo sugerida para clase

Una secuencia útil para explicar este bloque puede ser:

1.  declarar variables primitivas y comparar resultados de operaciones,
2.  mostrar precedencia con y sin paréntesis,
3.  construir textos con concatenación, interpolación y verbatim strings,
4.  comparar copia de un `int` con copia de una referencia,
5.  usar `??` para manejar un posible `null`,
6.  convertir un `string` a `int` con `Parse` y `TryParse`,
7.  crear un array, una tupla y un tipo anónimo.

## Preguntas para hacer al curso

- ¿Qué diferencia hay entre `char` y `string`?
- ¿Por qué `10 / 3` no siempre da el resultado que alguien espera matemáticamente?
- ¿Qué significa que un `string` sea inmutable?
- ¿Qué cambia entre copiar un valor y copiar una referencia?
- ¿Cuándo conviene usar `TryParse` en lugar de `Parse`?
- ¿Qué ventaja tiene `??` frente a un `if` simple?

## Cierre integrador

Hablar de tipos de datos no es memorizar una lista de nombres raros del lenguaje. Es entender cómo representa el programa la información y qué reglas usa para operar con ella.

En esta clase aparecen varias ideas fundamentales:

- tipos numéricos y lógicos,
- texto e inmutabilidad,
- diferencia entre valor y referencia,
- manejo de `null`,
- conversiones,
- y estructuras livianas para agrupar datos.

Si esto queda claro, después el estudiante puede leer código con más criterio y cometer menos errores de esos que parecen pequeños hasta que rompen todo con elegancia.

## Resumen final para proyectar

- los tipos primitivos permiten representar números, valores lógicos y caracteres,
- los operadores trabajan sobre tipos y respetan reglas de precedencia,
- `string` es inmutable y tiene herramientas cómodas como interpolación y verbatim strings,
- no es lo mismo copiar un valor que copiar una referencia,
- `null` debe tratarse con cuidado y el lenguaje ofrece herramientas para hacerlo,
- convertir tipos puede ser seguro, explícito o fallar si se usa mal,
- arrays, tuplas y tipos anónimos ayudan a organizar datos sin ir todavía a estructuras más complejas.