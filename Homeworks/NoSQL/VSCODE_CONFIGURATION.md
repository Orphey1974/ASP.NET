# Конфигурация VS Code для работы с проектом

## Настройка F5 для запуска проектов и задач
В проекте настроена конфигурация VS Code для удобного запуска проектов с автоматической сборкой и очисткой.

### Доступные конфигурации запуска (F5):

#### Запуск проектов:

1. **Run Administration (Clean, Build, Run)** - Полный цикл для Administration

   - Очищает проект (clean)

   - Собирает проект (build)

   - Запускает сервис на порту 8091

   - Автоматически открывает Swagger UI

2. **Run Administration (Build Only)** - Быстрый запуск для Administration

   - Только сборка проекта (build)

   - Запускает сервис на порту 8091

   - Автоматически открывает Swagger UI

3. **Run Preferences (Clean, Build, Run)** - Полный цикл для Preferences

   - Очищает проект (clean)

   - Собирает проект (build)

   - Запускает сервис на порту 8094

   - Автоматически открывает Swagger UI

4. **Run Preferences (Build Only)** - Быстрый запуск для Preferences

   - Только сборка проекта (build)

   - Запускает сервис на порту 8094

   - Автоматически открывает Swagger UI

#### Исправление Markdown:

5. **Fix Markdown Errors (Dry Run)** - Режим тестирования

   - Показывает что будет изменено без реального изменения файлов

   - Рекурсивный поиск по всем папкам

   - Подробный вывод изменений

6. **Fix Markdown Errors (Apply Changes)** - Применение изменений

   - Реально исправляет ошибки в файлах

   - Рекурсивный поиск по всем папкам

   - Подробный вывод изменений

7. **Fix Markdown Errors (Current Folder Only)** - Только текущая папка

   - Режим тестирования только для файлов в корневой папке

   - Не затрагивает подпапки

### Как использовать:

1. **Нажмите F5** - откроется меню выбора конфигурации

2. **Выберите нужную конфигурацию** из списка

3. **Нажмите Enter** для запуска

### Доступные задачи (Ctrl+Shift+P → Tasks: Run Task):

#### Сборка проектов:

- **Clean Solution** - Очистка всего решения

- **Build Solution** - Сборка всего решения (по умолчанию)

- **Clean and Build Solution** - Очистка и сборка всего решения

- **Clean Administration** - Очистка проекта Administration

- **Build Administration** - Сборка проекта Administration

- **Clean and Build Administration** - Очистка и сборка Administration

- **Clean Preferences** - Очистка проекта Preferences

- **Build Preferences** - Сборка проекта Preferences

- **Clean and Build Preferences** - Очистка и сборка Preferences

#### Исправление Markdown:

- **Fix Markdown Errors (Dry Run)** - Тестирование исправлений

- **Fix Markdown Errors (Apply)** - Применение исправлений

- **Fix Markdown Errors (Current Folder)** - Только текущая папка

### Альтернативные способы запуска:

#### Через Command Palette (Ctrl+Shift+P):

- `Tasks: Run Task` → выберите нужную задачу

- `Debug: Select and Start Debugging` → выберите конфигурацию F5

#### Через терминал:

```powershell

# Сборка проектов
dotnet clean src/Pcf.Administration/Pcf.Administration.sln
dotnet build src/Pcf.Administration/Pcf.Administration.sln
dotnet run --project src/Pcf.Administration/Pcf.Administration.WebHost

# Исправление Markdown
powershell -ExecutionPolicy Bypass -File fix-markdown-errors.ps1 -Recursive -DryRun -Verbose
powershell -ExecutionPolicy Bypass -File fix-markdown-errors.ps1 -Recursive -Verbose

```

### Рекомендации:

1. **Всегда начинайте с Dry Run** - проверьте что будет изменено

2. **Используйте Verbose режим** - для подробного просмотра изменений

3. **Делайте коммит перед применением** - для возможности отката изменений

### Настройки проекта:

- PowerShell установлен как терминал по умолчанию

- Markdown preview настроен с поддержкой разрывов строк

- Автоматическое обновление оглавления при сохранении

- Ассоциация .ps1 файлов с PowerShell

### Безопасность:
⚠️ **Конфигурация "Apply Changes" реально изменяет файлы!**
⚠️ **Всегда используйте Dry Run для предварительного просмотра!**
⚠️ **Рекомендуется сделать коммит перед применением изменений!**
