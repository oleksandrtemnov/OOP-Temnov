Лабораторна робота №17 (Варіант 5): Обробка IO/мережевих помилок та патерн Retry

Реалізація:
- FileProcessor: метод UpdateConfig імітує IOException перші 2 рази, після чого успішно оновлює файл.
- NetworkClient: метод SendConfigUpdate імітує HttpRequestException перші 3 рази, після чого успішно відправляє дані.
- RetryHelper: узагальнений метод ExecuteWithRetry<T> з експоненційною затримкою, логуванням спроб і фільтрацією винятків через shouldRetry (лише для IOException та HttpRequestException).

Вивід програми:
=== Сценарій 1: Оновлення локальної конфігурації (FileProcessor) ===
[FileProcessor] Спроба #1: оновлення 'MaxConnections=100' у файлі 'appsettings.json'...
[RetryHelper] Помилка (IOException): Файл 'appsettings.json' заблокований іншим процесом.
[RetryHelper] Повторна спроба #1/3 через 200 мс...

[FileProcessor] Спроба #2: оновлення 'MaxConnections=100' у файлі 'appsettings.json'...
[RetryHelper] Помилка (IOException): Файл 'appsettings.json' заблокований іншим процесом.
[RetryHelper] Повторна спроба #2/3 через 400 мс...

[FileProcessor] Спроба #3: оновлення 'MaxConnections=100' у файлі 'appsettings.json'...
[FileProcessor] Успіх! Параметр 'MaxConnections=100' записано у 'appsettings.json'.

=== Сценарій 2: Відправка конфігурації на сервер (NetworkClient) ===
[NetworkClient] Спроба #1: відправка конфігурації на 'https://api.example.com/config'...
[RetryHelper] Помилка (HttpRequestException): Помилка з'єднання з сервером 'https://api.example.com/config' (503 Service Unavailable).
[RetryHelper] Повторна спроба #1/4 через 200 мс...

[NetworkClient] Спроба #2: відправка конфігурації на 'https://api.example.com/config'...
[RetryHelper] Помилка (HttpRequestException): Помилка з'єднання з сервером 'https://api.example.com/config' (503 Service Unavailable).
[RetryHelper] Повторна спроба #2/4 через 400 мс...

[NetworkClient] Спроба #3: відправка конфігурації на 'https://api.example.com/config'...
[RetryHelper] Помилка (HttpRequestException): Помилка з'єднання з сервером 'https://api.example.com/config' (503 Service Unavailable).
[RetryHelper] Повторна спроба #3/4 через 800 мс...

[NetworkClient] Спроба #4: відправка конфігурації на 'https://api.example.com/config'...
[NetworkClient] Успіх! Сервер 'https://api.example.com/config' прийняв конфігурацію: {"MaxConnections":100}
[Main] Результат відправки по мережі: True

=== Сценарій 3: Перевірка shouldRetry для ArgumentException (без повторів) ===
[Validation] Перевірка формату конфігурації...
[RetryHelper] Виняток ArgumentException не підлягає повтору (shouldRetry = false).
[Main] Перехоплено критичну помилку без повторів: Некоректний ключ конфігурації.

Висновок:
Експоненційна затримка працює за формулою initialDelay * 2^(retryAttempt - 1), подвоюючи час очікування після кожної невдалої спроби (200 мс, 400 мс, 800 мс). Це знижує навантаження на заблокований файл чи сервер і дає ресурсу час відновити роботу.