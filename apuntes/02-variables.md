# Declaración de variables

## Introducción

Las variables son mucho más que "nombres para guardar cosas". Son una parte central del modelo mental de cualquier programa.

No alcanza con saber escribir `int edad = 20;`. También hace falta comprender:

- qué información guarda una variable,
- qué se puede hacer con esa información,
- dónde existe,
- cuánto tiempo vive,
- y cómo se relaciona con la memoria del programa.

Si esta base queda clara temprano, después resulta mucho más fácil entender funciones, objetos, colecciones y paso de parámetros.

## Qué es una variable

Una variable es un nombre asociado a un dato que el programa puede usar durante su ejecución.

Ese dato tiene un **tipo**, y ese tipo determina:

- qué valores son válidos,
- qué operaciones se pueden hacer,
- cuánto espacio suele ocupar,
- y cómo debe interpretarlo el compilador.

En C#, toda variable debe declararse antes de usarse.

## C# como lenguaje estático y de tipado fuerte

> C# es un lenguaje de **tipado estático** y **tipado fuerte**.

### Tipado estático

Decir que C# tiene **tipado estático** significa que el tipo de cada variable se conoce y se controla antes de ejecutar el programa, principalmente durante la compilación.

Por ejemplo:

```cs
int edad = 20;
edad = 30;
```

Eso es válido porque ambos valores son `int`.

Pero esto no lo es:

```cs
int edad = 20;
edad = "treinta";
```

El compilador detecta el problema antes de que el programa corra. Una variable puede cambiar de **valor**, pero no de **tipo**.

### Tipado fuerte

Decir que C# tiene **tipado fuerte** significa que el lenguaje no mezcla tipos incompatibles de manera arbitraria ni permite conversiones implícitas peligrosas.

Por ejemplo, si una operación espera números, no se puede pasar texto como si nada:

```cs
int numero = 10;
string texto = "20";

// Esto no funciona automáticamente.
// numero = texto;
```

Si el programador quiere convertir un valor, debe hacerlo de forma explícita:

```cs
int numero = int.Parse(texto);
```

Este enfoque ayuda a detectar errores antes de ejecutar, escribir código más predecible y reducir comportamientos ambiguos.

## Sintaxis básica

La forma general de declarar una variable es:

```cs
int edad = 20;
string nombre = "Ana";
bool activo = true;
double precio = 15.5;
```

En cada declaración aparece:

1.  el **tipo**,
2.  el **nombre de la variable**,
3.  y opcionalmente una **inicialización**.

### Declarar vs. inicializar

Declarar e inicializar no son exactamente lo mismo:

```cs
int cantidad;
cantidad = 10;
```

Primero se declara la variable y después se le asigna un valor. Una variable puede cambiar de valor a lo largo del programa, pero su tipo no cambia.

## Asignación y actualización

Una vez declarada, una variable puede recibir nuevos valores compatibles con su tipo:

```cs
int contador = 0;
contador = contador + 1;
contador += 1;
```

Una variable no es una "etiqueta fija", sino un elemento cuyo valor puede evolucionar durante la ejecución.

Muchos estudiantes leen `x = x + 1;` como si fuera una igualdad matemática imposible. En programación no significa eso. Significa: tomar el valor actual de `x`, sumarle `1` y guardar el resultado nuevamente en `x`.

## `var` e inferencia de tipos

En C# se puede declarar una variable indicando el tipo de forma explícita:

```cs
int edad = 20;
string ciudad = "Rosario";
```

o usando `var`:

```cs
var edad = 20;
var ciudad = "Rosario";
```

**`var` no significa "tipo dinámico"**. El tipo sigue existiendo y siendo estático; lo que cambia es que el compilador lo **infiere** a partir del valor inicial:

```cs
var cantidad = 20;    // int
var precio = 19.99;   // double
var activo = true;    // bool
```

Esto no contradice que C# sea de tipado estático: `var` solo le evita al programador escribir el tipo cuando el compilador puede deducirlo.

### Cuándo conviene usar `var`

Tiene sentido cuando el tipo es evidente porque evita repetir tipos largos y mejora la legibilidad:

```cs
var alumnos = new List<string>();
```

Puede perjudicar cuando el valor inicial no deja claro el tipo real:

```cs
var dato = ObtenerResultado(); // ¿qué devuelve esto?
```

> Usar `var` cuando el tipo sea evidente; usar tipo explícito cuando ayude a leer mejor.

## Constantes y `readonly`

A veces conviene declarar datos que no deberían modificarse.

### `const`

Una constante se declara con `const`:

```cs
const double Pi = 3.14159;
```

Debe inicializarse al declararse y su valor no puede cambiar. Se usa para valores fijos conocidos en tiempo de compilación.

### `readonly`

`readonly` se usa en campos de una clase o estructura para indicar que el valor solo puede asignarse en la declaración o dentro del constructor:

```cs
class Persona {
    public readonly DateTime FechaCreacion;

    public Persona() {
        FechaCreacion = DateTime.Now;
    }
}
```

`const` y `readonly` comparten la idea de inmutabilidad pero no son sinónimos: `const` es para valores conocidos en compilación; `readonly` es para datos que se fijan una vez al construir el objeto.

## Alcance y tiempo de vida

No todas las variables existen en todos lados ni durante todo el programa.

### Alcance

El **alcance** indica desde qué partes del código una variable puede ser accedida:

```cs
if (true) {
    int numero = 5;
    Console.WriteLine(numero);
}

// Acá `numero` ya no existe.
```

Los bloques delimitan zonas de visibilidad.

### Tiempo de vida

El **tiempo de vida** indica durante cuánto tiempo esa variable existe en memoria durante la ejecución. Por ejemplo:

- una variable local vive mientras se ejecuta el bloque o método donde fue declarada,
- un campo de un objeto vive mientras viva ese objeto,
- un dato estático puede vivir durante toda la ejecución de la aplicación.

```cs
void Saludar() {
    string mensaje = "Hola";
    Console.WriteLine(mensaje);
}
```

La variable `mensaje` nace cuando el método comienza y deja de existir al terminar.

### Recolección de basura

En .NET los objetos no se liberan manualmente. El runtime usa la **recolección de basura** (*garbage collection*): cuando un objeto ya no tiene referencias activas que permitan seguir usándolo, el recolector puede recuperar eventualmente la memoria que estaba ocupando.

El recolector no destruye un objeto en el instante exacto en que deja de usarse, sino cuando el runtime considera conveniente. Que un objeto sea inaccesible significa que **puede** ser recolectado, no que será eliminado de inmediato.

Por eso, al pensar en tiempo de vida no alcanza con ver dónde fue declarada una variable: también hay que considerar si el objeto al que apunta sigue siendo alcanzable desde alguna parte del programa.

## La stack y el heap

Como primera aproximación:

- la **stack** suele asociarse con llamadas a métodos, variables locales y datos de vida corta, con administración automática y muy rápida.
- el **heap** suele asociarse con objetos creados dinámicamente, cuya memoria administra el recolector de basura.

