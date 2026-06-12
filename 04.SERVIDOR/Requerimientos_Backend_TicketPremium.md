# Documento de Requerimientos — Backend Web Services
## Proyecto TicketPremium — FIFA World Cup 2026

**Universidad de las Fuerzas Armadas**  
Departamento de Ciencias de la Computación — Ingeniería de Software  
Versión: 1.0 | Junio 2026

---

## 1. Tema

Definición de requerimientos para la capa de Web Services del proyecto TicketPremium, sistema mashup orientado a la comercialización de boletos para los partidos del Campeonato Mundial de Fútbol FIFA 2026, celebrado en Estados Unidos, Canadá y México.

---

## 2. Visión General

### 2.1 Objetivo General

Diseñar e implementar la capa de Web Services que permita la integración de los tres sistemas que conforman el proyecto TicketPremium: la Aplicación de la Federación FIFA, la Aplicación Comercializadora (TicketPremium) y el Core Bancario con su Módulo de Crédito.

Esta capa actúa como el contrato de integración que permite a TicketPremium orquestar el flujo completo de compra de localidades de manera consistente y controlada.

### 2.2 Alcance

**Está dentro del alcance:**

- Web Services SOAP expuestos por la Aplicación FIFA (partidos, localidades, disponibilidad).
- Web Services SOAP expuestos por el Core Bancario (validación de crédito, registro de amortización).
- Lógica de orquestación en TicketPremium que consume dichos servicios siguiendo el patrón **Saga Orquestada**.
- Manejo de errores y acciones compensatorias ante fallos en la secuencia de compra.
- Contratos WSDL de cada servicio expuesto.
- Modelo de datos de soporte para facturación, crédito y tabla de amortización.

**Está fuera del alcance:**

- Implementación del frontend (web, móvil, consola): se documenta únicamente como consumidor.
- Infraestructura de despliegue (servidores, contenedores, CI/CD).
- Integración con pasarelas de pago externas reales.
- Sistema de notificaciones por email o generación de PDFs de boletos.
- ESB o message broker (RabbitMQ u otros): no aplica por razones de diseño arquitectónico.

---

## 3. Requerimientos

### 3.1 Requerimientos Funcionales

#### Aplicación FIFA — Web Services expuestos

| ID | Nombre | Descripción | Sistema Origen |
|----|--------|-------------|----------------|
| RF-01 | Listar partidos disponibles | Exponer un WS SOAP que retorne todos los partidos cuya fecha sea mayor o igual a la fecha actual, incluyendo código, equipos, fecha, hora y lugar. | App FIFA |
| RF-02 | Listar localidades y estado de asientos por partido | Exponer un WS SOAP que, dado el código de un partido, retorne las localidades del estadio con su estado general (`LIBRE`, `PARCIAL`, `AGOTADA`) y, para cada localidad, el detalle de sus asientos individuales con su estado (`LIBRE`, `RESERVADO`, `VENDIDO`). Para los asientos en estado `VENDIDO`, retornar además: número de factura, cédula del cliente y nombre del cliente que ocupa ese asiento. Permite al frontend renderizar el mapa completo del estadio e identificar quién está sentado en cada localidad. | App FIFA |
| RF-03 | Reservar asiento (locking optimista) | Exponer un WS SOAP que intente cambiar el estado de un asiento de `LIBRE` a `RESERVADO`, usando locking optimista: el UPDATE aplica solo si `estado = 'LIBRE'`. Si otra transacción ganó la carrera, retorna un fault indicando que el asiento ya no está disponible. Al reservar, registra un `timestamp_reserva` para control de TTL. | App FIFA |
| RF-04 | Confirmar venta de asiento | Exponer un WS SOAP que cambie el estado de un asiento de `RESERVADO` a `VENDIDO`. Solo se invoca tras completar exitosamente el flujo de pago en TicketPremium. | App FIFA |
| RF-05 | Compensación: liberar asiento | Exponer un WS SOAP que revierta un asiento de `RESERVADO` a `LIBRE`. Utilizado como acción compensatoria ante fallo posterior en el flujo de compra, o al expirar el TTL de una reserva. Debe ser idempotente. | App FIFA |
| RF-06 | Expirar reservas vencidas | Proceso interno (o WS invocable por scheduler) que revierte a `LIBRE` todos los asientos en estado `RESERVADO` cuyo `timestamp_reserva` supere el TTL configurado (recomendado: 10 minutos). | App FIFA |

#### Core Bancario — Web Services expuestos

