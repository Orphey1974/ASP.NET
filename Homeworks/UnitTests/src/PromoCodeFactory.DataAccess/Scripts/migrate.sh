#!/bin/bash

# Скрипт для выполнения миграций Entity Framework в CI окружении
set -e

echo "Начинаем выполнение миграций базы данных..."

# Ждем, пока база данных будет готова
echo "Ожидание готовности базы данных..."
until pg_isready -h promocode-factory-db-ci -p 5432 -U postgres -d promocode_factory_ci; do
    echo "База данных еще не готова, ждем..."
    sleep 2
done

echo "База данных готова!"

# Выполняем миграции Entity Framework
echo "Выполняем миграции Entity Framework..."

# Создаем базу данных если она не существует
dotnet ef database update --project /app/PromoCodeFactory.DataAccess --startup-project /app/PromoCodeFactory.WebHost --connection "Host=promocode-factory-db-ci;Database=promocode_factory_ci;Username=postgres;Password=ci_password;Port=5432"

echo "Миграции выполнены успешно!"

# Проверяем состояние базы данных
echo "Проверяем состояние базы данных..."
dotnet ef database info --project /app/PromoCodeFactory.DataAccess --startup-project /app/PromoCodeFactory.WebHost --connection "Host=promocode-factory-db-ci;Database=promocode_factory_ci;Username=postgres;Password=ci_password;Port=5432"

echo "Миграции завершены успешно!"