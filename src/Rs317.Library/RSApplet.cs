
using System.Threading.Tasks;

public class RSApplet : Applet, Runnable, MouseListener, MouseMotionListener, KeyListener, FocusListener, WindowListener
{
	private int gameState;

	private int delayTime;

	int minDelay;

	private long[] otims;

	int fps;

	bool debugRequested;

	int width;

	int height;

	Graphics gameGraphics;

	RSImageProducer fullGameScreen;

	RSFrame gameFrame;

	private bool clearScreen;

	bool awtFocus;

	int idleTime;

	int mouseButton;

	public int mouseX;

	public int mouseY;

	private int eventMouseButton;

	private int eventClickX;

	private int eventClickY;

	private long eventClickTime;

	int clickType;

	int clickX;

	int clickY;

	long clickTime;

	int[] keyStatus;

	private int[] inputBuffer;

	private int readIndex;

	private int writeIndex;

	RSApplet()
	{
		delayTime = 20;
		minDelay = 1;
		otims = new long[10];
		debugRequested = false;
		clearScreen = true;
		awtFocus = true;
		keyStatus = new int[128];
		inputBuffer = new int[128];
	}

	void cleanUpForQuit()
	{
	}

	void createClientFrame(int width, int height)
	{
		this.width = width;
		this.height = height;
		gameFrame = new RSFrame(this, this.width, this.height);
		gameGraphics = getGameComponent().getGraphics();
		fullGameScreen = new RSImageProducer(this.width, height, getGameComponent());
		startRunnable(this, 1);
	}

	public sealed override void destroy()
	{
		gameState = -1;
		try
		{
			Thread.sleep(5000L);
		}
		catch(Exception _ex)
		{
		}
		if(gameState == -1)
			exit();
	}

	void drawLoadingText(int percentage, String s)
	{
		while(gameGraphics == null)
		{
			gameGraphics = getGameComponent().getGraphics();
			try
			{
				getGameComponent().repaint();
			}
			catch(Exception _ex)
			{
			}
			try
			{
				Thread.sleep(1000L);
			}
			catch(Exception _ex)
			{
			}
		}
		Font helveticaBold = new Font("Helvetica", 1, 13);
		FontMetrics fontmetrics = getGameComponent().getFontMetrics(helveticaBold);
		Font helvetica = new Font("Helvetica", 0, 13);
		getGameComponent().getFontMetrics(helvetica);
		if(clearScreen)
		{
			gameGraphics.setColor(Color.black);
			gameGraphics.fillRect(0, 0, width, height);
			clearScreen = false;
		}
		Color color = new Color(140, 17, 17);
		int centerHeight = height / 2 - 18;
		gameGraphics.setColor(color);
		gameGraphics.drawRect(width / 2 - 152, centerHeight, 304, 34);
		gameGraphics.fillRect(width / 2 - 150, centerHeight + 2, percentage * 3, 30);
		gameGraphics.setColor(Color.black);
		gameGraphics.fillRect((width / 2 - 150) + percentage * 3, centerHeight + 2, 300 - percentage * 3, 30);
		gameGraphics.setFont(helveticaBold);
		gameGraphics.setColor(Color.white);
		gameGraphics.drawString(s, (width - fontmetrics.stringWidth(s)) / 2, centerHeight + 22);
	}

	private void exit()
	{
		gameState = -2;
		cleanUpForQuit();
		if(gameFrame != null)
		{
			try
			{
				Thread.sleep(1000L);
			}
			catch(Exception _ex)
			{
			}
			try
			{
				System.exit(0);
			}
			catch(Throwable _ex)
			{
			}
		}
	}

	public sealed override void focusGained(FocusEvent focusevent)
	{
		awtFocus = true;
		clearScreen = true;
		redraw();
	}

	public sealed override void focusLost(FocusEvent focusevent)
	{
		awtFocus = false;
		for(int key = 0; key < 128; key++)
			keyStatus[key] = 0;

	}

	Component getGameComponent()
	{
		if(gameFrame != null)
			return gameFrame;
		else
			return this;
	}

	void initClientFrame(int width, int height)
	{
		this.width = width;
		this.height = height;
		gameGraphics = getGameComponent().getGraphics();
		fullGameScreen = new RSImageProducer(this.width, this.height, getGameComponent());
		startRunnable(this, 1);
	}