| ID | Nombre | Descripción | Sistema Origen |
|----|--------|-------------|----------------|
| RF-07 | Verificar sujeto de crédito | Exponer un WS SOAP que, dado el número de cédula del cliente, evalúe las 4 reglas de elegibilidad (cliente activo, depósito en último mes, edad ≥ 25 si género masculino, sin crédito activo) y retorne aprobado/rechazado. | Core Bancario |
| RF-08 | Obtener monto máximo de crédito | Exponer un WS SOAP que calcule el monto máximo según la fórmula: `((Promedio Depósitos 3 meses – Promedio Retiros 3 meses) * 30%) * 6`. | Core Bancario |
| RF-09 | Registrar crédito y amortización | Exponer un WS SOAP que, aprobado el crédito, reciba monto y plazo (3–18 meses) y registre el crédito junto con la tabla de amortización de cuota fija al 16.5% anual. | Core Bancario |

#### TicketPremium — Orquestación y servicios propios

| ID | Nombre | Descripción | Sistema Origen |
|----|--------|-------------|----------------|
| RF-10 | Orquestar compra en efectivo | Invocar en secuencia para **cada asiento seleccionado**: reservar asiento (RF-03) → registrar factura con descuento del 12% → confirmar venta (RF-04). Un cliente puede seleccionar múltiples asientos de diferentes localidades y diferentes partidos en una misma compra; la factura consolida todos los ítems. Ante fallo en cualquier asiento, liberar todos los asientos ya reservados en esa compra (RF-05). | TicketPremium |
| RF-11 | Orquestar compra a crédito | Invocar en secuencia: reservar todos los asientos seleccionados (RF-03 por cada uno) → verificar sujeto de crédito (RF-07) → obtener monto máximo (RF-08) → **verificar que el total de la factura sea menor o igual al monto máximo aprobado** (si no, liberar asientos y rechazar) → registrar factura consolidada → confirmar venta de todos los asientos (RF-04) → registrar crédito (RF-09). Ante cualquier fallo, ejecutar compensaciones en orden inverso, liberando todos los asientos reservados (RF-05). | TicketPremium |
| RF-12 | Calcular factura con IVA | El servicio de facturación debe calcular el subtotal consolidado de todos los asientos seleccionados (que pueden pertenecer a distintas localidades y distintos partidos), aplicar el IVA vigente y, en pagos en efectivo, aplicar el 12% de descuento antes del IVA. | TicketPremium |
| RF-13 | Resumen de ventas por partido | Exponer un endpoint que, dado el código de un partido, retorne la cantidad de boletos vendidos y el total recaudado agrupado por tipo de localidad. | TicketPremium |
| RF-14 | Resumen de ventas por CLIENTE | Exponer un endpoint que de el reporte de ventas de un partido y por cliente. | TicketPremium |

#### TicketPremium — CRUD de tablas maestras (patrón MVC)

Los siguientes requerimientos cubren la gestión administrativa del catálogo de datos propio de TicketPremium. Cada entidad debe implementar las operaciones Crear, Leer, Actualizar y Eliminar, expuestas mediante controladores bajo el patrón MVC.

| ID | Nombre | Descripción | Sistema Origen |
|----|--------|-------------|----------------|
| RF-14 | CRUD Países | Gestión de países sede y países participantes. Campos mínimos: código, nombre, continente. | TicketPremium |
| RF-15 | CRUD Estadios | Gestión de estadios donde se disputarán los partidos. Campos mínimos: código, nombre, ciudad, capacidad total, país (FK). | TicketPremium |
| RF-16 | CRUD Localidades | Gestión de tipos de localidad por estadio (PALCO, TRIBUNA, GENERAL, GENERAL VISITA, etc.). Campos mínimos: código, descripción, estadio (FK), capacidad, precio base. | TicketPremium |
| RF-17 | CRUD Partidos | Gestión del catálogo de partidos en el sistema TicketPremium, sincronizable con los datos de la App FIFA. Campos mínimos: código, equipo local, equipo visitante, fecha/hora, estadio (FK). | TicketPremium |
| RF-18 | CRUD Clientes | Gestión de clientes de TicketPremium. Campos mínimos: cédula, nombre, apellido, fecha de nacimiento, género, teléfono, email. La cédula actúa como identificador para la consulta de crédito al Core Bancario. | TicketPremium |

---

### 3.2 Requerimientos No Funcionales

