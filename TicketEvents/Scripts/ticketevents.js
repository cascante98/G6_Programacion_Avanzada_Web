/* TicketEvents - utilidades comunes del cliente.
 * Todas las llamadas AJAX consumen la Web API interna (/api/...). */
(function ($) {
    'use strict';

    var raiz = (document.body.getAttribute('data-app-root') || '/');

    var TE = window.TE = {
        /** Construye la URL de la API respetando la ruta virtual de la aplicación. */
        api: function (ruta) { return raiz + 'api/' + ruta.replace(/^\/+/, ''); },

        /** Muestra el Toast global de Bootstrap. tipo: success | danger | warning | info */
        toast: function (mensaje, tipo) {
            var el = document.getElementById('appToast');
            if (!el) { alert(mensaje); return; }
            el.className = 'toast align-items-center border-0 text-bg-' + (tipo || 'success');
            document.getElementById('appToastTexto').textContent = mensaje;
            bootstrap.Toast.getOrCreateInstance(el, { delay: 4500 }).show();
        },

        /** Extrae un mensaje legible de una respuesta de error de la API. */
        mensajeError: function (xhr, porDefecto) {
            if (xhr.status === 401) return (xhr.responseJSON && xhr.responseJSON.Message) || 'Tu sesión expiró. Inicia sesión de nuevo.';
            if (xhr.status === 403) return 'No tienes permiso para realizar esta acción.';
            return (xhr.responseJSON && (xhr.responseJSON.Message || xhr.responseJSON.message)) || porDefecto || 'Ocurrió un error.';
        },

        /** Activa/desactiva el spinner dentro de un botón. */
        cargando: function ($boton, activo) {
            $boton.prop('disabled', activo);
            $boton.find('.spinner-border').toggleClass('d-none', !activo);
        },

        actualizarCarrito: function (totalEntradas) {
            $('#carrito-badge').text(totalEntradas).toggleClass('d-none', !totalEntradas);
        },

        moneda: function (valor) {
            return new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC', maximumFractionDigits: 0 }).format(valor || 0);
        }
    };

    /* Agregar al carrito (POST api/carrito) sin recargar la página. */
    $(document).on('submit', '.form-agregar-carrito', function (e) {
        e.preventDefault();
        var $form = $(this), $boton = $form.find('button[type=submit]');
        var cantidad = parseInt($form.find('[name=Cantidad]').val(), 10);

        if (!(cantidad >= 1 && cantidad <= 20)) {
            TE.toast('Seleccione una cantidad entre 1 y 20.', 'warning');
            return;
        }

        TE.cargando($boton, true);
        $.ajax({
            url: TE.api('carrito'),
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ EventoId: parseInt($form.find('[name=EventoId]').val(), 10), Cantidad: cantidad })
        }).done(function (r) {
            TE.toast(r.mensaje, 'success');
            TE.actualizarCarrito(r.totalEntradas);
        }).fail(function (xhr) {
            TE.toast(TE.mensajeError(xhr, 'No fue posible agregar al carrito.'), 'danger');
        }).always(function () {
            TE.cargando($boton, false);
        });
    });
})(jQuery);