La implementación real del runtime es más sutil que la versión escolar "todo valor va a stack, todo objeto va a heap". La idea clave no es memorizar internals del CLR, sino entender que no toda la memoria se comporta igual y que el tipo de dato influye en cómo se manipulan los valores.

## Tipos por valor y por referencia

> ¿Una variable "contiene" un valor, o representa una referencia a algún lugar donde ese valor está guardado?

Esta distinción es central para entender funciones, objetos, colecciones y paso de parámetros.

### Tipos por valor

Cuando se copia una variable de tipo por valor, se copia el dato:

```cs
int a = 10;
int b = a;
b = 20;
```

Después de esto, `a` sigue valiendo `10` y `b` vale `20`.

### Tipos por referencia

Cuando se trabaja con objetos, lo que suele copiarse es la referencia al objeto, no el objeto en sí. Los arreglos son un buen ejemplo:

```cs
int[] numeros = { 1, 2, 3 };
int[] copia = numeros;

copia[0] = 99;

Console.WriteLine(numeros[0]); // 99
```

Aunque se modificó `copia[0]`, el cambio se ve también en `numeros`, porque ambas variables apuntan al mismo arreglo. No se creó un segundo arreglo al hacer `copia = numeros`.

Para obtener una copia independiente hace falta crearla explícitamente:

```cs
int[] copia = (int[])numeros.Clone();
```

Este comportamiento es común a todos los tipos por referencia.

## Namespaces y clases estáticas para organización

Cuando el código crece, también hay que organizar los tipos dentro de una estructura razonable.

### Namespace

Un `namespace` permite agrupar tipos relacionados y evitar conflictos de nombres:

```cs
namespace MiAplicacion.Utilidades;
```

### Clases estáticas

Una clase estática puede agrupar miembros utilitarios que no dependen de una instancia:

```cs
static class ConstantesDelSistema {
    public const int EdadMinima = 18;
    public static readonly string MensajeBienvenida = "Bienvenido";
}
```

Esto es útil para centralizar constantes, configuraciones simples, funciones auxiliares y valores reutilizables. Agrupar todo en una única clase estática gigantesca suele ser una mala idea: organizar no es amontonar prolijamente.

## Alcance de las variables.

El alcance de una variable es el bloque de codigo donde esa variable es accesible. Por ejemplo:

```cs
void Metodo(int z) {
    Console.WriteLine(z); // z es accesible aquí (Variable de parámetro)
    int x = 10; // x es accesible dentro de Metodo (Variable local)
    if (true) {
        int y = 20; // y es accesible dentro de este bloque if (Variable local)
        Console.WriteLine(x); // x es accesible aquí
    }
    Console.WriteLine(y); // Error: y no es accesible aquí
}
Console.WriteLine(x); // Error: x no es accesible aquí
```

Los parametros de un método tienen alcance dentro de ese método. Las variables locales solo existen dentro del bloque donde fueron declaradas. No se pueden usar fuera de ese bloque.

Las variables locales se destruyen al salir del bloque, mientras que los parámetros existen durante toda la ejecución del método. No existen variables globales en C#, pero se pueden usar campos estáticos para simular ese comportamiento.

```cs
static class Global {
    public static int ContadorGlobal = 0; // Variable global simulada con un campo
}

Console.WriteLine(Global.ContadorGlobal); // Acceso a la variable global
Global.ContadorGlobal++; // Modificación de la variable global
```

En este caso la variable `ContadorGlobal` es accesible desde cualquier parte del programa a través de `Global.ContadorGlobal`.

La vida de una variable estatica es durante toda la ejecución del programa, mientras que las variables locales solo existen durante la ejecución del bloque donde fueron declaradas.

Las variables estaticas se inicializan una sola vez y mantienen su valor durante toda la ejecución, mientras que las variables locales se crean y destruyen cada vez que se ejecuta el bloque donde fueron declaradas.

Cuando una variable estatica se inicializa llamando a una funcion la misma solo se ejecuta una vez, mientras que si se hace lo mismo con una variable local la funcion se ejecuta cada vez que se ejecuta el bloque donde fue declarada la variable local. La ejecución de la función de inicialización de una variable estatica se realiza en un momento determinado por el runtime, generalmente antes de la primera vez que se accede a esa variable o cuando se carga la clase que la contiene.

Una variable de un ambito menor siempre tapa a una variable de un ambito mayor. Por ejemplo:

```cs
int x = 10; // Variable global simulada con un campo
void Metodo() {
    int x = 20; // Variable local que tapa a la variable global
    Console.WriteLine(x); // Imprime 20, no 10
}
```

Las variables por referencia se crear al asignale un valor, mientras que las variables por valor se crean al declararlas. Por ejemplo:

```cs
int x; // Variable por valor, se crea al declararla
int[] arr; // Variable por referencia, se crea al asignarle un valor
arr = new int[5]; // Ahora arr apunta a un nuevo arreglo en el heap
```

Las variables por valor se liberan automáticamente al salir del bloque donde fueron declaradas, mientras que las variables por referencia se liberan cuando el objeto al que apuntan ya no tiene referencias activas y el recolector de basura decide recuperar su memoria.

Las variables por valor son mucho mas rapidas de crear y destruir que las variables por referencia, ya que no requieren la sobrecarga de administrar memoria en el heap ni la intervención del recolector de basura. Sin embargo, las variables por referencia son necesarias para trabajar con objetos complejos y estructuras de datos dinámicas.

## Tipos de datos primitivos

### Enteros

