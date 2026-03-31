# Evaluador recursivo descendente

## Idea general

Un analizador recursivo descendente es un conjunto de funciones, donde cada función representa una parte de la gramática de la expresión.

En este caso no solo analiza la expresión, sino que además la evalúa mientras la recorre.

La idea es separar el problema en niveles de precedencia:

- `Expresion`: suma y resta.
- `Termino`: multiplicación y división.
- `Unario`: signo `+` o `-` delante de un operando.
- `Primario`: números y paréntesis.

De esta forma, la precedencia surge de la estructura de llamadas:

- `Expresion` llama a `Termino`,
- `Termino` llama a `Unario`,
- `Unario` llama a `Primario`,
- y `Primario` puede volver a llamar a `Expresion` si encuentra paréntesis.

## Tokens

Se supone que la expresión ya fue tokenizada en un arreglo `tokens` y que existe una variable `posicion` que indica el token actual.

Ejemplo:

```text
12 + 3 * ( 4 - 1 )
```

puede transformarse en:

```text
["12", "+", "3", "*", "(", "4", "-", "1", ")", "$"]
```

El token `$` marca el fin de la entrada.

## Regla informal

```text
Expresion -> Termino { ("+" | "-") Termino }
Termino   -> Unario { ("*" | "/") Unario }
Unario    -> "+" Unario
          | "-" Unario
          | Primario
Primario  -> numero
          | "(" Expresion ")"
```

## Funcion principal

La función principal llama a `Expresion` y al final verifica que no hayan quedado tokens sin consumir.

```text
Funcion Evaluar(tokens)
    posicion <- 0
    resultado <- Expresion(tokens, posicion)

    Si tokens[posicion] <> "$" Entonces
        Error("Expresion invalida")
    FinSi

    Retornar resultado
FinFuncion
```

## Expresión

`Expresion` resuelve sumas y restas.  
Primero calcula un término completo.  
Después, mientras encuentre `+` o `-`, sigue leyendo otro término y aplica la operación.

```text
Funcion Expresion(tokens, posicion)
    valor <- Termino(tokens, posicion)

    Mientras tokens[posicion] = "+" O tokens[posicion] = "-" Hacer
        operador <- tokens[posicion]
        posicion <- posicion + 1

        derecho <- Termino(tokens, posicion)

        Si operador = "+" Entonces
            valor <- Sumar(valor, derecho)
        Sino
            valor <- Restar(valor, derecho)
        FinSi
    FinMientras

    Retornar valor
FinFuncion
```

## Término

`Termino` resuelve multiplicaciones y divisiones.  
Funciona igual que `Expresion`, pero en su nivel de precedencia.

```text
Funcion Termino(tokens, posicion)
    valor <- Unario(tokens, posicion)

    Mientras tokens[posicion] = "*" O tokens[posicion] = "/" Hacer
        operador <- tokens[posicion]
        posicion <- posicion + 1

        derecho <- Unario(tokens, posicion)

        Si operador = "*" Entonces
            valor <- Multiplicar(valor, derecho)
        Sino
            valor <- Dividir(valor, derecho)
        FinSi
    FinMientras

    Retornar valor
FinFuncion
```

## Unario

`Unario` reconoce signos delante de una expresión simple.

Si encuentra `+`, simplemente sigue evaluando.

Si encuentra `-`, evalúa lo que sigue y luego cambia el signo del resultado.

```text
Funcion Unario(tokens, posicion)
    Si tokens[posicion] = "+" Entonces
        posicion <- posicion + 1
        Retornar Unario(tokens, posicion)
    FinSi

    Si tokens[posicion] = "-" Entonces
        posicion <- posicion + 1
        Retornar Negar(Unario(tokens, posicion))
    FinSi

    Retornar Primario(tokens, posicion)
FinFuncion
```

## Primario

`Primario` reconoce los casos más básicos:

- un número,
- o una subexpresión entre paréntesis.

```text
Funcion Primario(tokens, posicion)
    token <- tokens[posicion]

    Si EsNumero(token) Entonces
        posicion <- posicion + 1
        Retornar LeerNumero(token)
    FinSi

    Si token = "(" Entonces
        posicion <- posicion + 1
        valor <- Expresion(tokens, posicion)

        Si tokens[posicion] <> ")" Entonces
            Error("Falta parentesis de cierre")
        FinSi

        posicion <- posicion + 1
        Retornar valor
    FinSi

    Error("Se esperaba un numero o un parentesis")
FinFuncion
```

## Por qué respeta la precedencia

La precedencia no se maneja con una tabla ni con casos especiales.  
Se logra porque cada función delega en otra más específica:

- `Expresion` no combina números directamente, sino resultados de `Termino`.
- `Termino` no combina números directamente, sino resultados de `Unario`.
- `Unario` no combina operadores binarios, solo signos.
- `Primario` resuelve lo más básico.

Por eso, en una expresión como:

```text
2 + 3 * 4
```

ocurre esto:

1. `Expresion` lee primero `2`.
2. Encuentra `+` y necesita calcular el término de la derecha.
3. `Termino` ve `3 * 4` y lo resuelve completo.
4. Recién entonces `Expresion` suma `2 + 12`.

## Por qué los paréntesis funcionan

Cuando `Primario` encuentra un `(`, llama otra vez a `Expresion`.

Eso hace que todo lo que está entre `(` y `)` se evalúe como una expresión independiente antes de continuar con el resto.

Ejemplo:

```text
(2 + 3) * 4
```

Primero se evalúa `2 + 3`, luego ese resultado se usa en la multiplicación.

## Resumen del algoritmo

```text
Funcion Evaluar(tokens)
    posicion <- 0
    Retornar Expresion(tokens, posicion)
FinFuncion

Funcion Expresion(tokens, posicion)
    valor <- Termino(tokens, posicion)

    Mientras haya + o - Hacer
        leer operador
        derecho <- Termino(tokens, posicion)
        combinar valor con derecho
    FinMientras

    Retornar valor
FinFuncion

Funcion Termino(tokens, posicion)
    valor <- Unario(tokens, posicion)

    Mientras haya * o / Hacer
        leer operador
        derecho <- Unario(tokens, posicion)
        combinar valor con derecho
    FinMientras

    Retornar valor
FinFuncion

Funcion Unario(tokens, posicion)
    Si hay + Entonces
        avanzar
        Retornar Unario(tokens, posicion)
    FinSi

    Si hay - Entonces
        avanzar
        Retornar Negar(Unario(tokens, posicion))
    FinSi

    Retornar Primario(tokens, posicion)
FinFuncion

Funcion Primario(tokens, posicion)
    Si hay numero Entonces
        leer numero
    SinoSi hay "(" Entonces
        avanzar
        valor <- Expresion(tokens, posicion)
        exigir ")"
        Retornar valor
    Sino
        Error
    FinSi
FinFuncion
```
