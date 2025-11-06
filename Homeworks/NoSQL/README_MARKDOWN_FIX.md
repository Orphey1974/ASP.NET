# Markdown Error Fix Script

PowerShell скрипт для автоматического исправления типичных ошибок форматирования в Markdown файлах.

## Быстрый старт

```powershell
# Проверить что будет исправлено (режим тестирования)
powershell -ExecutionPolicy Bypass -File fix-markdown-errors-simple.ps1 -DryRun

# Исправить все markdown файлы в текущей папке
powershell -ExecutionPolicy Bypass -File fix-markdown-errors-simple.ps1

# Исправить все markdown файлы рекурсивно
powershell -ExecutionPolicy Bypass -File fix-markdown-errors-simple.ps1 -Recursive
```

## Что исправляет

- ✅ Удаляет пробелы в конце строк
- ✅ Исправляет множественные пустые строки
- ✅ Добавляет пустые строки перед заголовками, списками, блоками кода
- ✅ Добавляет финальный перевод строки
- ✅ Удаляет лишние пробелы в начале/конце файла

## Параметры

- `-Path` - Путь к папке (по умолчанию: текущая папка)
- `-Recursive` - Рекурсивный поиск
- `-DryRun` - Режим тестирования
- `-Verbose` - Подробный вывод

## Безопасность

⚠️ **Всегда используйте `-DryRun` для предварительного просмотра!**

## Документация

Подробная документация: [MARKDOWN_FIX_SCRIPT_DOCUMENTATION.md](MARKDOWN_FIX_SCRIPT_DOCUMENTATION.md)