```cs
int i1 = 42;       // Literal entero de 32 bits (int literal)
long i2 = 1234567890123L; // Literal entero de 64 bits (long literal)


short s1 = 30000;   // Literal entero de 16 bits 
byte b1 = 255;      // Literal entero de 8 bits 
unsigned int ui1 = 4000000000u; // Literal entero sin signo de 32 bits 
uint ui2 = 4000000000U; // Literal entero sin signo de 32 bits 
ulong ul1 = 12345678901234567890UL; // Literal entero sin signo de 64 bits 
uint8 u8 = 255; // Literal entero sin signo de 8 bits 
uint16 u16 = 65535; // Literal entero sin signo de 16 bits 
uint64 u64 = 12345678901234567890UL; // Literal entero sin signo de 64 bits 
int8 i8 = 127; // Literal entero de 8 bits 
int16 i16 = 32767; // Literal entero de 16 bits 
int64 i64 = 1234567890123456789L; // Literal entero de 64 bits 

Console.WriteLine($"El tamaño de int es {sizeof(int)} bytes");          // > El tamaño de int es 4 bytes
Console.WriteLine($"El tamaño de long es {sizeof(long)} bytes");        // > El tamaño de long es 8 bytes
Console.WriteLine($"El tamaño de short es {sizeof(short)} bytes");      // > El tamaño de short es 2 bytes
Console.WriteLine($"El tamaño de byte es {sizeof(byte)} bytes");        // > El tamaño de byte es 1 byte
Console.WriteLine($"El tamaño de unsigned int es {sizeof(uint)} bytes");// > El tamaño de unsigned int es 4 bytes
Console.WriteLine($"El tamaño de ulong es {sizeof(ulong)} bytes");      // > El tamaño de ulong es 8 bytes
Console.WriteLine($"El tamaño de uint8 es {sizeof(uint8)} bytes");      // > El tamaño de uint8 es 1 byte
Console.WriteLine($"El tamaño de uint16 es {sizeof(uint16)} bytes");    // > El tamaño de uint16 es 2 bytes
Console.WriteLine($"El tamaño de uint64 es {sizeof(uint64)} bytes");    // > El tamaño de uint64 es 8 bytes
Console.WriteLine($"El tamaño de int8 es {sizeof(int8)} bytes");        // > El tamaño de int8 es 1 byte
Console.WriteLine($"El tamaño de int16 es {sizeof(int16)} bytes");      // > El tamaño de int16 es 2 bytes
Console.WriteLine($"El tamaño de int64 es {sizeof(int64)} bytes");      // > El tamaño de int64 es 8 bytes

### Representación de números enteros

```cs
int i3 = 0xFFFF;    // Literal entero hexadecimal
int i4 = 0b1010;    // Literal entero binario
```

### Operaciones con enteros

```cs
int a = 10;
int b = 3;
```

#### Operadores aritméticos

```cs
int suma = a + b;         // 13
int resta = a - b;        // 7
int multiplicacion = a * b; // 30
int division = a / b;     // 3 (división entera)
int modulo = a % b;       // 1 (resto de la división)
int potencia = (int)Math.Pow(a, b); // 1000 (a elevado a b)
```

#### Operadores de incremento y decremento

```cs
a++; // a ahora es 11   (Posincremento: se incrementa después de usar el valor)
b--; // b ahora es 2    (Posdecremento: se decrementa después de usar el valor)
++a; // a ahora es 12   (Preincremento: se incrementa antes de usar el valor)
--b; // b ahora es 1    (Predescremento: se decrementa antes de usar el valor)
```

#### Operadores de comparación

```cs
bool esIgual = (a == b);        // false
bool esDiferente = (a != b);   // true
bool esMayor = (a > b);        // true
bool esMenor = (a < b);        // false
bool esMayorOIgual = (a >= b); // true
bool esMenorOIgual = (a <= b); // false
```

#### Operadores de bit a bit

```cs
int andBit = a & b; // AND bit a bit
int orBit = a | b;  // OR bit a bit
int xorBit = a ^ b; // XOR bit a bit
int notBit = ~a;    // NOT bit a bit
int shiftLeft = a << 1; // Desplazamiento a la izquierda (multiplicacion entera por 2)
int shiftRight = a >> 1; // Desplazamiento a la derecha (división entera por 2)
```

#### Precedencia de operadores

```cs
int resultado = a + b * 2; // 16, porque la multiplicación se evalúa antes que la suma
int resultado2 = (a + b) * 2; // 26, porque los paréntesis cambian el orden de evaluación
int resultado3 = a + b * 2 > 15 ? 100 : 200; // ((a + (b * 2)) > 15) ? 100 : 200 => resultado3 es 100 porque 16 > 15 es true
```

## Lista de precedencia de operadores

La lista va de **mayor a menor precedencia**. Los paréntesis `()` no forman parte de la tabla: sirven para alterar el orden de evaluación.

1.  Operadores de incremento y decremento `++`, `--`
2.  Operadores unarios `+`, `-`, `!`, `~`
3.  Operadores aritméticos `*`, `/`, `%`
4.  Operadores aritméticos `+`, `-`
5.  Operadores de desplazamiento `<<`, `>>`
6.  Operadores relacionales `<`, `>`, `<=`, `>=`
7.  Operadores de igualdad `==`, `!=`
8.  Operadores bit a bit `&`
9.  Operadores bit a bit `^`
10. Operadores bit a bit `|`
11. Operadores lógicos `&&`
12. Operadores lógicos `||`
13. Operador ternario `?:`
14. Operadores de asignación `=`, `+=`, `-=`, `*=`, `/=`, `%=` y otros

### Asignación compuesta

int x = 10; x += 5; // Equivale a x = x + 5; x ahora es 15 x -= 3; // Equivale a x = x - 3; x ahora es 12 x *= 2; // Equivale a x = x* 2; x ahora es 24 x /= 4; // Equivale a x = x / 4; x ahora es 6 x %= 2; // Equivale a x = x % 2; x ahora es 0

x \<\<= 1; // Equivale a x = x \<\< 1; x ahora es 0 x \>\>= 1; // Equivale a x = x \>\> 1; x ahora es 0 x &= 1; // Equivale a x = x & 1; x ahora es 1 x \|= 2; // Equivale a x = x \| 2; x ahora es 3 x \^= 1; // Equivale a x = x \^ 1; x ahora es 2

### Metodos útiles para enteros

- `Math.Abs(valor)` → valor absoluto.

- `Math.Max(a, b)` → máximo entre a y b.

- `Math.Min(a, b)` → mínimo entre a y b.

- `Math.Pow(base, exponente)` → potencia.

- `Math.Sqrt(valor)` → raíz cuadrada.

- `Math.Round(valor)` → redondeo al entero más cercano.

- `Math.Ceiling(valor)` → techo, redondea hacia arriba.

- `Math.Floor(valor)` → piso, redondea hacia abajo.

- `Math.Truncate(valor)` → quita la parte decimal, devuelve solo la parte entera.

- `valor.ToString("X")` → convierte a hexadecimal.

- `valor.ToString("D")` → convierte a decimal.

- `valor.ToString("B")` → convierte a binario (no es un formato estándar, se puede implementar con código personalizado).

- `int.Parse(cadena)` → convierte una cadena a entero.

- `int.TryParse(cadena, out int resultado)` → intenta convertir una cadena a entero, devuelve true si tuvo éxito y false si no.

- `Convert.ToInt32(valor)` → convierte un valor a entero, con manejo de conversiones entre tipos.

- `BitConverter.GetBytes(valor)` → obtiene un arreglo de bytes que representa el valor entero, útil para manipulación a bajo nivel o serialización.

- 

  ## Tipos de datos primitivos: punto flotante y decimal

Los tipos de punto flotante (`float`, `double`) y el tipo `decimal` se usan para representar números con parte fraccionaria, pero tienen diferencias importantes en precisión y uso recomendado.

- `float` y `double` se apoyan principalmente en `MathF` y `Math`.
- `decimal` se usa cuando importa más la precisión decimal que la velocidad de cálculo.

```cs
float f1 = 3.14f;       // Literal de punto flotante de precisión
var f2 = 2.71828f;     // Literal de punto flotante de precisión con inferencia de tipo
decimal m1 = 1.618m;    // Literal de tipo decimal
var m2 = 0.57721m;     // Literal de tipo decimal con inferencia de tipo

double d4 = 0.1;        // Literal de punto flotante de doble precisión
var d5 = 0.1;          // Literal de punto flotante de doble precisión con inferencia de tipo
```

### Ejemplo de uso

```cs
double raiz = Math.Sqrt(2); // Raíz cuadrada de 2
double potencia = Math.Pow(2, 3); // 2 elevado a la 3
double seno = Math.Sin(Math.PI / 4); // Seno de 45 grados

