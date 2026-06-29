# Example Usage & Sample Output

## 🎯 Complete Workflow Example

### Step 1: Start Application

```bash
cd src/EtsyAnalyzer.ConsoleApp
dotnet run
```

### Step 2: Main Menu Display

```
── EtsyAnalyzer ──────────────────────────────────────────────────

Professional Etsy Market Analysis Tool
Find profitable niches with data-driven insights

What would you like to do?
> Analyze New Niche
  View Statistics
  Settings
  Exit
```

### Step 3: Enter Search Query

```
── Niche Analysis ────────────────────────────────────────────────

Enter search query: handmade wood cutting board
```

### Step 4: Progress Display

```
┌────────────────────────────────────────────────────────────────┐
│ Fetching data from Etsy API...     ████████████████ 100% ⠙    │
│ Analyzing market data...            ████████████████ 100% ⠙    │
│ Generating Excel report...          ████████████████ 100% ⠙    │
└────────────────────────────────────────────────────────────────┘
```

### Step 5: Results Display

```
── Analysis Complete! ────────────────────────────────────────────

╭──────────────────────┬──────────────────────────────────────╮
│        Metric        │                Value                 │
├──────────────────────┼──────────────────────────────────────┤
│    Search Query      │  handmade wood cutting board         │
│   Total Listings     │  156                                 │
│   Average Price      │  45.50 USD                           │
│   Median Price       │  42.00 USD                           │
│   Price Range        │  15.00 - 125.00                      │
│   Competition        │  Medium                              │
│   Niche Score        │  7.8                                 │
╰──────────────────────┴──────────────────────────────────────╯

Top 5 Keywords:
  wood                 ████████████████████ 156 (100.0%)
  cutting              ████████████████     142 (91.0%)
  board                ███████████████      138 (88.5%)
  handmade             ██████████           98 (62.8%)
  personalized         ████████             67 (42.9%)

Top 3 Competitors:
  • WoodCraftShop - 12 listings in search
  • CustomWoodWorks - 8 listings in search
  • ArtisanKitchen - 6 listings in search

✓ Excel report generated in ./Reports/

Press any key to continue...
```

---

## 📊 Sample Excel Report Structure

### Worksheet 1: Summary

```
╔════════════════════════════════════════════════════════════════╗
║              ETSY MARKET ANALYSIS - SUMMARY                    ║
╚════════════════════════════════════════════════════════════════╝

Search Query: handmade wood cutting board
Date: 2026-06-27 11:30:45
Data Source: Etsy API v3

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OVERVIEW METRICS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Total Listings:        156
Competition Level:     Medium
Niche Score:           7.8 / 10  ⭐⭐⭐⭐

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PRICE STATISTICS (USD)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Average Price:         $45.50
Median Price:          $42.00
Min Price:             $15.00
Max Price:             $125.00

Price Distribution:
  $0-25:               18 listings (11.5%)
  $25-50:              89 listings (57.1%)  ← MOST COMMON
  $50-75:              38 listings (24.4%)
  $75+:                11 listings (7.1%)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
TOP KEYWORDS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Keyword             Count    Percentage
wood                156      100.0%
cutting             142      91.0%
board               138      88.5%
handmade            98       62.8%
personalized        67       42.9%
```

### Worksheet 2: Listings

```
┌────┬────────────┬──────────────────────────┬────────┬──────────────┐
│ #  │ Listing ID │ Title                    │ Price  │ Shop Name    │
├────┼────────────┼──────────────────────────┼────────┼──────────────┤
│ 1  │ 1234567890 │ Custom Wood Cutting Boa..│ $42.50 │ WoodCraftShop│
│ 2  │ 1234567891 │ Personalized Walnut Boa..│ $55.00 │ CustomWood.. │
│ 3  │ 1234567892 │ Handmade Maple Board wi..│ $38.00 │ ArtisanKitch.│
│... │ ...        │ ...                      │ ...    │ ...          │
└────┴────────────┴──────────────────────────┴────────┴──────────────┘

(Columns: #, Listing ID, Title, Price, Currency, Shop Name, Rating,
 Reviews, Category, Tags, URL - all hyperlinked)
```

### Worksheet 3: Shops

```
┌────┬─────────────────┬──────────┬────────┬─────────┐
│Rank│ Shop Name       │ Listings │ % Share│ Rating  │
├────┼─────────────────┼──────────┼────────┼─────────┤
│ 🥇 │ WoodCraftShop   │    12    │ 7.7%   │ 4.9 ⭐  │
│ 🥈 │ CustomWoodWorks │     8    │ 5.1%   │ 4.8 ⭐  │
│ 🥉 │ ArtisanKitchen  │     6    │ 3.8%   │ 5.0 ⭐  │
│  4 │ HandmadeWood    │     5    │ 3.2%   │ 4.7 ⭐  │
│... │ ...             │    ...   │ ...    │ ...     │
└────┴─────────────────┴──────────┴────────┴─────────┘

(Top 3 rows highlighted with medals, dominant shops in red)
```

### Worksheet 4: Keywords

