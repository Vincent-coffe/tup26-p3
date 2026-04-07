namespace Tup26.AlumnosApp;

static class EstadoExtensions {
    public static string ToEmoji(this Estado estado) {
        return estado switch {
            Estado.Desaprobado => "🔴",
            Estado.Revision => "🟠",
            Estado.Pendiente => "🟡",
            Estado.Aprobado => "🟢",
            Estado.Vacio => "⚪️",
            _ => string.Empty
        };
    }

    public static Estado Parse(string? valor) {
        string? v = valor?.Trim().ToUpperInvariant();

        return v switch {
            "🔴" or "D" => Estado.Desaprobado,
            "🟠" or "R" => Estado.Revision,
            "🟡" or "P" => Estado.Pendiente,
            "🟢" or "A" => Estado.Aprobado,
            "" or "-" or "⚪️" or null => Estado.Vacio,
            _ => Estado.Vacio,
        };
    }
}
