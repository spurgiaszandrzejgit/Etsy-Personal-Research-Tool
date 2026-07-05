using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Infrastructure.Reporting.Interfaces;
using EtsyAnalyzer.Infrastructure.Reporting.Utilities;

namespace EtsyAnalyzer.Infrastructure.Reporting.Builders;

/// <summary>
/// Построитель листа Action Plan с практическими рекомендациями по монетизации ниши
/// </summary>
public class ActionPlanWorksheetBuilder : IWorksheetBuilder
{
    public string WorksheetName => "Action Plan";
    public int Order => 10; // После всех аналитических листов

    public void Build(IXLWorkbook workbook, AnalyticsSummaryDto summary)
    {
        var ws = workbook.Worksheets.Add(WorksheetName);
        int currentRow = 1;

        // Заголовок
        BuildHeader(ws, ref currentRow, summary);

        // Секция 1: Design Brief
        BuildDesignBrief(ws, ref currentRow, summary);

        // Секция 2: Technical Specifications
        BuildTechnicalSpecs(ws, ref currentRow, summary);

        // Секция 3: AI Prompts
        BuildAiPrompts(ws, ref currentRow, summary);

        // Секция 4: Product Matrix & Monetization
        BuildProductMatrix(ws, ref currentRow, summary);

        // Секция 5: Implementation Roadmap
        BuildImplementationRoadmap(ws, ref currentRow, summary);

        // Форматирование
        CellFormatter.AutoFitColumns(ws);
        ws.Column(2).Width = 60; // Wide column for descriptions
        CellFormatter.FreezeHeader(ws, 1);
    }

    private void BuildHeader(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        var titleCell = ws.Cell(row, 1);
        titleCell.Value = "🎯 ACTION PLAN & MONETIZATION STRATEGY";
        titleCell.Style.Font.FontSize = 16;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontColor = XLColor.White;
        titleCell.Style.Fill.BackgroundColor = ReportColors.HeaderBackground;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        var subtitleCell = ws.Cell(row, 1);
        subtitleCell.Value = $"Niche: \"{summary.SearchQuery}\" | Avg Price: {summary.CurrencySymbol}{summary.AveragePrice:F2} | Competition: {GetCompetitionLevel(summary)}";
        subtitleCell.Style.Font.FontSize = 11;
        subtitleCell.Style.Font.Italic = true;
        ws.Range(row, 1, row, 4).Merge();
        row += 2;
    }