	public sealed override void keyPressed(KeyEvent keyevent)
	{
		idleTime = 0;
		int keyCode = keyevent.getKeyCode();
		int keyChar = keyevent.getKeyChar();

		if(keyChar < 30)
			keyChar = 0;
		if(keyCode == 37) // Left
			keyChar = 1;
		if(keyCode == 39) // Right
			keyChar = 2;
		if(keyCode == 38) // Up
			keyChar = 3;
		if(keyCode == 40) // Down
			keyChar = 4;
		if(keyCode == 17) // CTRL
			keyChar = 5;
		if(keyCode == 8) // Backspace
			keyChar = 8;
		if(keyCode == 127) // Delete
			keyChar = 8;
		if(keyCode == 9) // Meant to be tab but doesn't work
			keyChar = 9;
		if(keyCode == 10) // Enter / return
			keyChar = 10;
		if(keyCode >= 112 && keyCode <= 123) // F keys
			keyChar = (1008 + keyCode) - 112;
		if(keyCode == 36) // Home
			keyChar = 1000;
		if(keyCode == 35) // End
			keyChar = 1001;
		if(keyCode == 33) // Page up
			keyChar = 1002;
		if(keyCode == 34) // Page down
			keyChar = 1003;
		if(keyChar > 0 && keyChar < 128)
			keyStatus[keyChar] = 1;
		if(keyChar > 4)
		{
			inputBuffer[writeIndex] = keyChar;
			writeIndex = writeIndex + 1 & 0x7f;
		}
	}

	public override void keyReleased(KeyEvent keyevent)
	{
		idleTime = 0;
		int keyCode = keyevent.getKeyCode();
		char keyChar = keyevent.getKeyChar();
		if(keyChar < '\036')
			keyChar = '\0';
		if(keyCode == 37)
			keyChar = '\001';
		if(keyCode == 39)
			keyChar = '\002';
		if(keyCode == 38)
			keyChar = '\003';
		if(keyCode == 40)
			keyChar = '\004';
		if(keyCode == 17)
			keyChar = '\005';
		if(keyCode == 8)
			keyChar = '\b';
		if(keyCode == 127)
			keyChar = '\b';
		if(keyCode == 9)
			keyChar = '\t';
		if(keyCode == 10)
			keyChar = '\n';
		if(keyChar > 0 && keyChar < '\200')
			keyStatus[keyChar] = 0;
	}

	public sealed override void keyTyped(KeyEvent keyevent)
	{
	}

	public sealed override void mouseClicked(MouseEvent mouseevent)
	{
	}

	public sealed override void mouseDragged(MouseEvent mouseevent)
	{
		int x = mouseevent.getX();
		int y = mouseevent.getY();
		if(gameFrame != null)
		{
			x -= 4;
			y -= 22;
		}
		idleTime = 0;
		mouseX = x;
		mouseY = y;
	}

	public sealed override void mouseEntered(MouseEvent mouseevent)
	{
	}

	public sealed override void mouseExited(MouseEvent mouseevent)
	{
		idleTime = 0;
		mouseX = -1;
		mouseY = -1;
	}

	public sealed override void mouseMoved(MouseEvent mouseevent)
	{
		int x = mouseevent.getX();
		int y = mouseevent.getY();
		if(gameFrame != null)
		{
			x -= 4;
			y -= 22;
		}
		idleTime = 0;
		mouseX = x;
		mouseY = y;
	}

	public sealed override void mousePressed(MouseEvent mouseEvent)
	{
		int x = mouseEvent.getX();
		int y = mouseEvent.getY();
		if(gameFrame != null)
		{
			x -= 4;
			y -= 22;
		}
		idleTime = 0;
		eventClickX = x;
		eventClickY = y;
		eventClickTime = System.currentTimeMillis();
		if(mouseEvent.isMetaDown())
		{
			eventMouseButton = 2;
			mouseButton = 2;
		}
		else
		{
			eventMouseButton = 1;
			mouseButton = 1;
		}
	}

	public sealed override void mouseReleased(MouseEvent mouseevent)
	{
		idleTime = 0;
		mouseButton = 0;
	}

	public sealed override void paint(Graphics g)
	{
		if(gameGraphics == null)
			gameGraphics = g;
		clearScreen = true;
		redraw();
	}

	void processDrawing()
	{
	}

