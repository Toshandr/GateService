# Gates API

ASP.NET Core Web API для проверки кодов доступа

## Требования

- .NET SDK 10.0 (для запуска без Docker)
- Docker (для запуска в контейнере)
- kubectl и локальный кластер (для запуска в Kubernetes, необязательно)

## Запуск без Docker

```bash
dotnet run --project src/Gates.Api --urls http://localhost:8080
```

## Запуск в Docker

```bash
docker compose up --build
```

Или без compose:

```bash
docker build -t gates-api .
docker run --rm -p 8080:8080 gates-api
```

После запуска:

- Swagger UI: http://localhost:8080/swagger
- Endpoint: `POST http://localhost:8080/api/gates/validate`

## Запуск в Kubernetes

Нужен кластер (minikube, kind, Docker Desktop) и `kubectl`. Манифесты лежат в `k8s/`.

```bash
docker build -t gates-api .

# загрузить образ в локальный кластер (один из вариантов)
minikube image load gates-api:latest
kind load docker-image gates-api:latest

kubectl apply -f k8s/
kubectl port-forward svc/gates-api 8080:80
```

Swagger будет доступен на http://localhost:8080/swagger.

Удаление: `kubectl delete -f k8s/`.

Для реального кластера образ нужно отправить в registry и указать его имя в `k8s/deployment.yaml`.

## Пример запроса

```bash
curl -X POST http://localhost:8080/api/gates/validate \
  -H "Content-Type: application/json" \
  -d '{"accessCode": "dragon123"}'
```

Ответ:

```json
{
  "isValid": false,
  "errors": [
    "Нет заглавной латинской буквы.",
    "Нет специального символа.",
    "Обнаружена последовательность из трёх или более подряд идущих цифр."
  ],
  "message": "Код отклонён."
}
```

```bash
dotnet test
```

- `AccessCodeValidatorTests` - модульные тесты каждого правила и граничных длин (8 и 16).
- `GatesApiTests` - тесты API через `WebApplicationFactory`: корректный код, некорректный код, пустой и отсутствующий код, пустое тело.


## Опущенное

- Правило "три одинаковых символа подряд" чувствительно к регистру: `aaa` нарушает его, `aAa` - нет.
- "Первый символ - буква": любая буква Unicode. Заглавная и строчная проверяются только по латинице.
- Пробелом считается любой пробельный символ (пробел, табуляция, перенос строки).
- Некорректный пароль не является ошибкой запроса, поэтому возвращается 200 с `isValid: false`.
