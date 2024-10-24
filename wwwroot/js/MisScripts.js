function CambiarEspecialidad( Id,Nombre) {

    var boton = document.getElementById('dropdownMenuButton1');
    boton.textContent = Nombre;

    var InputEsp = document.getElementById('InputEspecialidad');
    InputEsp.value = Id;

   
   
}

function VerificarFecha() {

    const input = document.getElementById('fechaHoraInput');
    const fechaSeleccionada = new Date(input.value);

    // Obtener el día de la semana: 0 = Domingo, 6 = Sábado
    const diaSemana = fechaSeleccionada.getDay();

    if (diaSemana === 0 || diaSemana === 6) { // 0: Domingo, 6: Sábado
        alert('Por favor, seleccione una fecha de lunes a viernes.');
        return false; // Evita el envío del formulario
    }
    return true; // Permite el envío del formulario
}

