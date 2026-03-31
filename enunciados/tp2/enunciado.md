# Trabajo Práctico 01

## Tema

Calculadora de expresiones aritméticas con enteros de tamaño arbitrario.

## Objetivo

Desarrollar un programa en C# file-based que permita ingresar expresiones aritméticas por consola, evaluarlas y mostrar el resultado, implementando manualmente tanto el análisis de la expresión como las operaciones sobre números grandes.

El propósito del trabajo es practicar:

- descomposición de problemas,
- tokenización,
- análisis sintáctico recursivo descendente,
- manejo de precedencia de operadores,
- e implementación manual de algoritmos aritméticos.

## Enunciado

Construir una aplicación de consola cuyo funcionamiento principal sea un bucle interactivo:

1. El programa solicita al usuario una ecuación.
2. El usuario ingresa una expresión aritmética.
3. El programa evalúa la expresión.
4. El programa muestra el resultado.
5. Si el usuario ingresa `fin`, la aplicación termina.

## Requisitos funcionales

El programa debe aceptar expresiones que incluyan:

- números enteros positivos,
- operadores binarios `+`, `-`, `*` y `/`,
- paréntesis `(` y `)`,
- operadores unarios `+` y `-`.

Debe respetarse la precedencia tradicional de operadores:

- primero los paréntesis,
- luego los operadores unarios,
- después multiplicación y división,
- por último suma y resta.

Ejemplos de expresiones válidas:

- `2+3`
- `10-4*2`
- `(8+2)*5`
- `-5+3`
- `2*-3`
- `-(7+8)`

## Requisito sobre los números

Los números deben manejarse como enteros de tamaño arbitrario. Esto significa que no se puede depender de tipos como `int`, `long`, `decimal`, `BigInteger` ni similares para resolver el cálculo.

Cada número debe representarse mediante una clase inmutable definida por el alumno.

Esa clase debe encapsular el signo y un arreglo de dígitos decimales como representación interna.

Una vez construido un número, su estado no debe poder modificarse. Todas las operaciones aritméticas deben devolver una nueva instancia, sin alterar las ya existentes.

## Implementación obligatoria

La solución debe implementarse siguiendo estas etapas:

### 1. Tokenización

La expresión ingresada debe transformarse en una secuencia de tokens.

Como mínimo, deben reconocerse estos tipos de token:

- número,
- `+`,
- `-`,
- `*`,
- `/`,
- `(`,
- `)`.

### 2. Análisis recursivo descendente

La evaluación de la expresión debe hacerse mediante un analizador recursivo descendente.

Se espera una estructura similar a la siguiente:

- expresión: maneja suma y resta,
- término: maneja multiplicación y división,
- unario: maneja `+` y `-` unarios,
- primario: maneja números y paréntesis.

El analizador debe respetar la precedencia de operadores sin usar librerías externas ni convertir la expresión a otro formato con herramientas ya resueltas.

### 3. Operaciones aritméticas manuales

Las operaciones deben implementarse manualmente sobre arreglos de dígitos:

- suma,
- resta,
- multiplicación,
- división.

Se deben aplicar los algoritmos tradicionales “a mano”, equivalentes a los que se usan en papel.

## Restricciones

La solución debe cumplir obligatoriamente con las siguientes restricciones:
- usar una clase inmutable para representar los números,
- usar solamente funciones y estructuras de datos básicas,
- no usar librerías externas,
- no usar funciones o utilidades del lenguaje que evalúen expresiones,
- no usar tipos numéricos grandes ya implementados por la plataforma,
- no delegar el cálculo a componentes ya resueltos.

## Alcance esperado

Se asume camino feliz. No es necesario implementar una validación exhaustiva de errores de entrada, pero el programa debe funcionar correctamente para expresiones bien formadas.

Como los operandos son enteros, la división debe resolverse como división entera.

## Sugerencia de diseño

Se recomienda dividir la implementación en una clase de número con responsabilidades bien acotadas y funciones auxiliares para el parser y la tokenización.

Por ejemplo:

- una clase inmutable para representar el número interno,
- un método o función para crear un número a partir de texto,
- un método o función para comparar magnitudes,
- un método o función para sumar,
- un método o función para restar,
- un método o función para multiplicar,
- un método o función para dividir,
- un método o función para convertir el número interno a texto,
- una función para tokenizar,
- una función por regla del analizador.

## Entrega

La entrega debe consistir en un único archivo llamado `12-calculadora.cs`.

Debe ser un programa file-based ejecutable desde consola.

## Criterios de evaluación

Se valorará especialmente:

- que la solución cumpla exactamente con la consigna,
- que el análisis sintáctico sea recursivo descendente,
- que las operaciones estén implementadas manualmente,
- que la clase de número sea efectivamente inmutable,
- que los números se representen como arreglos de dígitos,
- que el código sea claro, simple y coherente,
- que la calculadora respete correctamente la precedencia de operadores.

## Pruebas mínimas sugeridas

Antes de entregar, verificar al menos estos casos:

- `2+3` -> `5`
- `7-10` -> `-3`
- `12*12` -> `144`
- `25/4` -> `6`
- `(2+3)*4` -> `20`
- `-8+5` -> `-3`
- `2*-3` -> `-6`
- `-(3+4)*2` -> `-14`
- `999999999999999999+1` -> `1000000000000000000`

## Observación final

El estilo esperado es minimalista. La prioridad no está en construir una arquitectura compleja, sino en mostrar comprensión del problema y dominio de los algoritmos básicos.
