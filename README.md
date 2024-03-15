# EGGSCAPE_TEST

Descripcion:

Juego multijugador simple de combate mele implementando FishNet

Cada instancia de juego puede decidir si funciona como Servidor, Cliente o Servidor y Cliente al mismo tiempo. Se utiliza la interfaz de FishNet

Cada instancia de jugador controlar un personaje en primera persona que puede moverse con A-W-S-D, controlar su direccion con el mouse y efectuar un ataque con la tecla Espacio
Cada instancia de jugador posee una UI simple en primera persona para mostrar su vida actual y una en 3ra persona para mostrar lo mismo al resto de los clientes

Detalle:

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

Arquitectura:

La estructura basica del juego es controlada por el GameManager
El UIManager se encarga de actualizar los cambios en la UI general

El PlayerController se encarga de los movimientos y acciones del jugador asi como de avisar al networkManager de sus cambios de estado y acciones

El NetworkManager se encarga de sincronizar el estado de juego

Detalle de Networking:
El juego optimiza con prediccion del lado del cliente la deteccion de los ataques efectuados por otros jugadores (La prediccion se basa en ajustar la animacion de ataque en los clientes comparando el tick del cliente que ejecuta el ataque con el delay de llegada al resto de los clientes)
