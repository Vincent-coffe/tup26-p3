# Trabajo Práctico 1: Análisis de algoritmos

## Calculadora básica

### Consigna

Desarrollar una aplicación de consola que lea una expresión matemática ingresada por el usuario, la evalúe y muestre el resultado en pantalla.

### Requisitos

- La aplicación debe ejecutarse desde la línea de comandos.
- Debe solicitar una expresión matemática por vez.
- Debe soportar suma, resta, multiplicación y división entre números enteros.
- Debe aceptar números de cualquier cantidad de dígitos.
- Debe ignorar los espacios en blanco.
- Debe respetar la precedencia habitual de operaciones:
  - primero paréntesis,
  - luego multiplicación y división,
  - por último suma y resta.
- Debe aceptar signos unarios `+` y `-`.
- Debe continuar solicitando expresiones hasta que el usuario escriba `fin.`.
- Si se intenta dividir por cero, la aplicación debe informar el error de manera clara.

### Ejemplos de ejecución

```text
Ingrese una expresión matemática:
> (2 + 3) * 4 - 6 / 2
Resultado: 17

Ingrese una expresión matemática:
> 10 + 5 * 2 - (8 / 4)
Resultado: 18

Ingrese una expresión matemática:
> 123456789 * 987654321 + 111111111
Resultado: 121932631112635269

Ingrese una expresión matemática:
> fin.
```

### Como realizar el trabajo práctico

1. Debe bajarse el repositorio del curso.
2. Implementar la solucion en una nueva rama.
3. Realizar commits frecuentes con mensajes descriptivos.
4. Al finalizar, abrir un pull request para que el docente pueda revisar su código.
5. Volver a la rama principal después de abrir el pull request.



- Debe bajar el repositorio del curso.
```bash
git clone https://github.com/tup26-p3/tup26-p3.git
```
- Debe crear una rama nueva para el trabajo práctico.
```bash
git checkout -b {legajo}-{tp1}-calculadora
```
- Debe implementar la solución en un archivo llamado `calculadora.cs`.
```bash

code calculadora.cs
```
- Debe realizar commits frecuentes con mensajes descriptivos.
```bash
git add calculadora.cs
git commit -m "Implementación inicial de la calculadora"
```
- Al finalizar, debe abrir un pull request para que el docente pueda revisar su código.
```bash
git push origin tp1-calculadora
```
- Debe volver a la rama principal después de abrir el pull request.
```bash
git checkout main
```
- El docente revisará el código y proporcionará feedback. Se recomienda responder a los comentarios y realizar las correcciones necesarias para mejorar la solución.