raiz.ToString("F2"); // Formatear con 2 decimales

double valor = -3.75;
double absoluto = Math.Abs(valor);
double redondeado = Math.Round(valor);
double piso = Math.Floor(valor);
double techo = Math.Ceiling(valor);


### Metodos útiles para flotantes y decimales

- `Math.Abs(valor)` → valor absoluto.
- `Math.Max(a, b)` → máximo entre a y b.
- `Math.Min(a, b)` → mínimo entre a y b.
- `Math.Pow(base, exponente)` → potencia.
- `Math.Sqrt(valor)` → raíz cuadrada.
- `Math.Round(valor, decimales)` → redondeo al número de decimales especificado.
- `Math.Ceiling(valor)` → techo, redondea hacia arriba.
- `Math.Floor(valor)` → piso, redondea hacia abajo.
- `Math.Truncate(valor)` → quita la parte decimal, devuelve solo la parte entera.
- `valor.ToString("F2")` → formatea el número con 2 decimales.

- 123.456.ToString("F2") → "123.46"
- 123.456.ToString("F0") → "123"
- 123.456.ToString("F4") → "123.4560"

- `double.Parse(cadena)` → convierte una cadena a double.
- `decimal.Parse(cadena)` → convierte una cadena a decimal.
- `double.TryParse(cadena, out double resultado)` → intenta convertir una cadena a double, devuelve true si tuvo éxito y false si no.
- `decimal.TryParse(cadena, out decimal resultado)` → intenta convertir una cadena a decimal, devuelve true si tuvo éxito y false si no.
- `Convert.ToDouble(valor)` → convierte un valor a double, con manejo de conversiones entre tipos.
- `Convert.ToDecimal(valor)` → convierte un valor a decimal, con manejo de conversiones entre tipos.


## Conversiones entre tipos numericos.
Las conversion de un tipo numerico de uno mas pequeño a uno mas grande se llama conversion implicita, y se realiza automaticamente por el compilador sin necesidad de que el programador haga nada. Por ejemplo:

```cs
int i = 10;
long l = i; // Conversion implicita de int a long
float f = i; // Conversion implicita de int a float
double d = i; // Conversion implicita de int a double
decimal m = i; // Conversion implicita de int a decimal
```

Pero cuando se quiere convertir un tipo numerico de uno mas grande a uno mas pequeño se llama conversion explicita, y el programador debe indicarle al compilador que realice la conversion utilizando un cast. Por ejemplo:

```cs
long l = 1234567890123L;
int i = (int)l; // Conversion explicita de long a int, puede perder informacion si el valor de l es mayor que el maximo valor de int
double d = 3.14;
float f = (float)d; // Conversion explicita de double a float, puede perder precision
decimal m = 1.618m;
float f2 = (float)m; // Conversion explicita de decimal a float, puede perder precision
```

## Tipo de datos primitivos: booleanos

Los tipos de datos booleanos representan valores de verdad: `true` o `false`. En C#, el tipo booleano se llama `bool`.

```cs
bool activo = true; // Literal booleano verdadero
bool inactivo = false; // Literal booleano falso
```

Siempre son el resultado de una comparación o una operación lógica:

```cs
int a = 10;
int b = 20;
bool esMayor = a > b; // false
bool esIgual = a == b; // false
bool esMenor = a < b; // true
```

o de una operación lógica:

```cs
bool c = true;
bool d = false;
bool and = c && d; // false  (Observe que && es un operador de logico mientras que & es un operador bit a bit, ambos pueden usarse con booleanos pero && tiene cortocircuito y & no)
bool or = c || d; // true
bool notC = !c; // false
```

Los operadoras logicos tienen una propiedad de cortocircuito, lo que significa que si el resultado de la operación se puede determinar con el primer operando, el segundo operando no se evalúa. Por ejemplo:

```cs
bool a = true;
bool b = false;
bool resultado1 = a || b; // resultado1 es true, b no se evalúa
bool resultado2 = a && b; // resultado2 es false, b no se evalúa
```

Por ejemplo...

```cs
var x = 0;
bool resultado = (x != 0) && (10 / x > 1); // resultado es false, no se evalúa la segunda parte (10 / x > 1) 
```

Se usa mucho para controlar el flujo del programa con estructuras de decisión (`if`, `else`, `switch`) y bucles (`while`, `for`):

```cs
var nombre = "Ana";
if (nombre != null && nombre.Length > 0) {
    Console.WriteLine($"Hola, {nombre}!");
} else {
    Console.WriteLine("Nombre no válido.");
}
```

## Tipo de datos primitivos: caracteres y cadenas

Los caracteres y las cadenas son tipos de datos que representan texto. En C#, el tipo de dato para un solo carácter es `char`, mientras que el tipo para una secuencia de caracteres (cadena) es `string`.

### Caracteres (`char`)

```cs
char letra = 'A'; // Literal de carácter, se usa comillas simples
char digito = '5'; // Literal de carácter
char simbolo = '@'; // Literal de carácter

char letra2 = (char)65; // Conversión de entero a char, letra2 es 'A' porque 65 es el código ASCII de 'A'
char retornoDeCarro = '\r'; // Literal de carácter con secuencia de escape para retorno de carro
char nuevaLinea = '\n'; // Literal de carácter con secuencia de escape para nueva línea
char tabulacion = '\t'; // Literal de carácter con secuencia de escape para tabulación

char c = '☺';   // puede funcionar`
// char feliz = '😊'; // Literal de carácter con emoji, no se puede usar en todos los entornos

string feliz = "😊"; // Literal de cadena con emoji, se puede usar sin problemas

### Unicode y codificación

Unicode es un estándar que asigna un número único a cada carácter, sin importar la plataforma, el programa o el idioma. En C#, los caracteres `char` se representan internamente como valores Unicode de 16 bits (UTF-16). 
Esto permite representar una amplia gama de caracteres de diferentes idiomas y símbolos. Sin embargo, algunos caracteres, como ciertos emojis o caracteres raros, pueden requerir más de 16 bits para representarse correctamente. En esos casos, se utilizan pares sustitutos (surrogate pairs) para representar caracteres que no caben en un solo `char`. Por eso, aunque un emoji pueda parecer un solo carácter, internamente puede estar compuesto por dos `char` en C#.

Los caracteres Unicode se pueden representar de diferente forma.

- UTF-8: es una codificación de longitud variable que puede usar entre 1 y 4 bytes para representar cada carácter. Es eficiente para texto principalmente en inglés, pero puede ser menos eficiente para idiomas con muchos caracteres especiales.
Es compatible con ASCII, lo que significa que los caracteres ASCII se representan con un solo byte, mientras que los caracteres Unicode más complejos pueden requerir más bytes. Es el formato más común para almacenar y transmitir texto en la web.


- UTF-16: es una codificación de longitud variable que usa 2 bytes para la mayoría de los caracteres comunes, pero puede usar 4 bytes para caracteres menos comunes (como ciertos emojis). Es el formato interno que usa C# para los `char`.

- UTF-32: es una codificación de longitud variable que usa 4 bytes para representar cada carácter. Es menos común en C#, pero puede ser útil en ciertos escenarios donde se necesita garantizar la representación de todos los caracteres Unicode.

