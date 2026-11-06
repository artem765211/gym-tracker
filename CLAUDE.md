# Gymtracker: инструкции для агента

Курсовая: веб-приложение «трекер силовых тренировок» (мобильная версия в браузере).
Полный план: `gym-tracker-plan.md` (в корне репозитория). Читай его только когда нужны детали, целиком не пересказывай.

## Как со мной работать
- Одна небольшая задача за раз. Перед правками скажи, что и зачем будешь менять.
- После задачи запусти `dotnet build` и `dotnet test`, покажи итог.
- `git commit` и `git push` делаю я сам, только предложи текст коммита.
- Формат коммитов (Conventional Commits): `тип: описание` на английском. Типы: `feat` (новая функция), `fix` (исправление ошибки), `refactor` (переделка без новых функций), `docs` (документация), `test` (тесты), `chore` (служебное).
- Не пиши за один раз весь проект и не делай лишних улучшений, о которых я не просил.
- Я только учусь: объясняй простым языком, новые термины расшифровывай.

## Структура решения (один .sln, четыре проекта)
- `Gymtracker.Api`: контроллеры, DTO, Program.cs
- `Gymtracker.Core`: сущности, перечисления, калькуляторы (1RM, прирост, КБЖУ). Без ссылок на EF Core и ASP.NET.
- `Gymtracker.Infrastructure`: `AppDbContext`, миграции, Identity
- `Gymtracker.Tests`: xUnit
- Зависимости: Api -> Core, Infrastructure; Infrastructure -> Core; Tests -> Core.

## Стек
ASP.NET Core Web API (C#, net10.0), PostgreSQL 16 в Docker, EF Core (Code First) + Npgsql,
Identity + JWT, xUnit, Swagger. Фронтенд позже: React + Vite + TypeScript + Tailwind + shadcn/ui.

## Правила кода
- В API отдаём DTO, а не сущности EF.
- `UserId` берём из токена, не из тела запроса. Каждый запрос к данным фильтруем по текущему пользователю.
- Всё, кроме регистрации и входа, закрыто `[Authorize]`.
- Ошибки: Problem Details, коды 201/204/400/401/403/404.
- Вес храним как `decimal`, даты тренировок как `DateOnly`.
- Упражнения с собственным весом (подтягивания и т.п.) пока в справочнике не используем.

## Секреты
- Не читай, не изменяй и не выводи `.env`. Не печатай содержимое user-secrets.
- В user-secrets лежат строка подключения `ConnectionStrings:DefaultConnection` и ключ подписи `Jwt:Key`.

## Команды (Windows, корень репозитория)
Мой терминал: PowerShell. Команды для меня пиши под него.
- `docker compose up -d`: поднять PostgreSQL (сначала должен быть запущен Docker Desktop)
- `dotnet build` / `dotnet test`
- `dotnet run --project Gymtracker.Api --launch-profile http`: запустить API, Swagger на `http://localhost:5036/swagger`
- `dotnet ef migrations add Name --project Gymtracker.Infrastructure --startup-project Gymtracker.Api`
- `dotnet ef database update --project Gymtracker.Infrastructure --startup-project Gymtracker.Api`
- Если сборка падает с MSB3027 (файл занят) или порт 5036 занят, API уже запущен в Rider или терминале: попроси меня его остановить.

## Состояние
Готово:
- Недели 1-2: структура, Docker с PostgreSQL, сущности, `AppDbContext`, калькуляторы с тестами, seed 25 упражнений.
- Неделя 3: Identity, внешние ключи `UserId` на `AspNetUsers`, JWT (срок 12 часов), `POST /api/auth/register` и `/login`,
  Swagger UI с Authorize, Problem Details, глобальный обработчик исключений, CORS для `http://localhost:5173`.
- Неделя 4, начало: `GET /api/exercises`, `GET /api/exercises/{id}`, `POST /api/exercises` (общие и личные упражнения),
  `DELETE /api/exercises/{id}` (только свои личные; 403 для общих, 404 для чужих, 400 если упражнение есть в тренировках).

Принятые решения:
- Одна тренировка на день у пользователя (запрет на уровне API и БД ещё не сделан, войдёт в шаг с тренировками).
- Личные упражнения можно удалять. Изменение пока не делаем.

Дальше: неделя 4, тренировки (день + упражнения из справочника), фильтр по датам, пагинация.
Когда появятся тренировки, проверить вручную ответ 400 при удалении упражнения, которое в них используется.

Известные ограничения: нет блокировки при переборе паролей; чужие данные дают 404, а 403 используется только при удалении общего упражнения;
автотесты есть только на калькуляторы, API проверяется вручную (интеграционные тесты по плану на неделе 5).
Отметки в `gym-tracker-plan.md` не обновлялись, ориентируйся на этот раздел.
