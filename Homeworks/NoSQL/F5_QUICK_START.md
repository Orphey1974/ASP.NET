# Краткая инструкция: Запуск проектов через F5

## Быстрый запуск:

1. **Нажмите F5** в VS Code

2. **Выберите конфигурацию:**

   - `Run Administration (Clean, Build, Run)` - для полной пересборки

   - `Run Administration (Build Only)` - для быстрого запуска

   - `Run Preferences (Clean, Build, Run)` - для полной пересборки

   - `Run Preferences (Build Only)` - для быстрого запуска

## Что происходит:
✅ **Clean** - удаляет папки `bin` и `obj`
✅ **Build** - собирает проект
✅ **Run** - запускает сервис
✅ **Auto-open Swagger** - открывает Swagger UI в браузере

## Полезные комбинации клавиш:

- **F5** - Запуск выбранной конфигурации

- **Ctrl+Shift+P** → `Tasks: Run Task` - Запуск задач (clean, build)

- **Ctrl+Shift+B** - Быстрая сборка (Build Solution)

## Требования:

- Установлено расширение **C# Dev Kit** или **C#** для VS Code

- Установлен **.NET 8.0 SDK**

- Запущен **MongoDB** (для Administration) и **SQLite** (для Preferences)

## Порты:

- **Administration API**: http://localhost:8091

- **Preferences API**: http://localhost:8094

- **Swagger UI**: автоматически открывается при запуске
