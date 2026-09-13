# QUE ES SilentEye?
> SilentEye es un simple .exe creado en c# que al momento de ser ejecutado extrae informacion de la pc victima (informacion de la maquina) y todas las contraseñas Wifi guardadas en la misma.

# Como puedo crear el autorun?
> Descarga [samlogic autorun creator](https://www.samlogic.net/usb-autorun-creator-spanish/usb-autorun-creator-spanish.htm)

Cuando abras el programa debes seleccinar el archivo .exe (cuando lo compiles estará en ./bin/realse/publish) luego vas a seleccionar el pendrive (memoria USB) que vas a usar para crear el autorun, puedes ponerle un nombre nuevo a la USB y tambien un icono si quieres, cuando tengas el USB listo puedes eliminar todas las carpetas y archivos que no sean:

_SilentEye.exe_

_AUTORUN_

_OpenFile_


`Nota: si no sabes como compilar el archivo usa:`
```
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
(De todas formas el archivo ya está compilado en ./bin/realse/publish)

# Whitelist
> El proyecto tiene un sistema de whitelist para evitar que el archivo se ejecute cuando lo conectas a tu pc (puedes agregar tu pc en Program.cs linea 13, solo pega el nombre de tu equipo y listo) esto fue implementado con el fin de evitar molestias al conectar la USB.

# Use case
> Imagina que estas en un espacio en el cual no tienes internet pero hay computadoras que puedes usar o puedes acceder a ellas, entonces solo conectas tu USB y listo, ya tienes todas las contraseñas wifi de la maquina victima.

# Esto es detectable? 
> No, al momento de pasarle el archivo a un detector de virus (Como virusTotal) obtienes una cantidad muy baja (casi nula) de detecciones y ni siquiera windows defender lo detecta

## Escaneos
>jotti: 1/13

> internxt: 0

> internxt: 0
 

>VirusTotal: (no suban sus archivos a esta pagina porque los antivirus se actualizan con la informacion que les proporciona virusTotal)

`Cuando se incerta la USB no sale nada en la pantalla y puedes retirar la USB despues de 3-5 segundos`

# Por qué este proyecto fue creado?
> No se hermano, estaba aburrido.

# Hay algún truco en estos archivo?
> No, todo el proyecto fue creado con una finalidad y esta no es afectar a las personas que lo descarguen, de todas formas el proyecto es Opensource y puedes revisarlo tu mismo.

# Como se ve la informacion que extrae SilentEye?
```
### RESUMEN DE SISTEMA ###
Equipo:         Nombre del quipo
Dominio/Grupo:  
Usuario Sesión: User
OS Destino:     Microsoft Windows 10.0.26200 (X64)
Runtime .NET:   10.0.10
Cores CPU:      X
Uptime Total:   X horas

### CONFIGURACIÓN DE RED ###
IP Pública (API Externa): {IP PUBLICA}
Direcciones LAN detectadas:
 -> [Ethernet 6] IP: {IP} | Máscara: 255.255.255.0
 -> [Wi-Fi 2] IP: 10.0.0.5 | Máscara: 255.255.255.0
 
### PUERTOS TCP EN ESCUCHA ###
Total de sockets escuchando: X cantidad
 -> 0.0.0.0:80


### CUENTAS DE USUARIO LOCALES ###
 -> Administrador            DefaultAccount           Invitado
 -> User                     WDAGUtilityAccount

### AUDITORÍA WI-FI (PERFILES GUARDADOS) ###
 RED: Nombre -> CLAVE: Contraseña

```
# Updates
> Planeo segir actualizando el proyecto para mejorar cosas y meter más funciones.
