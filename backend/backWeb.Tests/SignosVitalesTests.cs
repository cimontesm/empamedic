using backWeb.Models;

namespace backWeb.Tests
{
    /// <summary>
    /// Pruebas de dominio de SignosVitales: no usan base de datos.
    /// </summary>
    public class SignosVitalesTests
    {
        // Un adulto sano: punto de partida que cada prueba modifica.
        private static SignosVitales CrearValidos()
        {
            return new SignosVitales
            {
                SaturacionOxigeno = 98,
                Temperatura = 36.5m,
                FrecuenciaCardiaca = 72,
                FrecuenciaRespiratoria = 16,
                PresionArterial = "115/75",
                PesoKg = 70m,
                AlturaMetros = 1.75m,
            };
        }

        [Fact]
        public void Validar_ValoresNormales_NoDevuelveErrores()
        {
            var signos = CrearValidos();

            Assert.Empty(signos.Validar());
        }

        [Fact]
        public void Validar_ValoresEnLosExtremosDelRango_SonValidos()
        {
            var signos = new SignosVitales
            {
                SaturacionOxigeno = SignosVitales.SaturacionMaxima,
                Temperatura = SignosVitales.TemperaturaMinima,
                FrecuenciaCardiaca = SignosVitales.FrecuenciaCardiacaMaxima,
                FrecuenciaRespiratoria = SignosVitales.FrecuenciaRespiratoriaMinima,
                PresionArterial = "300/200",
                PesoKg = SignosVitales.PesoMaximoKg,
                AlturaMetros = SignosVitales.AlturaMinimaMetros,
            };

            Assert.Empty(signos.Validar());
        }

        [Theory]
        [InlineData(49)]
        [InlineData(101)]
        public void Validar_SaturacionFueraDeRango_DevuelveError(int saturacion)
        {
            var signos = CrearValidos();
            signos.SaturacionOxigeno = saturacion;

            var error = Assert.Single(signos.Validar());
            Assert.Contains("saturación", error);
        }

        [Theory]
        [InlineData("29.9")]
        [InlineData("45.1")]
        [InlineData("370")]
        public void Validar_TemperaturaFueraDeRango_DevuelveError(string temperatura)
        {
            var signos = CrearValidos();
            signos.Temperatura = decimal.Parse(temperatura, System.Globalization.CultureInfo.InvariantCulture);

            var error = Assert.Single(signos.Validar());
            Assert.Contains("temperatura", error);
        }

        [Theory]
        [InlineData(19)]
        [InlineData(251)]
        public void Validar_FrecuenciaCardiacaFueraDeRango_DevuelveError(int frecuencia)
        {
            var signos = CrearValidos();
            signos.FrecuenciaCardiaca = frecuencia;

            var error = Assert.Single(signos.Validar());
            Assert.Contains("frecuencia cardíaca", error);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(61)]
        public void Validar_FrecuenciaRespiratoriaFueraDeRango_DevuelveError(int frecuencia)
        {
            var signos = CrearValidos();
            signos.FrecuenciaRespiratoria = frecuencia;

            var error = Assert.Single(signos.Validar());
            Assert.Contains("frecuencia respiratoria", error);
        }

        [Theory]
        [InlineData("")]
        [InlineData("120")]
        [InlineData("120-80")]
        [InlineData("abc/80")]
        [InlineData("1200/80")]
        [InlineData("120/80/60")]
        public void Validar_PresionConFormatoIncorrecto_DevuelveErrorDeFormato(string presion)
        {
            var signos = CrearValidos();
            signos.PresionArterial = presion;

            var error = Assert.Single(signos.Validar());
            Assert.Contains("formato", error);
        }

        [Fact]
        public void Validar_PresionConEspaciosAlrededor_EsValida()
        {
            var signos = CrearValidos();
            signos.PresionArterial = " 120/80 ";

            Assert.Empty(signos.Validar());
        }

        [Theory]
        [InlineData("40/30", "sistólica debe estar")]
        [InlineData("310/90", "sistólica debe estar")]
        [InlineData("100/25", "diastólica debe estar")]
        [InlineData("80/90", "mayor que la diastólica")]
        public void Validar_PresionFueraDeRango_DevuelveError(string presion, string mensajeEsperado)
        {
            var signos = CrearValidos();
            signos.PresionArterial = presion;

            Assert.Contains(signos.Validar(), e => e.Contains(mensajeEsperado));
        }

        [Theory]
        [InlineData("19.9")]
        [InlineData("300.1")]
        public void Validar_PesoFueraDeRango_DevuelveError(string peso)
        {
            var signos = CrearValidos();
            signos.PesoKg = decimal.Parse(peso, System.Globalization.CultureInfo.InvariantCulture);

            var error = Assert.Single(signos.Validar());
            Assert.Contains("peso", error);
        }

        [Theory]
        [InlineData("0.99")]
        [InlineData("2.51")]
        [InlineData("175")]
        public void Validar_TallaFueraDeRango_DevuelveError(string altura)
        {
            var signos = CrearValidos();
            signos.AlturaMetros = decimal.Parse(altura, System.Globalization.CultureInfo.InvariantCulture);

            var error = Assert.Single(signos.Validar());
            Assert.Contains("talla", error);
        }

        [Fact]
        public void Validar_VariosCamposMal_DevuelveUnErrorPorCadaUno()
        {
            var signos = new SignosVitales();

            // Todo en cero y presión vacía: fallan los 7 signos.
            Assert.Equal(7, signos.Validar().Count);
        }

        [Fact]
        public void CalcularIMC_PesoYTallaValidos_RedondeaADosDecimales()
        {
            var signos = CrearValidos();

            // 70 / (1.75 * 1.75) = 22.857...
            Assert.Equal(22.86m, signos.CalcularIMC());
        }

        [Fact]
        public void CalcularIMC_SinTalla_LanzaExcepcion()
        {
            var signos = CrearValidos();
            signos.AlturaMetros = 0m;

            Assert.Throws<InvalidOperationException>(() => signos.CalcularIMC());
        }

        [Theory]
        [InlineData("85/55", "Hipotensión")]
        [InlineData("110/70", "Normal")]
        [InlineData("119/79", "Normal")]
        [InlineData("125/75", "Elevada")]
        [InlineData("135/75", "Hipertensión etapa 1")]
        [InlineData("118/85", "Hipertensión etapa 1")]
        [InlineData("145/85", "Hipertensión etapa 2")]
        [InlineData("125/95", "Hipertensión etapa 2")]
        [InlineData("185/100", "Crisis hipertensiva")]
        [InlineData("150/125", "Crisis hipertensiva")]
        public void ClasificarPresion_DevuelveLaCategoriaCorrecta(string presion, string categoria)
        {
            var signos = CrearValidos();
            signos.PresionArterial = presion;

            Assert.Equal(categoria, signos.ClasificarPresion());
        }

        [Fact]
        public void ClasificarPresion_FormatoIncorrecto_LanzaExcepcion()
        {
            var signos = CrearValidos();
            signos.PresionArterial = "120-80";

            Assert.Throws<InvalidOperationException>(() => signos.ClasificarPresion());
        }

        [Theory]
        [InlineData("36.5", false)]
        [InlineData("37.9", false)]
        [InlineData("38.0", true)]
        [InlineData("39.2", true)]
        public void TieneFiebre_UsaElUmbralDe38Grados(string temperatura, bool esperado)
        {
            var signos = CrearValidos();
            signos.Temperatura = decimal.Parse(temperatura, System.Globalization.CultureInfo.InvariantCulture);

            Assert.Equal(esperado, signos.TieneFiebre());
        }
    }
}