    private void BuildDesignBrief(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Section header
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "📋 DESIGN BRIEF";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.Primary;
        sectionHeader.Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        // Generate design concepts
        var concepts = GenerateDesignConcepts(summary);

        // Table header
        ws.Cell(row, 1).Value = "Design #";
        ws.Cell(row, 2).Value = "Concept Description";
        ws.Cell(row, 3).Value = "Style";
        ws.Cell(row, 4).Value = "Key Elements";
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, 4));
        row++;

        // Table rows
        int startRow = row;
        for (int i = 0; i < concepts.Count; i++)
        {
            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = concepts[i].Description;
            ws.Cell(row, 3).Value = concepts[i].Style;
            ws.Cell(row, 4).Value = concepts[i].KeyElements;

            // Wrap text
            ws.Cell(row, 2).Style.Alignment.WrapText = true;
            ws.Cell(row, 4).Style.Alignment.WrapText = true;
            ws.Row(row).Height = 60;

            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }

        row++;
    }

    private void BuildTechnicalSpecs(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "⚙️ TECHNICAL SPECIFICATIONS";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.Success;
        sectionHeader.Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        var specs = new[]
        {
            ("Size", "8×10 inches (standard frame size)"),
            ("Resolution", "300 DPI (print quality)"),
            ("Color Profile", "RGB for digital, CMYK for print"),
            ("File Formats", "PNG (transparent), JPG (white bg), PDF (high-res)"),
            ("Scalable Sizes", "11×14 in, 16×20 in (for upsell)"),
            ("Bleed", "+0.125 inch on all sides (for physical prints)"),
            ("Color Palette", GetColorPalette(summary)),
            ("Style Guidelines", GetStyleGuidelines(summary))
        };

        foreach (var (label, value) in specs)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 2).Value = value;
            ws.Cell(row, 2).Style.Alignment.WrapText = true;
            ws.Range(row, 2, row, 4).Merge();
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }

        row++;
    }

    private void BuildAiPrompts(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "🤖 AI PROMPTS (Midjourney / DALL-E / Stable Diffusion)";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.Warning;
        sectionHeader.Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        var prompts = GenerateAiPrompts(summary);

        ws.Cell(row, 1).Value = "Design #";
        ws.Cell(row, 2).Value = "AI Prompt (Ready to Use)";
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, 4));
        ws.Range(row, 2, row, 4).Merge();
        row++;

        for (int i = 0; i < prompts.Count; i++)
        {
            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = prompts[i];
            ws.Cell(row, 2).Style.Alignment.WrapText = true;
            ws.Cell(row, 2).Style.Font.FontName = "Consolas";
            ws.Cell(row, 2).Style.Font.FontSize = 9;
            ws.Range(row, 2, row, 4).Merge();
            ws.Row(row).Height = 80;
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }

        row++;
    }

    private void BuildProductMatrix(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "💰 PRODUCT MATRIX & REVENUE PROJECTIONS";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.Success;
        sectionHeader.Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        // Table header
        ws.Cell(row, 1).Value = "Product Type";
        ws.Cell(row, 2).Value = "Price";
        ws.Cell(row, 3).Value = "Est. Sales/Month";
        ws.Cell(row, 4).Value = "Monthly Profit";
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, 4));
        row++;

        // Calculate products based on average price
        var products = GenerateProductMatrix(summary);

        decimal totalProfit = 0;
        foreach (var product in products)
        {
            ws.Cell(row, 1).Value = product.Type;
            ws.Cell(row, 2).Value = summary.CurrencySymbol + product.Price.ToString("F2");
            ws.Cell(row, 3).Value = product.EstimatedSales;
            ws.Cell(row, 4).Value = summary.CurrencySymbol + product.MonthlyProfit.ToString("F2");
            ws.Cell(row, 4).Style.Font.Bold = true;

            totalProfit += product.MonthlyProfit;
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }

        // Total row
        ws.Cell(row, 1).Value = "TOTAL PROJECTED PROFIT";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 4).Value = summary.CurrencySymbol + totalProfit.ToString("F2");
        ws.Cell(row, 4).Style.Font.Bold = true;
        ws.Cell(row, 4).Style.Fill.BackgroundColor = ReportColors.Success;
        ws.Cell(row, 4).Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 3).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row += 2;

        // Revenue scenarios
        BuildRevenueScenarios(ws, ref row, summary, totalProfit);
        row++;
    }

    private void BuildRevenueScenarios(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary, decimal baseProfit)
    {
        ws.Cell(row, 1).Value = "📈 REVENUE SCENARIOS (Month 3)";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Range(row, 1, row, 4).Merge();
        row++;

        var scenarios = new[]
        {
            ("Conservative (50% of projections)", baseProfit * 0.5m),
            ("Realistic (100% of projections)", baseProfit),
            ("Optimistic (200% with scaling)", baseProfit * 2.0m)
        };

        foreach (var (label, value) in scenarios)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 2).Value = summary.CurrencySymbol + value.ToString("F2");
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Range(row, 1, row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range(row, 2, row, 4).Merge();
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }
    }

    private void BuildImplementationRoadmap(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "🚀 IMPLEMENTATION ROADMAP";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.HeaderBackground;
        sectionHeader.Style.Font.FontColor = XLColor.White;
        ws.Range(row, 1, row, 4).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
        row++;

        var roadmap = new[]
        {
            ("Week 1-2", "Design Creation", "• Create 5 designs (Canva/Fiverr)\n• Export in all formats (PNG, JPG, PDF)\n• Create mockups for listings"),
            ("Week 3", "Etsy Store Setup", "• Set up Etsy shop (if new)\n• Connect Printful/Printify\n• Upload products (digital + POD)"),
            ("Week 4", "Listing Optimization", "• Write SEO-optimized titles/tags\n• Create compelling descriptions\n• Set competitive pricing"),
            ("Month 2", "Launch & Promotion", "• Launch Etsy Ads ($5-10/day)\n• Create Pinterest pins (organic traffic)\n• Post on Instagram/TikTok"),
            ("Month 3+", "Scale & Optimize", "• Add new designs based on bestsellers\n• Expand product line (mugs, totes)\n• Analyze metrics, adjust pricing")
        };

        ws.Cell(row, 1).Value = "Timeline";
        ws.Cell(row, 2).Value = "Phase";
        ws.Cell(row, 3).Value = "Action Steps";
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, 4));
        ws.Range(row, 3, row, 4).Merge();
        row++;

        foreach (var (timeline, phase, actions) in roadmap)
        {
            ws.Cell(row, 1).Value = timeline;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 2).Value = phase;
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 3).Value = actions;
            ws.Cell(row, 3).Style.Alignment.WrapText = true;
            ws.Range(row, 3, row, 4).Merge();
            ws.Row(row).Height = 60;
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }

        row++;

        // Resources section
        BuildResourcesSection(ws, ref row);
    }

    private void BuildResourcesSection(IXLWorksheet ws, ref int row)
    {
        ws.Cell(row, 1).Value = "🛠️ RECOMMENDED TOOLS & RESOURCES";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Range(row, 1, row, 4).Merge();
        row++;

        var resources = new[]
        {
            ("Design Tools", "Canva Pro ($12.99/mo), Procreate (iPad), Adobe Illustrator"),
            ("POD Services", "Printful (best quality), Printify (more options), Gelato (fast EU shipping)"),
            ("Hire Designers", "Fiverr ($10-50/design), Upwork (hourly rates), Dribbble (portfolio-based)"),
            ("AI Art Tools", "Midjourney ($10/mo), DALL-E 3 (ChatGPT Plus), Stable Diffusion (free)"),
            ("Marketing", "Pinterest (free organic), Etsy Ads (pay-per-click), Instagram Reels")
        };

        foreach (var (category, tools) in resources)
        {
            ws.Cell(row, 1).Value = category;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 2).Value = tools;
            ws.Cell(row, 2).Style.Alignment.WrapText = true;
            ws.Range(row, 2, row, 4).Merge();
            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 4));
            row++;
        }
    }

    // Helper methods for generating content

    private List<DesignConcept> GenerateDesignConcepts(AnalyticsSummaryDto summary)
    {
        // Extract top keywords from the niche
        var keywords = ExtractKeywords(summary);
        var concepts = new List<DesignConcept>();

        // Generate 5 design concepts based on keywords
        var templates = GetDesignTemplates(keywords);

        for (int i = 0; i < Math.Min(5, templates.Count); i++)
        {
            concepts.Add(templates[i]);
        }

        return concepts;
    }

    private List<string> GenerateAiPrompts(AnalyticsSummaryDto summary)
    {
        var keywords = ExtractKeywords(summary);
        var prompts = new List<string>();

        var concepts = GenerateDesignConcepts(summary);
        foreach (var concept in concepts)
        {
            var prompt = $"Watercolor illustration of {concept.Description}, " +
                        $"{concept.Style} style, {concept.KeyElements}, " +
                        $"cottagecore aesthetic, soft muted colors, vintage feel, " +
                        $"8x10 aspect ratio, high quality, --ar 4:5 --style raw --v 6";
            prompts.Add(prompt);
        }

        return prompts;
    }

    private List<ProductRecommendation> GenerateProductMatrix(AnalyticsSummaryDto summary)
    {
        var avgPrice = (decimal)summary.AveragePrice;
        var products = new List<ProductRecommendation>();

        // Digital downloads (100% profit)
        products.Add(new ProductRecommendation
        {
            Type = "Digital Download Bundle (3-5 designs)",
            Price = Math.Max(8, avgPrice * 0.15m),
            EstimatedSales = 30,
            MonthlyProfit = Math.Max(8, avgPrice * 0.15m) * 30
        });

        // Physical prints (POD)
        products.Add(new ProductRecommendation
        {
            Type = "Physical Prints (via Printful)",
            Price = Math.Max(25, avgPrice * 0.4m),
            EstimatedSales = 15,
            MonthlyProfit = Math.Max(25, avgPrice * 0.4m) * 0.6m * 15 // 60% margin
        });

        // Framed prints
        products.Add(new ProductRecommendation
        {
            Type = "Framed Prints (Premium)",
            Price = Math.Max(60, avgPrice * 0.85m),
            EstimatedSales = 5,
            MonthlyProfit = Math.Max(60, avgPrice * 0.85m) * 0.5m * 5 // 50% margin
        });

        // Add-ons (mugs, tote bags, etc.)
        products.Add(new ProductRecommendation
        {
            Type = "Mugs & Accessories",
            Price = 22,
            EstimatedSales = 10,
            MonthlyProfit = 22 * 0.4m * 10 // 40% margin
        });

        return products;
    }

    private List<string> ExtractKeywords(AnalyticsSummaryDto summary)
    {
        var keywords = new List<string> { summary.SearchQuery };

        // Try to extract keywords from TopKeywords if available
        if (summary.TopKeywords != null && summary.TopKeywords.Any())
        {
            keywords.AddRange(summary.TopKeywords.Take(10).Select(k => k.Keyword));
        }

        return keywords;
    }

    private List<DesignConcept> GetDesignTemplates(List<string> keywords)
    {
        // Analyze keywords and generate appropriate design concepts
        var mainKeyword = keywords.FirstOrDefault()?.ToLower() ?? "art";

        var concepts = new List<DesignConcept>();

        if (mainKeyword.Contains("cottage") || mainKeyword.Contains("mushroom"))
        {
            concepts.Add(new DesignConcept
            {
                Description = "Trio of watercolor mushrooms with moss and wildflowers",
                Style = "Whimsical watercolor",
                KeyElements = "Red fly agaric, cream button mushrooms, ferns, soft beige background"
            });
        }

        if (mainKeyword.Contains("art") || mainKeyword.Contains("botanical"))
        {
            concepts.Add(new DesignConcept
            {
                Description = "Vintage botanical herb illustration collection",
                Style = "18th century botanical print",
                KeyElements = "Rosemary, thyme, lavender, sage with Latin names, aged paper texture"
            });
        }

        if (mainKeyword.Contains("fairy") || mainKeyword.Contains("whimsical"))
        {
            concepts.Add(new DesignConcept
            {
                Description = "Fairy reading under a toadstool in golden hour light",
                Style = "Soft fantasy illustration",
                KeyElements = "Delicate wings, natural fabrics, glowing fireflies, peaceful mood"
            });
        }

        concepts.Add(new DesignConcept
        {
            Description = $"Wildflower meadow inspired by {mainKeyword}",
            Style = "Impressionist watercolor",
            KeyElements = "Daisies, poppies, cornflowers, soft focus background, dreamy atmosphere"
        });

        concepts.Add(new DesignConcept
        {
            Description = $"Cozy cottage window scene featuring {mainKeyword} theme",
            Style = "Realistic with soft edges",
            KeyElements = "Weathered wood frame, herb pots, linen curtains, garden view, warm light"
        });

        return concepts;
    }

    private string GetColorPalette(AnalyticsSummaryDto summary)
    {
        return "Sage green (#9CAF88), Soft beige (#E8DCC4), Warm brown (#A0826D), " +
               "Muted terracotta (#C18C74), Cream white (#FFF8E7), Dusty rose (#D4A5A5)";
    }

    private string GetStyleGuidelines(AnalyticsSummaryDto summary)
    {
        return "Watercolor or watercolor-style, medium detail, cozy and nostalgic mood, " +
               "soft natural textures, avoid neon colors and harsh blacks";
    }

    private string GetCompetitionLevel(AnalyticsSummaryDto summary)
    {
        var listings = summary.Listings?.Count ?? 0;
        if (listings < 50) return "Low";
        if (listings < 150) return "Medium";
        return "High";
    }

    private class DesignConcept
    {
        public string Description { get; set; } = "";
        public string Style { get; set; } = "";
        public string KeyElements { get; set; } = "";
    }

    private class ProductRecommendation
    {
        public string Type { get; set; } = "";
        public decimal Price { get; set; }
        public int EstimatedSales { get; set; }
        public decimal MonthlyProfit { get; set; }
    }
}