### Cadenas (`string`)
```cs
string saludo = "Hola, mundo!"; // Literal de cadena, se usa comillas dobles
string multilinea = "Primera línea\nSegunda línea"; // Literal de cadena con secuencia de escape para nueva línea
string conTabulacion = "Columna1\tColumna2"; // Literal de cadena con secuencia de escape para tabulación
string conComillas = "Ella dijo: \"Hola\""; // Literal de cadena con secuencia de escape para comillas

string ruta = @"C:\Usuarios\Ana\Documentos"; // Literal de cadena verbatim, se usa @ para evitar la necesidad de escapar las barras invertidas
string multilineaVerbatim = @"
    Primera línea
    Segunda línea
"; // Literal de cadena verbatim con salto de línea literal
```

#### Interpolación de cadenas

```cs
string nombre = "Ana";
int edad = 30;
string mensaje = $"Hola, {nombre}. Tienes {edad} años."; // Literal de cadena interpolada, se usa $ para permitir la inserción de variables dentro de la cadena
```

#### Métodos útiles para cadenas

- `cadena` + `otraCadena` → concatenación de cadenas.

- `cadena.Length` → devuelve la longitud de la cadena.

- `cadena.ToUpper()` → convierte la cadena a mayúsculas.

- `cadena.ToLower()` → convierte la cadena a minúsculas.

- `cadena.Contains(subcadena)` → devuelve true si la cadena contiene la subcadena especificada.

- `cadena.StartsWith(prefijo)` → devuelve true si la cadena comienza con el prefijo especificado.

- `cadena.EndsWith(sufijo)` → devuelve true si la cadena termina con el sufijo especificado.

- `cadena.Replace(viejo, nuevo)` → devuelve una nueva cadena donde todas las ocurrencias de "viejo" son reemplazadas por "nuevo".

- `cadena.Substring(inicio, longitud)` → devuelve una subcadena que comienza en la posición "inicio" y tiene la longitud especificada.

### Las cadenas son inmutables

Las cadenas en C# son inmutables, lo que significa que una vez creada una cadena, no se puede modificar su contenido. Cuando se realizan operaciones que parecen modificar una cadena, en realidad se está creando una nueva cadena con el resultado de esa operación. Por ejemplo:

```cs
string original = "Hola";
string modificada = original + " Mundo"; // Se crea una nueva cadena "Hola Mundo", original sigue siendo "Hola"

original.Replace("o", "a"); 
// Se crea una nueva cadena "Hala", original sigue siendo 
// Solo que el nuevo valor no es asignado a ninguna variable
```

Esta característica de las cadenas tiene implicaciones importantes para el rendimiento, especialmente cuando se realizan muchas operaciones de concatenación o modificación. En esos casos, es recomendable usar la clase `StringBuilder`, que permite construir cadenas de manera más eficiente sin crear múltiples objetos intermedios.

Las cadenas permite acceder a cada uno de sus caracteres mediante un índice, comenzando desde 0:

```cs
string texto = "Hola";
char primeraLetra = texto[0]; // 'H'
char segundaLetra = texto[1]; // 'o'
char terceraLetra = texto[2]; // 'l'
char ultimaLetra = texto[^1]; // 
char ultimoCaracter = texto[texto.Length - 1]; // 'a'

char letraInexistente = texto[10]; // Esto lanzará una excepción IndexOutOfRangeException, porque el índice 10 está fuera del rango de la cadena "Hola" que tiene longitud 4
char penultimaLetra = texto[^2]; // 'l', usando índice desde el final
char pernultimoCaracter = texto[texto.Length - 2]; // 'l', usando índice desde el final
```

### Tipos de datos primitivos: `object` y `dynamic`

Un tipo muy especial del que desciedo todos los tipos es `object`. Es el tipo raíz de la jerarquía de tipos en C#. Todos los tipos, ya sean primitivos o definidos por el usuario, heredan de `object`. Esto significa que cualquier valor puede ser tratado como un `object`, lo que permite una gran flexibilidad pero también requiere cuidado para evitar errores de tipo.

Cuando una variable es declarada como `object`, puede contener cualquier tipo de dato, pero para usarlo como su tipo original, es necesario realizar un proceso llamado "unboxing" o "casting". Por ejemplo:

```cs
object obj = 42; // Se asigna un entero a una variable de tipo object
int numero = (int)obj; // Se realiza un cast para recuperar el valor entero original
obj = "Hola"; // Ahora obj contiene una cadena
string texto = (string)obj; // Se realiza un cast para recuperar el valor de cadena original
```

Tiene dos metodos importantes ToString() y GetHashCode() que son heredados de object y pueden ser sobrescritos por los tipos derivados para proporcionar una representación personalizada del objeto o un código hash único.

El tipo `dynamic` es un tipo especial que permite la resolución de tipos en tiempo de ejecución en lugar de en tiempo de compilación. Esto significa que las operaciones realizadas sobre una variable de tipo `dynamic` no se verifican hasta que el programa se ejecuta, lo que puede ser útil para trabajar con datos que no se conocen hasta ese momento, como datos provenientes de fuentes externas o interacciones con APIs dinámicas.

```cs
dynamic dyn = 42; // Se asigna un entero a una variable de tipo dynamic
Console.WriteLine(dyn); // Imprime 42
dyn = "Hola"; // Ahora dyn contiene una cadena
Console.WriteLine(dyn); // Imprime "Hola"
```

Diferente a `object`, las variables de tipo `dynamic` no requieren un cast para acceder a sus miembros, pero esto también significa que si se intenta acceder a un miembro que no existe o realizar una operación no válida, se producirá una excepción en tiempo de ejecución.

```cs
dynamic dyn = 42;
Console.WriteLine(dyn.Length); // Esto lanzará una excepción RuntimeBinderException, porque un entero no tiene una propiedad Length
```

Esto hace que el uso de `dynamic` sea poderoso pero también riesgoso, ya que se pierde la seguridad de tipos en tiempo de compilación. Es importante usar `dynamic` con precaución y solo cuando sea necesario para casos específicos donde la flexibilidad es más importante que la seguridad de tipos.

## Tipos compuestos: arrays y colecciones

Cuando se tiene que acceder a un conjunto de datos homogéneos, es decir, del mismo tipo, se pueden usar arrays o colecciones.

```cs
int[] numeros = new int[5]; // Array de enteros con capacidad para 5 elementos
numeros[0] = 10; // Asignar valor al primer elemento del array
numeros[1] = 20; // Asignar valor al segundo elemento del array
numeros[2] = 30; // Asignar valor al tercer elemento del array
numeros[3] = 40; // Asignar valor al cuarto elemento del array
numeros[4] = 50; // Asignar valor al quinto elemento del array
```

Los arrays tienen un tamaño fijo, lo que significa que una vez creado un array con una capacidad determinada, no se puede cambiar su tamaño. Existe funciones para crear arrays con valores iniciales:

