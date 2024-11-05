function CambiarEspecialidad( Id,Nombre) {

    var boton = document.getElementById('dropdownMenuButton1');
    boton.textContent = Nombre;

    var InputEsp = document.getElementById('InputEspecialidad');
    InputEsp.value = Id;

   
   
}

function VerificarFecha() {
    const input = document.getElementById('fechaHoraInput');
    const fechaSeleccionada = new Date(input.value);
    const fechaActual = new Date();

    // Verificar si el mes o año de la fecha seleccionada está en el pasado
    if (
        fechaSeleccionada.getFullYear() < fechaActual.getFullYear() ||
        (fechaSeleccionada.getFullYear() === fechaActual.getFullYear() && fechaSeleccionada.getMonth() < fechaActual.getMonth())
    ) {
        alert('Por favor, seleccione un mes actual o a futuro.');
        return false; // Evita el envío del formulario
    }

    // Obtener el día de la semana: 0 = Domingo, 6 = Sábado
    const diaSemana = fechaSeleccionada.getDay();
    if (diaSemana === 0 || diaSemana === 6) { // 0: Domingo, 6: Sábado
        alert('Por favor, seleccione una fecha de lunes a viernes.');
        return false; // Evita el envío del formulario
    }

    return true; // Permite el envío del formulario
}

function IngresarDevolucion() {

    const boton = document.getElementById("Devolucion");
    if (boton.value == "") {
        alert('Debe ingresar una devolucion para el paciente...');
        return false;
    }

    return true;

}
