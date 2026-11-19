namespace appEmpamedic.Models
{
    /// <summary>
    /// Medicamento del inventario del dispensario.
    /// DTO del lado de la app: debe coincidir con lo que devuelva la API (integrante 3).
    /// </summary>
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }

        public bool StockBajo => StockActual <= StockMinimo;
    }
}
