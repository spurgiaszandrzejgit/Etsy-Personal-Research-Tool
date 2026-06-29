# 🔧 Troubleshooting Guide

## ❌ Error: "404 Not Found" from Etsy API

### Problem
You see an error like:
```
Error: Etsy API request failed with status NotFound: <!DOCTYPE HTML...>
<h1>Not Found</h1>
```

### Causes & Solutions

#### 1. **API Key Not Configured** ⚠️ (Most Common)

**Check**: Open `src/EtsyAnalyzer.ConsoleApp/appsettings.json`

If you see:
```json
"ApiKey": "YOUR_ETSY_API_KEY_HERE"
```

**Solution**:
1. Get your API key from [Etsy Developers Portal](https://www.etsy.com/developers/register)
2. Sign in with your Etsy account
3. Click **"Apps"** → **"New App"**
4. Fill in:
   - App Name: `EtsyAnalyzer`
   - Description: `Market analysis tool`
   - Website: `http://localhost`
5. Copy the **Keystring** (your API key)
6. Paste it in `appsettings.json`:
   ```json
   "ApiKey": "your_actual_keystring_here"
   ```

#### 2. **Invalid API Key**

**Symptoms**: 401 Unauthorized or 404 errors

**Solution**:
- Verify the key is copied correctly (no extra spaces)
- Check the key is active in [Etsy Developer Console](https://www.etsy.com/developers/your-apps)
- Regenerate the key if needed

#### 3. **API Endpoint Changed**

**Check**: Etsy might have updated their API

**Solution**:
- Visit [Etsy API v3 Documentation](https://developer.etsy.com/documentation/reference)
- Verify the endpoint: `/v3/application/listings/active`
- Update `EtsyApiClient.cs` if needed

---

## ⚠️ Other Common Issues

### "Network error while calling Etsy API"

**Causes**:
- No internet connection
- Firewall blocking the request
- Proxy configuration needed

**Solution**:
```bash
# Test internet connection
ping openapi.etsy.com

# Check if API is accessible
curl https://openapi.etsy.com/v3/application/openapi-ping/ping
```

### "Rate limit exceeded"

**Cause**: Too many requests to Etsy API (max 10/second)

**Solution**: Wait 1 minute and try again

### "Database error"

**Cause**: SQLite database file corrupted

**Solution**:
```bash
# Delete database and restart
cd src/EtsyAnalyzer.ConsoleApp/bin/Debug/net10.0
del etsyanalyzer.db
cd ../../..
dotnet run
```

---

## 🧪 Testing Without Real API Key

If you don't have an Etsy API key yet, you can:

### Option 1: Use Mock Data (Future Enhancement)
Create a test mode that uses sample data instead of real API calls.

### Option 2: Request Access
1. Go to https://www.etsy.com/developers/register
2. Create a free developer account
3. Register your app
4. Get instant API access (free tier: 10k requests/day)

---

## 📝 Checking Logs

View detailed error messages in logs:

**Windows**:
```powershell
cd src/EtsyAnalyzer.ConsoleApp/logs
notepad log-$(Get-Date -Format yyyyMMdd).txt
```

**Linux/Mac**:
```bash
cd src/EtsyAnalyzer.ConsoleApp/logs
cat log-$(date +%Y%m%d).txt
```

---

## 🔍 Verifying Configuration

Run this check:

```powershell
cd src/EtsyAnalyzer.ConsoleApp
$config = Get-Content appsettings.json | ConvertFrom-Json
if ($config.Etsy.ApiKey -eq "YOUR_ETSY_API_KEY_HERE") {
	Write-Host "❌ API Key not configured!" -ForegroundColor Red
} else {
	Write-Host "✅ API Key configured" -ForegroundColor Green
}
```

---

## 📞 Still Having Issues?

1. **Check logs** in `logs/` folder
2. **Verify API key** in Etsy Developer Console
3. **Test endpoint** manually:
   ```bash
   curl -H "x-api-key: YOUR_KEY" https://openapi.etsy.com/v3/application/listings/active?keywords=soap&limit=10
   ```
4. **Review documentation**: [Etsy API Docs](https://developer.etsy.com/documentation)

---

## ✅ Success Checklist

Before running the app, ensure:

- [ ] .NET 10 SDK installed
- [ ] Etsy developer account created
- [ ] App registered on Etsy
- [ ] API key copied to `appsettings.json`
- [ ] No typos in the API key
- [ ] Internet connection active
- [ ] Firewall allows HTTPS requests

---

**Need more help?** See [SETUP_CHECKLIST.md](SETUP_CHECKLIST.md) for detailed setup guide.