```cs
int[] numeros = new int[] { 10, 20, 30, 40, 50 };   // Array de enteros con valores iniciales
int[] pares = {2, 4, 6, 8};                         // Array de enteros con valores iniciales usando sintaxis abreviada
var impares = new[] {1, 3, 5, 7};                   // Array de enteros con valores iniciales usando inferencia de tipo
var primos = new int[] { 2, 3, 5, 7, 11 };          // Array de enteros con valores iniciales usando inferencia de tipo y sintaxis explícita
```

## Acceso a elementos de un array

```cs
int[] numeros = { 10, 20, 30, 40, 50 };
int primerNumero = numeros[0]; // Acceso al primer elemento del array, primerNumero es 10
int segundoNumero = numeros[1]; // Acceso al segundo elemento del array, segundoNumero es 20
int ultimoNumero = numeros[numeros.Length - 1]; // Acceso al último elemento del array, ultimoNumero es 50
int penultimoNumero = numeros[^2]; // Acceso al penúltimo elemento del array usando índice desde el final, penultimoNumero es 40
```

## Recorrer un array con un bucle

```cs
int[] numeros = { 10, 20, 30, 40, 50 };

for (int i = 0; i < numeros.Length; i++) {  // Recorrer el array usando un bucle for
    Console.WriteLine(numeros[i]); // Imprime cada elemento del array
}

foreach (int numero in numeros) {           // Recorrer el array usando un bucle foreach
    Console.WriteLine(numero); // Imprime cada elemento del array
}
```

## Manipulación de arrays

```cs
int[] numeros = { 10, 20, 30, 40, 50 };
numeros.Reverse(); // Invierte el orden de los elementos del array, numeros ahora es { 50, 40, 30, 20, 10 }
Array.Sort(numeros); // Ordena los elementos del array en orden ascendente, numeros ahora es { 10, 20, 30, 40, 50 }
Array.Resize(ref numeros, 10); // Cambia el tamaño del array a 10 elementos, los nuevos elementos se inicializan con el valor predeterminado (0 para int),
Array.Copy(numeros, 0, numeros, 5, 5); // Copia los primeros 5 elementos del array a partir del índice 5, numeros ahora es { 10, 20, 30, 40, 50, 10, 20, 30, 40, 50 }   
```

Los array guarda los elementos de forma contigua en memoria, lo que permite un acceso rápido a cada elemento mediante su índice. Sin embargo, debido a que los arrays tienen un tamaño fijo, no son ideales para situaciones donde se necesita agregar o eliminar elementos dinámicamente. En esos casos, es recomendable usar colecciones como `List<T>`, `Dictionary<TKey, TValue>`, entre otras, que ofrecen mayor flexibilidad y funcionalidades adicionales para manejar conjuntos de datos de manera más eficiente.

### Tipo genericos

Algunas veces es necesario escribir funciones que puedan trabajar con diferentes tipos de datos sin duplicar el código para cada tipo específico. Para esto se pueden usar tipos genéricos, que permiten definir una función o clase con un tipo de dato como parámetro. Por ejemplo, si queremos escribir una función que calcule el promedio de un array de números, podríamos escribirla de la siguiente manera:

```cs
int Promedio(int[] numeros) {
    int suma = 0;
    foreach (int numero in numeros) {
        suma += numero; // Suma cada número del array
    }
    return suma / numeros.Length; // Devuelve el promedio dividiendo la suma por la cantidad de elementos
}

float Promedio(float[] numeros) {
    float suma = 0;
    foreach (float numero in numeros) {
        suma += numero; // Suma cada número del array
    }
    return suma / numeros.Length; // Devuelve el promedio dividiendo la suma por la cantidad de elementos
}

long Promedio(long[] numeros) {
    long suma = 0;
    foreach (long numero in numeros) {
        suma += numero; // Suma cada número del array
    }
    return suma / numeros.Length; // Devuelve el promedio dividiendo la suma por la cantidad de elementos
}

T Promedio<T>(T[] numeros) where T : INumber<T>
{
    T suma = T.Zero;

    foreach (T numero in numeros) {
        suma += numero;
    }

    return suma / T.CreateChecked(numeros.Length);
}
```

### Colecciones: List<T>

Cuando tenemos que trabajar con conjuntos de datos que pueden cambiar de tamaño dinámicamente, es recomendable usar colecciones como `List<T>`, que ofrecen mayor flexibilidad y funcionalidades adicionales para manejar conjuntos de datos de manera más eficiente. Por ejemplo:

```cs
List<int> numeros = new List<int>(); // Crear una lista de enteros vacía
numeros.Add(10); // Agregar un número a la lista
numeros.Add(20); // Agregar otro número a la lista
numeros.Add(30); // Agregar otro número a la lista
numeros.Remove(20); // Eliminar el número 20 de la lista
numeros.Insert(1, 25); // Insertar el número 25 en la posición 1 de la lista
int primerNumero = numeros[0]; // Acceder al primer número de la lista, primerNumero es 10
int segundoNumero = numeros[1]; // Acceder al segundo número de la lista, segundoNumero es 25
int ultimoNumero = numeros[numeros.Count - 1]; // Acceder al último número de la lista, ultimoNumero es 30
```

Funcionan igual que los arrays pero con la ventaja de que pueden crecer o reducir su tamaño dinámicamente según sea necesario, lo que las hace ideales para situaciones donde no se conoce de antemano la cantidad de elementos que se van a manejar. Además, `List<T>` ofrece métodos adicionales como `Add`, `Remove`, `Insert`, entre otros, que facilitan la manipulación de los datos sin tener que preocuparse por el manejo manual del tamaño del array.

> Un detalle en lugar de Length, las listas usan la propiedad Count para obtener la cantidad de elementos que contienen.

### Recorrer una lista con un bucle

```cs
List<int> numeros = new List<int> { 10, 25, 30 };
for (int i = 0; i < numeros.Count; i++) { // Recorrer la lista usando un bucle for
    Console.WriteLine(numeros[i]); // Imprime cada elemento de la lista
}

foreach (int numero in numeros) { // Recorrer la lista usando un bucle foreach
    Console.WriteLine(numero); // Imprime cada elemento de la lista
}
```

#### Operaciones con listas

```cs
List<int> numeros = new List<int> { 10, 25, 30 };
numeros.Sort(); // Ordena los elementos de la lista en orden ascendente, numeros ahora es { 10, 25, 30 }
numeros.Reverse(); // Invierte el orden de los elementos de la lista, numeros ahora es { 30, 25, 10 }
numeros.Clear(); // Elimina todos los elementos de la lista, numeros ahora está vacía

var nueva = new List<int>(numeros); // Crea una nueva lista a partir de otra lista, nueva es una copia de numeros
var ordenada = nueva.Sorted(); // Crea una nueva lista ordenada a partir de otra lista, ordenada es una nueva lista con los elementos de nueva ordenados
var filtrada = nueva.Where(n => n > 20).ToList(); // Crea una nueva lista filtrada a partir de otra lista, filtrada es una nueva lista con los elementos de nueva que son mayores que 20
var transformada = nueva.Select(n => n * 2).ToList(); // Crea una nueva lista transformada a partir de otra lista, transformada es una nueva lista con los elementos de nueva multiplicados por 2
var agrupada = nueva.GroupBy(n => n % 2).ToList(); // Crea una nueva lista agrupada a partir de otra lista, agrupada es una nueva lista de grupos de elementos de nueva según su residuo al dividir por 2 (pares e impares)
var ordenadaDesc = nueva.OrderByDescending(n => n).ToList(); // Crea una nueva lista ordenada en orden descendente a partir de otra lista, ordenadaDesc es una nueva lista con los elementos de nueva ordenados de mayor a menor
```

