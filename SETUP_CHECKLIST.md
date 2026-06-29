# ✅ EtsyAnalyzer - Complete Setup Checklist

## 📋 Pre-Installation Checklist

- [ ] **Windows 10/11** (or macOS/Linux for cross-platform)
- [ ] **Internet connection** (for Etsy API access)
- [ ] **4GB+ RAM** (8GB recommended)
- [ ] **500MB free disk space**
- [ ] **Etsy seller account** (free to create)

---

## 🛠️ Installation Steps

### Step 1: Install .NET 10 SDK

- [ ] Go to https://dotnet.microsoft.com/download/dotnet/10.0
- [ ] Download .NET 10 SDK for your OS
- [ ] Run installer
- [ ] Verify installation:
  ```bash
  dotnet --version
  ```
  Expected output: `10.x.x`

### Step 2: Clone/Download the Project

**Option A: Git Clone**
- [ ] Install Git (https://git-scm.com/downloads)
- [ ] Open terminal/command prompt
- [ ] Run:
  ```bash
  git clone https://github.com/yourusername/EtsyAnalyzer.git
  cd EtsyAnalyzer
  ```

**Option B: Download ZIP**
- [ ] Download ZIP from repository
- [ ] Extract to desired folder
- [ ] Open terminal in that folder

### Step 3: Restore Dependencies

- [ ] Open terminal in project root
- [ ] Run:
  ```bash
  dotnet restore
  ```
- [ ] Wait for all packages to download (~2-3 minutes)
- [ ] ✅ Should complete without errors

### Step 4: Get Etsy API Key

- [ ] Visit https://www.etsy.com/developers/
- [ ] Sign in with your Etsy account
- [ ] Click **"Register a New App"**
- [ ] Fill in app details:
  - **App Name**: `EtsyAnalyzer` (or your choice)
  - **App Description**: `Market analysis tool`
  - **Website URL**: `http://localhost`
- [ ] Click **Create**
- [ ] Copy your **Keystring** (this is your API key)
- [ ] ⚠️ **IMPORTANT**: Save this key securely!

### Step 5: Configure the Application

- [ ] Navigate to: `src/EtsyAnalyzer.ConsoleApp/`
- [ ] Open `appsettings.json` in any text editor
- [ ] Find this section:
  ```json
  "Etsy": {
	"ApiKey": "YOUR_ETSY_API_KEY_HERE"
  }
  ```
- [ ] Replace `YOUR_ETSY_API_KEY_HERE` with your actual API key
- [ ] Save the file
- [ ] ✅ Configuration complete!

### Step 6: Build the Project

- [ ] Open terminal in project root
- [ ] Run:
  ```bash
  dotnet build
  ```
- [ ] Wait for build to complete (~30 seconds)
- [ ] ✅ Should show: `Build succeeded`

### Step 7: First Run

- [ ] Navigate to console app:
  ```bash
  cd src/EtsyAnalyzer.ConsoleApp
  ```
- [ ] Run the application:
  ```bash
  dotnet run
  ```
- [ ] ✅ You should see the main menu!

---

## 🧪 First Analysis Test

### Quick Test Run

- [ ] Choose **"Analyze New Niche"** from menu
- [ ] Enter test query: `handmade soap`
- [ ] Wait for analysis (5-10 seconds)
- [ ] ✅ Results displayed on screen
- [ ] ✅ Excel file created in `Reports/` folder
- [ ] ✅ Database file created: `etsyanalyzer.db`

### Verify Excel Report

- [ ] Navigate to: `bin/Debug/net10.0/Reports/`
- [ ] Open the generated `.xlsx` file
- [ ] ✅ Check 4 worksheets exist:
  - [ ] Summary
  - [ ] Listings
  - [ ] Shops
  - [ ] Keywords
- [ ] ✅ Data looks correct and formatted

---

## 🔧 Troubleshooting

### Problem: "dotnet command not found"

**Solution:**
- [ ] Restart terminal/command prompt after installing .NET
- [ ] Check PATH environment variable includes .NET SDK
- [ ] Re-install .NET SDK

### Problem: "API Key Not Configured"

**Solution:**
- [ ] Verify `appsettings.json` has real API key (not "YOUR_ETSY_API_KEY_HERE")
- [ ] Check for typos in API key
- [ ] Ensure no extra spaces around the key

### Problem: "Network error" or "Unauthorized"

**Solution:**
- [ ] Check internet connection
- [ ] Verify API key is valid (test on Etsy Developer Console)
- [ ] Ensure Etsy API is not down (check https://status.etsy.com/)
- [ ] Check firewall/antivirus blocking the app

### Problem: Build fails with NU1903 warning

**Solution:**
- [ ] This is a known warning (SQLite package)
- [ ] Safe to ignore for local development
- [ ] Build should still succeed despite warning

### Problem: "No listings found"

**Solution:**
- [ ] Try a different search query (more generic)
- [ ] Verify API key is working (check Settings screen)
- [ ] Check Etsy API rate limits (max 10 requests/second)

### Problem: Excel file won't open

**Solution:**
- [ ] Ensure Microsoft Excel or LibreOffice is installed
- [ ] Try opening with Google Sheets (upload to Drive)
- [ ] Check file isn't corrupted (re-run analysis)

---

## 📚 Next Steps

### Learn the Basics

- [ ] Read [QUICKSTART.md](QUICKSTART.md) for detailed usage guide
- [ ] Review [EXAMPLES.md](EXAMPLES.md) for sample outputs
- [ ] Check [PROJECT_STATUS.md](PROJECT_STATUS.md) for features

### Explore Advanced Features

- [ ] Try different search queries (see QUICKSTART.md)
- [ ] View database contents (use DB Browser for SQLite)
- [ ] Check logs in `logs/` folder
- [ ] Analyze trends over time (re-run same query weekly)

### Customize & Extend

- [ ] Read [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)
- [ ] Modify Excel report formatting
- [ ] Add custom analyzers
- [ ] Integrate additional data sources

---

## 🎯 Quick Reference Commands

### Run Application
```bash
cd src/EtsyAnalyzer.ConsoleApp
dotnet run
```

### Rebuild Everything
```bash
dotnet clean
dotnet restore
dotnet build
```

### View Logs
```bash
# Windows
notepad logs\log-[today's date].txt

# macOS/Linux
cat logs/log-$(date +%Y%m%d).txt
```

### Reset Database
```bash
# Delete database file (in bin/Debug/net10.0/)
rm etsyanalyzer.db

# Or on Windows:
del etsyanalyzer.db
```

### Update Packages (future)
```bash
dotnet list package --outdated
dotnet add package [PackageName]
```

---

## 📞 Support & Resources

### Documentation
- [README.md](README.md) - Project overview
- [QUICKSTART.md](QUICKSTART.md) - User guide
- [EXAMPLES.md](EXAMPLES.md) - Sample outputs
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical details
- [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Extension guide
- [PROJECTS.md](PROJECTS.md) - Project structure

### External Resources
- [Etsy API Documentation](https://developer.etsy.com/documentation/reference)
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet)
- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [ClosedXML Guide](https://closedxml.readthedocs.io/)

### Community
- GitHub Issues: Report bugs or request features
- Stack Overflow: Tag `etsy-api` + `c#`
- Etsy Developer Forums

---

## ✅ Final Verification

Before starting your niche research:

- [ ] Application runs without errors
- [ ] Main menu displays correctly
- [ ] Test analysis completes successfully
- [ ] Excel report generates properly
- [ ] Database file created
- [ ] Logs are being written

**All checked? You're ready to find profitable niches! 🚀**

---

## 🎓 Tips for Success

### Best Practices

1. **Start Broad, Then Narrow**
   - First search: "candles"
   - Then: "soy candles"
   - Finally: "personalized soy candles"

2. **Track Over Time**
   - Save Excel reports with dates
   - Re-analyze weekly
   - Spot trends early

3. **Compare Alternatives**
   - Run 5-10 related queries
   - Compare niche scores
   - Choose highest score + interest

4. **Use Keywords Wisely**
   - Check "Keywords" tab in Excel
   - Find less common terms
   - Lower competition opportunity

5. **Study Top Competitors**
   - "Shops" tab shows leaders
   - Visit their Etsy stores
   - Learn from their success

### What to Avoid

❌ Don't enter too many queries rapidly (API rate limit)  
❌ Don't ignore low niche scores (< 6.0)  
❌ Don't compete in saturated markets (500+ listings)  
❌ Don't solely focus on low-priced items (< $10)  
❌ Don't forget to save/backup your Excel reports

---

## 🎉 You're All Set!

**Congratulations!** EtsyAnalyzer is configured and ready to use.

Start your first real analysis and discover your profitable niche today!

**Happy analyzing! 📊✨**

---

**Last Updated**: June 27, 2026  
**Version**: 1.0  
**Support**: See documentation files for help
