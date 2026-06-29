# EtsyAnalyzer - Quick Start Guide

## 🚀 First Time Setup

### 1. Get Your Etsy API Key

1. Visit [Etsy Developers Portal](https://www.etsy.com/developers/)
2. Sign in with your Etsy account
3. Click **"Register a New App"** or **"Create App"**
4. Fill in the required information:
   - **App Name**: EtsyAnalyzer (or any name)
   - **App Description**: Market analysis tool
   - **Website URL**: http://localhost (for development)
5. After creation, copy your **Keystring** (this is your API key)

### 2. Configure the Application

Open `src/EtsyAnalyzer.ConsoleApp/appsettings.json` and replace:

```json
{
  "Etsy": {
	"ApiKey": "YOUR_ETSY_API_KEY_HERE"  ← Paste your key here
  }
}
```

### 3. Run the Application

```bash
cd src/EtsyAnalyzer.ConsoleApp
dotnet run
```

## 📊 Sample Queries to Try

Here are some profitable niches to analyze:

### Handmade & Crafts
- `handmade wood cutting board`
- `personalized wooden sign`
- `custom leather wallet`
- `handwoven macrame wall hanging`

### Jewelry
- `sterling silver minimalist necklace`
- `birthstone ring`
- `custom name bracelet`
- `vintage inspired earrings`

### Home Decor
- `modern farmhouse wall art`
- `boho throw pillow`
- `scandinavian wall clock`
- `minimalist shelf decor`

### Wedding
- `rustic wedding invitation`
- `bridesmaid proposal box`
- `personalized wedding gift`
- `wedding table number`

### Digital Products
- `printable planner pages`
- `digital wall art download`
- `wedding invitation template`
- `business card template`

## 🎯 Understanding the Results

### Niche Score (1-10)

- **8-10**: 🟢 Excellent niche! Low competition, good demand
- **6-7.9**: 🟡 Good opportunity with moderate competition
- **4-5.9**: 🟠 Challenging, high competition
- **1-3.9**: 🔴 Very difficult, saturated market

### Competition Level

- **Low** (< 50 listings): Easy to enter, less competition
- **Medium** (50-200 listings): Balanced opportunity
- **High** (200+ listings): Crowded market, needs differentiation

### What to Look For

**Ideal Niche Characteristics:**
- ✅ 50-200 total listings (Medium competition)
- ✅ Average price $30-100 (good profit margin)
- ✅ Wide price range (room for different tiers)
- ✅ No single shop dominating (< 20% market share)
- ✅ Niche score 7+

**Red Flags:**
- ❌ 500+ listings (oversaturated)
- ❌ Average price < $10 (low margins)
- ❌ One shop has 50%+ of listings
- ❌ Very narrow price range (no differentiation)

## 📈 Step-by-Step Analysis Workflow

### Step 1: Broad Research
Start with general categories:
```
wood wall art
```

### Step 2: Narrow Down
Based on keywords from Step 1, get specific:
```
rustic wood wall art
modern geometric wood art
personalized family name sign
```

### Step 3: Analyze Competition
Check the Excel report's **Shops** tab:
- Are top shops selling many items in this niche?
- Can you differentiate from them?

### Step 4: Keyword Optimization
Use the **Keywords** tab to:
- Find popular search terms
- Discover trending tags
- Optimize your product titles

### Step 5: Pricing Strategy
Use the **Summary** price distribution:
- Position your product in the sweet spot
- Avoid the oversaturated price range
- Consider premium positioning if quality justifies

## 💡 Pro Tips

### 1. Compare Multiple Queries
Run several related searches:
```
vintage inspired ring
antique style ring
retro ring design
```
Compare niche scores to find the best variant.

### 2. Seasonal Trends
Some niches are seasonal:
- **Q4**: Christmas ornaments, gifts
- **Q2**: Wedding season items
- **Summer**: Beach/outdoor decor

### 3. Price Positioning
- **Low-end** ($10-25): High volume needed
- **Mid-range** ($25-75): Best balance
- **Premium** ($75+): Lower volume, higher margin

### 4. Differentiation Strategies
If competition is high:
- ✅ Offer personalization
- ✅ Unique materials or techniques
- ✅ Better photography
- ✅ Faster shipping
- ✅ Superior customer service

## 📁 Understanding the Excel Report

### Summary Tab
Quick overview of all key metrics - start here!

### Listings Tab
- All products found
- Click URLs to view on Etsy
- Sort by price to find positioning gaps

### Shops Tab
- Top competitors ranked
- Medal indicators for top 3
- Red highlighting = dominant shops

### Keywords Tab
- Most used words in titles
- Popular tags
- Color-coded by frequency

## 🔧 Troubleshooting

### "API Key Not Configured"
Edit `appsettings.json` and add your real Etsy API key.

### "Network Error"
Check your internet connection and verify API key is correct.

### "No listings found"
Try a different search query - might be too specific.

### "Database error"
Delete `etsyanalyzer.db` file and restart the app.

## 📞 Need Help?

Check the logs in `logs/` folder for detailed error messages.

---

**Happy analyzing! 🎉**
