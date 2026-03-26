using System.Text.Json.Nodes;
using SkiaSharp;

namespace OpsT360.Services;

public sealed record ValidationBox(int X1, int Y1, int X2, int Y2, double Area);

public sealed record RoboflowValidationResult(
    bool IsSuccessful,
    string? ValidatedImageBase64,
    string? OutputImageBase64,
    IReadOnlyList<ValidationBox> DetectedLabels);

public sealed class RoboflowValidationService
{
    private const double BlueRatioThreshold = 0.3;
    private const double AreaSimilarityThreshold = 0.4;
    private const double PanelMin = 0.4;
    private const double PanelMax = 0.6;
    private const double PanelOverlapTolerance = 0.03;
    private const int PanelShrinkPx = 2;

    public RoboflowValidationResult AnalyzeValidation(JsonNode? response, string? baseImageBase64)
    {
        var normalizedBaseImage = NormalizeBaseImage(baseImageBase64);
        var predictions = ExtractPredictions(response);
        var outputImage = ExtractOutputImage(response);

        if (!predictions.Any() || string.IsNullOrWhiteSpace(normalizedBaseImage))
        {
            return new RoboflowValidationResult(false, null, outputImage, Array.Empty<ValidationBox>());
        }

        using var bitmap = DecodeBitmap(normalizedBaseImage);
        if (bitmap is null)
        {
            return new RoboflowValidationResult(false, null, outputImage, Array.Empty<ValidationBox>());
        }

        var validLabelBoxes = predictions
            .Where(p => string.Equals(p.Class, "stickerOK", StringComparison.OrdinalIgnoreCase))
            .Select(BuildBox)
            .Where(box => IsBlueLabel(bitmap, box))
            .ToList();

        var finalBoxes = ReduceLabelBoxes(validLabelBoxes);

        var accessPanels = predictions
            .Where(p => string.Equals(p.Class, "access_panel", StringComparison.OrdinalIgnoreCase))
            .Select(p => new { Box = BuildBox(p), Confidence = p.Confidence ?? 0d })
            .ToList();

        var mainPanel = accessPanels.Count == 0
            ? null
            : accessPanels.OrderByDescending(p => p.Confidence).First();

        var validatedImage = BuildValidatedImage(bitmap, accessPanels.Select(p => p.Box).ToList(), finalBoxes, mainPanel?.Box);

        if (mainPanel is null || finalBoxes.Count == 0)
        {
            return new RoboflowValidationResult(false, validatedImage, outputImage, finalBoxes);
        }

        var panelBox = ShrinkBox(mainPanel.Box, PanelShrinkPx);
        var lowerBound = PanelMin - PanelOverlapTolerance;
        var upperBound = PanelMax + PanelOverlapTolerance;

        var success = finalBoxes.All(box =>
        {
            var overlap = IntersectionArea(box, panelBox);
            var ratio = box.Area > 0 ? overlap / box.Area : 0;
            return ratio >= lowerBound && ratio <= upperBound;
        });

        return new RoboflowValidationResult(success, validatedImage, outputImage, finalBoxes);
    }

