# Быстрый старт: Запуск проектов через F5

## Как использовать:

1. **Откройте проект в VS Code**
2. **Нажмите F5** - откроется меню выбора конфигурации
3. **Выберите нужную конфигурацию:**

### Запуск проектов:

- **Run Administration (Clean, Build, Run)** - Полный цикл: очистка → сборка → запуск
- **Run Administration (Build Only)** - Быстрый запуск: только сборка → запуск
- **Run Preferences (Clean, Build, Run)** - Полный цикл: очистка → сборка → запуск
- **Run Preferences (Build Only)** - Быстрый запуск: только сборка → запуск

### Исправление Markdown:

- **Fix Markdown Errors (Dry Run)** - **РЕКОМЕНДУЕТСЯ** для начала
- **Fix Markdown Errors (Apply Changes)** - для реального исправления
- **Fix Markdown Errors (Current Folder Only)** - только корневая папка

## Что происходит при запуске проекта:

1. **Clean** (если выбран) - очищает папки bin и obj
2. **Build** - собирает проект
3. **Run** - запускает сервис
4. **Auto-open Swagger** - автоматически открывает Swagger UI в браузере

## Полезные задачи (Ctrl+Shift+P → Tasks: Run Task):

- **Clean Solution** - Очистка всего решения
- **Build Solution** - Сборка всего решения
- **Clean and Build Solution** - Очистка и сборка
- **Clean Administration** / **Build Administration** - Для конкретного проекта
- **Clean Preferences** / **Build Preferences** - Для конкретного проекта

## Безопасность:

⚠️ **Для Markdown: всегда начинайте с Dry Run!**
⚠️ **Сделайте коммит перед применением изменений!**

## Альтернативный запуск:

- **Ctrl+Shift+P** → `Tasks: Run Task` → выберите задачу
- **Ctrl+Shift+P** → `Debug: Select and Start Debugging` → выберите конфигурацию
- **Терминал:** `dotnet run --project src/Pcf.Administration/Pcf.Administration.WebHost`
