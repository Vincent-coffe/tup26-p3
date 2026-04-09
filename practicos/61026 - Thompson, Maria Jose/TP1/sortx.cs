using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

record CriterioOrden(string Campo, bool EsNumerico, bool EsDescendente);
record Configuracion(
    string? ArchivoEntrada,
    string? ArchivoSalida,
    string Delimitador,
    bool SinEncabezado,
    List<CriterioOrden> Criterios
);