### LinQ (Language Integrated Query)

LinQ es una característica de C# que permite realizar consultas sobre colecciones de datos de manera integrada en el lenguaje. Con LinQ, se pueden escribir consultas de forma declarativa utilizando una sintaxis similar a SQL, lo que facilita la manipulación y transformación de datos en colecciones como `List<T>`, `IEnumerable<T>`, entre otras. Por ejemplo:

```cs
List<int> numeros = new List<int> { 10, 25, 30, 15, 5 };
var numerosPares = from n in numeros
                   where n % 2 == 0 select n; // Consulta para obtener los números pares de la lista, numerosPares es una colección de los números pares { 10, 30 }     
var pares = numeros.Where(n => n % 2 == 0).ToList(); 

## Tipos de datos compuestos: tuplas
Las tuplas son tipos de datos compuestos que permiten agrupar varios valores de diferentes tipos en una sola entidad. En C#, las tuplas se pueden crear utilizando la sintaxis de paréntesis y pueden contener cualquier cantidad de elementos. Por ejemplo:

```cs
var tupla = (nombre: "Ana", edad: 30, activo: true);
string nombre = tupla.nombre; // Acceso al elemento "nombre" de la tupla
int edad = tupla.edad; // Acceso al elemento "edad" de la tupla
bool activo = tupla.activo; // Acceso al elemento "activo" de la tupla
```

Las tuplas son inmutables, lo que significa que una vez creada una tupla, no se pueden modificar sus elementos. Sin embargo, se pueden crear nuevas tuplas a partir de las existentes con valores modificados. Por ejemplo:

```cs
var tupla = (nombre: "Ana", edad: 30, activo: true);
var nuevaTupla = (tupla.nombre, tupla.edad + 1, tupla.activo); // Se crea una nueva tupla con el mismo nombre, edad incrementada en 1 y el mismo estado activo
```

Se pueden usar tuplas para devolver múltiples valores desde una función sin necesidad de crear una clase o estructura personalizada. Por ejemplo:

```cs
(string nombre, int edad) ObtenerDatos() {
    return ("Ana", 30); // Devuelve una tupla con el nombre y la edad
}  

var datos = ObtenerDatos(); // Llama a la función y obtiene la tupla con los datos
string nombre = datos.nombre; // Acceso al elemento "nombre" de la tupla devuelta por la función
int edad = datos.edad; // Acceso al elemento "edad" de la tupla devuelta por la función
```

Se puede usar patrones para descomponer tuplas en variables individuales de manera más concisa. Por ejemplo:

```cs
var tupla = (nombre: "Ana", edad: 30, activo: true);
var (nombre, edad, activo) = tupla; // Descomposición de la tupla en variables individuales
```

Tambien pueden ser anónimas, sin nombres para los elementos, y se acceden por su posición:

```cs
var tupla = ("Ana", 30, true);
string nombre = tupla.Item1;
int edad = tupla.Item2;
bool activo = tupla.Item3;
```

### Tipos de datos compuestos: Diccionarios

Los diccionarios son tipos de datos compuestos que permiten almacenar pares de clave-valor, donde cada clave es única y se asocia a un valor. En C#, los diccionarios se pueden crear utilizando la clase `Dictionary<TKey, TValue>` del espacio de nombres `System.Collections.Generic`. Por ejemplo:

```cs
Dictionary<string, int> edades = new Dictionary<string, int>(); // Crear un diccionario con claves de tipo string y valores de tipo int
edades.Add("Ana", 30); // Agregar un par clave-valor al diccionario
edades.Add("Juan", 25); // Agregar otro par clave-valor al diccionario
edades["Pedro"] = 35; // Agregar un par clave-valor al diccionario usando la sintaxis de índice

int edadAna = edades["Ana"]; // Acceder al valor asociado a la clave "Ana", edadAna es 30
int edadJuan = edades["Juan"]; // Acceder al valor asociado a la clave "Juan", edadJuan es 25
```

Se pueden asignar mas valores a una clave existente, lo que actualizará el valor asociado a esa clave:

```cs
Dictionary<string, int> numerosLetrasDigitos = new Dictionary<string, int> {
    { "uno", 1 },
    { "dos", 2 },
    { "tres", 3 }
};

Dictionary<int, string> letrasNumeros = new Dictionary<int, string> {
    { 1, "uno" },
    { 2, "dos" },
    { 3, "tres" }
};
```

Los diccionarios son útiles para almacenar y acceder a datos de manera eficiente utilizando claves únicas, lo que los hace ideales para situaciones donde se necesita una asociación rápida entre claves y valores, como en la implementación de tablas de búsqueda, conteo de elementos, entre otros casos. Además, ofrecen métodos adicionales como `ContainsKey`, `Remove`, `TryGetValue`, entre otros, que facilitan la manipulación de los datos sin tener que preocuparse por el manejo manual de las claves y valores.

```cs
Dictionary<string, int> edades = new Dictionary<string, int> {
    { "Ana", 30 },
    { "Juan", 25 },
    { "Pedro", 35 }
};

if (edades.ContainsKey("Ana")) { // Verificar si la clave "Ana" existe en el diccionario
    int edadAna = edades["Ana"]; // Acceder al valor asociado a la clave "Ana", edadAna es 30
}

if (edades.TryGetValue("Juan", out int edadJuan)) { // Intentar obtener el valor asociado a la clave "Juan" de manera segura
    Console.WriteLine($"La edad de Juan es {edadJuan}"); // Imprime la edad de Juan si la clave existe
} else {
    Console.WriteLine("La clave 'Juan' no existe en el diccionario."); // Imprime un mensaje si la clave no existe
}
```

Los diccionarios se pueden recorrer de varias formas, como por ejemplo usando un bucle `foreach` para iterar sobre los pares clave-valor:

```cs
Dictionary<string, int> edades = new Dictionary<string, int> {
    { "Ana", 30 },
    { "Juan", 25 },
    { "Pedro", 35 }
};

foreach (KeyValuePair<string, int> par in edades) { // Recorrer el diccionario usando un bucle foreach
    Console.WriteLine($"Clave: {par.Key}, Valor: {par.Value}"); // Imprime la clave y el valor de cada par en el diccionario
}

foreach (var (clave, valor) in edades) { // Recorrer el diccionario usando un bucle foreach con descomposición de tupla
    Console.WriteLine($"Clave: {clave}, Valor: {valor}"); // Imprime la clave y el valor de cada par en el diccionario
}   

foreach(var key in edades.Keys) { // Recorrer solo las claves del diccionario
    Console.WriteLine($"Clave: {key} = {edades[key]}"); //
}