```
┌────┬──────────────┬────────┬────────────┬─────────────────┐
│ #  │ Keyword      │ Count  │ Percentage │ Frequency Bar   │
├────┼──────────────┼────────┼────────────┼─────────────────┤
│  1 │ wood         │  156   │  100.0%    │ ████████████████│
│  2 │ cutting      │  142   │   91.0%    │ ██████████████▌ │
│  3 │ board        │  138   │   88.5%    │ ██████████████▏ │
│  4 │ handmade     │   98   │   62.8%    │ ██████████      │
│  5 │ personalized │   67   │   42.9%    │ ██████▉         │
│  6 │ custom       │   54   │   34.6%    │ █████▌          │
│  7 │ engraved     │   42   │   26.9%    │ ████▎           │
│... │ ...          │  ...   │   ...      │ ...             │
└────┴──────────────┴────────┴────────────┴─────────────────┘

(Color-coded: High frequency = Green, Medium = Yellow, Low = Gray)
```

---

## 💡 Real-World Examples

### Example 1: High Competition Niche

**Query**: `printable wall art`

**Results**:
```
Total Listings:     847
Competition:        High
Niche Score:        3.2 / 10  ⚠️
Average Price:      $8.50

Analysis:
❌ Very saturated market
❌ Low price point (digital products)
❌ Difficult for new sellers
✅ High demand exists
```

**Recommendation**: Avoid unless you have unique style or existing audience

---

### Example 2: Ideal Niche

**Query**: `custom leather dog collar`

**Results**:
```
Total Listings:     82
Competition:        Low
Niche Score:        8.9 / 10  ⭐⭐⭐⭐⭐
Average Price:      $45.00

Analysis:
✅ Low competition (< 100 listings)
✅ Good price point ($40-60 range)
✅ Personalization opportunity
✅ Clear target audience (pet owners)
```

**Recommendation**: Excellent opportunity for new sellers!

---

### Example 3: Niche Too Narrow

**Query**: `steampunk octopus lamp handmade bronze`

**Results**:
```
Total Listings:     3
Competition:        Low
Niche Score:        2.1 / 10  ⚠️
Average Price:      $350.00

Analysis:
⚠️ Very niche market
⚠️ Low search volume
✅ High price point
❌ Limited audience
```

**Recommendation**: Too specific - broaden search to "steampunk lamp"

---

## 🎓 Interpreting Results

### Niche Score Breakdown

| Score | Competition | Price Range | Recommendation |
|-------|-------------|-------------|----------------|
| 9-10  | Low (<50)   | $30-100     | 🟢 Start today! |
| 7-8.9 | Medium      | $25-75      | 🟡 Good opportunity |
| 5-6.9 | High        | $15-50      | 🟡 Needs differentiation |
| 3-4.9 | Very High   | <$15        | 🔴 Challenging |
| 1-2.9 | Any         | Any         | 🔴 Avoid |

### Competition Level Guide

**Low (< 50 listings)**
- Easy entry
- Less saturation
- Higher visibility chance
- Example: "custom leather dog collar"

**Medium (50-200 listings)**
- Balanced opportunity
- Proven demand
- Manageable competition
- Example: "handmade wood cutting board"

**High (200+ listings)**
- Crowded market
- Needs unique selling point
- Strong marketing required
- Example: "printable wall art"

---

## 📁 Generated Files

After analysis, you'll find:

```
EtsyAnalyzer.ConsoleApp/
├── bin/Debug/net10.0/
│   ├── etsyanalyzer.db                    (SQLite database)
│   └── Reports/
│       └── EtsyAnalysis_handmade_wood_cutting_board_20260627_113045.xlsx
└── logs/
	└── log-20260627.txt                   (Serilog logs)
```

---

## 🔍 Database Contents

Query the SQLite database to see historical data:

```sql
-- View all analyzed niches
SELECT Query, ExecutedAt, ResultCount, DataSource 
FROM SearchQueries 
ORDER BY ExecutedAt DESC;

-- Top shops across all searches
SELECT ShopName, COUNT(*) as AppearanceCount
FROM Listings
GROUP BY ShopName
ORDER BY AppearanceCount DESC
LIMIT 10;

-- Price trends for a specific query
SELECT 
	sq.Query,
	ar.AveragePrice,
	ar.MedianPrice,
	ar.CompetitionLevel,
	ar.NicheScore,
	ar.AnalyzedAt
FROM AnalysisResults ar
JOIN SearchQueries sq ON ar.SearchQueryId = sq.Id
WHERE sq.Query LIKE '%wood%'
ORDER BY ar.AnalyzedAt DESC;
```

---

## 🚀 Advanced Usage Tips

### 1. Compare Related Niches

Run multiple searches and compare Excel reports:

```
Search 1: "wood cutting board"         → Score: 7.8
Search 2: "personalized cutting board" → Score: 8.2
Search 3: "engraved cutting board"     → Score: 6.9

Winner: "personalized cutting board" (best score + keywords)
```

### 2. Track Niche Over Time

Re-run the same query weekly/monthly:

```
Week 1: handmade candles → 150 listings, score 7.5
Week 4: handmade candles → 189 listings, score 6.8
Week 8: handmade candles → 234 listings, score 5.9

Trend: Competition increasing rapidly! ⚠️
```

### 3. Find Keyword Gaps

Look at "Keywords" sheet for underutilized terms:

```
Common: wood (100%), cutting (91%), board (88%)
Opportunity: "maple" (only 15%) ← Less competition!
```

### 4. Identify Dominant Competitors

Check "Shops" sheet for market leaders:

```
If one shop has > 20% market share → Avoid (dominated)
If top 3 shops have < 30% combined → Good (distributed)
```

---

**Ready to find your profitable niche? Start analyzing! 🎯**
