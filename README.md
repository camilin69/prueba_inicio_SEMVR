## 📌 Descripción General  
Este proyecto consiste en controlar la animación de una mano virtual en Unity, permitiendo cerrar el puño con los pulgares arriba y abrirla mediante teclas. Maneja huesos individuales de los dedos con rotaciones personalizadas y un sistema de temporización para controlar los estados de la animación.

---

## 📖 Librerías Utilizadas
- UnityEngine: Para acceso a componentes de Unity (Transform, Renderer, Corrutinas)

- System.Collections: Para usar corrutinas (IEnumerator)

- System.Collections.Generic: Para usar estructuras de datos como List y Dictionary

---

## ⚙️ Mecánicas
- Teclas Disparadoras de Evento: 
    * fistKey (T): Cierra la mano
    * openKey (R): Abre la mano
- Funcionamiento:
El cubo que está en frente de las manos se puede interpretar como un semáforo que representa el estado de el evento, rojo para decir que no se ha activado ningún evento o el sistema está en reposo, amarillo cuando el sistema activa el evento y está esperando que pase 1.5s (delay), verde cuando se está ejecutando la animación de la mano (abriendose o cerrandose).
---

## ⌛ Manejo de Delay

El método utilizado para el manejo de los tiempos entre el reposo y las animaciones fué IEnumerator (corrutina), es un código que se ejecuta en varios frames, cuando `isAnimationLocked = true;` impide que nuevas animaciones se inicien mientras está en curso.
`SetStateColor(waitingColor); // Cambia a amarillo`,
`yield return new WaitForSeconds(animationDelay); // Pausa mágica`
'yield return' le dice a unity que pause la ejecución aquí, 'waitForSeconds' hace que espere exactamente lo de que tarde la animación, durante este tiempo el cubo permanece amarillo, el método no consume recursos, el juego sigue funcionando normalmente.

- Ventajas de las Corrutinas:
    * Precisión temporal: WaitForSeconds usa el tiempo de juego, no el reloj real

    * Eficiencia: No necesita Update() ni variables temporales complejas

    * Legibilidad: Flujo secuencial claro (parece código síncrono)

    * Control frame a frame: Permite animaciones suaves interpoladas

- Características Clave del Delay:

    * No es un simple timer: La corrutina puede ser pausada/continuada

    * Escala con timeScale: Afectado por la escala de tiempo del juego

    * Cancelable: Podría interrumpirse con StopCoroutine()

    * Independiente del framerate: Usa tiempo real, no frames
---

## 📈 Posibles mejoras técnicas

- Gestión de memoria ineficiente: Uso de Dictionary para almacenar transformaciones que crece indefinidamente y causa garbage collection.
- Validación de NULL Reference: Falta de validación robusta causando crashes en runtime.
- Eliminación de Búsquedas en Dictionary:  60+ búsquedas en Dictionary por frame (O(n) complexity).
- Sistema de Input ineficiente: Input.GetKeyDown() llamado cada frame innecesariamente.
- Sistema de estados robusto.
- Sistema de animación modular.
- Manejo de errores en corrutinas.

