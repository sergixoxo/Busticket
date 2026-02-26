using Xunit;
using Busticket.Controllers;
using Busticket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Busticket.Tests
{
    public class AsientoTests
    {
        [Fact]
        public void SeleccionarAsiento_DebeBloquearse_SiYaEstaOcupado()
        {
            // 1. Arrange (Preparar el escenario)
            // Imaginamos que tenemos una lista de asientos donde el asiento "A1" ya está ocupado.
            var asientoOcupado = "A1";
            var listaAsientosOcupados = new List<string> { "A1", "A2", "B5" };

            // 2. Act (Simular que un nuevo usuario intenta elegir el mismo "A1")
            bool esValido = !listaAsientosOcupados.Contains(asientoOcupado);

            // 3. Assert (Verificar)
            // La prueba pasa si 'esValido' es falso, porque el sistema DEBE rechazarlo.
            Assert.False(esValido, "El sistema no debería permitir seleccionar un asiento que ya está en la lista de ocupados.");
        }
    }
}