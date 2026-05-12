<div align="center">

<br/>

![Unity](https://img.shields.io/badge/Unity-6000.4+_LTS-black?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Active_Dev-brightgreen?style=for-the-badge)

<p align="center">
  <b>3D-прототип фермерской игры с современной архитектурой Unity</b><br/>
  <sub>Вдохновлён Fancy Farm · DI · Async · Addressables</sub>
</p>

</div>

---

## 📖 О проекте

Полностью трёхмерный прототип фермерской игры, вдохновлённый **Fancy Farm**. Проект создавался как полигон для отработки:

- современной **архитектуры Unity** на базе DI-контейнера
- **асинхронного программирования** без корутин и GC-аллокаций
- **динамической загрузки контента** через Addressables

> Это не просто игра — это демонстрация чистого, масштабируемого кода в Unity.

---

## 🎬 Геймплей

<!-- 
  ВАРИАНТ 1 (рекомендуется): Загрузи видео на YouTube, сделай скриншот и замени ссылки ниже.
  Кликабельное превью откроется в YouTube.
-->

[![Gameplay Preview](https://img.youtube.com/vi/EfrN4kinnQU/maxresdefault.jpg)](https://www.youtube.com/watch?v=EfrN4kinnQU)

<!--
  ВАРИАНТ 2: Загрузи .mp4 через GitHub Issues (перетащи файл в поле комментария),
  скопируй полученную ссылку и вставь вместо этого комментария:
  ![Gameplay](https://github.com/user-attachments/assets/ВАШ_ASSET_ID.mp4)

  ВАРИАНТ 3: Запиши GIF (ScreenToGif / LICEcap) и вставь как обычное изображение:
  ![Gameplay](Assets/Docs/gameplay.gif)
-->

---

## ✨ Особенности

| Фича | Описание |
|------|----------|
| 🌍 **3D-окружение** | Полностью трёхмерное пространство со свободной камерой |
| 🌱 **Фермерский цикл** | Посадка, рост и сбор урожая |
| 🎒 **Инвентарь** | Система предметов и взаимодействия с объектами |
| 💉 **DI-архитектура** | ZenJect-контейнер: менеджеры фермы, экономики, загрузки |
| ⚡ **Async-first** | UniTask вместо корутин — нулевые GC-аллокации |
| 📦 **Addressables** | Асинхронная загрузка уровней, префабов и ассетов |

---

## 🛠️ Технологии

<table>
<tr>
<td align="center" width="200">

**[ZenJect](https://github.com/modesttree/Zenject)**

Dependency Injection для модульности и тестируемости. Все сервисы биндятся через контейнер.

</td>
<td align="center" width="200">

**[UniTask](https://github.com/Cysharp/UniTask)**

Асинхронное программирование на `async/await`. Замена корутинам без аллокаций в heap.

</td>
<td align="center" width="200">

**[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@latest)**

Умная загрузка контента: ассеты грузятся только когда нужны, автоматическое управление памятью.

</td>
</tr>
</table>

---

## 🚀 Быстрый старт

### Требования

- **Unity 6000.4 LTS** или новее
- Git

### Установка

```bash
# 1. Клонировать репозиторий
git clone https://github.com/ваш-аккаунт/название-проекта.git

# 2. Открыть папку в Unity Hub
```

### Установка зависимостей

Откройте **Window → Package Manager** и добавьте если не подтянулись автоматически:

```
com.cysharp.unitask
com.unity.addressables
```

> **ZenJect**: если используется `.unitypackage` — установите через **Assets → Import Package**.

### Запуск

1. Откройте сцену `Assets/Scenes/TestScene.unity`
2. Нажмите ▶️ **Play**

---

<div align="center">
  <sub>Сделано с ❤️ на Unity · 2026</sub>
</div>