| ID | Categoría | Descripción | Criterio de Aceptación |
|----|-----------|-------------|------------------------|
| RNF-01 | Protocolo | Todos los Web Services deben implementarse con SOAP sobre HTTP usando **.NET con CoreWCF** como tecnología base, conforme a la restricción del enunciado. | WSDL publicado y validado con SoapUI. |
| RNF-02 | Consistencia transaccional | El flujo de compra debe garantizar consistencia eventual mediante el patrón Saga Orquestada: si algún paso falla, se ejecutan las acciones compensatorias en orden inverso. | Prueba de fallo inyectado en cada paso verifica rollback correcto. |
| RNF-03 | Separación de sistemas | Los tres sistemas deben desplegarse como aplicaciones independientes. Ningún sistema accede directamente a la BD de otro; toda comunicación ocurre vía SOAP. | URLs de despliegue distintas y ausencia de acceso directo a BD ajena. |
| RNF-04 | Concurrencia y condiciones de carrera | El WS de reserva de asiento (RF-03) debe garantizar que dos clientes no puedan reservar el mismo asiento simultáneamente. Se implementa mediante **locking optimista**: el `UPDATE` aplica solo si `estado = 'LIBRE'`; si 0 filas fueron afectadas, se retorna fault inmediato al cliente que llegó segundo. Complementado con **reserva temporal (TTL)**: un asiento en `RESERVADO` que supere 10 minutos sin confirmación vuelve automáticamente a `LIBRE` (RF-06). | Prueba de dos requests simultáneos al mismo asiento: exactamente uno recibe confirmación y el otro recibe fault `ASIENTO_NO_DISPONIBLE`. |
| RNF-05 | Idempotencia de compensaciones | El WS de liberación de asiento (RF-05) y la anulación de factura deben ser idempotentes: invocarlos más de una vez no produce efectos duplicados. | Doble invocación del WS de liberación no altera el estado si el asiento ya estaba en `LIBRE`. |
| RNF-06 | Manejo de errores | Cada WS debe retornar SOAP faults descriptivos ante errores de negocio (asiento no disponible, crédito denegado, cliente no encontrado) y errores técnicos (timeout, BD no disponible). | Revisión de SOAP faults con código y mensaje legible en cada escenario de error. |
| RNF-07 | Trazabilidad | Cada invocación a un WS debe registrar en log: timestamp, operación, parámetros de entrada y resultado (éxito/fallo). Mínimo logging en consola o archivo. | Reproducir un flujo completo y verificar que cada paso quede registrado. |
| RNF-08 | Validación de entrada | Los WS deben validar que los parámetros obligatorios estén presentes y sean del tipo correcto antes de ejecutar lógica de negocio, retornando un fault descriptivo si no. | Enviar petición con campo vacío y verificar fault con mensaje claro. |

---

## 4. Consideraciones Técnicas

**Tecnología base**  
.NET con **CoreWCF** para todos los Web Services SOAP. CoreWCF es la implementación open-source de WCF sobre .NET 6+, compatible con los bindings `BasicHttpBinding` y `WSHttpBinding`. Se requiere .NET 6 o superior.

**Patrón de integración**  
Mashup Web implementado en TicketPremium como orquestador central. Los frontends se comunican exclusivamente con TicketPremium; nunca directamente con la App FIFA ni con el Core Bancario.

**Patrón de transacción**  
Saga Orquestada: el orquestador (TicketPremium) gestiona la secuencia de llamadas y ejecuta acciones compensatorias ante fallos. No se usa two-phase commit ni ESB.

**Mensajería asíncrona**  
No se incluye en el alcance del backend de Web Services. Los efectos secundarios post-compra (email, PDF de boleto) pueden implementarse con colas en una fase posterior, pero no forman parte de este documento.

**Modelo de datos — App FIFA**  
Tablas `PARTIDO_FUTBOL`, `LOCALIDAD_PARTIDO` y `ASIENTO`. La tabla `ASIENTO` tiene una relación con `LOCALIDAD_PARTIDO` y registra: `codigo_asiento`, `fila`, `numero`, `estado` (`LIBRE` / `RESERVADO` / `VENDIDO`) y `timestamp_reserva` (para control de TTL). La disponibilidad de una localidad se deriva del conteo de asientos en estado `LIBRE`.

**Modelo de datos — Core Bancario**  
Se extiende el schema existente del Core con las tablas de Crédito y Tabla de Amortización. Solo el Core Bancario escribe en estas tablas; TicketPremium accede únicamente vía WS.

**Modelo de datos — TicketPremium**  
El modelo propio de TicketPremium cubre las siguientes entidades:

