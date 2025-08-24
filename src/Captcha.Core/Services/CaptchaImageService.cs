namespace Captcha.Core.Services;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Models;

public class CaptchaImageService : ICaptchaImageService
{
    public Bitmap CreateCaptchaImage(CaptchaConfigurationData config)
    {
        var bitmap = new Bitmap(config.Width, config.Height, PixelFormat.Format16bppRgb555);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rectangle = new Rectangle(0, 0, config.Width, config.Height);

        using var font = GetFontThatFitsRectangle(config, rectangle, graphics);
        FillInTheBackground(config, rectangle, graphics);
        DrawWarpedText(config, font, rectangle, graphics);
        AddRandomNoise(config, rectangle, graphics);

        return bitmap;
    }

    private static void AddRandomNoise(CaptchaConfigurationData config, Rectangle rectangle, Graphics graphics)
    {
        using var hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, config.SecondaryColor, config.PrimaryColor);
        var max = Math.Max(rectangle.Width, rectangle.Height);
        for (var i = 0; i < (int)(rectangle.Width * rectangle.Height / config.Frequency); i++)
        {
            var x = Random.Shared.Next(rectangle.Width);
            var y = Random.Shared.Next(rectangle.Height);
            var width = Random.Shared.Next(max / Constants.CaptchaNoise);
            var height = Random.Shared.Next(max / Constants.CaptchaNoise);
            graphics.FillEllipse(hatchBrush, x, y, width, height);
        }
    }

    private static void DrawWarpedText(CaptchaConfigurationData config, Font font, Rectangle rectangle, Graphics graphics)
    {
        // Set up the text to be in the middle
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        // Create a path using the text and warp it randomly.
        using var path = new GraphicsPath();
        path.AddString(config.Text, font.FontFamily, (int)font.Style, font.Size, rectangle, format);
        PointF[] points =
        [
            new(Random.Shared.Next(rectangle.Width) / Constants.WarpCaptchaTextFrequency, Random.Shared.Next(rectangle.Height) / Constants.WarpCaptchaTextFrequency),
            new(rectangle.Width - (Random.Shared.Next(rectangle.Width) / Constants.WarpCaptchaTextFrequency), Random.Shared.Next(rectangle.Height) / Constants.WarpCaptchaTextFrequency),
            new(Random.Shared.Next(rectangle.Width) / Constants.WarpCaptchaTextFrequency, rectangle.Height - (Random.Shared.Next(rectangle.Height) / Constants.WarpCaptchaTextFrequency)),
            new(rectangle.Width - (Random.Shared.Next(rectangle.Width) / Constants.WarpCaptchaTextFrequency), rectangle.Height - (Random.Shared.Next(rectangle.Height) / Constants.WarpCaptchaTextFrequency))
        ];
        using var matrix = new Matrix();
        matrix.Translate(0F, 0F);
        path.Warp(points, rectangle, matrix, WarpMode.Perspective, 0F);

        // Draw the text.
        using var hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, config.SecondaryColor, config.PrimaryColor);
        graphics.FillPath(hatchBrush, path);
    }

    private static Font GetFontThatFitsRectangle(CaptchaConfigurationData config, Rectangle rectangle, Graphics graphics)
    {
        SizeF size;
        float fontSize = rectangle.Height;

        // Adjust the font size until the text fits within the image.
        do
        {
            fontSize--;
            var font = new Font(config.Font, fontSize, FontStyle.Bold);
            size = graphics.MeasureString(config.Text, font);
        } while (size.Width > rectangle.Width);

        return new Font(config.Font, fontSize, FontStyle.Bold);
    }

    private static void FillInTheBackground(CaptchaConfigurationData config, Rectangle rectangle, Graphics graphics)
    {
        using var hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, config.SecondaryColor, config.ThirdColor);
        graphics.FillRegion(hatchBrush, new Region(rectangle));
    }
}