foreach(var value in edades.Values) { // Recorrer solo los valores del diccionario
    Console.WriteLine($"Valor: {value}"); // Imprime cada valor del diccionario
}

foreach(var (k, v) in edades.Items) { // Recorrer el diccionario usando un bucle foreach con descomposición de tupla
    Console.WriteLine($"Clave: {k}, Valor: {v}"); // Imprime la clave y el valor de cada par en el diccionario
}   
```

### Metodos útiles para diccionarios

- `diccionario.Add(clave, valor)` → Agrega un par clave-valor al diccionario.
- `diccionario.Remove(clave)` → Elimina el par clave-valor asociado a la clave especificada.
- `diccionario.ContainsKey(clave)` → Devuelve true si el diccionario contiene la clave especificada.
- `diccionario.TryGetValue(clave, out valor)` → Intenta obtener el valor asociado a la clave especificada, devuelve true si la clave existe y false si no.
- `diccionario.Keys` → Devuelve una colección de las claves del diccionario.
- `diccionario.Values` → Devuelve una colección de los valores del diccionario.
- `diccionario.Count` → Devuelve la cantidad de pares clave-valor en el diccionario.
- `diccionario.Clear()` → Elimina todos los pares clave-valor del diccionario.
- `diccionario.ContainsValue(valor)` → Devuelve true si el diccionario contiene el valor especificado.
- `diccionario.GetEnumerator()` → Devuelve un enumerador que itera

#### Contar la cantidad de veces que se repite cada palabra en un texto utilizando un diccionario

```cs
// Contar la cantidad de veces que se repite cada palabra en un texto utilizando un diccionario
string texto = "hola mundo hola universo";
string[] palabras = texto.Split(' '); // Dividir el texto en palabras usando el espacio
Dictionary<string, int> conteoPalabras = new Dictionary<string, int>(); // Crear un diccionario para contar las palabras
foreach (string palabra in palabras) { // Recorrer cada palabra en el array de palabras
    if (conteoPalabras.ContainsKey(palabra)) { // Verificar si la palabra ya existe en el diccionario
        conteoPalabras[palabra]++; // Incrementar el conteo de la palabra si ya existe en el diccionario    
    } else {
        conteoPalabras[palabra] = 1; // Agregar la palabra al diccionario con un conteo inicial de 1 si no existe en el diccionario
    }
}

// O tambien usarndo el metodo TryGetValue para evitar la doble busqueda en el diccionario
conteoPalabras.Clear();
foreach (string palabra in palabras) { // Recorrer cada palabra en el array de palabras
    if (conteoPalabras.TryGetValue(palabra, out int conteo)) { // Intentar obtener el conteo de la palabra de manera segura
        conteoPalabras[palabra] = conteo + 1; // Incrementar el conteo de la palabra si ya existe en el diccionario
    } else {
        conteoPalabras[palabra] = 1; // Agregar la palabra al diccionario con un conteo inicial de 1 si no existe en el diccionario
    }
}

// O usando += para simplificar el incremento del conteo
conteoPalabras.Clear();
foreach (string palabra in palabras) { // Recorrer cada palabra en el array de palabras
    if(conteoPalabras.ContainsKey(palabra)) { // Verificar si la palabra ya existe en el diccionario
        conteoPalabras[palabra] += 1; // Incrementar el conteo de la palabra si ya existe en el diccionario    
    } else {
        conteoPalabras[palabra] = 1; // Agregar la palabra al diccionario con un conteo inicial de 1 si no existe en el diccionario
    }
}   

// O usando el valor por defecto de int que es 0 para evitar la necesidad de verificar si la clave existe en el diccionario
conteoPalabras.Clear();
foreach (string palabra in palabras) { // Recorrer cada palabra en el array de palabras
    conteoPalabras[palabra] = conteoPalabras.GetValueOrDefault(palabra) + 1; // Incrementar el conteo de la palabra, si la palabra no existe en el diccionario, GetValueOrDefault devuelve 0, por lo que se asigna 1 al conteo de la palabra
}   
```

### Tipos de datos compuestos: Conjuntos (HashSet<T>)

Los conjuntos son tipos de datos compuestos que permiten almacenar una colección de elementos únicos, es decir, no permiten elementos duplicados. En C#, los conjuntos se pueden crear utilizando la clase `HashSet<T>` del espacio de nombres `System.Collections.Generic`. Por ejemplo:

```cs
HashSet<int> numeros = new HashSet<int>(); // Crear un conjunto de enteros vacío
numeros.Add(10); // Agregar un número al conjunto
numeros.Add(20); // Agregar otro número al conjunto
numeros.Add(10); // Intentar agregar un número duplicado al conjunto, no se agregará porque el conjunto no permite duplicados
```

Se pueden realizar operaciones de conjuntos como unión, intersección, diferencia, entre otras. Por ejemplo:

```cs
HashSet<int> conjuntoA = new HashSet<int> { 1, 2, 3 };
HashSet<int> conjuntoB = new HashSet<int> { 3, 4, 5 };  
var union = new HashSet<int>(conjuntoA); // Crear un nuevo conjunto para la unión
union.UnionWith(conjuntoB); // Realizar la unión de conjuntoA y conjuntoB
var interseccion = new HashSet<int>(conjuntoA); // Crear un nuevo conjunto para la intersección
interseccion.IntersectWith(conjuntoB); // Realizar la intersección de conjuntoA y conjuntoB
var diferencia = new HashSet<int>(conjuntoA); // Crear un nuevo conjunto para la diferencia
diferencia.ExceptWith(conjuntoB); // Realizar la diferencia de conjuntoA y conjuntoB
```

Los conjuntos son útiles para almacenar y manipular colecciones de elementos únicos, lo que los hace ideales para situaciones donde se necesita garantizar la ausencia de duplicados, como en la implementación de conjuntos matemáticos, filtrado de datos, entre otros casos. Además, ofrecen métodos adicionales como `Contains`, `Remove`, `Clear`, entre otros, que facilitan la manipulación de los datos sin tener que preocuparse por el manejo manual de los elementos únicos.

```cs
HashSet<string> frutas = new HashSet<string> { "manzana", "banana", "naranja" };
if (frutas.Contains("banana")) { // Verificar si el conjunto contiene el elemento "banana"
    Console.WriteLine("El conjunto contiene banana."); // Imprime un mensaje si el elemento existe en el conjunto
} else {
    Console.WriteLine("El conjunto no contiene banana."); // Imprime un mensaje si el elemento no existe en el conjunto
}
```

```cs
// Contar la cantidad de palabras unicas en un texto utilizando un conjunto
string texto = "hola mundo hola universo";
string[] palabras = texto.Split(' '); // Dividir el texto en palabras usando el espacio
HashSet<string> palabrasUnicas = new HashSet<string>(palabras); // Crear un conjunto a partir del array de palabras, lo que eliminará los duplicados
int cantidadPalabrasUnicas = palabrasUnicas.Count; // Obtener la cantidad de palabras únicas en el conjunto
Console.WriteLine($"Cantidad de palabras únicas: {cantidadPalabrasUnicas}"); // Imprime la cantidad de palabras únicas
```