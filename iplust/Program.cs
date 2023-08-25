using iplust;

using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

using System.CommandLine;
using System.CommandLine.Binding;

var cmd = new QWQ();
await cmd.InvokeAsync(args);

public class QWQ : RootCommand
{
    public QWQ() : base("把文字放在图片下面组成一个表情包")
    {
        var binder = new RootArgumentBinder();

        AddArgument(binder.imageFileArgument);
        AddArgument(binder.textArgument);
        AddOption(binder.fontOption);
        AddOption(binder.reserveColorOption);
        AddOption(binder.debugOption);
        AddOption(binder.outputOption);

        this.SetHandler(DoCommand, binder);
    }

    private void DoCommand(RootArgument argument)
    {
        Logger.debugEnabled = argument.Debug;
        Logger.Info("启动！");
        Logger.Info("要加载图片了");
        var image = Utils.TryRunAndExitIfError(() => Image.Load(argument.ImageFile.FullName));
        Logger.Debug($"图片宽: {image.Size.Width}, 图片高: {image.Size.Height}");
        Logger.Info("要加载字体了");
        var fonts = ResolveFont();
        var fontName = argument.Font ?? "Microsoft YaHei";
        if (!fonts.TryGet(fontName, out var fontFamily))
        {
            Logger.Error($"无法找到字体 {fontName}");
            Environment.Exit(1);
        }

        Logger.Info("要计算文字尺寸了");
        var (fontSize, imageExpandHeight, location) = CalculateDrawInfo(ref fontFamily, image.Size, argument.Text);
        Logger.Debug($"绘制的字体大小: {fontSize}, 图片扩大高度为: {imageExpandHeight}, 绘制位置为: {location}");
        var font = fontFamily.CreateFont(fontSize, FontStyle.Regular);
        var bgColor = argument.ReserveColor ? Color.Black : Color.White;
        var textColor = argument.ReserveColor ? Color.Wheat : Color.Black;

        Logger.Info("要开始画图了");
        var resizeOptions = new ResizeOptions()
        {
            Size = new Size(image.Size.Width, image.Size.Height + imageExpandHeight),
            Mode = ResizeMode.BoxPad,
            PadColor = bgColor,
            Position = AnchorPositionMode.Top
        };
        image.Mutate(x => x.Resize(resizeOptions).DrawText(argument.Text, font, textColor, location));

        Logger.Info("要保存了");
        if (argument.OutputName == null)
            argument.OutputName = $"{Path.GetFileNameWithoutExtension(argument.ImageFile.Name)}_{Utils.FilterFileName(argument.Text)}.jpg";
        image.SaveAsJpeg(argument.OutputName);
        Logger.Info("搞定");
    }

    // 计算文字大小和新的图片尺寸
    private (int fontSize, int imageExpandHeight, PointF location) CalculateDrawInfo(ref FontFamily fontFamily, Size originalImageSize, string text)
    {
        bool IsOutOfMaxSize(ref FontRectangle textBounds, ref Size maxSize) => textBounds.Width > maxSize.Width || textBounds.Height > maxSize.Height;

        var expandMaxHeight = originalImageSize.Height / 10;  // 最大扩大1/10
        var padding = 0.1;
        var textBoundsMax = originalImageSize - new Size(2, 2) * (int)Math.Round(expandMaxHeight * padding);
        Logger.Debug($"文字最大边界为{textBoundsMax}");

        var prevFontSize = 20;
        var currentFontSize = 50;
        FontRectangle textBounds;
        do
        {
            var font = fontFamily.CreateFont(currentFontSize, FontStyle.Regular);
            textBounds = TextMeasurer.MeasureBounds(text, new TextOptions(font));
            int newFontSize = currentFontSize + Math.Abs(currentFontSize - prevFontSize) / 2 * (IsOutOfMaxSize(ref textBounds, ref textBoundsMax) ? -1 : 1);
            Logger.Debug($"计算文字尺寸中...，上一次: {prevFontSize}，这次: {currentFontSize}，下次: {newFontSize}，文字边界为: {textBounds}");
            prevFontSize = currentFontSize;
            currentFontSize = newFontSize;
        } while (currentFontSize != prevFontSize);
        var expandHeight = (int)Math.Round(textBounds.Height * (1 + padding * 2));
        var location = new PointF((originalImageSize.Width - textBounds.Width) / 2, (expandHeight - textBounds.Height) / 2) + new PointF(0f, originalImageSize.Height);
        return (currentFontSize, expandHeight, location);
    }

    private FontCollection ResolveFont()
    {
        var collection = new FontCollection();
        collection.AddSystemFonts();
        var customFontPath = Path.Combine(Environment.ProcessPath ?? Environment.CurrentDirectory, "Fonts");
        if (Directory.Exists(customFontPath))
        {
            foreach (var file in Directory.EnumerateFiles(customFontPath))
            {
                Utils.RunAndIgnoreException(() => collection.AddCollection(file));
            }
        }
        return collection;
    }
}

public class RootArgument
{
    public FileInfo ImageFile { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public string? Font { get; set; } = null;
    public bool ReserveColor { get; set; } = false;
    public bool Debug { get; set; } = false;
    public string? OutputName { get; set; } = null;
}

public class RootArgumentBinder : BinderBase<RootArgument>
{
    public readonly Argument<FileInfo> imageFileArgument = new("image", "The image you want to add text");
    public readonly Argument<string> textArgument = new("text", "The text you want to add to the image");
    public readonly Option<string> fontOption = new("--font", "The text font");
    public readonly Option<bool> reserveColorOption = new("--reserve", "Use white as text color, black as background color");
    public readonly Option<bool> debugOption = new("--debug", "Debug mode");
    public readonly Option<string> outputOption = new(new string[] { "--out", "-o" }, "Output file name");
    public readonly Option<bool> grayOption = new("--gray", "遗照");

    protected override RootArgument GetBoundValue(BindingContext bindingContext) => new RootArgument()
    {
        ImageFile = bindingContext.ParseResult.GetValueForArgument(imageFileArgument),
        Text = bindingContext.ParseResult.GetValueForArgument(textArgument),
        Font = bindingContext.ParseResult.GetValueForOption(fontOption),
        ReserveColor = bindingContext.ParseResult.GetValueForOption(reserveColorOption),
        Debug = bindingContext.ParseResult.GetValueForOption(debugOption),
        OutputName = bindingContext.ParseResult.GetValueForOption(outputOption),
    };
}