    private static string? BuildValidatedImage(SKBitmap source, List<ValidationBox> accessPanels, List<ValidationBox> labelBoxes, ValidationBox? mainPanel)
    {
        using var image = SKImage.FromBitmap(source);
        using var surface = SKSurface.Create(new SKImageInfo(source.Width, source.Height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);
        canvas.DrawImage(image, 0, 0);

        foreach (var panel in accessPanels)
        {
            DrawBox(canvas, panel, SKColor.Parse("#2563eb"), "access_panel", panel.X1, panel.Y1);
        }

        if (mainPanel is not null && labelBoxes.Count > 0)
        {
            var panelBox = ShrinkBox(mainPanel, PanelShrinkPx);
            var lowerBound = PanelMin - PanelOverlapTolerance;
            var upperBound = PanelMax + PanelOverlapTolerance;

            foreach (var box in labelBoxes)
            {
                var overlap = IntersectionArea(box, panelBox);
                var ratio = box.Area > 0 ? overlap / box.Area : 0;
                var isValid = ratio >= lowerBound && ratio <= upperBound;
                var label = $"stickerOK {(isValid ? "OK" : "BAD")} ({Math.Round(ratio * 100)}% panel)";
                DrawBox(canvas, box, isValid ? SKColor.Parse("#22c55e") : SKColor.Parse("#ef4444"), label, box.X1, box.Y2 + 20);
            }
        }

        using var snapshot = surface.Snapshot();
        using var data = snapshot.Encode(SKEncodedImageFormat.Jpeg, 92);
        if (data is null)
            return null;

        return $"data:image/jpeg;base64,{Convert.ToBase64String(data.ToArray())}";
    }

    private static void DrawBox(SKCanvas canvas, ValidationBox box, SKColor color, string label, float labelX, float labelY)
    {
        using var stroke = new SKPaint
        {
            Color = color,
            IsStroke = true,
            StrokeWidth = 8,
            IsAntialias = true
        };

        using var text = new SKPaint
        {
            Color = color,
            TextSize = 24,
            IsAntialias = true
        };

        canvas.DrawRect(new SKRect(box.X1, box.Y1, box.X2, box.Y2), stroke);
        canvas.DrawText(label, labelX, Math.Max(labelY, 24), text);
    }

    private static bool IsBlueLabel(SKBitmap bitmap, ValidationBox box)
    {
        var startX = Math.Max(0, box.X1);
        var startY = Math.Max(0, box.Y1);
        var endX = Math.Min(bitmap.Width, box.X2);
        var endY = Math.Min(bitmap.Height, box.Y2);

        if (endX <= startX || endY <= startY)
            return false;

        var bluePixels = 0;
        var total = 0;

        for (var y = startY; y < endY; y++)
        {
            for (var x = startX; x < endX; x++)
            {
                var color = bitmap.GetPixel(x, y);
                var (h, s, v) = RgbToHsv(color.Red, color.Green, color.Blue);
                var hueOpenCv = h / 2.0;

                total++;
                if (hueOpenCv >= 95 && hueOpenCv <= 140 && s >= 0.235 && v >= 0.196)
                    bluePixels++;
            }
        }

        if (total == 0)
            return false;

        return (double)bluePixels / total >= BlueRatioThreshold;
    }

    private static (double H, double S, double V) RgbToHsv(byte r, byte g, byte b)
    {
        var rNorm = r / 255d;
        var gNorm = g / 255d;
        var bNorm = b / 255d;

        var max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
        var min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
        var delta = max - min;

        double h = 0;
        if (delta != 0)
        {
            if (max == rNorm)
                h = ((gNorm - bNorm) / delta) % 6;
            else if (max == gNorm)
                h = (bNorm - rNorm) / delta + 2;
            else
                h = (rNorm - gNorm) / delta + 4;

            h *= 60;
            if (h < 0)
                h += 360;
        }

        var s = max == 0 ? 0 : delta / max;
        var v = max;
        return (h, s, v);
    }

    private static SKBitmap? DecodeBitmap(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        try
        {
            byte[]? bytes = null;
            if (source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                var comma = source.IndexOf(',');
                if (comma > -1)
                    bytes = Convert.FromBase64String(source[(comma + 1)..]);
            }
            else if (LooksLikeBase64(source))
            {
                bytes = Convert.FromBase64String(source);
            }
            else if (File.Exists(source))
            {
                bytes = File.ReadAllBytes(source);
            }

            if (bytes is null || bytes.Length == 0)
                return null;

            return SKBitmap.Decode(bytes);
        }
        catch
        {
            return null;
        }
    }

    private static List<Prediction> ExtractPredictions(JsonNode? response)
    {
        if (response is null)
            return new List<Prediction>();

        foreach (var output in EnumerateOutputs(response))
        {
            var predictionsNode = output?["predictions"];
            if (predictionsNode is JsonArray directArray)
            {
                return directArray
                    .Select(ParsePrediction)
                    .Where(p => p is not null)
                    .Select(p => p!)
                    .ToList();
            }

            var nested = predictionsNode?["predictions"] as JsonArray;
            if (nested is not null)
            {
                return nested
                    .Select(ParsePrediction)
                    .Where(p => p is not null)
                    .Select(p => p!)
                    .ToList();
            }
        }

        return new List<Prediction>();
    }

    private static string? ExtractOutputImage(JsonNode? response)
    {
        if (response is null)
            return null;

        foreach (var output in EnumerateOutputs(response))
        {
            var imageObj = output?["output_image"];
            var value = imageObj?["value"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(value))
                continue;

            var type = imageObj?["type"]?.GetValue<string>();
            return NormalizeOutputImage(value, type);
        }

        return null;
    }

    private static IEnumerable<JsonNode?> EnumerateOutputs(JsonNode response)
    {
        if (response["outputs"] is JsonArray outputsArray)
        {
            foreach (var item in outputsArray)
                yield return item;

            yield break;
        }

        if (response is JsonArray rootArray)
        {
            foreach (var item in rootArray)
                yield return item;

            yield break;
        }

        yield return response;
    }

    private static string? NormalizeOutputImage(string value, string? type)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return value;

        if (string.Equals(type, "base64", StringComparison.OrdinalIgnoreCase) || LooksLikeBase64(value))
            return $"data:image/jpeg;base64,{value}";

        return value;
    }

