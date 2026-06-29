# ⚠️ ВАЖНО: Настройка API ключа

## Вы увидели ошибку 404? Вот как исправить:

### Шаг 1: Получите API ключ Etsy (5 минут)

1. **Откройте**: https://www.etsy.com/developers/register
2. **Войдите** с вашим Etsy аккаунтом (или создайте бесплатный)
3. **Нажмите**: "Apps" → "New App"
4. **Заполните форму**:
   ```
   App Name: EtsyAnalyzer
   App Description: Market analysis tool
   Website URL: http://localhost
   ```
5. **Нажмите**: "Create" или "Register"
6. **Скопируйте** ваш **Keystring** (это API ключ)

### Шаг 2: Настройте приложение (1 минута)

1. **Откройте файл**:
   ```
   C:\Users\andre\source\repos\EtsyAnalyzer\src\EtsyAnalyzer.ConsoleApp\appsettings.json
   ```

2. **Найдите строку**:
   ```json
   "ApiKey": "YOUR_ETSY_API_KEY_HERE"
   ```

3. **Замените** на ваш реальный ключ:
   ```json
   "ApiKey": "ваш_скопированный_keystring_здесь"
   ```

4. **Сохраните** файл (Ctrl+S)

### Шаг 3: Запустите снова

```powershell
cd C:\Users\andre\source\repos\EtsyAnalyzer\src\EtsyAnalyzer.ConsoleApp
dotnet run
```

Теперь введите: `handmade soap` и всё должно работать! ✅

---

## Быстрая проверка конфигурации

Выполните в PowerShell:

```powershell
cd C:\Users\andre\source\repos\EtsyAnalyzer\src\EtsyAnalyzer.ConsoleApp
$config = Get-Content appsettings.json | ConvertFrom-Json
if ($config.Etsy.ApiKey -eq "YOUR_ETSY_API_KEY_HERE") {
	Write-Host "❌ API ключ НЕ настроен! Следуйте инструкциям выше." -ForegroundColor Red
} else {
	Write-Host "✅ API ключ настроен: $($config.Etsy.ApiKey.Substring(0,10))..." -ForegroundColor Green
	Write-Host "Запустите: dotnet run" -ForegroundColor Cyan
}
```

---

## Альтернатива: Редактирование в Visual Studio

1. **Откройте** Solution Explorer
2. **Найдите**: EtsyAnalyzer.ConsoleApp → appsettings.json
3. **Двойной клик** для открытия
4. **Измените** строку с API ключом
5. **Сохраните** (Ctrl+S)
6. **Запустите** (F5 или Ctrl+F5)

---

## Помощь

Если проблемы остались:
- 📖 [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - детальное руководство
- 📖 [QUICKSTART.md](QUICKSTART.md) - полная инструкция
- 📖 [SETUP_CHECKLIST.md](SETUP_CHECKLIST.md) - чеклист настройки

---

**После настройки API ключа приложение будет работать отлично! 🚀**
