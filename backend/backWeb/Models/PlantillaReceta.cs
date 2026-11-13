namespace backWeb.Models
{
    /// <summary>
    /// Plantilla de receta predefinida para una patología común (RF06, HU4).
    /// Agrupa los medicamentos sugeridos con su dosis, frecuencia y duración.
    /// </summary>
    public class PlantillaReceta
    {
        public int IdPlantilla { get; set; }

        public string NombrePlantilla { get; set; } = string.Empty;

        public string DiagnosticoAsociado { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public List<DetallePlantillaReceta> Items { get; set; } = new();

        public void AgregarItem(Medicamento med, string dosis, string frec, string dur, string ind)
        {
            ArgumentNullException.ThrowIfNull(med);

            Items.Add(new DetallePlantillaReceta
            {
                Plantilla = this,
                Medicamento = med,
                MedicamentoId = med.IdMedicamento,
                DosisSugerida = dosis,
                FrecuenciaSugerida = frec,
                DuracionSugerida = dur,
                IndicacionesSugeridas = ind,
            });
        }

        public void RemoverItem(int idDetalle)
        {
            var item = Items.FirstOrDefault(i => i.IdDetallePlantilla == idDetalle);
            if (item is null)
            {
                throw new KeyNotFoundException($"La plantilla no tiene un ítem con id {idDetalle}.");
            }

            Items.Remove(item);
        }

        public IReadOnlyList<DetallePlantillaReceta> ObtenerItems()
        {
            return Items.AsReadOnly();
        }
    }
}
