namespace backWeb.Models
{
    /// <summary>
    /// Medicamento del inventario del dispensario (diagrama de clases: Medicamento).
    /// Además de los atributos del diagrama, guarda el Stock disponible y un
    /// RowVersion para el control de concurrencia al descontar stock (HU1, RNF07).
    /// </summary>
    public class Medicamento
    {
        public int IdMedicamento { get; set; }

        public string NombreComercial { get; set; } = string.Empty;

        public string PrincipioActivo { get; set; } = string.Empty;

        public string Presentacion { get; set; } = string.Empty;

        public string Concentracion { get; set; } = string.Empty;

        /// <summary>Unidades disponibles en el dispensario. Nunca puede ser negativo.</summary>
        public int Stock { get; set; }

        /// <summary>
        /// Versión de la fila (rowversion en SQL Server). La base la cambia en cada UPDATE;
        /// si dos usuarios descuentan stock a la vez, el segundo recibe un conflicto
        /// en lugar de sobrescribir el valor del primero.
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public List<DetallePlantillaReceta> DetallesPlantilla { get; set; } = new();

        public string ObtenerDescripcionCompleta()
        {
            return $"{NombreComercial} ({PrincipioActivo}) {Concentracion} - {Presentacion}";
        }

        /// <summary>
        /// Descuenta del stock la cantidad exacta entregada.
        /// Rechaza cantidades no positivas y cantidades mayores al stock disponible.
        /// </summary>
        public void DescontarStock(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad a entregar debe ser mayor que cero.");
            }

            if (cantidad > Stock)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente para {NombreComercial}: disponible {Stock}, solicitado {cantidad}.");
            }

            Stock -= cantidad;
        }
    }
}
