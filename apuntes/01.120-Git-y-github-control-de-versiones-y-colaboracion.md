# Git y GitHub: control de versiones y colaboración

## Objetivo

La idea de este apunte es entender **qué problema resuelven Git y GitHub**, cuáles son sus conceptos básicos y cómo se usan en un flujo de trabajo simple.

Al terminar, deberías poder responder tres preguntas:

- ¿Qué es Git?
- ¿Qué es GitHub?
- ¿Cómo se usa todo eso en un proyecto real?

## ¿Qué es Git?

**Git** es un sistema de control de versiones distribuido.

Eso significa que permite registrar cambios en archivos a lo largo del tiempo, volver a versiones anteriores si hace falta y trabajar con ramas sin romper el trabajo principal.

En la práctica, Git sirve para:

- guardar el historial de un proyecto,
- comparar cambios,
- deshacer errores,
- trabajar en equipo,
- y probar ideas en ramas separadas.

Una forma simple de pensarlo es esta:

> Git es la memoria del proyecto.

## ¿Qué es GitHub?

**GitHub** es una plataforma web para alojar repositorios Git y colaborar sobre ellos.

Git guarda el historial.
GitHub agrega una capa social y de trabajo en equipo:

- repositorios remotos,
- issues,
- pull requests,
- revisiones de código,
- acciones automatizadas,
- y publicación de proyectos.

Git puede usarse sin GitHub.
GitHub, en cambio, necesita Git para que el repositorio tenga historial real.

### GitHub Desktop

**GitHub Desktop** es una aplicación gráfica para usar Git sin escribir tantos comandos.

Es útil para empezar porque permite:

- ver cambios,
- crear commits,
- cambiar de rama,
- y sincronizar con GitHub.

Sirve mucho para aprender el flujo básico, aunque después conviene conocer también la línea de comandos.

### GitHub CLI

**GitHub CLI** (`gh`) es la herramienta de línea de comandos de GitHub.

Permite hacer desde la terminal varias tareas que normalmente harías en la web:

- iniciar sesión,
- crear repositorios,
- abrir pull requests,
- ver issues,
- y revisar información de repositorios.

Es muy práctica cuando ya estás cómodo trabajando en consola.

## Conceptos básicos

### Repositorio

Un **repositorio** es la carpeta del proyecto más el historial que Git guarda de ese proyecto.

Puede ser:

- **local**, si está en tu computadora,
- **remoto**, si está alojado en GitHub u otro servicio.

### Commit

Un **commit** es una foto del estado del proyecto en un momento dado.

Cada commit guarda:

- qué cambió,
- quién lo cambió,
- cuándo se cambió,
- y un mensaje descriptivo.

Un buen mensaje de commit ayuda a entender el historial.

### Rama

Una **rama** es una línea de desarrollo independiente.

La rama principal suele llamarse `main`.

Las ramas sirven para:

- desarrollar una funcionalidad nueva,
- corregir un error,
- o probar una idea sin tocar la rama principal.

### Staging area

El **staging area** es una zona intermedia entre los cambios que hiciste y el commit final.

Primero modificás archivos.
Después elegís qué cambios van al commit.
Recién ahí registrás el commit.

### Remote

Un **remote** es una copia del repositorio en otro lugar, normalmente en GitHub.

El remote más común se llama `origin`.

### Merge y pull request

**Merge** significa unir el contenido de una rama con otra.

En GitHub, el proceso suele pasar por un **pull request**:

1. trabajás en una rama,
2. subís los cambios,
3. abrís un pull request,
4. revisan el código,
5. y luego se fusiona con la rama principal.

## Flujo de trabajo básico

Un flujo simple y muy usado es este:

1. crear un repositorio,
2. clonar el repositorio en tu computadora,
3. hacer cambios,
4. revisar el estado,
5. agregar los cambios al staging area,
6. crear un commit,
7. subir los cambios a GitHub,
8. y repetir.

