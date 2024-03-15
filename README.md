# EGGSCAPE_TEST

**Descripcion:**

Juego multijugador simple de combate mele implementando FishNet

Cada instancia de juego puede decidir si funciona como Servidor, Cliente o Servidor y Cliente al mismo tiempo. 
Se utiliza la interfaz de FishNet

Cada instancia de jugador controlar un personaje en primera persona que puede moverse con A-W-S-D, controlar su direccion con el mouse y efectuar un ataque con la tecla Espacio
Cada instancia de jugador posee una UI simple en primera persona para mostrar su vida actual y una en 3ra persona para mostrar lo mismo al resto de los clientes

**Detalle:**

Scripts Managers
 - GameManager
 - UIManager
 - NetworkManager

 Scripts del Jugador
 - PlayerController
 - PlayerNetworkSync

Scripts de Arma
- Sword
- OnSwordCollision

Script Interfaces
- IDamageable

 Scripts Misc
 - ConstantRotation

**Arquitectura:**

La estructura basica del juego es controlada por el GameManager
(Realmente no hay mucho que controlar en este caso)

El UIManager se encarga de actualizar los cambios en la UI general

El PlayerController se encarga de los movimientos y acciones del jugador asi como de avisar al networkManager de sus cambios de estado y acciones

El NetworkManager se encarga de sincronizar el estado de juego

**Detalle de Networking:**
El juego optimiza con prediccion del lado del cliente la deteccion de los ataques efectuados por otros jugadores (La prediccion se basa en ajustar la animacion de ataque en los clientes comparando el tick del cliente que ejecuta el ataque con el delay de llegada al resto de los clientes)

**Tecnologias utilizadas:**
- UnityEngine ver: 2022.3.20f1
- Fish-Net ver: 3.11.18R

**Problemas enfrentados:**
Principalmente la integracion de Fish-Net ya que nunca antes lo habia utilizado.

Me tomo un tiempo entender como sincronizar variables y aprender sobre client-prediction ya que si bien trabaje sistemas de multiplayer antes, el optimizado de latencia no es algo que enfrentara muy a menudo.

Un tema que me queda pendiente es implementar la autoridad de servidor (En el documento de requerimientos decia la "autoridad del cliente").

**Conclucion:**
Entiendo la mecanica y hay un par de ejemplos en el package de Fish-Net pero me va a tomar unos cuantos dias mas comprender del todo e implementarlo, voy a estar trabajando en eso y voy a reenviar esta prueba.

Comprendo si no es posible que esperen hasta entonces, despues de todo estoy enviando esto completo hasta donde creo que se solicitaba en el doc de requerimientos pero tambien entiendo que la autoridad del servidor permitiria disminuir la latencia y prevenir cheats.

El proyecto implementa el script de tugboat para simular la latencia, hice lo mejor que pude aprendiendo en unos dias sobre Fish-Net y sobre los conceptos de Prediccion de Cliente y Autoridad de Cliente. 

Estoy seguro de que con un poco mas de tiempo puedo lograr una mejor optimizacion y mecanicas de prediccion mucho mejores, seguramente ajustando la forma en la que se ejecutan y predicen los taques y comprendiendo mejor el sincronismo servidor-cliente con el PredictedObject e implementando mi propia version del TransformPredict.