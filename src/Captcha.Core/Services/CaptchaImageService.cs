namespace Captcha.Core.Services;

using System;
using Models;
using SkiaSharp;

public class CaptchaImageService : ICaptchaImageService
{
    public SKBitmap CreateCaptchaImage(CaptchaConfigurationData config)
    {
        var bitmap = new SKBitmap(new SKImageInfo(config.Width, config.Height));
        using var graphics = new SKCanvas(bitmap);
        var rectangle = new SKRect(0, 0, config.Width, config.Height);

        using var font = GetFontThatFitsRectangle(config, rectangle);
        FillInTheBackground(config, rectangle, graphics);
        DrawWarpedText(config, font, rectangle, graphics);
        AddRandomNoise(config, rectangle, graphics);

        return bitmap;
    }

    private static void AddRandomNoise(CaptchaConfigurationData config, SKRect rectangle, SKCanvas graphics)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = config.PrimaryColor
        };

        var max = (int)Math.Max(rectangle.Width, rectangle.Height);

        for (var i = 0; i < (int)(rectangle.Width * rectangle.Height / config.Frequency); i++)
        {
            var x = Random.Shared.Next((int)rectangle.Width);
            var y = Random.Shared.Next((int)rectangle.Height);
            var width = Random.Shared.Next(max / Constants.CaptchaNoise);
            var height = Random.Shared.Next(max / Constants.CaptchaNoise);

            var areaToAddNoise = new SKRect(x, y, x + width, y + height);
            graphics.DrawOval(areaToAddNoise.MidX, areaToAddNoise.MidY,
                areaToAddNoise.Width / 2f, areaToAddNoise.Height / 2f, paint);
        }
    }

    private static void DrawWarpedText(CaptchaConfigurationData config, SKFont font, SKRect rectangle, SKCanvas graphics)
    {
        var path = font.GetTextPath(config.Text);
        var bounds = path.Bounds;

        // Center the text approximately within the rectangle
        var middleX = rectangle.MidX - bounds.MidX;
        var middleY = rectangle.MidY - bounds.MidY;
        var centerMatrix = SKMatrix.CreateTranslation(middleX, middleY);
        path.Transform(centerMatrix);

        // Warp the text
        var topLeft = new SKPoint(Random.Shared.Next((int)rectangle.Width) / Constants.WarpCaptchaTextFrequency, Random.Shared.Next((int)rectangle.Height) / Constants.WarpCaptchaTextFrequency);
        var topRight = new SKPoint(rectangle.Width - (Random.Shared.Next((int)rectangle.Width) / Constants.WarpCaptchaTextFrequency), Random.Shared.Next((int)rectangle.Height) / Constants.WarpCaptchaTextFrequency);
        var bottomLeft = new SKPoint(Random.Shared.Next((int)rectangle.Width) / Constants.WarpCaptchaTextFrequency, rectangle.Height - (Random.Shared.Next((int)rectangle.Height) / Constants.WarpCaptchaTextFrequency));
        var bottomRight = new SKPoint(rectangle.Width - (Random.Shared.Next((int)rectangle.Width) / Constants.WarpCaptchaTextFrequency), rectangle.Height - (Random.Shared.Next((int)rectangle.Height) / Constants.WarpCaptchaTextFrequency));

        var warpMatrix = Warp(topLeft, topRight, bottomRight, bottomLeft, rectangle.Width, rectangle.Height);
        path.Transform(warpMatrix);

        using var fillPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = config.PrimaryColor
        };
        graphics.DrawPath(path, fillPaint);
    }

    private static SKFont GetFontThatFitsRectangle(CaptchaConfigurationData config, SKRect rectangle)
    {
        var typeface = GetTypefaceThatCanRenderText(config.Text);

        SKFont font;
        var fontSize = rectangle.Height;
        float measuredWidth;

        // Adjust the font size until the text fits within the image.
        do
        {
            fontSize--;
            font = new SKFont(typeface, fontSize)
            {
                Edging = SKFontEdging.Antialias
            };

            measuredWidth = font.MeasureText(config.Text);
        } while (measuredWidth > rectangle.Width);

        return font;
    }


    private static void FillInTheBackground(CaptchaConfigurationData config, SKRect rectangle, SKCanvas graphics)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = config.SecondaryColor
        };
        graphics.DrawRect(rectangle, paint);
    }

    private static SKTypeface GetTypefaceThatCanRenderText(string text)
    {
        // Try to load Caveat font from embedded resource
        var assembly = typeof(CaptchaImageService).Assembly;
        using var stream = assembly.GetManifestResourceStream("Captcha.Core.Resources.Fonts.Caveat-SemiBold.ttf");
        if (stream != null)
        {
            var typeface = SKTypeface.FromStream(stream);
            if (typeface != null)
            {
                var font = new SKFont(typeface);
                if (font.ContainsGlyphs(text))
                {
                    return typeface;
                }
            }
        }

        // Fallback if Caveat is missing or doesn't support text
        return SKTypeface.Default;
    }


    /// <summary>
    /// This method applies similar logic to GraphicsPath.Warp() from System.Drawing.Common
    /// https://stackoverflow.com/questions/48416118/perspective-transform-in-skia
    /// </summary>
    private static SKMatrix Warp(SKPoint topLeft, SKPoint topRight, SKPoint botRight, SKPoint botLeft, float width, float height)
    {
        var (x1, y1) = (topLeft.X, topLeft.Y);
        var (x2, y2) = (topRight.X, topRight.Y);
        var (x3, y3) = (botRight.X, botRight.Y);
        var (x4, y4) = (botLeft.X, botLeft.Y);
        var (w, h) = (width, height);

        var scaleX = ((y1 * x2 * x4) - (x1 * y2 * x4) + (x1 * y3 * x4) - (x2 * y3 * x4) - (y1 * x2 * x3) + (x1 * y2 * x3) - (x1 * y4 * x3) + (x2 * y4 * x3)) / ((x2 * y3 * w) + (y2 * x4 * w) - (y3 * x4 * w) - (x2 * y4 * w) - (y2 * w * x3) + (y4 * w * x3));
        var skewX = ((-x1 * x2 * y3) - (y1 * x2 * x4) + (x2 * y3 * x4) + (x1 * x2 * y4) + (x1 * y2 * x3) + (y1 * x4 * x3) - (y2 * x4 * x3) - (x1 * y4 * x3)) / ((x2 * y3 * h) + (y2 * x4 * h) - (y3 * x4 * h) - (x2 * y4 * h) - (y2 * h * x3) + (y4 * h * x3));
        var transX = x1;
        var skewY = ((-y1 * x2 * y3) + (x1 * y2 * y3) + (y1 * y3 * x4) - (y2 * y3 * x4) + (y1 * x2 * y4) - (x1 * y2 * y4) - (y1 * y4 * x3) + (y2 * y4 * x3)) / ((x2 * y3 * w) + (y2 * x4 * w) - (y3 * x4 * w) - (x2 * y4 * w) - (y2 * w * x3) + (y4 * w * x3));
        var scaleY = ((-y1 * x2 * y3) - (y1 * y2 * x4) + (y1 * y3 * x4) + (x1 * y2 * y4) - (x1 * y3 * y4) + (x2 * y3 * y4) + (y1 * y2 * x3) - (y2 * y4 * x3)) / ((x2 * y3 * h) + (y2 * x4 * h) - (y3 * x4 * h) - (x2 * y4 * h) - (y2 * h * x3) + (y4 * h * x3));
        var transY = y1;
        var persp0 = ((x1 * y3) - (x2 * y3) + (y1 * x4) - (y2 * x4) - (x1 * y4) + (x2 * y4) - (y1 * x3) + (y2 * x3)) / ((x2 * y3 * w) + (y2 * x4 * w) - (y3 * x4 * w) - (x2 * y4 * w) - (y2 * w * x3) + (y4 * w * x3));
        var persp1 = ((-y1 * x2) + (x1 * y2) - (x1 * y3) - (y2 * x4) + (y3 * x4) + (x2 * y4) + (y1 * x3) - (y4 * x3)) / ((x2 * y3 * h) + (y2 * x4 * h) - (y3 * x4 * h) - (x2 * y4 * h) - (y2 * h * x3) + (y4 * h * x3));
        float persp2 = 1;

        return new SKMatrix(scaleX, skewX, transX, skewY, scaleY, transY, persp0, persp1, persp2);
    }
}
