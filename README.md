# 🎮 2110Pets-Game2D

> **Proyecto Unity 2D** desarrollado con **Unity 6000.2.7 (URP 2D)** | Juego de mascotas multijugador

[![Unity Version](https://img.shields.io/badge/Unity-6000.2.7-black.svg?style=flat&logo=unity)](https://unity.com/)
[![Pipeline](https://img.shields.io/badge/Pipeline-URP%202D-blue.svg)](https://unity.com/srp/universal-render-pipeline)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Tabla de Contenidos

- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Flujo de Trabajo con Git](#-flujo-de-trabajo-con-git)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Buenas Prácticas](#-buenas-prácticas)
- [Solución de Problemas](#-solución-de-problemas)
- [Equipo de Desarrollo](#-equipo-de-desarrollo)

---

## 🛠️ Requisitos Previos

Antes de clonar el proyecto, asegúrate de tener instalado:

- **Unity Hub**: [Descargar aquí](https://unity.com/download)
- **Unity Editor 6000.2.7**: Instalado desde Unity Hub
- **Git**: [Descargar aquí](https://git-scm.com/downloads)
- **Git LFS** (opcional, para archivos grandes): [Descargar aquí](https://git-lfs.github.com/)
- Cuenta de **GitHub** con acceso al repositorio

### ⚠️ Versión de Unity Requerida

Este proyecto utiliza **Unity 6000.2.7** con **Universal Render Pipeline (URP) 2D**. Es crítico usar exactamente esta versión para evitar incompatibilidades en shaders, materiales y configuraciones.

---

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

Abre tu terminal o PowerShell en la carpeta donde deseas guardar el proyecto y ejecuta:
```bash
git clone https://github.com/RicardojCh10/2110Pets-Game2D.git
```

Esto descargará el proyecto completo (excepto las carpetas ignoradas como `Library/`, `Temp/`, etc.).

### 2. Abrir el Proyecto en Unity

1. Abre **Unity Hub**
2. Haz clic en **"Add"** → **"Add project from disk"**
3. Navega y selecciona la carpeta `2110Pets-Game2D`
4. Verifica que Unity Hub muestre la versión **6000.2.7**
5. Haz clic en el proyecto para abrirlo

> **Nota**: La primera vez que se abre el proyecto, Unity regenerará la carpeta `Library/`. Este proceso puede tardar varios minutos.

### 3. Configurar Git LFS (Opcional pero Recomendado)

Si el proyecto usa Git LFS para archivos grandes (sprites, audio, texturas), ejecuta:
```bash
git lfs install
git lfs pull
```

---

## 🌳 Flujo de Trabajo con Git

### Crear una Nueva Rama

**Nunca trabajes directamente en `main` o `develop`**. Siempre crea una rama para tu tarea:
```bash
git checkout -b feature/nombre-descriptivo
```

**Ejemplos de nombres de ramas:**
- `feature/player-movement`
- `feature/main-menu`
- `feature/level-1`
- `fix/camera-bug`
- `hotfix/save-system`

### Guardar y Subir Cambios

1. **Revisa los archivos modificados:**
```bash
   git status
```

2. **Añade los cambios al staging:**
```bash
   git add .
```

3. **Crea un commit con un mensaje descriptivo:**
```bash
   git commit -m "Añadido sistema de movimiento del jugador"
```

4. **Sube los cambios a tu rama remota:**
```bash
   git push origin feature/nombre-descriptivo
```

5. **Crea un Pull Request** en GitHub hacia la rama `develop` (o `main` según la configuración del equipo)

### Actualizar tu Rama Local

Antes de comenzar a trabajar cada día, actualiza tu código:
```bash
git checkout main
git pull origin main
git checkout feature/tu-rama
git merge main
```

---

## 📁 Estructura del Proyecto
```
2110Pets-Game2D/
├── Assets/
│   ├── Art/              # Sprites, animaciones, texturas
│   ├── Audio/            # Música y efectos de sonido
│   ├── Prefabs/          # GameObjects prefabricados
│   ├── Scenes/           # Escenas del juego
│   ├── Scripts/          # Código C#
│   ├── Settings/         # Configuraciones URP y Input System
│   └── UI/               # Elementos de interfaz
├── Packages/             # Dependencias del proyecto
├── ProjectSettings/      # Configuración de Unity
├── .gitignore           # Archivos ignorados por Git
├── .gitattributes       # Configuración de Git LFS
└── README.md            # Este archivo
```

---

## ✅ Buenas Prácticas

### 🚫 No Subir Estas Carpetas

El archivo `.gitignore` ya está configurado para ignorar:
```
Library/
Temp/
Obj/
Builds/
Logs/
UserSettings/
*.csproj
*.sln
```

**Nunca** agregues manualmente estas carpetas al repositorio.

### 🎨 Trabajo con Escenas

**Problema**: Unity guarda las escenas en formato de texto. Si dos personas editan la misma escena simultáneamente, habrá conflictos de merge difíciles de resolver.

**Soluciones:**

1. **Comunicación**: Usa Discord/Slack para avisar qué escena estás editando
2. **Escenas individuales**: Trabaja en escenas separadas por persona:
   - `Level1_Ricardo.unity`
   - `Level1_Maria.unity`
   - Luego combínalas en una escena final
3. **Prefabs**: Usa prefabs para compartir objetos entre escenas sin conflictos

### 📝 Mensajes de Commit

Usa mensajes claros y descriptivos:

✅ **Buenos ejemplos:**
```bash
git commit -m "Añadido sistema de salto del jugador"
git commit -m "Corregido bug en el menú de pausa"
git commit -m "Implementada animación de ataque"
```

❌ **Malos ejemplos:**
```bash
git commit -m "fix"
git commit -m "cambios"
git commit -m "asdfasdf"
```

### 🔄 Pull Requests

Antes de crear un PR:

1. ✅ Tu código compila sin errores
2. ✅ Has probado tu funcionalidad
3. ✅ Tu rama está actualizada con `main`/`develop`
4. ✅ Describiste claramente los cambios en el PR

---

## 🐛 Solución de Problemas

### Problema: "Unity muestra errores al abrir el proyecto"

**Solución:**
1. Verifica que estés usando Unity **6000.2.7**
2. Cierra Unity y elimina la carpeta `Library/`
3. Reabre el proyecto (Unity regenerará `Library/`)

### Problema: "Git LFS dice que faltan archivos"

**Solución:**
```bash
git lfs install
git lfs pull
```

### Problema: "Conflicto de merge en una escena"

**Solución:**
1. Si es tu escena: `git checkout --theirs path/to/scene.unity`
2. Si es la escena de otro: `git checkout --ours path/to/scene.unity`
3. Mejor prevención: no editar la misma escena simultáneamente

### Problema: "Mi rama está desactualizada"

**Solución:**
```bash
git checkout main
git pull origin main
git checkout feature/tu-rama
git merge main
# Resuelve conflictos si los hay
git add .
git commit -m "Merge main into feature branch"
git push origin feature/tu-rama
```

---

## 👥 Equipo de Desarrollo

- **Ricardo Chi** - [@RicardojCh10](https://github.com/RicardojCh10) - Project Lead
- **[Nombre]** - [Role]
- **[Nombre]** - [Role]
- **[Nombre]** - [Role]

---

## 📄 Licencia

Este proyecto es privado y está bajo desarrollo académico. Todos los derechos reservados al equipo de desarrollo.

---

## 📞 Contacto

Si tienes dudas sobre el proyecto o el flujo de trabajo con Git:

- **Discord del equipo**: [Enlace]
- **Issues de GitHub**: [Crear nuevo issue](https://github.com/RicardojCh10/2110Pets-Game2D/issues)

---

<div align="center">

**Desarrollado con 💜 por el equipo 2110Pets**

⭐ **¡No olvides hacer pull antes de empezar a trabajar!** ⭐

</div>