    private static string? NormalizeBaseImage(string? baseImage)
    {
        if (string.IsNullOrWhiteSpace(baseImage))
            return null;

        if (baseImage.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return baseImage;

        if (LooksLikeBase64(baseImage))
            return $"data:image/jpeg;base64,{baseImage}";

        return baseImage;
    }

    private static bool LooksLikeBase64(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 32)
            return false;

        return value.All(c => char.IsLetterOrDigit(c) || c is '+' or '/' or '=');
    }

    private static Prediction? ParsePrediction(JsonNode? node)
    {
        if (node is null)
            return null;

        var x = node["x"]?.GetValue<double>() ?? 0;
        var y = node["y"]?.GetValue<double>() ?? 0;
        var width = node["width"]?.GetValue<double>() ?? 0;
        var height = node["height"]?.GetValue<double>() ?? 0;
        var @class = node["class"]?.GetValue<string>();
        var confidence = node["confidence"]?.GetValue<double?>();

        if (string.IsNullOrWhiteSpace(@class))
            return null;

        return new Prediction(x, y, width, height, @class, confidence);
    }

    private static ValidationBox BuildBox(Prediction prediction)
    {
        var x1 = (int)Math.Round(prediction.X - prediction.Width / 2d);
        var y1 = (int)Math.Round(prediction.Y - prediction.Height / 2d);
        var x2 = (int)Math.Round(prediction.X + prediction.Width / 2d);
        var y2 = (int)Math.Round(prediction.Y + prediction.Height / 2d);

        var area = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
        return new ValidationBox(x1, y1, x2, y2, area);
    }

    private static List<ValidationBox> ReduceLabelBoxes(List<ValidationBox> validBoxes)
    {
        if (validBoxes.Count <= 1)
            return validBoxes;

        if (validBoxes.Count == 2)
        {
            var first = validBoxes[0];
            var second = validBoxes[1];
            var minArea = Math.Min(first.Area, second.Area);
            var maxArea = Math.Max(first.Area, second.Area);
            if (maxArea <= 0)
                return new List<ValidationBox>();

            return minArea / maxArea >= AreaSimilarityThreshold
                ? validBoxes
                : new List<ValidationBox> { first.Area >= second.Area ? first : second };
        }

        return validBoxes.OrderByDescending(b => b.Area).Take(2).ToList();
    }

    private static ValidationBox ShrinkBox(ValidationBox box, int pixels) =>
        new(box.X1 + pixels, box.Y1 + pixels, box.X2 - pixels, box.Y2 - pixels, Math.Max(0, (box.X2 - box.X1 - 2 * pixels) * (box.Y2 - box.Y1 - 2 * pixels)));

    private static double IntersectionArea(ValidationBox a, ValidationBox b)
    {
        var x1 = Math.Max(a.X1, b.X1);
        var y1 = Math.Max(a.Y1, b.Y1);
        var x2 = Math.Min(a.X2, b.X2);
        var y2 = Math.Min(a.Y2, b.Y2);
        if (x2 <= x1 || y2 <= y1)
            return 0;

        return (x2 - x1) * (y2 - y1);
    }

    private sealed record Prediction(double X, double Y, double Width, double Height, string Class, double? Confidence);
}
