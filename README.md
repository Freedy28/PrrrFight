# Prrrfight 🐾⚔️. 

**Prrrfight** es un videojuego de estrategia táctica por turnos (TRPG) con ambientación medieval, donde los valientes protagonistas son gatos guerreros. Desarrollado en **Unity**, diseñado específicamente para dispositivos móviles con una orientación vertical.

---

## 🎮 Resumen del Juego
- **Plataforma:** Android / iOS (Vertical).
- **Género:** Tactical RPG (TRPG).
- **Motor:** Unity 2022+.
- **Estilo Visual:** Sprites 2D animados.

## 📋 Mecánicas Principales
* **Tablero Táctico:** Movimiento basado en casillas con diferentes alcances según la clase.
* **Sistema de Turnos:** Orden estratégico para ejecutar ataques y habilidades.
* **Menú Dinámico:** Pantalla principal animada con los guerreros felinos.

---

## 🐈 Clases de Guerreros

* **🛡️ Tanque (Tank)**
    * **Estética:** Pelaje café o negro, caracterizado por un pequeño casco.
    * **Vida:** 20-22 PS (Alta).
    * **Movilidad:** Corta (2 casillas ortogonales / 1 diagonal).
    * **Rol:** Primera línea de batalla y mitigación de daño.

* **🕊️ Clérigo (Healer)**
    * **Estética:** Gato blanco (liso, atigrado o moteado) con indumentaria religiosa.
    * **Vida:** 16 PS (Media).
    * **Movilidad:** Corta (2 casillas ortogonales / 1 diagonal).
    * **Rol:** Curación y potenciamiento (buffs) de aliados.

* **💢 Luchador (Berserk)**
    * **Estética:** Tono rojizo con rasgos distintivos como cicatrices o espadas de madera.
    * **Vida:** 18 PS (Media).
    * **Movilidad:** Media (3 casillas ortogonales / 2 diagonales).
    * **Rol:** Peleador cuerpo a cuerpo de gran impacto.

* **🏹 Tirador (Archer)**
    * **Estética:** Color verdoso o gris atigrado con accesorios de arquero clásico.
    * **Vida:** 12 PS (Baja).
    * **Movilidad:** Alta (4 casillas ortogonales / 2 diagonales).
    * **Rol:** Ataque a distancia y control desde la retaguardia.

---

## 🔥 Habilidades por Clase

### Tanque (Tank)
* **Valor de lucha:** Obtiene un escudo de 1 PS (Costo 0).
* **Fuerza de lucha:** Siguiente ataque básico +2 PS y empuja al objetivo una casilla.

### Clérigo (Healer)
* **Bendición:** Cura 2 PS a cualquier unidad (Costo bajo).
* **Escudo de fe:** Reduce el daño recibido en un 50% (Costo medio).
* **Ruega por nosotros:** Cura 5 PS, pero queda inhabilitado por 2 turnos (Costo alto).

### Luchador (Berserk)
* **Fuerza:** Siguiente golpe potenciado a 3 PS (Costo bajo).
* **Desgarrar:** Quita 5 PS y aplica *Sangrado* (-1 PS al siguiente turno).
* **Multi-puño:** 4 golpes de 1 PS en direcciones cardinales y retrocede 2 casillas.

### Tirador (Archer)
* **Mil flechas:** Quita 1 PS a todos los enemigos en el mapa.
* **Coyeye:** Golpe cuerpo a cuerpo (1 PS) y corre 4 casillas en cualquier dirección.
* **Super flecha:** 6 PS de daño e ignora escudos (Costo muy alto).
* **Resortera:** Proyectil en línea recta que impacta al primer objetivo (2 PS).

## 🎨 Mockups 
### Menú Principal
Diseñado para ofrecer una entrada rápida a la acción, priorizando el arte visual de las unidades.
* **Encabezado:** Banner rústico de madera con el título dinámico del juego.
* **Acciones Principales:** Botonera simplificada que incluye el acceso directo al combate y el panel de configuración, evitando la saturación de opciones.
<img width="1664" height="2574" alt="Menu de inicio" src="https://github.com/user-attachments/assets/b0114fe1-c2e6-4664-9033-fd974d766fc2" />


### Perfil del Héroe
Panel detallado que aparece al seleccionar una unidad, proporcionando toda la información necesaria para la estrategia antes del despliegue.
* **Identidad:** Visualización del nombre de la unidad, clase (Iconografía) y nivel.
* **Matriz de Estadísticas:** Grilla de 2x2 que muestra de forma iconográfica los atributos base: Vida (PS), Ataque, Movimiento y Defensa.
* **Desglose de Habilidades:** Listado vertical que detalla el efecto de cada habilidad y su costo de ejecución, facilitando la comprensión de los roles (ej. el Tanque como mitigador de daño).
<img width="1664" height="2574" alt="Detalles personajes" src="https://github.com/user-attachments/assets/74e22413-cde6-4a60-b576-980e128a6e6e" />


### Panel de Ajustes
Una ventana emergente (pop-up) con efecto de desenfoque de fondo para no perder el contexto del juego.
* **Controles Temáticos:** Deslizadores (sliders) de volumen que utilizan huellas de gato como indicadores de posición.
* **Gestión de Sonido:** Separación de canales para música de ambiente y efectos de sonido (SFX), permitiendo una personalización completa de la experiencia auditiva.
<img width="1664" height="2574" alt="Ajustes" src="https://github.com/user-attachments/assets/a7e787a5-d8ec-4a06-bb4c-c9877670b773" />


### 🛠️ Especificaciones Técnicas del Diseño
* **Relación de Aspecto:** 9:16 (Optimizado para dispositivos móviles modernos).
* **Paleta de Colores:** Tonos tierra, maderas oscuras y colores vibrantes para indicadores de vida y habilidades.
* **Tipografía:** Estilo Pixel/Fantasy para coherencia con el género TRPG.


# Guía de Instalación: Unity y 3ds Max

Este repositorio contiene los recursos visuales necesarios para ayudarte en el proceso de descarga e instalación de las herramientas de desarrollo Unity y 3ds Max.

## 1. Instalación de Unity
Unity es el motor de desarrollo líder para la creación de juegos multiplataforma y experiencias interactivas en 2D y 3D.

### Video Tutorial: Cómo descargar Unity
En este video se explica cómo descargar el **Unity Hub** y seleccionar la versión del editor adecuada para tus proyectos.

(https://1drv.ms/v/c/07657d93c1d922b5/IQD8aZDmeUi7SJs0B7VHVZ0FAZtk0_Epsu294TDJ1JTY6pg?e=eFeohn)

## 2. Instalación de 3ds Max
Autodesk 3ds Max es una potente solución de modelado, animación y renderizado 3D utilizada por profesionales del diseño y la creación de videojuegos.

### Video Tutorial: Cómo descargar 3ds Max
Este video detalla los pasos para obtener la versión oficial (o educativa) de 3ds Max desde el portal de Autodesk.

(https://1drv.ms/v/c/07657d93c1d922b5/IQDaBYaMRFRxTaKT3AGqz9wUAb5v5vx8gh93gitl8wEXMZU?e=arkG2O)


## Requisitos Previos
- Conexión a internet estable.
- Cuenta de usuario en Unity (ID de Unity).
- Cuenta de usuario en Autodesk.





