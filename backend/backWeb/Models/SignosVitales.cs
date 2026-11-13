using System.Text.RegularExpressions;

namespace backWeb.Models
{
    /// <summary>
    /// Signos vitales tomados en una consulta médica (diagrama de clases: SignosVitales, RF04).
    /// Los rangos de <see cref="Validar"/> no dicen si el valor es "normal": solo descartan
    /// valores imposibles o errores de digitación (por ejemplo 370 en vez de 37.0).
    /// </summary>
    public class SignosVitales
    {
        // Rangos aceptados (incluyen los extremos).
        public const int SaturacionMinima = 50;
        public const int SaturacionMaxima = 100;
        public const decimal TemperaturaMinima = 30.0m;
        public const decimal TemperaturaMaxima = 45.0m;
        public const int FrecuenciaCardiacaMinima = 20;
        public const int FrecuenciaCardiacaMaxima = 250;
        public const int FrecuenciaRespiratoriaMinima = 4;
        public const int FrecuenciaRespiratoriaMaxima = 60;
        public const int SistolicaMinima = 50;
        public const int SistolicaMaxima = 300;
        public const int DiastolicaMinima = 30;
        public const int DiastolicaMaxima = 200;
        public const decimal PesoMinimoKg = 20m;
        public const decimal PesoMaximoKg = 300m;
        public const decimal AlturaMinimaMetros = 1.00m;
        public const decimal AlturaMaximaMetros = 2.50m;

        /// <summary>Temperatura (°C) desde la que se considera fiebre.</summary>
        public const decimal UmbralFiebre = 38.0m;

        // Formato "sistólica/diastólica" con 2 o 3 dígitos cada una, por ejemplo "120/80".
        private static readonly Regex _formatoPresion = new(@"^(\d{2,3})/(\d{2,3})$", RegexOptions.Compiled);

        public int IdSignoVital { get; set; }

        /// <summary>SpO2 en porcentaje.</summary>
        public int SaturacionOxigeno { get; set; }

        /// <summary>Temperatura en grados Celsius.</summary>
        public decimal Temperatura { get; set; }

        /// <summary>Latidos por minuto.</summary>
        public int FrecuenciaCardiaca { get; set; }

        /// <summary>Respiraciones por minuto.</summary>
        public int FrecuenciaRespiratoria { get; set; }

        /// <summary>Presión arterial como texto "sistólica/diastólica", por ejemplo "120/80" (mmHg).</summary>
        public string PresionArterial { get; set; } = string.Empty;

        public decimal PesoKg { get; set; }

        public decimal AlturaMetros { get; set; }

        /// <summary>
        /// Revisa todos los campos y devuelve la lista de errores encontrados.
        /// Si la lista está vacía, los signos vitales son válidos.
        /// </summary>
        public IReadOnlyList<string> Validar()
        {
            var errores = new List<string>();

            if (SaturacionOxigeno < SaturacionMinima || SaturacionOxigeno > SaturacionMaxima)
            {
                errores.Add($"La saturación de oxígeno debe estar entre {SaturacionMinima} y {SaturacionMaxima} %.");
            }

            if (Temperatura < TemperaturaMinima || Temperatura > TemperaturaMaxima)
            {
                errores.Add($"La temperatura debe estar entre {TemperaturaMinima} y {TemperaturaMaxima} °C.");
            }

            if (FrecuenciaCardiaca < FrecuenciaCardiacaMinima || FrecuenciaCardiaca > FrecuenciaCardiacaMaxima)
            {
                errores.Add($"La frecuencia cardíaca debe estar entre {FrecuenciaCardiacaMinima} y {FrecuenciaCardiacaMaxima} lpm.");
            }

            if (FrecuenciaRespiratoria < FrecuenciaRespiratoriaMinima || FrecuenciaRespiratoria > FrecuenciaRespiratoriaMaxima)
            {
                errores.Add($"La frecuencia respiratoria debe estar entre {FrecuenciaRespiratoriaMinima} y {FrecuenciaRespiratoriaMaxima} rpm.");
            }

            if (!IntentarLeerPresion(out var sistolica, out var diastolica))
            {
                errores.Add("La presión arterial debe tener el formato sistólica/diastólica, por ejemplo 120/80.");
            }
            else
            {
                if (sistolica < SistolicaMinima || sistolica > SistolicaMaxima)
                {
                    errores.Add($"La presión sistólica debe estar entre {SistolicaMinima} y {SistolicaMaxima} mmHg.");
                }

                if (diastolica < DiastolicaMinima || diastolica > DiastolicaMaxima)
                {
                    errores.Add($"La presión diastólica debe estar entre {DiastolicaMinima} y {DiastolicaMaxima} mmHg.");
                }

                if (sistolica <= diastolica)
                {
                    errores.Add("La presión sistólica debe ser mayor que la diastólica.");
                }
            }

            if (PesoKg < PesoMinimoKg || PesoKg > PesoMaximoKg)
            {
                errores.Add($"El peso debe estar entre {PesoMinimoKg} y {PesoMaximoKg} kg.");
            }

            if (AlturaMetros < AlturaMinimaMetros || AlturaMetros > AlturaMaximaMetros)
            {
                errores.Add($"La talla debe estar entre {AlturaMinimaMetros} y {AlturaMaximaMetros} m.");
            }

            return errores;
        }

        /// <summary>
        /// Índice de masa corporal: peso (kg) / talla² (m), redondeado a 2 decimales.
        /// </summary>
        public decimal CalcularIMC()
        {
            if (AlturaMetros <= 0)
            {
                throw new InvalidOperationException("No se puede calcular el IMC sin una talla mayor que cero.");
            }

            return Math.Round(PesoKg / (AlturaMetros * AlturaMetros), 2);
        }

        /// <summary>
        /// Clasifica la presión arterial según las categorías de la AHA (2017),
        /// más la categoría de hipotensión (menos de 90/60).
        /// </summary>
        public string ClasificarPresion()
        {
            if (!IntentarLeerPresion(out var sistolica, out var diastolica))
            {
                throw new InvalidOperationException(
                    $"La presión arterial '{PresionArterial}' no tiene el formato sistólica/diastólica.");
            }

            // Se revisa de la categoría más grave a la más leve: gana la primera que se cumpla.
            if (sistolica > 180 || diastolica > 120)
            {
                return "Crisis hipertensiva";
            }

            if (sistolica >= 140 || diastolica >= 90)
            {
                return "Hipertensión etapa 2";
            }

            if (sistolica >= 130 || diastolica >= 80)
            {
                return "Hipertensión etapa 1";
            }

            if (sistolica >= 120)
            {
                return "Elevada";
            }

            if (sistolica < 90 || diastolica < 60)
            {
                return "Hipotensión";
            }

            return "Normal";
        }

        /// <summary>Fiebre: temperatura de 38.0 °C o más.</summary>
        public bool TieneFiebre()
        {
            return Temperatura >= UmbralFiebre;
        }

        private bool IntentarLeerPresion(out int sistolica, out int diastolica)
        {
            sistolica = 0;
            diastolica = 0;

            var coincidencia = _formatoPresion.Match(PresionArterial.Trim());
            if (!coincidencia.Success)
            {
                return false;
            }

            sistolica = int.Parse(coincidencia.Groups[1].Value);
            diastolica = int.Parse(coincidencia.Groups[2].Value);
            return true;
        }
    }
}
