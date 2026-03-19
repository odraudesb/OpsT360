using System.Text.Json.Nodes;

namespace OpsT360.Services;

public sealed record ValidationBox(int X1, int Y1, int X2, int Y2, double Area);

public sealed record RoboflowValidationResult(
    bool IsSuccessful,
    string? ValidatedImageBase64,
    string? OutputImageBase64,
    IReadOnlyList<ValidationBox> DetectedLabels);

public sealed class RoboflowValidationService
{
    private const double AreaSimilarityThreshold = 0.4;
    private const double PanelMin = 0.4;
    private const double PanelMax = 0.6;
    private const double PanelOverlapTolerance = 0.03;
    private const int PanelShrinkPx = 2;

    public RoboflowValidationResult AnalyzeValidation(JsonNode? response, string? baseImageBase64)
    {
        var predictions = ExtractPredictions(response);
        var outputImage = ExtractOutputImage(response);

        if (predictions.Count == 0)
        {
            return new RoboflowValidationResult(false, outputImage ?? NormalizeBaseImage(baseImageBase64), outputImage, Array.Empty<ValidationBox>());
        }

        var validLabelBoxes = predictions
            .Where(p => string.Equals(p.Class, "stickerOK", StringComparison.OrdinalIgnoreCase))
            .Select(BuildBox)
            .ToList();

        var finalBoxes = ReduceLabelBoxes(validLabelBoxes);

        var accessPanels = predictions
            .Where(p => string.Equals(p.Class, "access_panel", StringComparison.OrdinalIgnoreCase))
            .Select(p => new { Box = BuildBox(p), Confidence = p.Confidence ?? 0d })
            .ToList();

        var mainPanel = accessPanels.Count == 0
            ? null
            : accessPanels.OrderByDescending(p => p.Confidence).First();

        if (mainPanel is null || finalBoxes.Count == 0)
        {
            var fallbackImage = outputImage ?? NormalizeBaseImage(baseImageBase64);
            return new RoboflowValidationResult(false, fallbackImage, outputImage, finalBoxes);
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

        var validatedImage = outputImage ?? NormalizeBaseImage(baseImageBase64);
        return new RoboflowValidationResult(success, validatedImage, outputImage, finalBoxes);
    }

    private static List<Prediction> ExtractPredictions(JsonNode? response)
    {
        if (response is null)
            return new List<Prediction>();

        var outputs = response["outputs"] as JsonArray;
        if (outputs is null)
            outputs = new JsonArray(response);

        foreach (var output in outputs)
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

            var nestedPredictions = predictionsNode?["predictions"] as JsonArray;
            if (nestedPredictions is not null)
            {
                return nestedPredictions
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

        var outputs = response["outputs"] as JsonArray;
        if (outputs is null)
            outputs = new JsonArray(response);

        foreach (var output in outputs)
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