- `PAIS`: código, nombre, continente.
- `ESTADIO`: código, nombre, ciudad, capacidad total, FK a `PAIS`.
- `LOCALIDAD`: código, descripción, capacidad, precio base, FK a `ESTADIO`.
- `PARTIDO`: código, equipo local, equipo visitante, fecha/hora, FK a `ESTADIO`.
- `CLIENTE`: cédula (PK), nombre, apellido, fecha de nacimiento, género, teléfono, email.
- `ASIENTO`: código, fila, número, estado (`LIBRE`/`RESERVADO`/`VENDIDO`), timestamp_reserva, FK a `LOCALIDAD` y `PARTIDO`.
- `FACTURA`: número, fecha, subtotal, descuento, IVA, total, método de pago (`EFECTIVO`/`CREDITO_DIRECTO`), FK a `CLIENTE`.
- `DETALLE_FACTURA`: FK a `FACTURA`, FK a `ASIENTO`, precio unitario al momento de la compra.

Cada `DETALLE_FACTURA` referencia un asiento específico, lo que permite al sistema saber en todo momento qué cliente está sentado en qué asiento de qué partido y en qué factura fue adquirido.

**Fórmula de amortización**

```
Cuota = Monto / ((1 - (1 + TasaPeriodo)^(-NCuotas)) / TasaPeriodo)

donde TasaPeriodo = 16.5% / 12
Plazo: mínimo 3 meses, máximo 18 meses
```

**Template visual del estadio (Mashup)**  
El enunciado requiere un template visual que muestre el estado de cada localidad (`LIBRE`, `PARCIAL`, `AGOTADA`) y, al seleccionar una localidad, el estado individual de cada asiento (`LIBRE`, `RESERVADO`, `VENDIDO`). Para los asientos vendidos debe mostrar: número de factura, nombre del cliente y cédula de quien está sentado ahí. Este componente es frontend pero consume los WS RF-02 y RF-13.

**Pruebas**  
Cada WS debe ser verificable con SoapUI o Postman (SOAP). Se recomienda al menos un caso de prueba por flujo feliz y uno por fallo con compensación para RF-08 y RF-09. Para CoreWCF se puede usar el cliente WCF generado vía `dotnet-svcutil` como parte de las pruebas de integración.

---

## Anexo A — Flujo de Compra (Saga Orquestada)

El siguiente cuadro describe la secuencia de invocaciones durante el proceso de compra, incluyendo las acciones compensatorias ante cada posible fallo. Los pasos marcados con *(por asiento)* se repiten para cada asiento seleccionado en la compra.

| Paso | Actor | Acción | WS invocado | Fallo → Compensación |
|------|-------|--------|-------------|----------------------|
| 1 *(por asiento)* | TicketPremium | Reservar asiento con locking optimista | RF-03 (FIFA WS) | Asiento ocupado → fault inmediato; liberar asientos ya reservados en esta compra (RF-05) |
| 2a *(Efectivo)* | TicketPremium | Registrar factura consolidada con descuento del 12% | BD local TicketPremium | Fallo BD → liberar todos los asientos reservados (RF-05) |
| 2b *(Crédito)* | TicketPremium | Verificar sujeto de crédito, obtener monto máximo y comparar con total de factura | RF-07, RF-08 (Banco WS) | Rechazado o total > monto máximo → liberar todos los asientos reservados (RF-05) |
| 3b *(Crédito)* | TicketPremium | Registrar factura consolidada | BD local TicketPremium | Fallo → liberar todos los asientos reservados (RF-05) |
| 3 *(por asiento)* | TicketPremium | Confirmar venta de asiento (RESERVADO → VENDIDO) | RF-04 (FIFA WS) | Fallo → anular factura + liberar asientos pendientes (RF-05) |
| 4b *(Crédito)* | TicketPremium | Registrar crédito y tabla de amortización | RF-09 (Banco WS) | Fallo → revertir todos los asientos a LIBRE (RF-05) + anular factura |

> **Principio clave:** las compensaciones se ejecutan en orden inverso al flujo original. TicketPremium es el único responsable de decidir cuándo y qué compensar; los sistemas FIFA y Banco solo exponen los WS, sin conocimiento del flujo global.
>
> **Protección adicional contra abandono:** los asientos en estado `RESERVADO` expiran automáticamente a los 10 minutos (RF-06), garantizando que ningún asiento quede bloqueado indefinidamente si el cliente abandona el flujo a mitad de la compra.
