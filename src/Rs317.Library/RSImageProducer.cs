
sealed class RSImageProducer : ImageProducer, ImageObserver
{
	public final int[] pixels;

	public final int width;

	public final int height;

	private final ColorModel colourModel;

	private ImageConsumer imageConsumer;

	private final Image image;

	public RSImageProducer(int width, int height, Component component)
	{
		this.width = width;
		this.height = height;
		pixels = new int[width * height];
		colourModel = new DirectColorModel(32, 0xff0000, 65280, 255);
		image = component.createImage(this);
		drawPixels();
		component.prepareImage(image, this);
		drawPixels();
		component.prepareImage(image, this);
		drawPixels();
		component.prepareImage(image, this);
		initDrawingArea();
	}

	public override synchronized void addConsumer(ImageConsumer imageConsumer)
	{
		this.imageConsumer = imageConsumer;
		imageConsumer.setDimensions(width, height);
		imageConsumer.setProperties(null);
		imageConsumer.setColorModel(colourModel);
		imageConsumer.setHints(14);
	}

	public void drawGraphics(int y, Graphics g, int x)
	{
		drawPixels();
		g.drawImage(image, x, y, this);
	}

	private synchronized void drawPixels()
	{
		if(imageConsumer != null)
		{
			imageConsumer.setPixels(0, 0, width, height, colourModel, pixels, 0, width);
			imageConsumer.imageComplete(2);
		}
	}

	public override boolean imageUpdate(Image image, int i, int j, int k, int l, int i1)
	{
		return true;
	}

	public void initDrawingArea()
	{
		DrawingArea.initDrawingArea(height, width, pixels);
	}

	public override synchronized boolean isConsumer(ImageConsumer imageconsumer)
	{
		return imageConsumer == imageconsumer;
	}

	public override synchronized void removeConsumer(ImageConsumer imageconsumer)
	{
		if(imageConsumer == imageconsumer)
			imageConsumer = null;
	}

	public override void requestTopDownLeftRightResend(ImageConsumer imageconsumer)
	{
		System.out.println("TDLR");
	}

	public override void startProduction(ImageConsumer imageconsumer)
	{
		addConsumer(imageconsumer);
	}
}