	void processGameLoop()
	{
	}

	int readCharacter()
	{
		int character = -1;
		if(writeIndex != readIndex)
		{
			character = inputBuffer[readIndex];
			readIndex = readIndex + 1 & 0x7f;
		}
		return character;
	}

	void redraw()
	{
	}

	public override void run()
	{
		getGameComponent().addMouseListener(this);
		getGameComponent().addMouseMotionListener(this);
		getGameComponent().addKeyListener(this);
		getGameComponent().addFocusListener(this);
		if(gameFrame != null)
			gameFrame.addWindowListener(this);
		drawLoadingText(0, "Loading...");
		startUp();
		int opos = 0;
		int ratio = 256;
		int delay = 1;
		int count = 0;
		int intex = 0;
		for(int otim = 0; otim < 10; otim++)
			otims[otim] = System.currentTimeMillis();

		while(gameState >= 0)
		{
			if(gameState > 0)
			{
				gameState--;
				if(gameState == 0)
				{
					exit();
					return;
				}
			}
			int i2 = ratio;
			int j2 = delay;
			ratio = 300;
			delay = 1;
			long currentTime = System.currentTimeMillis();
			if(otims[opos] == 0L)
			{
				ratio = i2;
				delay = j2;
			}
			else if(currentTime > otims[opos])
				ratio = (int)(2560 * delayTime / (currentTime - otims[opos]));
			if(ratio < 25)
				ratio = 25;
			if(ratio > 256)
			{
				ratio = 256;
				delay = (int)(delayTime - (currentTime - otims[opos]) / 10L);
			}
			if(delay > delayTime)
				delay = delayTime;
			otims[opos] = currentTime;
			opos = (opos + 1) % 10;
			if(delay > 1)
			{
				for(int otim = 0; otim < 10; otim++)
					if(otims[otim] != 0L)
						otims[otim] += delay;

			}
			if(delay < minDelay)
				delay = minDelay;
			try
			{
				Thread.sleep(delay);
			}
			catch(InterruptedException _ex)
			{
				intex++;
			}
			for(; count < 256; count += ratio)
			{
				clickType = eventMouseButton;
				clickX = eventClickX;
				clickY = eventClickY;
				clickTime = eventClickTime;
				eventMouseButton = 0;
				processGameLoop();
				readIndex = writeIndex;
			}

			count &= 0xff;
			if(delayTime > 0)
				fps = (1000 * ratio) / (delayTime * 256);
			processDrawing();
			if(debugRequested)
			{
				System.out.println("ntime:" + currentTime);
				for(int i = 0; i < 10; i++)
				{
					int otim = ((opos - i - 1) + 20) % 10;
					System.out.println("otim" + otim + ":" + otims[otim]);
				}

				System.out.println("fps:" + fps + " ratio:" + ratio + " count:" + count);
				System.out.println("del:" + delay + " deltime:" + delayTime + " mindel:" + minDelay);
				System.out.println("intex:" + intex + " opos:" + opos);
				debugRequested = false;
				intex = 0;
			}
		}
		if(gameState == -1)
			exit();
	}

	void setFrameRate(int frameRate)
	{
		delayTime = 1000 / frameRate;
	}

	public override void start()
	{
		if(gameState >= 0)
			gameState = 0;
	}

	public void startRunnable(IRunnable runnable, int priority)
	{
		//Run it on the threadpool instead.
		Task.Factory.StartNew(runnable.run);
	}

	void startUp()
	{
	}

	public sealed override void stop()
	{
		if(gameState >= 0)
			gameState = 4000 / delayTime;
	}

	public sealed override void update(Graphics g)
	{
		if(gameGraphics == null)
			gameGraphics = g;
		clearScreen = true;
		redraw();
	}

	public sealed override void windowActivated(WindowEvent windowevent)
	{
	}

	public sealed override void windowClosed(WindowEvent windowevent)
	{
	}

	public sealed override void windowClosing(WindowEvent windowevent)
	{
		destroy();
	}

	public sealed override void windowDeactivated(WindowEvent windowevent)
	{
	}

	public sealed override void windowDeiconified(WindowEvent windowevent)
	{
	}

	public sealed override void windowIconified(WindowEvent windowevent)
	{
	}

	public sealed override void windowOpened(WindowEvent windowevent)
	{
	}
}
